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
using ZWaveLib.Enums;
using ZWaveLib.Utilities;

namespace ZWaveLib.CommandClasses
{
    /// <summary> 
    /// The Binary Sensor Command Class is used to realize binary sensors, such as movement sensors and door/window sensors.
    /// </summary>
    /// <remarks>SDS13781 4.14 Binary Sensor Command Class, version 2 [DEPRECATED]</remarks>
    public class SensorBinary : ICommandClass
    {
        public CommandClass GetClassId()
        {
            return CommandClass.SensorBinary;
        }

        // SDS13781 4.14.2 Binary Sensor Report Command
        // SDS13781 4.14.4 Binary Sensor Supported Sensor Report Command
        public NodeEvent GetEvent(IZWaveNode node, byte[] message)
        {
            NodeEvent nodeEvent;
            var cmdType = message[1];
            switch (cmdType)
            {
                case Command.SensorBinary.Report:
                {
                    var cc = node.GetCommandClass(GetClassId());
                    var version = (cc != null ? cc.Version : 0);
                    var value = message[2];

                    if (version == 1 || message.Length <= 3)
                    {
                        nodeEvent = new NodeEvent(node, EventParameter.SensorGeneric, value, 0);
                    }
                    else
                    {
                        byte tmp = message[3];
                        var sensorType = ZWaveSensorBinaryType.General;
                        EventParameter eventType;

                        if (Enum.IsDefined(typeof(ZWaveSensorBinaryType), tmp))
                        {
                            sensorType = (ZWaveSensorBinaryType)tmp;
                        }

                        switch (sensorType)
                        {
                            case ZWaveSensorBinaryType.Smoke:
                                eventType = EventParameter.AlarmSmoke;
                                break;
                            case ZWaveSensorBinaryType.CarbonMonoxide:
                                eventType = EventParameter.AlarmCarbonMonoxide;
                                break;
                            case ZWaveSensorBinaryType.CarbonDioxide:
                                eventType = EventParameter.AlarmCarbonDioxide;
                                break;
                            case ZWaveSensorBinaryType.Heat:
                                eventType = EventParameter.AlarmHeat;
                                break;
                            case ZWaveSensorBinaryType.Water:
                                eventType = EventParameter.AlarmFlood;
                                break;
                            case ZWaveSensorBinaryType.Tamper:
                                eventType = EventParameter.AlarmTampered;
                                break;
                            case ZWaveSensorBinaryType.DoorWindow:
                                eventType = EventParameter.AlarmDoorWindow;
                                break;
                            case ZWaveSensorBinaryType.Motion:
                                eventType = EventParameter.SensorMotion;
                                break;
                            case ZWaveSensorBinaryType.Freeze:
                            case ZWaveSensorBinaryType.Auxiliary:
                            case ZWaveSensorBinaryType.Tilt:
                            case ZWaveSensorBinaryType.General:
                            case ZWaveSensorBinaryType.GlassBreak:
                            default:
                                // Catch-all for the undefined types above.
                                eventType = EventParameter.SensorGeneric;
                                break;
                        }

                        nodeEvent = new NodeEvent(node, eventType, value, 0);
                    }
                    break;
                }

                case Command.SensorBinary.SupportedReport:
                    throw new NotImplementedException();

                default:
                    throw new UnsupportedCommandException(cmdType);
            }

            return nodeEvent;
        }

        /// <summary>
        /// This command is used to request the status of the specific sensor device.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        /// <remarks>SDS13781 4.14.1 Binary Sensor Get Command</remarks>
        public static ZWaveMessage Get(IZWaveNode node)
        {
            return node.SendDataRequest(new[]
            {
                (byte) CommandClass.SensorBinary,
                Command.SensorBinary.Get
            });
        }

        /// <summary>
        /// This command is used to request the supported sensor types from the binary sensor device.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        /// <remarks>SDS13781 4.14.3 Binary Sensor Get Supported Sensor Command</remarks>
        public static ZWaveMessage SupportedGet(IZWaveNode node)
        {
            return node.SendDataRequest(new[]
            {
                (byte) CommandClass.SensorBinary,
                Command.SensorBinary.SupportedGet
            });
        }
    }
}
