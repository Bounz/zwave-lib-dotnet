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
    /// The Thermostat Operating State Command Class is used to obtain the operating state of the thermostat.
    /// </summary>
    /// <remarks>SDS12652 3.41 Thermostat Operating State Command Class, version 1</remarks>
    public class ThermostatOperatingState : ICommandClass
    {
        [Obsolete("Use OperatingState enum")]
        public enum Value
        {
            Idle = 0x00,
            Heating = 0x01,
            Cooling = 0x02,
            FanOnly = 0x03,
            PendingHeat = 0x04,
            PendingCool = 0x05,
            VentEconomizer = 0x06,
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

        public enum OperatingState
        {
            Idle = 0x00,
            Heating = 0x01,
            Cooling = 0x02,
            FanOnly = 0x03,
            PendingHeat = 0x04,
            PendingCool = 0x05,
            VentEconomizer = 0x06,
            AuxHeating = 0x07,
            SecondStageHeating = 0x08,
            SecondStageCooling = 0x09,
            SecondStageAuxHeat = 0x0A,
            ThirdStageAuxHeatg = 0x0B
        }

        public CommandClass GetClassId()
        {
            return CommandClass.ThermostatOperatingState;
        }

        // SDS12652 3.41.2 Thermostat Operating State Report Command
        // SDS12652 3.42.2 Thermostat Operating State Report Command V2
        // SDS12652 3.42.4 Thermostat Operating State Logging Supported Report - not implemented yet
        // SDS12652 3.42.6 Thermostat Operating State Logging Report - not implemented yet
        public NodeEvent GetEvent(IZWaveNode node, byte[] message)
        {
            NodeEvent nodeEvent = null;
            var cmdType = message[1];
            switch (cmdType)
            {
                case Command.Thermostat.OperatingStateReport:
                    var value = (OperatingState) message[2];
                    nodeEvent = new NodeEvent(node, EventParameter.ThermostatOperatingState, value, 0);
                    break;

                case Command.Thermostat.OperatingLoggingSupportedReport:
                case Command.Thermostat.OperatingStateLoggingReport:
                    break;

                default:
                    throw new UnsupportedCommandException(cmdType);
            }

            return nodeEvent;
        }

        /// <summary>
        /// The Thermostat Operating State Get Command is used to request the operating state.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        /// <remarks>SDS12652 3.41.1 Thermostat Operating State Get Command</remarks>
        public static ZWaveMessage Get(IZWaveNode node)
        {
            return node.SendDataRequest(new[] { 
                (byte)CommandClass.ThermostatOperatingState, 
                Command.Basic.Get
            });
        }

        [Obsolete("Use Get(IZWaveNode node) method instead")]
        public static ZWaveMessage GetOperatingState(IZWaveNode node)
        {
            return node.SendDataRequest(new[] {
                (byte)CommandClass.ThermostatOperatingState,
                Command.Basic.Get
            });
        }

        /// <summary>
        /// The Thermostat Operating State Logging Supported Get Command is used to request the operating
        /// state logging supported by the device.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        /// <remarks>SDS12652 3.42.3 Thermostat Operating State Logging Supported Get</remarks>
        public static ZWaveMessage LoggingSupportedGet(IZWaveNode node)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The Thermostat Operating State Logging Get Command is used to request the operating state logging
        /// supported by the device.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        /// <remarks>SDS12652 3.42.5 Thermostat Operating State Logging Get</remarks>
        public static ZWaveMessage LoggingGet(IZWaveNode node)
        {
            throw new NotImplementedException();
        }
    }
}
