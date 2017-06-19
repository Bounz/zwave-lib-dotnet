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
    /// <summary>
    /// The Central Scene Command Class is used to communicate central scene activations to a central
    /// controller using the lifeline concept.
    /// </summary>
    /// <remarks>SDS13781-2 4.16 Central Scene Command Class, version 2 [OBSOLETED]</remarks>
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

        /// <summary>
        /// This command is used to request the maximum number of scenes that this device supports.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        /// <remarks>SDS13781-2 4.15.1 Central Scene Supported Get Command</remarks>
        public static ZWaveMessage SupportedGet(IZWaveNode node)
        {
            return node.SendDataRequest(new[] {
                (byte)CommandClass.CentralScene,
                Command.CentralScene.SupportedGet
            });
        }
    }
}