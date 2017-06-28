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

using ZWaveLib.Attributes;
using ZWaveLib.Enums;
using ZWaveLib.Utilities;
using ZWaveLib.Values;

namespace ZWaveLib.CommandClasses
{
    /// <summary>
    /// The Multilevel Sensor Command Class is used to control a multilevel sensor.
    /// </summary>
    /// <remarks>SDS13781 4.59 Multilevel Sensor Command Class, version 1-4</remarks>
    public class SensorMultilevel : ICommandClass
    {
        public CommandClass GetClassId()
        {
            return CommandClass.SensorMultilevel;
        }

        // SDS13781 4.59.2 Multilevel Sensor Report Command
        public NodeEvent GetEvent(IZWaveNode node, byte[] message)
        {
            NodeEvent nodeEvent;
            var cmdType = message[1];
            switch (cmdType)
            {
                case Command.SensorMultilevel.Report:
                {
                    var sensor = SensorValue.Parse(message);
                    if (sensor.Parameter == ZWaveSensorType.Unknown)
                    {
                        var sensorType = message[2];
                        nodeEvent = new NodeEvent(node, EventParameter.SensorGeneric, sensor.Value, 0);
                        Utility.Logger.Warn("Unhandled sensor parameter type: " + sensorType);
                    }
                    else
                    {
                        nodeEvent = new NodeEvent(node, sensor.EventType, sensor.Value, 0);
                    }
                    break;
                }

                default:
                    throw new UnsupportedCommandException(cmdType);
            }

            return nodeEvent;
        }

        /// <summary>
        /// This command is used to request the level of a multilevel sensor.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        /// <remarks>SDS13781 4.59.1 Multilevel Sensor Get Command</remarks>
        public static ZWaveMessage Get(IZWaveNode node)
        {
            return node.SendDataRequest(new[]
            {
                (byte) CommandClass.SensorMultilevel,
                Command.SensorMultilevel.Get
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <param name="sensorType"></param>
        /// <param name="scale"></param>
        /// <returns>SDS13781-2 4.57.6 Multilevel Sensor Get Command</returns>
        [CommandClassVersion(5)]
        public static ZWaveMessage Get(IZWaveNode node, ZWaveSensorType sensorType, byte scale = 0x00)
        {
            return node.SendDataRequest(new[]
            {
                (byte) CommandClass.SensorMultilevel,
                Command.SensorMultilevel.Get,
                (byte) sensorType,
                (byte) (scale << 3)
            });
        }
    }
}
