using System;
using System.Linq;
using Moq;
using NUnit.Framework;
using ZWaveLib;
using ZWaveLib.CommandClasses;
using ZWaveLib.Enums;
using ZWaveLib.Utilities;

namespace ZWaveLibTests.CommandClasses
{
    [TestFixture]
    public class SensorMultilevelTests
    {
        /* 
         * Information about the constants is taken from
         * "Sigma Designs, SDS13740, Software Design Specification, Z-Wave Device and Command Class Types and Defines Specification" page 99
         * Switch Binary command class commands
         */

        private const byte CommandClassSensorMultilevel = 0x31; // COMMAND_CLASS_SENSOR_MULTILEVEL

        /* Sensor Multilevel command class commands */
        private const byte SensorMultilevelVersionV4 = 0x04; // SENSOR_MULTILEVEL_VERSION_V4
        private const byte SensorMultilevelGetV4 = 0x04; // SENSOR_MULTILEVEL_GET_V4
        private const byte SensorMultilevelReportV4 = 0x05;  // SENSOR_MULTILEVEL_REPORT_V4
        /* Values used for Sensor Multilevel Report command */
        private const byte SensorMultilevelReportTemperatureVersion1V4 = 0x01;    // SENSOR_MULTILEVEL_REPORT_TEMPERATURE_VERSION_1_V4
        private const byte SensorMultilevelReportGeneralPurposeValueVersion1V4 = 0x02;  // SENSOR_MULTILEVEL_REPORT_GENERAL_PURPOSE_VALUE_VERSION_1_V4
        private const byte SensorMultilevelReportLuminanceVersion1V4 = 0x03;  // SENSOR_MULTILEVEL_REPORT_LUMINANCE_VERSION_1_V4
        private const byte SensorMultilevelReportPowerVersion2V4 = 0x04;  // SENSOR_MULTILEVEL_REPORT_POWER_VERSION_2_V4
        private const byte SensorMultilevelReportRelativeHumidityVersion2V4 = 0x05;  // SENSOR_MULTILEVEL_REPORT_RELATIVE_HUMIDITY_VERSION_2_V4
        private const byte SensorMultilevelReportVelocityVersion2V4 = 0x06;   // SENSOR_MULTILEVEL_REPORT_VELOCITY_VERSION_2_V4
        private const byte SensorMultilevelReportDirectionVersion2V4 = 0x07;  // SENSOR_MULTILEVEL_REPORT_DIRECTION_VERSION_2_V4
        private const byte SensorMultilevelReportAtmosphericPressureVersion2V4 = 0x08;   // SENSOR_MULTILEVEL_REPORT_ATMOSPHERIC_PRESSURE_VERSION_2_V4
        private const byte SensorMultilevelReportBarometricPressureVersion2V4 = 0x09;    // SENSOR_MULTILEVEL_REPORT_BAROMETRIC_PRESSURE_VERSION_2_V4
        private const byte SensorMultilevelReportSolarRadiationVersion2V4 = 0x0A;    // SENSOR_MULTILEVEL_REPORT_SOLAR_RADIATION_VERSION_2_V4
        private const byte SensorMultilevelReportDewPointVersion2V4 = 0x0B;  // SENSOR_MULTILEVEL_REPORT_DEW_POINT_VERSION_2_V4
        private const byte SensorMultilevelReportRainRateVersion2V4 = 0x0C;  // SENSOR_MULTILEVEL_REPORT_RAIN_RATE_VERSION_2_V4
        private const byte SensorMultilevelReportTideLevelVersion2V4 = 0x0D; // SENSOR_MULTILEVEL_REPORT_TIDE_LEVEL_VERSION_2_V4
        private const byte SensorMultilevelReportWeightVersion3V4 = 0x0E; // SENSOR_MULTILEVEL_REPORT_WEIGHT_VERSION_3_V4
        private const byte SensorMultilevelReportVoltageVersion3V4 = 0x0F;    // SENSOR_MULTILEVEL_REPORT_VOLTAGE_VERSION_3_V4
        private const byte SensorMultilevelReportCurrentVersion3V4 = 0x10;    // SENSOR_MULTILEVEL_REPORT_CURRENT_VERSION_3_V4
        private const byte SensorMultilevelReportCo2LevelVersion3V4 = 0x11;  // SENSOR_MULTILEVEL_REPORT_CO2_LEVEL_VERSION_3_V4
        private const byte SensorMultilevelReportAirFlowVersion3V4 = 0x12;   // SENSOR_MULTILEVEL_REPORT_AIR_FLOW_VERSION_3_V4
        private const byte SensorMultilevelReportTankCapacityVersion3V4 = 0x13;  // SENSOR_MULTILEVEL_REPORT_TANK_CAPACITY_VERSION_3_V4
        private const byte SensorMultilevelReportDistanceVersion3V4 = 0x14;   // SENSOR_MULTILEVEL_REPORT_DISTANCE_VERSION_3_V4
        private const byte SensorMultilevelReportAnglePositionVersion4V4 = 0x15; // SENSOR_MULTILEVEL_REPORT_ANGLE_POSITION_VERSION_4_V4
        private const byte SensorMultilevelReportLevelSizeMaskV4 = 0x07;  // SENSOR_MULTILEVEL_REPORT_LEVEL_SIZE_MASK_V4
        private const byte SensorMultilevelReportLevelScaleMaskV4 = 0x18; // SENSOR_MULTILEVEL_REPORT_LEVEL_SCALE_MASK_V4
        private const byte SensorMultilevelReportLevelScaleShiftV4 = 0x03;    // SENSOR_MULTILEVEL_REPORT_LEVEL_SCALE_SHIFT_V4
        private const byte SensorMultilevelReportLevelPrecisionMaskV4 = 0xE0; // SENSOR_MULTILEVEL_REPORT_LEVEL_PRECISION_MASK_V4
        private const byte SensorMultilevelReportLevelPrecisionShiftV4 = 0x05;	// SENSOR_MULTILEVEL_REPORT_LEVEL_PRECISION_SHIFT_V4

        [Test]
        public void GetCommandClass()
        {
            var commandClass = new SensorMultilevel();
            var classId = (byte)commandClass.GetClassId();

            Assert.That(classId, Is.EqualTo(CommandClassSensorMultilevel));
        }

        [Test]
        public void GetEvent_ParsesReportCommand()
        {
            var commandClass = new SensorMultilevel();
            var message = new byte[] { CommandClassSensorMultilevel, SensorMultilevelReportV4, SensorMultilevelReportLuminanceVersion1V4, 0x4A, 0x13, 0x88 };

            var nodeEvent = commandClass.GetEvent(Mock.Of<IZWaveNode>(), message);

            Assert.That(nodeEvent.Parameter, Is.EqualTo(EventParameter.SensorLuminance));
            Assert.That(nodeEvent.Value, Is.EqualTo(50));
        }

        [Test]
        public void GetEvent_ReturnsSensorGeneric_UnknownSensorType()
        {
            var commandClass = new SensorMultilevel();
            var message = new byte[] { CommandClassSensorMultilevel, SensorMultilevelReportV4, 0x98, 0x4A, 0x13, 0x88 };

            var nodeEvent = commandClass.GetEvent(Mock.Of<IZWaveNode>(), message);

            Assert.That(nodeEvent.Parameter, Is.EqualTo(EventParameter.SensorGeneric));
            Assert.That(nodeEvent.Value, Is.EqualTo(50));
        }

        [Test]
        public void GetEvent_ThrowsExceptionFor_UnknownCommand()
        {
            var commandClass = new SensorMultilevel();
            var message = new byte[] { CommandClassSensorMultilevel, 0xEE };
            
            Assert.That(() => commandClass.GetEvent(Mock.Of<IZWaveNode>(), message), Throws.TypeOf<UnsupportedCommandException>());
        }

        [Test]
        public void Get_Returns_ValidMessage()
        {
            var node = new Mock<IZWaveNode>();

            SensorMultilevel.Get(node.Object);

            var expectedMessage = new[] { CommandClassSensorMultilevel, SensorMultilevelGetV4 };
            node.Verify(x => x.SendDataRequest(It.Is<byte[]>(bytes => bytes.SequenceEqual(expectedMessage))));
        }

        [Test]
        public void GetV5_Returns_ValidMessage()
        {
            var node = new Mock<IZWaveNode>();

            SensorMultilevel.Get(node.Object, ZWaveSensorType.Ultraviolet);

            var expectedMessage = new byte[] { CommandClassSensorMultilevel, SensorMultilevelGetV4, 0x1B, 0x00 };
            node.Verify(x => x.SendDataRequest(It.Is<byte[]>(bytes => bytes.SequenceEqual(expectedMessage))));
        }
    }
}
