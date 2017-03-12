using System;

namespace ZWaveLib
{
    /// <summary>
    /// Node software version.
    /// </summary>
    [Serializable]
    public class NodeVersion
    {
        /// <summary>
        /// Gets or sets the Z-Wave Library Type.
        /// </summary>
        /// <value>Z-Wave Library Type.</value>
        public byte LibraryType { get; /*internal*/ set; }

        /// <summary>
        /// Gets or sets the Z-Wave Protocol Version.
        /// </summary>
        /// <value>Z-Wave Protocol Version.</value>
        public byte ProtocolVersion { get; /*internal*/ set; }

        /// <summary>
        /// Gets or sets the Z-Wave Protocol Sub Version.
        /// </summary>
        /// <value>Z-Wave Protocol Sub Version.</value>
        public byte ProtocolSubVersion { get; /*internal*/ set; }

        /// <summary>
        /// Gets or sets the Application Version.
        /// </summary>
        /// <value>Application Version.</value>
        public byte ApplicationVersion { get; /*internal*/ set; }

        /// <summary>
        /// Gets or sets the Application Sub Version.
        /// </summary>
        /// <value>Application Sub Version.</value>
        public byte ApplicationSubVersion { get; /*internal*/ set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ZWaveLib.NodeVersion"/> class.
        /// </summary>
        public NodeVersion()
        {
        }

        public override string ToString()
        {
            return string.Format("{{\"LibraryType\":{0}, \"ProtocolVersion\":{1}, \"ProtocolSubVersion\":{2}, \"ApplicationVersion\":{3}, \"ApplicationSubVersion\":{4}}}", 
                LibraryType, ProtocolVersion, ProtocolSubVersion, ApplicationVersion, ApplicationSubVersion);
        }
    }
}