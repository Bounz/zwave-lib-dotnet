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

namespace ZWaveLib.CommandClasses
{
    /// <summary>
    /// The Thermostat Setback Command Class is used to change the current state of a non-schedule setback
    /// thermostat
    /// </summary>
    /// <remarks>SDS12652 3.43 Thermostat Setback Command Class, version 1</remarks>
    public class ThermostatSetBack : ICommandClass
    {
        public CommandClass GetClassId()
        {
            return CommandClass.ThermostatSetBack;
        }

        // SDS12652 3.43.3 Thermostat Setback Report Command
        public NodeEvent GetEvent(IZWaveNode node, byte[] message)
        {
            return new NodeEvent(node, EventParameter.ThermostatSetBack, message[2], 0);
        }

        /// <summary>
        /// The Thermostat Setback Set Command is used to set the state of the thermostat.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        /// <remarks>SDS12652 3.43.1 Thermostat Setback Set Command</remarks>
        public static ZWaveMessage Set(IZWaveNode node)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The Thermostat Setback Get Command is used to request the current state of the thermostat
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        /// <remarks>SDS12652 3.43.2 Thermostat Setback Get Command</remarks>
        public static ZWaveMessage Get(IZWaveNode node)
        {
            throw new NotImplementedException();
        }
    }
}
