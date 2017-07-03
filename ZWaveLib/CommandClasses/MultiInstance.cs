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
using System.Collections.Generic;
using ZWaveLib.Enums;
using ZWaveLib.Utilities;

namespace ZWaveLib.CommandClasses
{
    [Obsolete("Use MultiChannel command class instead")]
    public class MultiInstance : ICommandClass
    {
        public CommandClass GetClassId()
        {
            return CommandClass.MultiInstance;
        }

        public NodeEvent GetEvent(IZWaveNode node, byte[] message)
        {
            NodeEvent nodeEvent = null;

            var cmdType = message[1];
            var instanceCmdClass = message[2];

            switch (cmdType)
            {
                case Command.MultiChannel.MultiInstanceCmdEncap:
                    nodeEvent = HandleMultiInstanceEncapReport(node, message);
                    break;

                case Command.MultiChannel.CmdEncap:
                    nodeEvent = HandleMultiChannelEncapReport(node, message);
                    break;

                case Command.MultiChannel.MultiInstanceReport:
                    var instanceCount = message[3];
                    switch (instanceCmdClass)
                    {
                        case (byte) CommandClass.SwitchBinary:
                            nodeEvent = new NodeEvent(node, EventParameter.MultiinstanceSwitchBinaryCount, instanceCount, 0);
                            break;
                        case (byte) CommandClass.SwitchMultilevel:
                            nodeEvent = new NodeEvent(node, EventParameter.MultiinstanceSwitchMultilevelCount, instanceCount, 0);
                            break;
                        case (byte) CommandClass.SensorBinary:
                            nodeEvent = new NodeEvent(node, EventParameter.MultiinstanceSensorBinaryCount, instanceCount, 0);
                            break;
                        case (byte) CommandClass.SensorMultilevel:
                            nodeEvent = new NodeEvent(node, EventParameter.MultiinstanceSensorMultilevelCount, instanceCount, 0);
                            break;
                    }
                    break;

            }

            return nodeEvent;
        }

        public static ZWaveMessage GetCount(ZWaveNode node, byte commandClass)
        {
            return node.SendDataRequest(new[] {
                (byte)CommandClass.MultiInstance,
                Command.MultiInstance.CountGet,
                commandClass
            });
        }

        [Obsolete("Use .ToMultiChannel() extension method")]
        public static ZWaveMessage SwitchBinaryGet(ZWaveNode node, byte instance)
        {
            return node.SendDataRequest(SwitchBinary.Get().ToMultiChannel(instance));
        }

        [Obsolete("Use .ToMultiChannel() extension method")]
        public static ZWaveMessage SwitchBinarySet(ZWaveNode node, byte instance, int value)
        {
            return node.SendDataRequest(SwitchBinary.Set(value).ToMultiChannel(instance));
        }

        [Obsolete("Use .ToMultiChannel() extension method")]
        public static ZWaveMessage SwitchMultiLevelGet(ZWaveNode node, byte instance)
        {
            return node.SendDataRequest(SwitchMultilevel.Get().ToMultiChannel(instance));
        }

        [Obsolete("Use .ToMultiChannel() extension method")]
        public static ZWaveMessage SwitchMultiLevelSet(ZWaveNode node, byte instance, int value)
        {
            return node.SendDataRequest(SwitchMultilevel.Set(value).ToMultiChannel(instance));
        }

        public static ZWaveMessage SensorBinaryGet(ZWaveNode node, byte instance)
        {
            return node.SendDataRequest(new byte[] {
                (byte)CommandClass.MultiInstance,
                0x06, // ??
                instance,
                (byte)CommandClass.SensorBinary,
                0x04 //
            });
        }

        public static ZWaveMessage SensorMultiLevelGet(ZWaveNode node, byte instance)
        {
            return node.SendDataRequest(new byte[] {
                (byte)CommandClass.MultiInstance,
                0x06, // ??
                instance,
                (byte)CommandClass.SensorMultilevel,
                0x04 //
            });
        }

        private NodeEvent HandleMultiInstanceEncapReport(IZWaveNode node, byte[] message)
        {
            if (message.Length < 5)
            {
                Utility.Logger.Error($"MultiInstance encapsulated message ERROR: message is too short: {BitConverter.ToString(message)}");
                return null;
            }

            var instanceNumber = message[2];
            var instanceCmdClass = message[3];
            var instanceMessage = new byte[message.Length - 3]; //TODO:
            Array.Copy(message, 3, instanceMessage, 0, message.Length - 3);

            Utility.Logger.Debug($"MultiInstance encapsulated message: CmdClass: {instanceCmdClass}; message: {BitConverter.ToString(instanceMessage)}");

            var cc = CommandClassFactory.GetCommandClass(instanceCmdClass);
            if (cc == null)
            {
                Utility.Logger.Error($"Can't find CommandClass handler for command class {instanceCmdClass}");
                return null;
            }
            var zevent = cc.GetEvent(node, instanceMessage);
            zevent.Instance = instanceNumber;
            zevent.NestedEvent = GetNestedEvent(instanceCmdClass, zevent);
            return zevent;
        }

        private NodeEvent HandleMultiChannelEncapReport(IZWaveNode node, byte[] message)
        {
            if (message.Length < 6)
            {
                Utility.Logger.Error($"MultiChannel encapsulated message ERROR: message is too short: {BitConverter.ToString(message)}");
                return null;
            }

            var instanceNumber = message[2];
            var instanceCmdClass = message[4];
            var instanceMessage = new byte[message.Length - 4]; //TODO
            Array.Copy(message, 4, instanceMessage, 0, message.Length - 4);

            Utility.Logger.Debug($"MultiChannel encapsulated message: CmdClass: {instanceCmdClass}; message: {BitConverter.ToString(instanceMessage)}");

            var cc = CommandClassFactory.GetCommandClass(instanceCmdClass);
            if (cc == null)
            {
                Utility.Logger.Error($"Can't find CommandClass handler for command class {instanceCmdClass}");
                return null;
            }
            var zevent = cc.GetEvent(node, instanceMessage);
            zevent.Instance = instanceNumber;
            zevent.NestedEvent = GetNestedEvent(instanceCmdClass, zevent);
            return zevent;
        }

        private NodeEvent GetNestedEvent(byte commandClass, NodeEvent nodeEvent)
        {
            NodeEvent nestedEvent = null;
            switch (commandClass)
            {
            case (byte) CommandClass.SwitchBinary:
                nestedEvent = new NodeEvent(nodeEvent.Node, EventParameter.MultiinstanceSwitchBinary, nodeEvent.Value, nodeEvent.Instance);
                break;
            case (byte) CommandClass.SwitchMultilevel:
                nestedEvent = new NodeEvent(nodeEvent.Node, EventParameter.MultiinstanceSwitchMultilevel, nodeEvent.Value, nodeEvent.Instance);
                break;
            case (byte) CommandClass.SensorBinary:
                nestedEvent = new NodeEvent(nodeEvent.Node, EventParameter.MultiinstanceSensorBinary, nodeEvent.Value, nodeEvent.Instance);
                break;
            case (byte) CommandClass.SensorMultilevel:
                nestedEvent = new NodeEvent(nodeEvent.Node, EventParameter.MultiinstanceSensorMultilevel, nodeEvent.Value, nodeEvent.Instance);
                break;
            }
            return nestedEvent;
        }
    }
}
