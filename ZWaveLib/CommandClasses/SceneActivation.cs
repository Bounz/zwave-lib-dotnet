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
using ZWaveLib.Utilities;
using ZWaveLib.Values;

namespace ZWaveLib.CommandClasses
{
    /// <summary>
    /// The Scene Activation Command Class used for the actual scene launching in a number of devices e.g. a
    /// another scene-controlling unit, in a multilevel switch, in a binary switch etc. This command class requires
    /// an initial configuration of the scenes to be launched by the Scene Actuator Configuration Set or Scene
    /// Controller Configuration Set Command depending on device used.
    /// </summary>
    /// <remarks>SDS13781 4.75 Scene Activation Command Class, version 1</remarks>
    public class SceneActivation : ICommandClass
    {
        public CommandClass GetClassId()
        {
            return CommandClass.SceneActivation;
        }

        // SDS13781 4.75.1 Scene Activation Set Command
        public NodeEvent GetEvent(IZWaveNode node, byte[] message)
        {
            NodeEvent nodeEvent;
            var cmdType = message[1];
            switch (cmdType)
            {
                case Command.Scene.ActivationSet:
                {
                    var sceneActivationValue = SceneActivationValue.Parse(message);
                    nodeEvent = new NodeEvent(node, EventParameter.SceneActivation, sceneActivationValue, 0);
                    break;
                }

                default:
                    throw new UnsupportedCommandException(cmdType);
            }

            return nodeEvent;
        }

        // TODO: I suppose we should implement Send method to be able to initiate SceneActivation
        // Also we need to implement sending multicast messages in ZWave network
    }
}
