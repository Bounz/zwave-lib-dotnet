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

namespace ZWaveLib.CommandClasses
{
    /// <summary>
    /// The Multilevel Switch Command Class is used to control devices with multilevel capability.
    /// </summary>
    /// <remarks>SDS12657 4.92 Multilevel Switch Command Class, version 1</remarks>
    public class SwitchMultilevel : ICommandClass
    {
        public CommandClass GetClassId()
        {
            return CommandClass.SwitchMultilevel;
        }

        // SDS12657 4.92.3 Multilevel Switch Report Command
        public NodeEvent GetEvent(IZWaveNode node, byte[] message)
        {
            NodeEvent nodeEvent;
            var cmdType = message[1];
            switch (cmdType)
            {
                case Command.SwitchMultilevel.Report:
                case Command.SwitchMultilevel.Set: // some devices use this instead of report
                    var value = (double) message[2];
                    nodeEvent = new NodeEvent(node, EventParameter.SwitchMultilevel, value, 0);
                    break;

                default:
                    throw new UnsupportedCommandException(cmdType);
            }

            return nodeEvent;
        }

        /// <summary>
        /// The Multilevel Switch Set command, version 1 is used to set a multilevel value in a supporting device.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <remarks>SDS12657 4.92.1 Multilevel Switch Set Command</remarks>
        public static ZWaveMessage Set(IZWaveNode node, int value)
        {
            return node.SendDataRequest(new[]
            {
                (byte) CommandClass.SwitchMultilevel,
                Command.SwitchMultilevel.Set,
                byte.Parse(value.ToString())
            });
        }

        /// <summary>
        /// The Multilevel Switch Get command, version 1 is used to request the status of a multilevel device.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        /// <remarks>SDS12657 4.92.2 Multilevel Switch Get Command</remarks>
        public static ZWaveMessage Get(IZWaveNode node)
        {
            return node.SendDataRequest(new[]
            {
                (byte) CommandClass.SwitchMultilevel,
                Command.SwitchMultilevel.Get
            });
        }

        /// <summary>
        /// The Multilevel Switch Start Level Change command, version 1 is used to initiate a transition to a new level.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="increase">if TRUE the level change will be increasing</param>
        /// <param name="ignoreStartLevel"></param>
        /// <param name="startLevel">specify the initial level of the level change</param>
        /// <returns></returns>
        /// <remarks>SDS12657 4.92.4 Multilevel Switch Start Level Change Command</remarks>
        public static ZWaveMessage StartLevelChange(IZWaveNode node, bool increase, bool ignoreStartLevel, byte startLevel)
        {
            var increaseFlag = (byte) ((increase ? 1 : 0) << 6);
            var ignoreStartLevelFlag = (byte) ((ignoreStartLevel ? 1 : 0) << 5);
            var flags = (byte) (increaseFlag | ignoreStartLevelFlag);
            return node.SendDataRequest(new[]
            {
                (byte) CommandClass.SwitchMultilevel,
                Command.SwitchMultilevel.StartLevelChange,
                flags,
                startLevel
            });
        }

        /// <summary>
        /// The Multilevel Switch Stop Level Change command, version 1 is used to stop an ongoing transition.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        /// <remarks>SDS12657 4.92.5 Multilevel Switch Stop Level Change Command</remarks>
        public static ZWaveMessage StopLevelChange(IZWaveNode node)
        {
            return node.SendDataRequest(new[]
            {
                (byte) CommandClass.SwitchMultilevel,
                Command.SwitchMultilevel.StopLevelChange
            });
        }
    }
}
