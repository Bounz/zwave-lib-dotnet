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
 *     Author: Délano Reijnierse (bluewalk)
 *     Project Homepage: https://github.com/genielabs/zwave-lib-dotnet
 */

using ZWaveLib.Enums;
using ZWaveLib.Utilities;
using ZWaveLib.Values;

namespace ZWaveLib.CommandClasses
{
    public class CentralScene : ICommandClass
    {
        public CommandClass GetClassId()
        {
            return CommandClass.CentralScene;
        }

        public NodeEvent GetEvent(IZWaveNode node, byte[] message)
        {
            if (message.Length == 0)
                return null;

            NodeEvent nodeEvent = null;
            var cmdType = message[1];
            switch (cmdType)
            {
                case Command.CentralScene.Notification:
                    var value = CentralSceneValue.Parse(message);
                    nodeEvent = new NodeEvent(node, EventParameter.CentralSceneNotification, value, 0);
                    break;

                case Command.CentralScene.SupportedReport:
                    nodeEvent = new NodeEvent(node, EventParameter.CentralSceneSupportedReport, (int)message[2], 0);
                    break;

                default:
                    throw new UnsupportedCommandException(cmdType);
            }

            return nodeEvent;
        }

        public static ZWaveMessage SupportedGet(ZWaveNode node)
        {
            return node.SendDataRequest(new byte[] {
                (byte)CommandClass.CentralScene,
                (byte)Command.CentralScene.SupportedGet
            });
        }
    }
}