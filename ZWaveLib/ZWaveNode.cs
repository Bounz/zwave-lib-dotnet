/*
    This file is part of ZWaveLib Project source code.

    ZWaveLib is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    ZWaveLib is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with ZWaveLib.  If not, see <http://www.gnu.org/licenses/>.  
*/

/*
 *     Author: Generoso Martello <gene@homegenie.it>
 *     Project Homepage: https://github.com/genielabs/zwave-lib-dotnet
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

using ZWaveLib.CommandClasses;
using System.Xml.Serialization;
using ZWaveLib.Enums;
using ZWaveLib.Utilities;

namespace ZWaveLib
{
    /// <summary>
    /// Z-wave node object.
    /// </summary>
    [Serializable]
    public class ZWaveNode : IZWaveNode
    {
        #region Private fields

        private ZWaveController _controller;

        #endregion

        #region Public fields and events

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        public byte Id { get; /*protected*/ set; }

        public NodeCapabilities ProtocolInfo { get; /*internal*/ set; }

        public NodeVersion Version { get; /*internal*/ set; }

        /// <summary>
        /// Gets or sets the node information frame.
        /// </summary>
        /// <value>The node information frame.</value>
        public byte[] NodeInformationFrame { get; /*internal*/ set; }

        /// <summary>
        /// Gets or sets the secured node information frame.
        /// </summary>
        /// <value>The secured node information frame.</value>
        public byte[] SecuredNodeInformationFrame { get; /*internal*/ set; }

        public List<NodeCommandClass> CommandClasses { get; /*internal*/ set; }

        /// <summary>
        /// Gets or sets the manufacturer specific.
        /// </summary>
        /// <value>The manufacturer specific.</value>
        public ManufacturerSpecificInfo ManufacturerSpecific { get; /*internal*/ set; }

        /// <summary>
        /// Gets or sets the data.
        /// </summary>
        /// <value>The data.</value>
        [XmlIgnore]
        public List<NodeData> Data { get; internal set; }

        /// <summary>
        /// Occurs when node is updated.
        /// </summary>
        public event Action<object, NodeEvent> NodeUpdated;

        #endregion

        #region Lifecycle

        /// <summary>
        /// Initializes a new instance of the <see cref="ZWaveLib.ZWaveNode"/> class.
        /// </summary>
        public ZWaveNode()
        {
            Data = new List<NodeData>();
            CommandClasses = new List<NodeCommandClass>();
            ProtocolInfo = new NodeCapabilities();
            NodeInformationFrame = new byte[]{ };
            SecuredNodeInformationFrame = new byte[]{ };
            ManufacturerSpecific = new ManufacturerSpecificInfo();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ZWaveLib.ZWaveNode"/> class.
        /// </summary>
        /// <param name="controller">Controller.</param>
        /// <param name="nodeId">Node identifier.</param>
        public ZWaveNode(ZWaveController controller, byte nodeId) : this()
        {
            _controller = controller;
            Id = nodeId;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ZWaveLib.ZWaveNode"/> class.
        /// </summary>
        /// <param name="controller">Controller.</param>
        /// <param name="nodeId">Node identifier.</param>
        /// <param name="genericType">Generic type.</param>
        public ZWaveNode(ZWaveController controller, byte nodeId, byte genericType) : this(controller, nodeId)
        {
            ProtocolInfo.GenericType = genericType;
        }

        #endregion

        #region Public members

        /// <summary>
        /// Gets the command class.
        /// </summary>
        /// <returns>The command class.</returns>
        /// <param name="cclass">Cclass.</param>
        public NodeCommandClass GetCommandClass(CommandClass cclass)
        {
            return this.CommandClasses.Find(cc => cc.Id.Equals((byte)cclass));
        }
            
        /// <summary>
        /// Supports the command class.
        /// </summary>
        /// <returns><c>true</c>, if command class is supported, <c>false</c> otherwise.</returns>
        /// <param name="commandClass">Command Class</param>
        public bool SupportCommandClass(CommandClass commandClass)
        {
            bool isSupported = false;
            isSupported = (Array.IndexOf(NodeInformationFrame, (byte)commandClass) >= 0);
            return isSupported;
        }

        /// <summary>
        /// Determines whether this instance command class specified by c is secured.
        /// </summary>
        /// <returns><c>true</c> true if is secured command class; otherwise, <c>false</c>.</returns>
        /// <param name="commandClass">Command Class.</param>
        public bool IsSecuredCommandClass(CommandClass commandClass)
        {
            bool isSecured = false;
            if (SecuredNodeInformationFrame != null)
            {
                isSecured = (Array.IndexOf(SecuredNodeInformationFrame, (byte)commandClass) >= 0);
            }
            return isSecured;
        }

        /// <summary>
        /// Gets the custom node data.
        /// </summary>
        /// <returns>The data.</returns>
        /// <param name="fieldId">Field identifier.</param>
        /// <param name="defaultValue">Default value.</param>
        public NodeData GetData(string fieldId, object defaultValue = null)
        {
            var item = Data.Find(d => d.Name == fieldId);
            if (item != null)
                return item;

            if (defaultValue != null)
            {
                item = new NodeData(fieldId, defaultValue);
                Data.Add(item);
            }
            return item;
        }

        /// <summary>
        /// Updates the custom node data.
        /// </summary>
        /// <param name="fieldId">Field identifier.</param>
        /// <param name="value">Value.</param>
        public void UpdateData(string fieldId, object value)
        {
            var item = GetData(fieldId, value);
            item.Value = value;
        }

        /// <summary>
        /// Sends the data request.
        /// </summary>
        /// <param name="request">Request.</param>
        public ZWaveMessage SendDataRequest(byte[] request)
        {
            byte cmdClass = request[0];
            byte[] message = ZWaveMessage.BuildSendDataRequest(Id, request);
            // when cmdClass belongs to SecuredNodeInformationFrame we need to encrypt the message
            if (cmdClass != (byte)CommandClass.Security && IsSecuredCommandClass((CommandClass)cmdClass))
            {
                Security.SendMessage(this, message);
                // TODO: not yet supported for Security Command Classs,
                // TODO: update Security.cs class
                return null;
            }
            else
            {
                return SendMessage(message);
            }
        }

        #endregion

        #region Private members

        internal virtual bool ApplicationCommandHandler(byte[] rawMessage)
        {
            NodeEvent messageEvent = null;
            int messageLength = rawMessage.Length;

            if (messageLength > 8)
            {
                byte commandLength = rawMessage[6];
                byte commandClass = rawMessage[7];
                // TODO: this should be moved inside the NodeCommandClass class
                // TODO: as "Instance" property
                var cc = CommandClassFactory.GetCommandClass(commandClass);
                byte[] message = new byte[commandLength];
                Array.Copy(rawMessage, 7, message, 0, commandLength);
                try
                {
                    if (cc != null)
                    {
                        messageEvent = cc.GetEvent(this, message);
                    }else
                    {
                        Utility.Logger.Debug("CommandClass {0} not supported yet", commandClass);
                    }
                }
                catch (Exception ex)
                {
                    Utility.Logger.Error(ex);
                }
            }

            if (messageEvent != null)
            {
                OnNodeUpdated(messageEvent);
            }
            else if (messageLength > 3 && rawMessage[3] != (byte)ZWaveFunction.SendData)
            {
                Utility.Logger.Warn("Unhandled message type: {0}", BitConverter.ToString(rawMessage));
            }

            return false;
        }

        public ZWaveMessage SendMessage(byte[] message)
        {
            var msg = new ZWaveMessage(message, MessageDirection.Outbound, true);
            _controller.QueueMessage(msg);
            return msg;
        }

        internal void UpdateCommandClassList()
        {
            // we only build the list once
            if (this.CommandClasses.Count != NodeInformationFrame.Length)
            {
                this.CommandClasses.Clear();
                foreach (var cc in NodeInformationFrame)
                {
                    var cclass = CommandClass.NotSet;
                    Enum.TryParse<CommandClass>(cc.ToString(), out cclass);
                    this.CommandClasses.Add(new NodeCommandClass((byte)cclass));
                }
            }
        }

        internal void SetController(ZWaveController controller)
        {
            this._controller = controller;
        }

        public void OnNodeUpdated(NodeEvent zevent)
        {
            if (NodeUpdated != null)
                NodeUpdated(this, zevent);
        }

        public void ResendQueuedMessages()
        {
            var queuedMessages = (Queue<byte[]>)GetData("WakeUpResendQueue", new Queue<byte[]>()).Value;
            
            var i = 0;
            while (queuedMessages.Count > 0)
            {
                var message = queuedMessages.Dequeue();
                Utility.Logger.Trace("Sending message {0} {1}", ++i, BitConverter.ToString(message));
                SendMessage(message);
            }
        }

        public void ResendOnWakeUp(byte[] msg)
        {
            var minCommandLength = 8;
            if (msg.Length < minCommandLength || msg[6] == (byte) CommandClass.WakeUp && msg[7] == Command.WakeUp.NoMoreInfo)
                return;

            var command = new byte[minCommandLength];
            Array.Copy(msg, 0, command, 0, minCommandLength);
            // discard any message having same header and command (first 8 bytes = header + command class + command)
            var wakeUpResendQueue = (Queue<byte[]>)GetData("WakeUpResendQueue", new Queue<byte[]>()).Value;
            for (int i = wakeUpResendQueue.Count - 1; i >= 0; i--)
            {
                var queuedMessage = wakeUpResendQueue.Peek();
                var queuedCommand = new byte[minCommandLength];
                Array.Copy(queuedMessage, 0, queuedCommand, 0, minCommandLength);
                if (queuedCommand.SequenceEqual(command))
                {
                    Utility.Logger.Trace("Removing old message {0}", BitConverter.ToString(queuedMessage));
                    wakeUpResendQueue.Dequeue();
                }
            }

            Utility.Logger.Trace("Adding message {0}", BitConverter.ToString(msg));
            wakeUpResendQueue.Enqueue(msg);
            var wakeUpStatus = (WakeUpStatus)GetData("WakeUpStatus", new WakeUpStatus()).Value;
            if (!wakeUpStatus.IsSleeping)
            {
                wakeUpStatus.IsSleeping = true;
                var nodeEvent = new NodeEvent(this, EventParameter.WakeUpSleepingStatus, 1 /* 1 = sleeping, 0 = awake */, 0);
                OnNodeUpdated(nodeEvent);
            }
        }

        #endregion

    }
}
