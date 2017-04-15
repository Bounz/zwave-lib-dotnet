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
    /// The Thermostat Mode Command Class is used to control a thermostat. These Commands allow
    /// applications to set and get the thermostat parameters. Version 2 extends the available number of modes.
    /// </summary>
    /// <remarks>SDS12652 3.39 Thermostat Mode Command Class, version 1-2</remarks>
    public class ThermostatMode : ICommandClass
    {
        [Obsolete("Use Mode enum instead")]
        public enum Value
        {
            Off = 0x00,
            Heat = 0x01,
            Cool = 0x02,
            Auto = 0x03,
            AuxHeat = 0x04,
            Resume = 0x05,
            FanOnly = 0x06,
            Furnace = 0x07,
            DryAir = 0x08,
            MoistAir = 0x09,
            AutoChangeover = 0x0A,
            HeatEconomy = 0x0B,
            CoolEconomy = 0x0C,
            Away = 0x0D
        }

        public enum Mode
        {
            Off = 0x00,
            Heat = 0x01,
            Cool = 0x02,
            Auto = 0x03,
            AuxHeat = 0x04,
            Resume = 0x05,
            FanOnly = 0x06,
            Furnace = 0x07,
            DryAir = 0x08,
            MoistAir = 0x09,
            AutoChangeover = 0x0A,
            HeatEconomy = 0x0B,
            CoolEconomy = 0x0C,
            Away = 0x0D
        }

        public CommandClass GetClassId()
        {
            return CommandClass.ThermostatMode;
        }

        // SDS12652 3.39.3 Thermostat Mode Report Command
        // SDS12652 3.39.5 Thermostat Mode Supported Report Command
        public NodeEvent GetEvent(IZWaveNode node, byte[] message)
        {
            NodeEvent nodeEvent = null;
            var cmdType = message[1];
            switch (cmdType)
            {
                case Command.Thermostat.ModeReport:
                    var value = (Mode)message[2];
                    nodeEvent = new NodeEvent(node, EventParameter.ThermostatMode, value, 0);
                    break;

                case Command.Thermostat.ModeSupportedReport:
                    break;

                default:
                    throw new UnsupportedCommandException(cmdType);
            }

            return nodeEvent;
        }

        /// <summary>
        /// The Thermostat Mode Set Command is used to set the wanted mode in the device
        /// </summary>
        /// <param name="node"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        /// <remarks>SDS12652 3.39.1 Thermostat Mode Set Command</remarks>
        public static ZWaveMessage Set(IZWaveNode node, Mode mode)
        {
            return node.SendDataRequest(new[]
            {
                (byte) CommandClass.ThermostatMode,
                Command.Thermostat.ModeSet,
                (byte) mode
            });
        }

        /// <summary>
        /// The Thermostat Mode Get Command is used to request the current mode from the device
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        /// <remarks>SDS12652 3.39.2 Thermostat Mode Get Command</remarks>
        public static ZWaveMessage Get(IZWaveNode node)
        {
            return node.SendDataRequest(new[]
            {
                (byte) CommandClass.ThermostatMode,
                Command.Basic.Get
            });
        }

        /// <summary>
        /// The Thermostat Mode Supported Get Command is used to request the supported modes
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        /// <remarks>SDS12652 3.39.4 Thermostat Mode Supported Get Command</remarks>
        public static ZWaveMessage SupportedGet(IZWaveNode node)
        {
            throw new NotImplementedException();
        }

        [Obsolete("Use Set(IZWaveNode node, Mode mode) method instead")]
        public static ZWaveMessage Set(ZWaveNode node, Value mode)
        {
            return node.SendDataRequest(new[] { 
                (byte)CommandClass.ThermostatMode, 
                Command.Basic.Set, 
                (byte)mode
            });
        }
    }
}
