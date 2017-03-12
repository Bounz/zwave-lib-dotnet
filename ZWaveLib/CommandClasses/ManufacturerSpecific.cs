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

using System;

namespace ZWaveLib.CommandClasses
{
    
    public class ManufacturerSpecificInfo
    {
        public string ManufacturerId { get; set; }

        public string TypeId { get; set; }

        public string ProductId { get; set; }
    }

    public class ManufacturerSpecific : ICommandClass
    {
        public CommandClass GetClassId()
        {
            return CommandClass.ManufacturerSpecific;
        }

        public NodeEvent GetEvent(ZWaveNode node, byte[] message)
        {
            NodeEvent nodeEvent = null;

            if (message.Length > 7)
            {
                byte[] manufacturerId = { message[2], message[3] };
                byte[] typeId = { message[4], message[5] };
                byte[] productId = { message[6], message[7] };

                var manufacturerSpecs = new ManufacturerSpecificInfo() {
                    TypeId = BitConverter.ToString(typeId).Replace("-", ""),
                    ProductId = BitConverter.ToString(productId).Replace("-", ""),
                    ManufacturerId = BitConverter.ToString(manufacturerId).Replace("-", "")
                };
                node.ManufacturerSpecific.ManufacturerId = manufacturerSpecs.ManufacturerId;
                node.ManufacturerSpecific.TypeId = manufacturerSpecs.TypeId;
                node.ManufacturerSpecific.ProductId = manufacturerSpecs.ProductId;
                nodeEvent = new NodeEvent(node, EventParameter.ManufacturerSpecific, manufacturerSpecs, 0);
            }

            return nodeEvent;
        }

        public static ZWaveMessage Get(ZWaveNode node)
        {
            byte[] request = {
                (byte)CommandClass.ManufacturerSpecific,
                Command.ManufacturerSpecific.Get
            }; 
            return node.SendDataRequest(request);
        }

    }
}

