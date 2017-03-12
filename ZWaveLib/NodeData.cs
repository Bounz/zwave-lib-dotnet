namespace ZWaveLib
{
    /// <summary>
    /// Custom node data.
    /// </summary>
    public class NodeData
    {
        /// <summary>
        /// Gets or sets the name for this data entry.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; private set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        public object Value { get; internal set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="NodeData"/> class.
        /// </summary>
        /// <param name="fieldName">Field name.</param>
        /// <param name="data">Data.</param>
        public NodeData(string fieldName, object data)
        {
            Name = fieldName;
            Value = data;
        }
    }
}