using System.Collections.Generic;
using System.Linq;

namespace ZWaveLib.CommandClasses.MultiChannel
{
    /// <summary>
    /// The Multi Channel Capability Report is used to advertise the generic and specific device class of the End Point and the supported command classes.
    /// </summary>
    public class MultiChannelCapabilityReport
    {
        /// <summary>
        /// This field is set to <c>true</c> if this end point is a dynamic end point. 
        /// When this bit is set in an end point, it cannot be assumed that it will reply to commands send to it because it could be gone again when a command is send to it.
        /// </summary>
        public bool IsDynamic { get; set; }

        /// <summary>
        /// This field advertises a valid End Point as advertised by the Multi Channel End Point Report.
        /// </summary>
        public byte EndPoint { get; set; }

        /// <summary>
        /// The generic Device Class of the advertised End Point.
        /// </summary>
        public byte GenericDeviceClass { get; set; }

        /// <summary>
        /// The specific Device Class of the advertised End Point.
        /// </summary>
        public byte SpecificDeviceClass { get; set; }

        /// <summary>
        /// This field advertises Command Classes supported or controlled by the End Point in question.
        /// </summary>
        public List<byte> CommandClasses { get; set; }

        /* Values used for Multi Channel Capability Report command */
        private const byte EndPointMask = 0x7F;
        private const byte DynamicBitMask = 0x80;

        internal static MultiChannelCapabilityReport Parse(byte[] message)
        {
            return new MultiChannelCapabilityReport
            {
                IsDynamic = (message[2] & DynamicBitMask) == DynamicBitMask,
                EndPoint = (byte) (message[2] & EndPointMask),
                GenericDeviceClass = message[3],
                SpecificDeviceClass = message[4],
                CommandClasses = message.Skip(5).ToList()
            };
        }
    }
}
