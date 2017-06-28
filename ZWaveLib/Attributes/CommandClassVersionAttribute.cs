using System;

namespace ZWaveLib.Attributes
{
    /// <summary>
    /// Represents the minimum version of Command Class where this method can be used
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    internal class CommandClassVersionAttribute : Attribute
    {
        public int MinVersion { get; set; }

        public CommandClassVersionAttribute(int version)
        {
            MinVersion = version;
        }
    }
}
