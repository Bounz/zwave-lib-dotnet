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
*     Author: https://github.com/mdave
*     Author: https://github.com/bounz
*     Project Homepage: https://github.com/genielabs/zwave-lib-dotnet
*/

using System;
using ZWaveLib.Enums;
using ZWaveLib.Utilities;
using ZWaveLib.Values;

namespace ZWaveLib.CommandClasses
{
    /// <summary>
    /// The Version Command Class may be used to obtain the Z-Wave library type, the Z-Wave protocol
    /// version used by the application, the individual command class versions used by the application and the
    /// vendor specific application version from a Z-Wave enabled device.
    /// </summary>
    /// <remarks>SDS12652 3.52 Version Command Class, version 1</remarks>
    public class Version : ICommandClass
    {
        public CommandClass GetClassId()
        {
            return CommandClass.Version;
        }

        // SDS12652 3.52.4 Version Report Command
        // SDS12652 3.52.6 Version Command Class Report Command
        public NodeEvent GetEvent(IZWaveNode node, byte[] message)
        {
            NodeEvent nodeEvent = null;
            var cmdType = message[1];

            switch (cmdType)
            {
                case Command.Version.Report:
                    var nodeVersion = new NodeVersion
                    {
                        LibraryType = message[2],
                        ProtocolVersion = message[3],
                        ProtocolSubVersion = message[4],
                        ApplicationVersion = message[5],
                        ApplicationSubVersion = message[6]
                    };
                    node.Version = nodeVersion;
                    nodeEvent = new NodeEvent(node, EventParameter.VersionCommandClass, nodeVersion, 0);
                    break;

                case Command.Version.CommandClassReport:
                
                    var cmdClass = (CommandClass)message[2];
                    var versionValue = new VersionValue(cmdClass, message[3]);
                    // Update node CC data
                    if (cmdClass != CommandClass.NotSet)
                    {
                        var nodeCommandClass = node.GetCommandClass(cmdClass);
                        if (nodeCommandClass != null)
                            nodeCommandClass.Version = versionValue.Version;
                        // Set the VersionCommandClass event
                        nodeEvent = new NodeEvent(node, EventParameter.VersionCommandClass, versionValue, 0);
                    }
                    else
                    {
                        Utility.Logger.Warn("Command Class {0} ({1:X2}) not supported yet", message[3], message[3]);
                    }
                    break;

                default:
                    throw new UnsupportedCommandException(cmdType);
            }

            return nodeEvent;
        }

        /// <summary>
        /// The Version Get Command is used to request the library type, protocol version and application version
        /// from a device that supports the Version Command Class.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        /// <remarks>SDS12652 3.52.3 Version Get Command</remarks>
        public static ZWaveMessage Get(IZWaveNode node)
        {
            return node.SendDataRequest(new[] {
                (byte)CommandClass.Version,
                Command.Version.Get,
            });
        }

        /// <summary>
        /// The Version Command Class Get Command is used to request the individual command class versions
        /// from a device.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="cmdClass"></param>
        /// <returns></returns>
        /// <remarks>SDS12652 3.52.5 Version Command Class Get Command</remarks>
        public static ZWaveMessage CommandClassGet(IZWaveNode node, CommandClass cmdClass)
        {
            return node.SendDataRequest(new[] {
                (byte)CommandClass.Version,
                Command.Version.CommandClassGet,
                (byte)cmdClass
            });
        }

        [Obsolete("Use method CommandClassGet()")]
        public static ZWaveMessage Get(IZWaveNode node, CommandClass cmdClass)
        {
            return node.SendDataRequest(new[] { 
                (byte)CommandClass.Version, 
                Command.Version.CommandClassGet,
                (byte)cmdClass
            });
        }

        [Obsolete("Use method Get()")]
        public static ZWaveMessage Report(IZWaveNode node)
        {
            return node.SendDataRequest(new[] { 
                (byte)CommandClass.Version, 
                Command.Version.Get,
            });
        }
    }
}
