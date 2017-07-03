using System.Collections.Generic;
using System.Linq;

namespace ZWaveLib.CommandClasses.MultiChannel
{
    /// <summary>
    /// The Multi Channel Aggregated Members Report is used to advertise the members of an Aggregated End Point.
    /// </summary>
    public class MultiChannelAggregatedMembersReport
    {
        /// <summary>
        /// This field advertises an Aggregated End Point.
        /// </summary>
        public byte AggregatedEndPoint { get; set; }

        /// <summary>
        /// This bit mask is used to advertise the End Point members of the Aggregated End Point advertised in the Aggregated End Point field.
        /// </summary>
        public List<byte> Members { get; set; }

        /* Values used for Multi Channel Aggregated Members Report command */
        private const byte AggregatedEndPointMask = 0x7F;
        private const byte ResBitMask = 0x80;

        internal static MultiChannelAggregatedMembersReport Parse(byte[] message)
        {
            return new MultiChannelAggregatedMembersReport
            {
                AggregatedEndPoint = (byte)(message[2] & AggregatedEndPointMask),
                Members = message[3] > 0
                    ? GetMembersFromBitMask(message.Skip(4).ToArray())
                    : new List<byte>()
            };
        }

        private static List<byte> GetMembersFromBitMask(byte[] bitMasks)
        {
            var listedEndPoints = new List<byte>();
            byte bitPos = 0;
            while (bitPos < 8 * bitMasks.Length)
            {
                var byteIndex = bitPos / 8;
                var offset = bitPos % 8;
                var isSet = (bitMasks[byteIndex] & (1 << offset)) != 0;

                bitPos++;
                if (isSet)
                    listedEndPoints.Add(bitPos);
            }
            return listedEndPoints;
        }
    }
}
