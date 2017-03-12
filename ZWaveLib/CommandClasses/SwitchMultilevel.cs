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

using ZWaveLib.Enums;

namespace ZWaveLib.CommandClasses
{
    public class SwitchMultilevel : ICommandClass
    {
        public CommandClass GetClassId()
        {
            return CommandClass.SwitchMultilevel;
        }

        // SDS12657 4.92.3 Multilevel Switch Report Command
        public NodeEvent GetEvent(IZWaveNode node, byte[] message)
        {
            NodeEvent nodeEvent = null;
            var cmdType = message[1];

            // some devices use SwitchMultilevelSet instead of report
            if (cmdType == Command.SwitchMultilevel.Report || cmdType == Command.SwitchMultilevel.Set)
            {
                var levelValue = (int) message[2];
                nodeEvent = new NodeEvent(node, EventParameter.SwitchMultilevel, (double) levelValue, 0);
            }
            return nodeEvent;
        }

        // SDS12657 4.92.1 Multilevel Switch Set Command
        public static ZWaveMessage Set(IZWaveNode node, int value)
        {
            return node.SendDataRequest(new[]
            {
                (byte) CommandClass.SwitchMultilevel,
                Command.SwitchMultilevel.Set,
                byte.Parse(value.ToString())
            });
        }

        // SDS12657 4.92.2 Multilevel Switch Get Command
        public static ZWaveMessage Get(IZWaveNode node)
        {
            return node.SendDataRequest(new[]
            {
                (byte) CommandClass.SwitchMultilevel,
                Command.SwitchMultilevel.Get
            });
        }
    }
}
