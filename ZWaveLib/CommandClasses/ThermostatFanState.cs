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
    /// The Thermostat Fan State Command Class is used to obtain the fan operating state of the thermostat.
    /// </summary>
    /// <remarks>SDS12652 3.38 Thermostat Fan State Command Class, version 1-2</remarks>
    public class ThermostatFanState :ICommandClass
    {
        [Obsolete("Use FanState enum instead")]
        public enum Value
        {
            Idle = 0x00,
            Running = 0x01,
            RunningHigh = 0x02,
            State03 = 0x03,
            State04 = 0x04,
            State05 = 0x05,
            State06 = 0x06,
            State07 = 0x07,
            State08 = 0x08,
            State09 = 0x09,
            State10 = 0x0A,
            State11 = 0x0B,
            State12 = 0x0C,
            State13 = 0x0D,
            State14 = 0x0E,
            State15 = 0x0F
        }

        public enum FanState
        {
            Idle = 0x00,
            Running = 0x01,
            RunningHigh = 0x02,
            RunningMedium = 0x03,
            Circulation = 0x04,
            HumidityCirculation = 0x05,
            RightLeftCirculation = 0x06,
            UpDownCirculation = 0x07,
            QuietCirculation = 0x08
        }

        public CommandClass GetClassId()
        {
            return CommandClass.ThermostatFanState;
        }

        // SDS12652 3.38.3 Thermostat Fan State Report Command
        public NodeEvent GetEvent(IZWaveNode node, byte[] message)
        {
            NodeEvent nodeEvent;
            var cmdType = message[1];
            switch (cmdType)
            {
                case Command.Thermostat.FanStateReport:
                    var value = (FanState) message[2];
                    nodeEvent = new NodeEvent(node, EventParameter.ThermostatFanState, value, 0);
                    break;

                default:
                    throw new UnsupportedCommandException(cmdType);
            }

            return nodeEvent;
        }

        /// <summary>
        /// The Thermostat Fan State Get Command is used to request the fan operating state from the device.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        /// <remarks>SDS12652 3.38.2 Thermostat Fan State Get Command</remarks>
        public static ZWaveMessage Get(IZWaveNode node)
        {
            return node.SendDataRequest(new[]
            {
                (byte) CommandClass.ThermostatFanState,
                Command.Thermostat.FanStateGet
            });
        }
    }
}
