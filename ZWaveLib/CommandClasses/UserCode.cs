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
 *     Author: Alexandre Schnegg <alexandre.schnegg@gmail.com>
 *     Author: Generoso Martello <gene@homegenie.it>
 *     Project Homepage: https://github.com/genielabs/zwave-lib-dotnet
 */

using System;
using ZWaveLib.Values;
using System.Collections.Generic;
using ZWaveLib.Enums;

namespace ZWaveLib.CommandClasses
{
    /// <summary>
    /// The purpose of the User Code Command Class is to supply a enabled Door Lock Device with a
    /// Command Class to manage user codes.
    /// </summary>
    /// <remarks>SDS12652 3.51 User Code Command Class, version 1</remarks>
    public class UserCode : ICommandClass
    {
        public CommandClass GetClassId()
        {
            return CommandClass.UserCode;
        }

        // SDS12652 3.51.3 User Code Report Command
        public NodeEvent GetEvent(IZWaveNode node, byte[] message)
        {
            NodeEvent nodeEvent = null;
            var cmdType = message[1];
            if (cmdType == Command.UserCode.Report)
            {
                var reportedUserCode = UserCodeValue.Parse(message);
                var userCode = GetUserCodeData(node);
                userCode.TagCode = reportedUserCode.TagCode;
                userCode.UserId = reportedUserCode.UserId;
                userCode.UserIdStatus = reportedUserCode.UserIdStatus;
                nodeEvent = new NodeEvent(node, EventParameter.UserCode, reportedUserCode, 0);
            }
            return nodeEvent;
        }

        /// <summary>
        /// The User Code Get Command is used to request the user code of a specific user identifier.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        /// <remarks>SDS12652 3.51.2 User Code Get Command</remarks>
        public static ZWaveMessage Get(IZWaveNode node)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The User Code Set Command is used to set a User Code in the device.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="newUserCode"></param>
        /// <returns></returns>
        /// <remarks>SDS12652 3.51.1 User Code Set Command</remarks>
        public static ZWaveMessage Set(IZWaveNode node, UserCodeValue newUserCode)
        {
            var userCode = GetUserCodeData(node);
            userCode.TagCode = newUserCode.TagCode;
            userCode.UserId = newUserCode.UserId;
            userCode.UserIdStatus = newUserCode.UserIdStatus;
            var message = new List<byte> { (byte)CommandClass.UserCode, Command.UserCode.Set, userCode.UserId, userCode.UserIdStatus };
            message.AddRange(userCode.TagCode);
            return node.SendDataRequest(message.ToArray());
        }

        /// <summary>
        /// The User Number Get Command is used to request the number of USER CODES that this node supports.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        /// <remarks>SDS12652 3.51.4 Users Number Get Command</remarks>
        public static ZWaveMessage UsersNumberGet(IZWaveNode node)
        {
            throw new NotImplementedException();
        }

        public static UserCodeValue GetUserCode(IZWaveNode node)
        {
            return GetUserCodeData(node);
        }

        private static UserCodeValue GetUserCodeData(IZWaveNode node)
        {
            return (UserCodeValue)node.GetData("UserCode", new UserCodeValue()).Value;
        }

    }
}
