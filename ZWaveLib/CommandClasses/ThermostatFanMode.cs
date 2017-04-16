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
    /// The Thermostat Fan Mode Command Class, version 1 used for the HVAC’s systems manual fan.
    /// </summary>
    /// <remarks>SDS12652 3.34 Thermostat Fan Mode Command Class, version 1</remarks>
    /// <remarks>SDS12652 3.35 Thermostat Fan Mode Command Class, Version 2</remarks>
    /// <remarks>SDS12652 3.36 Thermostat Fan Mode Command Class, Version 3</remarks>
    /// <remarks>SDS12652 3.37 Thermostat Fan Mode Command Class, Version 4</remarks>
    public class ThermostatFanMode : ICommandClass
    {
        [Obsolete("Use FanMode enum instead")]
        public enum Value
        {
            AutoLow = 0x00,
            OnLow = 0x01,
            AutoHigh = 0x02,
            OnHigh = 0x03,
            Unknown4 = 0x04,
            Unknown5 = 0x05,
            Circulate = 0x06
        }

        public enum FanMode
        {
            AutoLow = 0x00,
            Low = 0x01,
            AutoHigh = 0x02,
            High = 0x03,
            AutoMedium = 0x04,
            Medium = 0x05,
            Circulation = 0x06,
            Humidity = 0x07,
            LeftRight = 0x08,
            UpDown = 0x09,
            Quiet = 0x0A,
        }

        public CommandClass GetClassId()
        {
            return CommandClass.ThermostatFanMode;
        }

        // 3.37.3 Thermostat Fan Mode Report Command
        // 3.37.5 Thermostat Fan Mode Supported Report Command
        public NodeEvent GetEvent(IZWaveNode node, byte[] message)
        {
            NodeEvent nodeEvent = null;
            var cmdType = message[1];
            switch (cmdType)
            {
                case Command.Thermostat.FanModeReport:
                    var value = (FanMode) message[2];
                    nodeEvent = new NodeEvent(node, EventParameter.ThermostatFanMode, value, 0);
                    break;

                case Command.Thermostat.FanModeSupportedReport:
                    break;

                default:
                    throw new UnsupportedCommandException(cmdType);
            }

            return nodeEvent;
        }

        /// <summary>
        /// The Thermostat Fan Mode Set Command is used to set the fan mode in the device
        /// </summary>
        /// <param name="node"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        /// <remarks>SDS12652 3.37.1 Thermostat Fan Mode Set Command</remarks>
        public static ZWaveMessage Set(IZWaveNode node, FanMode mode)
        {
            return node.SendDataRequest(new[]
            {
                (byte) CommandClass.ThermostatFanMode,
                Command.Thermostat.FanModeSet,
                (byte) mode
            });
        }

        /// <summary>
        /// The Thermostat Fan Mode Get Command is used to request the fan mode in the device.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        /// <remarks>SDS12652 3.37.2 Thermostat Fan Mode Get Command</remarks>
        public static ZWaveMessage Get(IZWaveNode node)
        {
            return node.SendDataRequest(new[]
            {
                (byte) CommandClass.ThermostatFanMode,
                Command.Thermostat.FanModeGet
            });
        }

        /// <summary>
        /// The Thermostat Fan Mode Supported Get Command is used to request the supported modes from the device.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        /// <remarks>SDS12652 3.37.4 Thermostat Fan Mode Supported Get Command</remarks>
        public static ZWaveMessage SupportedGet(IZWaveNode node)
        {
            throw new NotImplementedException();
        }

        [Obsolete("Use Set(ZWaveNode node, FanMode mode) method instead")]
        public static ZWaveMessage Set(IZWaveNode node, Value mode)
        {
            return node.SendDataRequest(new[] {
                (byte)CommandClass.ThermostatFanMode,
                Command.Basic.Set,
                (byte)mode
            });
        }
    }

}
