using System;
using System.Xml.Serialization;

namespace ZWaveLib
{
    /// <summary>
    /// Node command class.
    /// </summary>
    [Serializable]
    public class NodeCommandClass
    {
        /// <summary>
        /// The CC identifier.
        /// </summary>
        public /* readonly */ byte Id;

        /// <summary>
        /// Gets or sets the version for this CC.
        /// </summary>
        /// <value>The version.</value>
        public int Version { get; /*internal*/ set; }

        /// <summary>
        /// Gets the command class enumeration entry.
        /// </summary>
        /// <value>The command class.</value>
        [XmlIgnore]
        public CommandClass CommandClass { get { return (CommandClass)Id; } }

        /// <summary>
        /// Initializes a new instance of the <see cref="ZWaveLib.NodeCommandClass"/> class.
        /// </summary>
        public NodeCommandClass()
        {
            Version = -1;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ZWaveLib.NodeCommandClass"/> class.
        /// </summary>
        /// <param name="id">Identifier.</param>
        /// <param name="version">Version.</param>
        public NodeCommandClass(byte id, int version = -1)
        {
            Id = id;
            Version = version;
        }
    }
}