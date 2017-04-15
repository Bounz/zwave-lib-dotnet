using System;

namespace ZWaveLib.Utilities
{
    public class UnsupportedCommandException : Exception
    {
        public UnsupportedCommandException(byte commandType)
            :base(string.Format("Unknown command: {0}", commandType))
        { }
    }
}
