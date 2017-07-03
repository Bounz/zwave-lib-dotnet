using System.Collections.Generic;
using ZWaveLib.Enums;

namespace ZWaveLib.CommandClasses
{
    public static class MultiChannelExtensions
    {
        public static byte[] ToMultiChannel(this byte[] originalCommand, byte destinationEndPoint, bool bitAddress = false)
        {
            if (bitAddress)
            {
                destinationEndPoint = (byte) (destinationEndPoint & 0x80);
            }

            var encapsulatedCommand = new List<byte>
            {
                (byte)CommandClass.MultiInstance,
                Command.MultiChannel.CmdEncap,
                0x00, // source end point
                destinationEndPoint
            };
            encapsulatedCommand.AddRange(originalCommand);
            return encapsulatedCommand.ToArray();
        }
    }
}
