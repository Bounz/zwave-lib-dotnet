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
    /// <summary>
    /// The Wake Up Command Class allows a battery-powered device to notify another device (always
    /// listening), that it is awake and ready to receive any queued commands.
    /// </summary>
    /// <remarks>SDS12652 3.54 Wake Up Command Class, version 1</remarks>
    public class WakeUp : ICommandClass
    {
        public CommandClass GetClassId()
        {
            return CommandClass.WakeUp;
        }

        // SDS12652 3.54.3 Wake Up Interval Report Command
        // SDS12652 3.54.4 Wake Up Notification Command
        public NodeEvent GetEvent(IZWaveNode node, byte[] message)
        {
            NodeEvent nodeEvent = null;
            var cmdType = message[1];
            switch (cmdType)
            {
                case Command.WakeUp.IntervalReport:
                    if (message.Length > 4)
                    {
                        var interval = (uint) message[2] << 16;
                        interval |= (uint) message[3] << 8;
                        interval |= message[4];
                        nodeEvent = new NodeEvent(node, EventParameter.WakeUpInterval, interval, 0);
                    }
                    break;

                case Command.WakeUp.Notification:
                    WakeUpNode(node);
                    nodeEvent = new NodeEvent(node, EventParameter.WakeUpNotify, 1, 0);
                    break;
            }
            return nodeEvent;
        }

        // 
        /// <summary>
        /// The Wake Up Interval Get Command is used to request the wake up interval of a device.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        /// <remarks>SDS12652 3.54.2 Wake Up Interval Get Command</remarks>
        public static ZWaveMessage Get(IZWaveNode node)
        {
            return node.SendDataRequest(new[] { 
                (byte)CommandClass.WakeUp, 
                Command.WakeUp.IntervalGet 
            });
        }

        // SDS12652 3.54.1 Wake Up Interval Set Command
        /// <summary>
        /// The Wake Up Interval Set Command is used to configure the wake up interval of a device and the
        /// NodeID of the device receiving the Wake Up Notification Command.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="interval"></param>
        /// <returns></returns>
        /// <remarks>SDS12652 3.54.1 Wake Up Interval Set Command</remarks>
        public static ZWaveMessage Set(IZWaveNode node, uint interval)
        {
            return node.SendDataRequest(new byte[] { 
                (byte)CommandClass.WakeUp, 
                Command.WakeUp.IntervalSet,
                (byte)((interval >> 16) & 0xff),
                (byte)((interval >> 8) & 0xff),
                (byte)(interval & 0xff),
                0x01
            });
        }

        // SDS12652 3.54.5 Wake Up No More Information Command
        /// <summary>
        /// The Wake Up No More Information Command is used to notify the sender of a Wake Up Notification
        /// Command that it MAY return to sleep to minimize power consumption.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        /// <remarks>SDS12652 3.54.5 Wake Up No More Information Command</remarks>
        public static ZWaveMessage SendToSleep(IZWaveNode node)
        {
            ZWaveMessage msg = null;
            var wakeUpStatus = (WakeUpStatus)node.GetData("WakeUpStatus", new WakeUpStatus()).Value;
            if (!wakeUpStatus.IsSleeping)
            {
                // 0x01, 0x09, 0x00, 0x13, 0x2b, 0x02, 0x84, 0x08, 0x25, 0xee, 0x8b
                msg = node.SendDataRequest(new[]
                {
                    (byte) CommandClass.WakeUp,
                    Command.WakeUp.NoMoreInfo,
                    (byte) 0x25 // TODO: WHAT IS THIS??? According to the specs we should not send any additional info here
                }).Wait();
                wakeUpStatus.IsSleeping = true;
                var nodeEvent = new NodeEvent(node, EventParameter.WakeUpSleepingStatus, 1 /* 1 = sleeping, 0 = awake */, 0);
                node.OnNodeUpdated(nodeEvent);
            }
            return msg;
        }

        public static void WakeUpNode(IZWaveNode node)
        {
            // If node was marked as sleeping, reset the flag
            var wakeUpStatus = node.GetData("WakeUpStatus");
            if (wakeUpStatus != null && wakeUpStatus.Value != null && ((WakeUpStatus)wakeUpStatus.Value).IsSleeping)
            {
                ((WakeUpStatus)wakeUpStatus.Value).IsSleeping = false;
                var wakeEvent = new NodeEvent(node, EventParameter.WakeUpSleepingStatus, 0 /* 1 = sleeping, 0 = awake */, 0);
                node.OnNodeUpdated(wakeEvent);
                // Resend queued messages while node was asleep
                node.ResendQueuedMessages();
            }
        }

        public static bool GetAlwaysAwake(IZWaveNode node)
        {
            var alwaysAwake = node.GetData("WakeUpAlwaysAwake");
            if (alwaysAwake != null && alwaysAwake.Value != null && (bool)alwaysAwake.Value == true)
                return true;
            return false;
        }

        public static void SetAlwaysAwake(IZWaveNode node, bool alwaysAwake)
        {
            node.GetData("WakeUpAlwaysAwake", false).Value = alwaysAwake;
            if (alwaysAwake)
                WakeUpNode(node);
        }
    }
}
