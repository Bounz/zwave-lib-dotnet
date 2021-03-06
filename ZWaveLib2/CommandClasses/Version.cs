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
*     Author: https://github.com/mdave
*     Author: https://github.com/bounz
*     Project Homepage: https://github.com/genielabs/zwave-lib-dotnet
*/

using Microsoft.Extensions.Logging;
using ZWaveLib2.Enums;
using ZWaveLib2.Utility;
using ZWaveLib2.Values;

namespace ZWaveLib2.CommandClasses
{
    public class Version : ICommandClass
    {
        private static readonly ILogger<Version> Logger = LogFactory.GetLogger<Version>();

        public CommandClass GetClassId()
        {
            return CommandClass.Version;
        }

        public NodeEvent GetEvent(ZWaveNode node, byte[] message)
        {
            NodeEvent nodeEvent = null;
            var type = (Command)message[1];

            if (type == Command.VersionReport)
            {
                var nodeVersion = new NodeVersion {
                    LibraryType = message[2],
                    ProtocolVersion = message[3],
                    ProtocolSubVersion = message[4],
                    ApplicationVersion = message[5],
                    ApplicationSubVersion = message[6]
                };
                node.Version = nodeVersion;
                nodeEvent = new NodeEvent(node, EventParameter.VersionCommandClass, nodeVersion, 0);
            }

            if (type == Command.VersionCommandClassReport)
            {
                var cmdClass = (CommandClass)message[2];
                var value = new VersionValue(cmdClass, message[3]);
                // Update node CC data
                if (cmdClass != CommandClass.NotSet)
                {
                    var nodeCc = node.GetCommandClass(cmdClass);
                    if (nodeCc != null)
                        nodeCc.Version = value.Version;
                    // Set the VersionCommandClass event
                    nodeEvent = new NodeEvent(node, EventParameter.VersionCommandClass, value, 0);
                }
                else
                {
                    Logger.LogWarning("Command Class {0} ({1}) not supported yet", message[3], message[3].ToString("X2"));
                }
            }

            return nodeEvent;
        }

        public static ZWaveMessage Get(ZWaveNode node, CommandClass cmdClass)
        {
            return node.SendDataRequest(new byte[] { 
                (byte)CommandClass.Version, 
                (byte)Command.VersionCommandClassGet,
                (byte)cmdClass
            });
        }

        public static ZWaveMessage Report(ZWaveNode node)
        {
            return node.SendDataRequest(new byte[] { 
                (byte)CommandClass.Version, 
                (byte)Command.VersionGet,
            });
        }
    }
}
