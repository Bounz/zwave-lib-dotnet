using ZWaveLib.CommandClasses;

namespace ZWaveLib.Values
{
    /// <summary>
    /// Represents value of thermostat setpoint
    /// </summary>
    public class SetpointValue
    {
        /// <summary>
        /// Setpoint type
        /// </summary>
        public ThermostatSetPoint.SetpointType Type { get; set; }

        /// <summary>
        /// Temperature value in Celsius
        /// </summary>
        public double Value { get; set; }
    }
}
