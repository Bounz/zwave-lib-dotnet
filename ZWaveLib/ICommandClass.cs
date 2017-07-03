﻿/*
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

namespace ZWaveLib
{
    public interface ICommandClass
    {
        /// <summary>
        /// Returns Id of Command class
        /// </summary>
        /// <returns>command class Id</returns>
        CommandClass GetClassId();

        /// <summary>Processes the message and returns corresponding ZWaveEvent</summary>
        /// <param name="node">the Node triggered the command</param>
        /// <param name="message">command part of ZWave message (without headers and checksum)</param>
        /// <returns></returns>
        NodeEvent GetEvent(IZWaveNode node, byte[] message);
    }
}
