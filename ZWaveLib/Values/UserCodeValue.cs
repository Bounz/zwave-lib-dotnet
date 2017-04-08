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

using ZWaveLib.Utilities;

namespace ZWaveLib.Values
{
    public class UserCodeValue
    {
        public byte UserId;
        public byte UserIdStatus;
        public byte[] TagCode = new byte[10];

        public UserCodeValue(byte userId, byte userIdStatus, byte[] tagCode)
        {
            UserId = userId;
            UserIdStatus = userIdStatus;
            tagCode.CopyTo(TagCode, 0);
        }

        public UserCodeValue()
        {
            UserId = 0;
            UserIdStatus = 0;
            TagCode = null;
        }

        // TODO: TagCode may be 4 - 10 bytes, we should consider this
        public static UserCodeValue Parse(byte[] message)
        {
            var userCode = new UserCodeValue
            {
                UserId = message[2],
                UserIdStatus = message[3],
                TagCode = new byte[10]
            };
            for (var i = 0; i < 10; i++)
            {
                userCode.TagCode[i] = message[4 + i];
            }
            return userCode;
        }

        public string TagCodeToHexString()
        {
            return Utility.ByteArrayToHexString(TagCode);
        }
    }
}