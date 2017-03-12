using System;

namespace ZWaveLib
{
    /// <summary>
    /// Node capabilities (Protocol Info).
    /// </summary>
    [Serializable]
    public class NodeCapabilities
    {
        /// <summary>
        /// Gets or sets the basic type.
        /// </summary>
        /// <value>The basic type.</value>
        public byte BasicType { get; /*internal*/ set; }

        /// <summary>
        /// Gets or sets the generic type.
        /// </summary>
        /// <value>The generic type.</value>
        public byte GenericType { get; /*internal*/ set; }

        /// <summary>
        /// Gets or sets the specific type.
        /// </summary>
        /// <value>The specific type.</value>
        public byte SpecificType { get; /*internal*/ set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ZWaveLib.NodeCapabilities"/> class.
        /// </summary>
        public NodeCapabilities()
        {
        }
    }
}