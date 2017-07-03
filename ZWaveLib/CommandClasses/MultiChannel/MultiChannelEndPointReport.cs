namespace ZWaveLib.CommandClasses.MultiChannel
{
    /// <summary>
    /// The Multi Channel End Point Report is used to advertise the number of End Points implemented by the node.
    /// </summary>
    public class MultiChannelEndPointReport
    {
        /// <summary>
        /// This field is set to <c>true</c> if the device has a dynamic number of end points. 
        /// When the dynamic bit is set the number of end points in the device can change over time. 
        /// Care should be taken when communicating with dynamic end points as the transmitter cannot be entirely sure the specific end point exists.
        /// </summary>
        public bool IsDynamic { get; set; }

        /// <summary>
        /// This bit is set to <c>true</c> if all End Points advertise the same generic and specific Device Class and optional Command Classes.
        /// </summary>
        public bool AreIdentical { get; set; }

        /// <summary>
        /// This field advertises the number of individual End Points implemented by this node.
        /// </summary>
        public byte IndividualEndPointsCount { get; set; }

        /// <summary>
        /// This field advertises the number of Aggregated End Points implemented by this node.
        /// </summary>
        /// <remarks>Introduced in version 4 of <see cref="MultiChannel"/> command class.</remarks>
        public byte AggregatedEndPointsCount { get; set; }

        /* Values used for Multi Channel End Point Report command */
        private const byte Properties1Res1Mask = 0x3F;
        private const byte Properties1IdenticalBitMask = 0x40;
        private const byte Properties1DynamicBitMask = 0x80;
        private const byte Properties2IndividualEndPointsMask = 0x7F;
        private const byte Properties2Res2BitMask = 0x80;
        private const byte Properties3AggregatedEndPointsMask = 0x7F;
        private const byte Properties3Res3BitMask = 0x80;

        internal static MultiChannelEndPointReport Parse(byte[] message)
        {
            return new MultiChannelEndPointReport
            {
                IsDynamic = (message[2] & Properties1DynamicBitMask) == Properties1DynamicBitMask,
                AreIdentical = (message[2] & Properties1IdenticalBitMask) == Properties1DynamicBitMask,
                IndividualEndPointsCount = (byte) (message[3] & Properties2IndividualEndPointsMask),
                AggregatedEndPointsCount = message.Length == 5
                    ? (byte) (message[4] & Properties3AggregatedEndPointsMask)
                    : (byte) 0
            };
        }
    }
}
