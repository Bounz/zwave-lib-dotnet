using System;
using System.Collections.Generic;
using ZWaveLib.Enums;
using ZWaveLib.Utilities;

namespace ZWaveLib.CommandClasses.Irrigation
{
    /// <summary>
    /// SDS13740-1 Z-Wave Plus Device and Command Class Types and Defines Specification 2016-08-26
    /// Sigma Designs Inc.Types and Defines Page 126 of 460
    /// </summary>
    public class Irrigation : ICommandClass
    {
        public CommandClass GetClassId()
        {
            return CommandClass.Irrigation;
        }

        public NodeEvent GetEvent(IZWaveNode node, byte[] message)
        {
            NodeEvent nodeEvent = null;
            var cmdType = message[1];
            switch (cmdType)
            {
                case Command.Irrigation.SystemInfoReport:
                    var value = new IrrigationSystemInfoReport
                    {
                        IsMasterValueSupported = message[2] == 1,
                        TotalNumberOfValves = message[3],
                        TotalNumberOfValveTables = message[4],
                        ValveTableMaxSize = message[5]
                    };
                    nodeEvent = new NodeEvent(node, EventParameter.IrrigationSystemInfoReport, value, 0);
                    break;

                case Command.Irrigation.SystemStatusReport:
                    var systemStatus = IrrigationSystemStatusReport.Parse(message);
                    nodeEvent = new NodeEvent(node, EventParameter.IrrigationSystemStatusReport, systemStatus, 0);
                    break;

                case Command.Irrigation.SystemConfigReport:
                    var systemConfig = IrrigationSystemConfig.Parse(message);
                    nodeEvent = new NodeEvent(node, EventParameter.IrrigationSystemConfigReport, systemConfig, 0);
                    break;

                case Command.Irrigation.ValveInfoReport:
                    var valveInfo = IrrigationValveInfo.Parse(message);
                    nodeEvent = new NodeEvent(node, EventParameter.IrrigationValveInfoReport, valveInfo, 0);
                    break;

                case Command.Irrigation.ValveConfigReport:
                    var valveConfig = IrrigationValveConfig.Parse(message);
                    nodeEvent = new NodeEvent(node, EventParameter.IrrigationValveConfigReport, valveConfig, 0);
                    break;

                case Command.Irrigation.ValveTableReport:
                    var valveTable = IrrigationValveTable.Parse(message);
                    nodeEvent = new NodeEvent(node, EventParameter.IrrigationValveTableReport, valveTable, 0);
                    break;

                default:
                    throw new UnsupportedCommandException(cmdType);
            }
            return nodeEvent;
        }

        /// <summary>
        /// This command is used to request a receiving node about its irrigation system information.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        /// <remarks>SDS13781-2 4.42.4 Irrigation System Info Get Command</remarks>
        public static ZWaveMessage SystemInfoGet(IZWaveNode node)
        {
            return node.SendDataRequest(new[]
            {
                (byte) CommandClass.Irrigation,
                Command.Irrigation.SystemInfoGet
            });
        }

        /// <summary>
        /// This command is used to request a receiving node about its irrigation system status.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        /// <remarks>SDS13781-2 4.42.6 Irrigation System Status Get Command</remarks>
        public static ZWaveMessage SystemStatusGet(IZWaveNode node)
        {
            return node.SendDataRequest(new[]
            {
                (byte) CommandClass.Irrigation,
                Command.Irrigation.SystemStatusGet
            });
        }

        /// <summary>
        /// This command allows the irrigation system to be configured accordingly.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="config"></param>
        /// <returns>SDS13781 4.42.8 Irrigation System Config Set Command</returns>
        public static ZWaveMessage SystemConfigSet(IZWaveNode node, IrrigationSystemConfig config)
        {
            var commandBytes = new List<byte>
            {
                (byte) CommandClass.Irrigation,
                Command.Irrigation.SystemConfigSet
            };
            commandBytes.AddRange(config.ToByteArray());
            return node.SendDataRequest(commandBytes.ToArray());
        }

        /// <summary>
        /// This command is used to request a receiving node about its current irrigation system configuration.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        /// <remarks>SDS13781 4.42.9 Irrigation System Config Get Command</remarks>
        public static ZWaveMessage SystemConfigGet(IZWaveNode node)
        {
            return node.SendDataRequest(new[]
            {
                (byte) CommandClass.Irrigation,
                Command.Irrigation.SystemConfigGet
            });
        }

        /// <summary>
        /// This command is used to request general information about the specified valve.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="valveId">This field is used to indicate the Valve ID if the sending node requests the information about a zone valve.</param>
        /// <param name="useMasterValve">This field is used to indicate whether the sending node requests the information of the master valve or of a zone valve.</param>
        /// <returns></returns>
        /// <remarks>SDS13781 4.42.11 Irrigation Valve Info Get Command</remarks>
        public static ZWaveMessage ValveInfoGet(IZWaveNode node, byte valveId, bool useMasterValve = false)
        {
            var masterValveByte = Convert.ToByte(useMasterValve);
            if (useMasterValve)
                valveId = 1;

            return node.SendDataRequest(new[]
            {
                (byte) CommandClass.Irrigation,
                Command.Irrigation.ValveInfoGet,
                masterValveByte,
                valveId
            });
        }

        /// <summary>
        /// This command allows an irrigation valve to be configured accordingly.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        /// <remarks>SDS13781 4.42.13 Irrigation Valve Config Set Command</remarks>
        public static ZWaveMessage ValveConfigSet(IZWaveNode node, IrrigationValveConfig config)
        {
            var commandBytes = new List<byte>
            {
                (byte) CommandClass.Irrigation,
                Command.Irrigation.ValveConfigSet
            };
            commandBytes.AddRange(config.ToByteArray());

            return node.SendDataRequest(commandBytes.ToArray());
        }

        /// <summary>
        /// This command is used to request the current configuration of an irrigation valve.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="valveId"></param>
        /// <param name="useMasterValve"></param>
        /// <returns></returns>
        /// <remarks>SDS13781 4.42.14 Irrigation Valve Config Get Command</remarks>
        public static ZWaveMessage ValveConfigGet(IZWaveNode node, byte valveId, bool useMasterValve)
        {
            var masterValveByte = Convert.ToByte(useMasterValve);
            if (useMasterValve)
                valveId = 1;

            return node.SendDataRequest(new[]
            {
                (byte) CommandClass.Irrigation,
                Command.Irrigation.ValveConfigGet,
                masterValveByte,
                valveId
            });
        }

        /// <summary>
        /// The Irrigation Valve Run Command will run the specified valve for a specified duration.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="valveId">This field is used to specify the actual Valve ID.</param>
        /// <param name="useMasterValve">This field is used to indicate if the specified valve is the master valve or a zone valve.</param>
        /// <param name="duration">This field is used to specify the duration of the run in seconds.</param>
        /// <returns></returns>
        /// <remarks>SDS13781 4.42.16 Irrigation Valve Run Command</remarks>
        public static ZWaveMessage ValveRun(IZWaveNode node, byte valveId, bool useMasterValve, ushort duration)
        {
            var masterValveByte = Convert.ToByte(useMasterValve);
            if (useMasterValve)
                valveId = 1;
            var valueBytes = duration.ToBigEndianBytes();

            return node.SendDataRequest(new[]
            {
                (byte) CommandClass.Irrigation,
                Command.Irrigation.ValveRun,
                masterValveByte,
                valveId,
                valueBytes[0],
                valueBytes[1]
            });
        }

        /// <summary>
        /// This command is used to set a valve table with a list of valves and durations.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="valveTable"></param>
        /// <returns></returns>
        /// <remarks>SDS13781 4.42.17 Irrigation Valve Table Set Command</remarks>
        public static ZWaveMessage ValveTableSet(IZWaveNode node, IrrigationValveTable valveTable)
        {
            var commandBytes = new List<byte>
            {
                (byte) CommandClass.Irrigation,
                Command.Irrigation.ValveTableSet
            };
            commandBytes.AddRange(valveTable.ToByteArray());
            return node.SendDataRequest(commandBytes.ToArray());
        }

        /// <summary>
        /// This command is used to request the contents of the specified Valve Table ID.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="valveTableId">
        /// This field is used to specify the valve table ID.
        /// Valves tables MUST be identified sequentially from 1 to the total number available on the device.
        /// </param>
        /// <returns></returns>
        /// <remarks>SDS13781 4.42.17 Irrigation Valve Table Set Command</remarks>
        public static ZWaveMessage ValveTableGet(IZWaveNode node, byte valveTableId)
        {
            return node.SendDataRequest(new[]
            {
                (byte) CommandClass.Irrigation,
                Command.Irrigation.ValveTableGet,
                valveTableId
            });
        }

        /// <summary>
        /// This command is used to run the specified valve tables sequentially.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="valveTableIds">List of Valve Tables to run sequentially.</param>
        /// <returns></returns>
        /// <remarks>SDS13781 4.42.20 Irrigation Valve Table Run Command</remarks>
        public static ZWaveMessage ValveTableRun(IZWaveNode node, byte[] valveTableIds)
        {
            var commandBytes = new List<byte>
            {
                (byte) CommandClass.Irrigation,
                Command.Irrigation.ValveTableRun
            };
            commandBytes.AddRange(valveTableIds);
            return node.SendDataRequest(commandBytes.ToArray());
        }

        /// <summary>
        /// This command is used to prevent any irrigation activity triggered by the Schedule CC for a specified duration.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="duration">
        /// Duration of the system shutoff.
        /// Values in the range 1..254 indicate how many hours the irrigation system must stay shut off after reception of this command.
        /// The value 0 indicates to turn off any running valve (including the master valve) as well as cancel any active Irrigation Valve Table Run or Schedule.
        /// The value 255 indicates that the irrigation system MUST stay permanently shut off until the node receives special commands.
        /// </param>
        /// <returns></returns>
        /// <remarks>SDS13781 4.42.21 Irrigation System Shutoff Command</remarks>
        public static ZWaveMessage SystemShutoff(IZWaveNode node, byte duration)
        {
            return node.SendDataRequest(new[]
            {
                (byte) CommandClass.Irrigation,
                Command.Irrigation.SystemShutoff,
                duration
            });
        }
    }
}
