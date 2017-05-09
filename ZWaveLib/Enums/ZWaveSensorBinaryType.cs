using System;

namespace ZWaveLib.Enums
{
    /// <summary>
    /// Enumerator for possible sensor binary parameters (only reported for v2)
    /// </summary>
    public enum ZWaveSensorBinaryType : byte
    {
        Unknown = 0x00,
        General = 0x01,
        Smoke = 0x02,
        CarbonMonoxide = 0x03,
        CarbonDioxide = 0x04,
        Heat = 0x05,
        Water = 0x06,
        Freeze = 0x07,
        Tamper = 0x08,
        Auxiliary = 0x09,
        DoorWindow = 0x0a,
        Tilt = 0x0b,
        Motion = 0x0c,
        GlassBreak = 0x0d
    }

    /// <summary>
    /// Enumerator for possible sensor binary parameters (only reported for v2)
    /// </summary>
    [Obsolete("Use ZWaveSensorBinaryType enum instead")]
    public enum ZWaveSensorBinaryParameter : byte
    {
        Unknown = 0x00,
        General = 0x01,
        Smoke = 0x02,
        CarbonMonoxide = 0x03,
        CarbonDioxide = 0x04,
        Heat = 0x05,
        Water = 0x06,
        Freeze = 0x07,
        Tamper = 0x08,
        Auxiliary = 0x09,
        DoorWindow = 0x0a,
        Tilt = 0x0b,
        Motion = 0x0c,
        GlassBreak = 0x0d
    }
}
