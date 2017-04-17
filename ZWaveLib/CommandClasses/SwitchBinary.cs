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
    /// The Binary Switch Command Class is used to control devices with On/Off or Enable/Disable capability
    /// </summary>
    /// <remarks>SDS12657 4.25 Binary Switch Command Class, version 1</remarks>
    /// <remarks>SDS12657 4.26 Binary Switch Command Class, version 2</remarks>
    public class SwitchBinary : ICommandClass
    {
        public CommandClass GetClassId()
        {
            return CommandClass.SwitchBinary;
        }

        // SDS12657 4.25.3 Binary Switch Report Command
        public NodeEvent GetEvent(IZWaveNode node, byte[] message)
        {
            NodeEvent nodeEvent;
            var cmdType = message[1];
            switch (cmdType)
            {
                case Command.SwitchBinary.Report:
                case Command.SwitchBinary.Set: // some devices use this instead of report
                    var value = (double) message[2];
                    nodeEvent = new NodeEvent(node, EventParameter.SwitchBinary, value, 0);
                    break;

                default:
                    throw new UnsupportedCommandException(cmdType);
            }

            return nodeEvent;
        }

        /// <summary>
        /// The Binary Switch Set command, version 1 is used to set a binary value.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <remarks>SDS12657 4.25.1 Binary Switch Set Command</remarks>
        public static ZWaveMessage Set(IZWaveNode node, int value)
        {
            if (!IsValidBinarySetValue(value))
                throw new ArgumentOutOfRangeException(nameof(value));

            return node.SendDataRequest(new[]
            {
                (byte) CommandClass.SwitchBinary,
                Command.SwitchBinary.Set,
                byte.Parse(value.ToString())
            });
        }

        /// <summary>
        /// The Binary Switch Get command, version 1 is used to request the status of a device with On/Off or Enable/Disable capability.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        /// <remarks>SDS12657 4.25.2 Binary Switch Get Command</remarks>
        public static ZWaveMessage Get(IZWaveNode node)
        {
            return node.SendDataRequest(new[]
            {
                (byte) CommandClass.SwitchBinary,
                Command.SwitchBinary.Get
            });
        }

        private static bool IsValidBinarySetValue(int value)
        {
            return value == 255 || (value >= 0 && value <= 99);
        }
    }
}
