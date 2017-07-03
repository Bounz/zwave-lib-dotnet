using System.Collections.Generic;
using System.Linq;

namespace ZWaveLib.CommandClasses.MultiChannel
{
    /// <summary>
    /// The Multi Channel End Point Find Report is used to advertise End Points which implement a given combination of Generic and Specific Device Classes.
    /// </summary>
    public class MultiChannelEndPointFindReport
    {
        /// <summary>
        /// The generic Device Class of the advertised End Point.
        /// </summary>
        public byte GenericDeviceClass { get; set; }

        /// <summary>
        /// The specific Device Class of the advertised End Point.
        /// </summary>
        public byte SpecificDeviceClass { get; set; }

        /// <summary>
        /// The End Point(s) that matches the advertised generic and specific device class values.
        /// </summary>
        public List<byte> EndPoints { get; set; }

        /* Values used for Multi Channel End Point Find Report command */
        private const byte EndPointMask = 0x7F;

        internal static MultiChannelEndPointFindReport Parse(byte[] message)
        {
            return new MultiChannelEndPointFindReport
            {
                GenericDeviceClass = message[3],
                SpecificDeviceClass = message[4],
                EndPoints = message[2] > 0
                    ? message.Skip(5).Select(x => (byte) (x & EndPointMask)).ToList()
                    : null
            };
        }
    }
}
