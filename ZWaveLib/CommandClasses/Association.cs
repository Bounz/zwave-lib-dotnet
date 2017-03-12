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
    public class Association : ICommandClass
    {
        public class AssociationResponse
        {
            public byte Max = 0;
            public byte Count = 0;
            public byte GroupId = 0;
            public string NodeList = "";
        }

        public CommandClass GetClassId()
        {
            return CommandClass.Association;
        }

        public NodeEvent GetEvent(IZWaveNode node, byte[] message)
        {
            NodeEvent nodeEvent = null;
            byte cmdType = message[1];
            
            // we want to get in to that we can handle NO Associations
            if (message.Length > 4 && cmdType == Command.Association.Report)
            {
                byte groupId = message[2];
                byte associationMax = message[3];
                byte associationCount = message[4]; // it is always zero ?!?
                string associationNodes = "";
                if (message.Length > 4)
                {
                    for (int a = 5; a < message.Length; a++)
                    {
                        associationNodes += message[a] + ",";
                    }
                }
                associationNodes = associationNodes.TrimEnd(',');

                // We don't want to send empty response since it will be handled as "timeout"
                // so setting it to "None"
                if (associationNodes.Length == 0)
                {
                    associationNodes = "None";
                }
                //
                var associationResponse = new AssociationResponse() {
                    Max = associationMax,
                    Count = associationCount,
                    NodeList = associationNodes,
                    GroupId = groupId
                };
                nodeEvent = new NodeEvent(node, EventParameter.Association, associationResponse, 0);
            }

            return nodeEvent;
        }

        public static ZWaveMessage Set(ZWaveNode node, byte groupid, byte targetNodeId)
        {
            return node.SendDataRequest(new[] { 
                (byte)CommandClass.Association, 
                Command.Association.Set, 
                groupid, 
                targetNodeId 
            });
        }

        public static ZWaveMessage Get(ZWaveNode node, byte groupId)
        {
            return node.SendDataRequest(new[] { 
                (byte)CommandClass.Association, 
                Command.Association.Get, 
                groupId 
            });
        }

        public static ZWaveMessage Remove(ZWaveNode node, byte groupId, byte targetNodeId)
        {
            return node.SendDataRequest(new[] { 
                (byte)CommandClass.Association, 
                Command.Association.Remove, 
                groupId, 
                targetNodeId 
            });
        }
    }
}

