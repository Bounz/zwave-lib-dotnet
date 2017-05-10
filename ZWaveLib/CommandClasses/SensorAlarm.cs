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
using ZWaveLib.Values;

namespace ZWaveLib.CommandClasses
{
    /// <summary>
    /// This command is used to request the status of a sensor.
    /// </summary>
    /// <remarks>SDS13781 4.3 Alarm Sensor Command Class, version 1 [DEPRECATED]</remarks>
    public class SensorAlarm : ICommandClass
    {
        public CommandClass GetClassId()
        {
            return CommandClass.SensorAlarm;
        }

        // SDS13781 4.3.2 Alarm Sensor Report Command
        // SDS13781 4.3.4 Alarm Sensor Supported Report Command - not implemented
        public NodeEvent GetEvent(IZWaveNode node, byte[] message)
        {
            NodeEvent nodeEvent;
            var cmdType = message[1];
            switch (cmdType)
            {
                case Command.SensorAlarm.Report:
                {
                    var alarm = AlarmValue.Parse(message);
                    nodeEvent = new NodeEvent(node, alarm.EventType, alarm.Value, 0);
                    break;
                }

                case Command.SensorAlarm.SupportedReport:
                    throw new NotImplementedException();

                default:
                    throw new UnsupportedCommandException(cmdType);
            }

            return nodeEvent;
        }

        /// <summary>
        /// This command is used to request the status of a sensor.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="sensorType"></param>
        /// <returns></returns>
        /// <remarks>SDS13781 4.3.1 Alarm Sensor Get Command</remarks>
        public static ZWaveMessage Get(IZWaveNode node, ZWaveAlarmType sensorType)
        {
            return node.SendDataRequest(new[]
            {
                (byte) CommandClass.SensorAlarm,
                Command.SensorAlarm.Get,
                (byte) sensorType
            });
        }

        /// <summary>
        /// This command is used to request the supported sensor types from the device.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        /// <remarks>SDS13781 4.3.3 Alarm Sensor Supported Get Command</remarks>
        public static ZWaveMessage SupportedGet(IZWaveNode node)
        {
            return node.SendDataRequest(new[]
            {
                (byte) CommandClass.SensorAlarm,
                Command.SensorAlarm.SupportedGet
            });
        }
    }
}


