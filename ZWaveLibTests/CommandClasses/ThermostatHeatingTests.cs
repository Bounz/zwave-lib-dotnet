using NUnit.Framework;
using ZWaveLib.CommandClasses;

namespace ZWaveLibTests.CommandClasses
{
    [TestFixture]
    public class ThermostatHeatingTests
    {
        /* 
         * Information about the constants is taken from
         * "Sigma Designs, SDS13740, Software Design Specification, Z-Wave Device and Command Class Types and Defines Specification" page 104-105
         * Thermostat Heating command class commands
         */

        private const byte CommandClassThermostatHeating = 0x38; // COMMAND_CLASS_THERMOSTAT_MODE

        /* Thermostat Heating command class commands */
        private const byte ThermostatHeatingVersion = 0x01;   // THERMOSTAT_HEATING_VERSION
        private const byte ThermostatHeatingStatusReport = 0x0D; // THERMOSTAT_HEATING_STATUS_REPORT
        private const byte ThermostatHeatingModeGet = 0x02;  // THERMOSTAT_HEATING_MODE_GET
        private const byte ThermostatHeatingModeReport = 0x03;   // THERMOSTAT_HEATING_MODE_REPORT
        private const byte ThermostatHeatingModeSet = 0x01;  // THERMOSTAT_HEATING_MODE_SET
        private const byte ThermostatHeatingRelayStatusGet = 0x09;  // THERMOSTAT_HEATING_RELAY_STATUS_GET
        private const byte ThermostatHeatingRelayStatusReport = 0x0A;   // THERMOSTAT_HEATING_RELAY_STATUS_REPORT
        private const byte ThermostatHeatingSetpointGet = 0x05;  // THERMOSTAT_HEATING_SETPOINT_GET
        private const byte ThermostatHeatingSetpointReport = 0x06;   // THERMOSTAT_HEATING_SETPOINT_REPORT
        private const byte ThermostatHeatingSetpointSet = 0x04;  // THERMOSTAT_HEATING_SETPOINT_SET
        private const byte ThermostatHeatingStatusGet = 0x0C;    // THERMOSTAT_HEATING_STATUS_GET
        private const byte ThermostatHeatingStatusSet = 0x0B;    // THERMOSTAT_HEATING_STATUS_SET
        private const byte ThermostatHeatingTimedOffSet = 0x11; // THERMOSTAT_HEATING_TIMED_OFF_SET
        /* Values used for Thermostat Heating Status Report command */
        private const byte ThermostatHeatingStatusReportHeating = 0x00; // THERMOSTAT_HEATING_STATUS_REPORT_HEATING
        private const byte ThermostatHeatingStatusReportCooling = 0x01; // THERMOSTAT_HEATING_STATUS_REPORT_COOLING
        /* Values used for Thermostat Heating Mode Report command */
        private const byte ThermostatHeatingModeReportOff = 0x00;   // THERMOSTAT_HEATING_MODE_REPORT_OFF
        private const byte ThermostatHeatingModeReportOffTimed = 0x01; // THERMOSTAT_HEATING_MODE_REPORT_OFF_TIMED
        private const byte ThermostatHeatingModeReportOff3Hours = 0x02;   // THERMOSTAT_HEATING_MODE_REPORT_OFF_3_HOURS
        private const byte ThermostatHeatingModeReportAntiFreeze = 0x03;   // THERMOSTAT_HEATING_MODE_REPORT_ANTI_FREEZE
        private const byte ThermostatHeatingModeReportManual = 0x04;    // THERMOSTAT_HEATING_MODE_REPORT_MANUAL
        private const byte ThermostatHeatingModeReportTemporaryManual = 0x05;  // THERMOSTAT_HEATING_MODE_REPORT_TEMPORARY_MANUAL
        private const byte ThermostatHeatingModeReportAutomatic = 0x06; // THERMOSTAT_HEATING_MODE_REPORT_AUTOMATIC
        private const byte ThermostatHeatingModeReportManualTimed = 0x07;  // THERMOSTAT_HEATING_MODE_REPORT_MANUAL_TIMED
        /* Values used for Thermostat Heating Mode Set command */
        private const byte ThermostatHeatingModeSetOff = 0x00;  // THERMOSTAT_HEATING_MODE_SET_OFF
        private const byte ThermostatHeatingModeSetOffTimed = 0x01;    // THERMOSTAT_HEATING_MODE_SET_OFF_TIMED
        private const byte ThermostatHeatingModeSetOff3Hours = 0x02;  // THERMOSTAT_HEATING_MODE_SET_OFF_3_HOURS
        private const byte ThermostatHeatingModeSetAntiFreeze = 0x03;  // THERMOSTAT_HEATING_MODE_SET_ANTI_FREEZE
        private const byte ThermostatHeatingModeSetManual = 0x04;   // THERMOSTAT_HEATING_MODE_SET_MANUAL
        private const byte ThermostatHeatingModeSetTemporaryManual = 0x05; // THERMOSTAT_HEATING_MODE_SET_TEMPORARY_MANUAL
        private const byte ThermostatHeatingModeSetAutomatic = 0x06;    // THERMOSTAT_HEATING_MODE_SET_AUTOMATIC
        private const byte ThermostatHeatingModeSetManualTimed = 0x07; // THERMOSTAT_HEATING_MODE_SET_MANUAL_TIMED
        /* Values used for Thermostat Heating Relay Status Report command */
        private const byte ThermostatHeatingRelayStatusReportOff = 0x00;   // THERMOSTAT_HEATING_RELAY_STATUS_REPORT_OFF
        private const byte ThermostatHeatingRelayStatusReportOn = 0x01;    // THERMOSTAT_HEATING_RELAY_STATUS_REPORT_ON
        /* Values used for Thermostat Heating Setpoint Report command */
        private const byte ThermostatHeatingSetpointReportProperties1SizeMask = 0x07; // THERMOSTAT_HEATING_SETPOINT_REPORT_PROPERTIES1_SIZE_MASK
        private const byte ThermostatHeatingSetpointReportProperties1ScaleMask = 0x18;    // THERMOSTAT_HEATING_SETPOINT_REPORT_PROPERTIES1_SCALE_MASK
        private const byte ThermostatHeatingSetpointReportProperties1ScaleShift = 0x03;   // THERMOSTAT_HEATING_SETPOINT_REPORT_PROPERTIES1_SCALE_SHIFT
        private const byte ThermostatHeatingSetpointReportProperties1PrecisionMask = 0xE0;    // THERMOSTAT_HEATING_SETPOINT_REPORT_PROPERTIES1_PRECISION_MASK
        private const byte ThermostatHeatingSetpointReportProperties1PrecisionShift = 0x05;   // THERMOSTAT_HEATING_SETPOINT_REPORT_PROPERTIES1_PRECISION_SHIFT
        /* Values used for Thermostat Heating Setpoint Set command */
        private const byte ThermostatHeatingSetpointSetProperties1SizeMask = 0x07;    // THERMOSTAT_HEATING_SETPOINT_SET_PROPERTIES1_SIZE_MASK
        private const byte ThermostatHeatingSetpointSetProperties1ScaleMask = 0x18;   // THERMOSTAT_HEATING_SETPOINT_SET_PROPERTIES1_SCALE_MASK
        private const byte ThermostatHeatingSetpointSetProperties1ScaleShift = 0x03;  // THERMOSTAT_HEATING_SETPOINT_SET_PROPERTIES1_SCALE_SHIFT
        private const byte ThermostatHeatingSetpointSetProperties1PrecisionMask = 0xE0;   // THERMOSTAT_HEATING_SETPOINT_SET_PROPERTIES1_PRECISION_MASK
        private const byte ThermostatHeatingSetpointSetProperties1PrecisionShift = 0x05;  // THERMOSTAT_HEATING_SETPOINT_SET_PROPERTIES1_PRECISION_SHIFT
        /* Values used for Thermostat Heating Status Set command */
        private const byte ThermostatHeatingStatusSetHeating = 0x00;    // THERMOSTAT_HEATING_STATUS_SET_HEATING
        private const byte ThermostatHeatingStatusSetCooling = 0x01;	// THERMOSTAT_HEATING_STATUS_SET_COOLING



        [Test]
        public void GetCommandClass()
        {
            var commandClass = new ThermostatHeating();
            var classId = (byte) commandClass.GetClassId();

            Assert.That(classId, Is.EqualTo(CommandClassThermostatHeating));
        }
    }
}
