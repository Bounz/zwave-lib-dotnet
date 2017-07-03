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
using ZWaveLib.Attributes;
using ZWaveLib.Enums;
using ZWaveLib.Utilities;

namespace ZWaveLib.CommandClasses.MultiChannel
{
    /// <summary>
    /// The Multi Channel command class is used to address one or more end points in a Multi Channel device.
    /// </summary>
    /// <remarks>SD13783-2 3.3 Multi Channel Command Class, version 4</remarks>
    public class MultiChannel : ICommandClass
    {
        public CommandClass GetClassId()
        {
            return CommandClass.MultiChannel;
        }

        public NodeEvent GetEvent(IZWaveNode node, byte[] message)
        {
            NodeEvent nodeEvent;
            var cmdType = message[1];
            switch (cmdType)
            {
                case Command.MultiChannel.EndPointReport:
                    var endPointReport = MultiChannelEndPointReport.Parse(message);
                    nodeEvent = new NodeEvent(node, EventParameter.MultiChannelEndPointReport, endPointReport, 0);
                    break;

                case Command.MultiChannel.CapabilityReport:
                    var capabilityReport = MultiChannelCapabilityReport.Parse(message);
                    nodeEvent = new NodeEvent(node, EventParameter.MultiChannelCapabilityReport, capabilityReport, 0);
                    break;

                case Command.MultiChannel.AggregatedMembersReport:
                    var aggregatedMembersReport = MultiChannelAggregatedMembersReport.Parse(message);
                    nodeEvent = new NodeEvent(node, EventParameter.MultiChannelAggregatedMembersReport, aggregatedMembersReport, 0);
                    break;



                case Command.MultiChannel.MultiInstanceCmdEncap:
                    nodeEvent = HandleMultiInstanceEncapReport(node, message);
                    break;

                case Command.MultiChannel.CmdEncap:
                    nodeEvent = HandleMultiChannelEncapReport(node, message);
                    break;

                //case Command.MultiChannel.MultiInstanceReport:
                //    var instanceCount = message[3];
                //    switch (instanceCmdClass)
                //    {
                //        case (byte) CommandClass.SwitchBinary:
                //            nodeEvent = new NodeEvent(node, EventParameter.MultiinstanceSwitchBinaryCount, instanceCount, 0);
                //            break;
                //        case (byte) CommandClass.SwitchMultilevel:
                //            nodeEvent = new NodeEvent(node, EventParameter.MultiinstanceSwitchMultilevelCount, instanceCount, 0);
                //            break;
                //        case (byte) CommandClass.SensorBinary:
                //            nodeEvent = new NodeEvent(node, EventParameter.MultiinstanceSensorBinaryCount, instanceCount, 0);
                //            break;
                //        case (byte) CommandClass.SensorMultilevel:
                //            nodeEvent = new NodeEvent(node, EventParameter.MultiinstanceSensorMultilevelCount, instanceCount, 0);
                //            break;
                //    }
                //    break;
                default:
                    throw new UnsupportedCommandException(cmdType);
            }

            return nodeEvent;
        }

        /// <summary>
        /// The Multi Channel End Point Get Command is used to query the number of Multi Channel End Points and other relevant Multi Channel attributes.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        /// <remarks>SD13783-2 3.3.5 Multi Channel End Point Get Command</remarks>
        public static ZWaveMessage EndPointGet(IZWaveNode node)
        {
            return node.SendDataRequest(new[]
            {
                (byte) CommandClass.MultiChannel,
                Command.MultiChannel.EndPointGet
            });
        }

        /// <summary>
        /// The Multi Channel Capability Get Command is used to query the capabilities of one individual End Point or Aggregated End Point.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="endPoint">This field MUST specify a valid End Point as advertised by the Multi Channel End Point Report.</param>
        /// <returns>SD13783-2 3.3.7 Multi Channel Capability Get Command</returns>
        public static ZWaveMessage CapabilityGet(IZWaveNode node, byte endPoint)
        {
            return node.SendDataRequest(new[]
            {
                (byte) CommandClass.MultiChannel,
                Command.MultiChannel.CapabilityGet,
                endPoint
            });
        }

        /// <summary>
        /// The Multi Channel End Point Find Command is used to query individual End Points as well as Aggregated End Points for a specified set 
        /// of generic and specific Device Class identifiers.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="genericDeviceClass">
        /// This field MUST specify the generic Device Class which an End Point has to match.
        /// The value 0xFF MAY be specified for the Generic Device Class field to indicate that all existing End Points are to be reported. If <c>0xFF</c> is specified, the Specific Device Class field MUST also be set to <c>0xFF</c>.
        /// </param>
        /// <param name="specificDeviceClass">This field MUST specify the specific Device Class which an End Point has to match.
        /// The value 0xFF MAY be specified. If specified, a responding device MUST treat the value as a wild-card; advertising all End Points which match the advertised Generic Device Class, regardless of the Specific Device Class of the individual End Point. If specified, the value <c>0xFF</c> MUST also be advertised in the Multi Channel End Point Find Report Command.</param>
        /// <returns>SD13783-2 3.3.9 Multi Channel End Point Find Command</returns>
        public static ZWaveMessage EndPointFind(IZWaveNode node, byte genericDeviceClass, byte specificDeviceClass)
        {
            return node.SendDataRequest(new[]
            {
                (byte) CommandClass.MultiChannel,
                Command.MultiChannel.EndPointFind,
                genericDeviceClass,
                specificDeviceClass
            });
        }

        /// <summary>
        /// The Multi Channel Aggregated Members Get Command is used to query the members of an Aggregated End Point.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="aggregatedEndPoint"></param>
        /// <returns>SD13783-2 3.3.11 Multi Channel Aggregated Members Get Command</returns>
        [CommandClassVersion(4)]
        public static ZWaveMessage AggregatedMembersGet(IZWaveNode node, byte aggregatedEndPoint)
        {
            return node.SendDataRequest(new[]
            {
                (byte) CommandClass.MultiChannel,
                Command.MultiChannel.AggregatedMembersGet,
                aggregatedEndPoint
            });
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
