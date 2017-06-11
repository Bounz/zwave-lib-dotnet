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
 *             Ben Voss
 *     Project Homepage: https://github.com/genielabs/zwave-lib-dotnet
 */

using System.Collections.Generic;
using ZWaveLib.Enums;
using ZWaveLib.Utilities;

namespace ZWaveLib.CommandClasses
{
    /// <summary>
    /// The Climate Control Schedule Command Class allows devices to exchange schedules and overrides, which specify when to perform a setback on the setpoint.
    /// </summary>
    /// <remarks>SDS13781-2 4.14 Climate Control Schedule Command Class, version 1 [DEPRECATED]</remarks>
    public class ClimateControlSchedule : ICommandClass
    {
        public CommandClass GetClassId()
        {
            return CommandClass.ClimateControlSchedule;
        }

        public NodeEvent GetEvent(IZWaveNode node, byte[] message)
        {
            if (message.Length == 0)
                return null;

            NodeEvent nodeEvent = null;
            var cmdType = message[1];
            switch (cmdType)
            {
                case Command.Schedule.Report:
                    var climateControlScheduleValue = ClimateControlScheduleValue.Parse(message);
                    nodeEvent = new NodeEvent(node, EventParameter.ClimateControlSchedule, climateControlScheduleValue, 0);
                    break;

                case Command.Schedule.ChangedReport:
                    nodeEvent = new NodeEvent(node, EventParameter.ClimateControlScheduleChanged, message[2], 0);
                    break;

                case Command.Schedule.OverrideReport:
                    var climateControlScheduleOverrideValue = ClimateControlScheduleOverrideValue.Parse(message);
                    nodeEvent = new NodeEvent(node, EventParameter.ClimateControlScheduleOverride, climateControlScheduleOverrideValue, 0);
                    break;

                default:
                    throw new UnsupportedCommandException(cmdType);
            }

            return nodeEvent;
        }

        /// <summary>
        /// This command is used to set the climate control schedule in a device for a specific weekday.
        /// A climate control schedule defines when to use a setback on the setpoint in a device.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <remarks>SDS13781-2 4.14.1 Schedule Set Command</remarks>
        public static ZWaveMessage Set(IZWaveNode node, ClimateControlScheduleValue value)
        {
            var message = new List<byte>();
            message.AddRange(new[] {
                (byte)CommandClass.ClimateControlSchedule,
                Command.Schedule.Set
            });
            message.AddRange(value.GetValueBytes());

            return node.SendDataRequest(message.ToArray());
        }

        /// <summary>
        /// This command is used to request the climate control schedule in a device for a specific weekday.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="weekday"></param>
        /// <returns>SDS13781-2 4.14.2 Schedule Get Command</returns>
        public static ZWaveMessage Get(IZWaveNode node, Weekday weekday)
        {
            return node.SendDataRequest(new[] {
                (byte)CommandClass.ClimateControlSchedule,
                Command.Schedule.Get,
                (byte)weekday
            });
        }

        /// <summary>
        /// This command is used to check if the climate control schedule has changed.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        /// <remarks>SDS13781-2 4.14.4 Schedule Changed Get Command</remarks>
        public static ZWaveMessage ChangedGet(IZWaveNode node)
        {
            return node.SendDataRequest(new[] {
                (byte)CommandClass.ClimateControlSchedule,
                Command.Schedule.ChangedGet
            });
        }

        /// <summary>
        /// This command is used to set the override in a device.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="scheduleOverride"></param>
        /// <returns></returns>
        /// <remarks>SDS13781-2 4.14.6 Schedule Override Set Command</remarks>
        public static ZWaveMessage OverrideSet(IZWaveNode node, ClimateControlScheduleOverrideValue scheduleOverride)
        {
            var message = new List<byte>();
            message.AddRange(new[] {
                (byte)CommandClass.ClimateControlSchedule,
                Command.Schedule.OverrideSet,
            });
            message.AddRange(scheduleOverride.GetValueBytes());

            return node.SendDataRequest(message.ToArray());
        }

        /// <summary>
        /// This command is used to request the override, currently in use in a device.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        /// <remarks>SDS13781-2 4.14.7 Schedule Override Get Command</remarks>
        public static ZWaveMessage OverrideGet(IZWaveNode node)
        {
            return node.SendDataRequest(new[] {
                (byte)CommandClass.ClimateControlSchedule,
                Command.Schedule.OverrideGet
            });
        }
    }
}
