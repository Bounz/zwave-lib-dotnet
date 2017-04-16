using System;
using System.Linq;
using Moq;
using NUnit.Framework;
using ZWaveLib;
using ZWaveLib.CommandClasses;
using ZWaveLib.Utilities;

namespace ZWaveLibTests.CommandClasses
{
    [TestFixture]
    public class ThermostatFanModeTests
    {
        /* 
         * Information about the constants is taken from
         * "Sigma Designs, SDS13740, Software Design Specification, Z-Wave Device and Command Class Types and Defines Specification" page 101-103
         * Thermostat Fan Mode command class commands
         */

        private const byte CommandClassThermostatFanMode = 0x44; // COMMAND_CLASS_THERMOSTAT_FAN_MODE

        /* Thermostat Fan Mode command class commands */
        private const byte ThermostatFanModeVersion = 0x01;         // THERMOSTAT_FAN_MODE_VERSION
        private const byte ThermostatFanModeGet = 0x02;             // THERMOSTAT_FAN_MODE_GET
        private const byte ThermostatFanModeReport = 0x03;          // THERMOSTAT_FAN_MODE_REPORT
        private const byte ThermostatFanModeSet = 0x01;             // THERMOSTAT_FAN_MODE_SET
        private const byte ThermostatFanModeSupportedGet = 0x04;    // THERMOSTAT_FAN_MODE_SUPPORTED_GET
        private const byte ThermostatFanModeSupportedReport = 0x05;	// THERMOSTAT_FAN_MODE_SUPPORTED_REPORT

        /* Values used for Thermostat Fan Mode Set command */
        private const byte ThermostatFanModeSetProperties1FanModeMask = 0x0F; // THERMOSTAT_FAN_MODE_SET_PROPERTIES1_FAN_MODE_MASK_V4
        private const byte ThermostatFanModeSetFanModeAutoLow = 0x00;         // THERMOSTAT_FAN_MODE_SET_FAN_MODE_AUTO_LOW_V4
        private const byte ThermostatFanModeSetFanModeLow = 0x01;             // THERMOSTAT_FAN_MODE_SET_FAN_MODE_LOW_V4
        private const byte ThermostatFanModeSetFanModeAutoHigh = 0x02;        // THERMOSTAT_FAN_MODE_SET_FAN_MODE_AUTO_HIGH_V4
        private const byte ThermostatFanModeSetFanModeHigh = 0x03;            // THERMOSTAT_FAN_MODE_SET_FAN_MODE_HIGH_V4
        private const byte ThermostatFanModeSetFanModeAutoMedium = 0x04;      // THERMOSTAT_FAN_MODE_SET_FAN_MODE_AUTO_MEDIUM_V4
        private const byte ThermostatFanModeSetFanModeMedium = 0x05;          // THERMOSTAT_FAN_MODE_SET_FAN_MODE_MEDIUM_V4
        private const byte ThermostatFanModeSetFanModeCirculation = 0x06;     // THERMOSTAT_FAN_MODE_SET_FAN_MODE_CIRCULATION_V4
        private const byte ThermostatFanModeSetFanModeHumidity = 0x07;        // THERMOSTAT_FAN_MODE_SET_FAN_MODE_HUMIDITY_V4
        private const byte ThermostatFanModeSetFanModeLeftRight = 0x08;       // THERMOSTAT_FAN_MODE_SET_FAN_MODE_LEFT_RIGHT_V4
        private const byte ThermostatFanModeSetFanModeUpDown = 0x09;          // THERMOSTAT_FAN_MODE_SET_FAN_MODE_UP_DOWN_V4
        private const byte ThermostatFanModeSetFanModeQuiet = 0x0A;           // THERMOSTAT_FAN_MODE_SET_FAN_MODE_QUIET_V4

        /* Values used for Thermostat Fan Mode Report command */
        private const byte ThermostatFanModeReportProperties1FanModeMaskV4 = 0x0F;  // THERMOSTAT_FAN_MODE_REPORT_PROPERTIES1_FAN_MODE_MASK_V4
        private const byte ThermostatFanModeReportFanModeAutoLowV4 = 0x00;  // THERMOSTAT_FAN_MODE_REPORT_FAN_MODE_AUTO_LOW_V4
        private const byte ThermostatFanModeReportFanModeLowV4 = 0x01;   // THERMOSTAT_FAN_MODE_REPORT_FAN_MODE_LOW_V4
        private const byte ThermostatFanModeReportFanModeAutoHighV4 = 0x02; // THERMOSTAT_FAN_MODE_REPORT_FAN_MODE_AUTO_HIGH_V4
        private const byte ThermostatFanModeReportFanModeHighV4 = 0x03;  // THERMOSTAT_FAN_MODE_REPORT_FAN_MODE_HIGH_V4
        private const byte ThermostatFanModeReportFanModeAutoMediumV4 = 0x04;   // THERMOSTAT_FAN_MODE_REPORT_FAN_MODE_AUTO_MEDIUM_V4
        private const byte ThermostatFanModeReportFanModeMediumV4 = 0x05;    // THERMOSTAT_FAN_MODE_REPORT_FAN_MODE_MEDIUM_V4
        private const byte ThermostatFanModeReportFanModeCirculationV4 = 0x06;   // THERMOSTAT_FAN_MODE_REPORT_FAN_MODE_CIRCULATION_V4
        private const byte ThermostatFanModeReportFanModeHumidityV4 = 0x07;  // THERMOSTAT_FAN_MODE_REPORT_FAN_MODE_HUMIDITY_V4
        private const byte ThermostatFanModeReportFanModeLeftRightV4 = 0x08;    // THERMOSTAT_FAN_MODE_REPORT_FAN_MODE_LEFT_RIGHT_V4
        private const byte ThermostatFanModeReportFanModeUpDownV4 = 0x09;   // THERMOSTAT_FAN_MODE_REPORT_FAN_MODE_UP_DOWN_V4
        private const byte ThermostatFanModeReportFanModeQuietV4 = 0x0A; // THERMOSTAT_FAN_MODE_REPORT_FAN_MODE_QUIET_V4
        private const byte ThermostatFanModeReportFanModeReservedbV4 = 0x0B; // THERMOSTAT_FAN_MODE_REPORT_FAN_MODE_RESERVEDB_V4
        private const byte ThermostatFanModeReportFanModeReservedcV4 = 0x0C; // THERMOSTAT_FAN_MODE_REPORT_FAN_MODE_RESERVEDC_V4
        private const byte ThermostatFanModeReportFanModeReserveddV4 = 0x0D; // THERMOSTAT_FAN_MODE_REPORT_FAN_MODE_RESERVEDD_V4
        private const byte ThermostatFanModeReportFanModeReservedeV4 = 0x0E; // THERMOSTAT_FAN_MODE_REPORT_FAN_MODE_RESERVEDE_V4
        private const byte ThermostatFanModeReportFanModeReservedfV4 = 0x0F; // THERMOSTAT_FAN_MODE_REPORT_FAN_MODE_RESERVEDF_V4
        private const byte ThermostatFanModeReportProperties1ReservedMaskV4 = 0x70;  // THERMOSTAT_FAN_MODE_REPORT_PROPERTIES1_RESERVED_MASK_V4
        private const byte ThermostatFanModeReportProperties1ReservedShiftV4 = 0x04; // THERMOSTAT_FAN_MODE_REPORT_PROPERTIES1_RESERVED_SHIFT_V4
        private const byte ThermostatFanModeReportProperties1OffBitMaskV4 = 0x80;	// THERMOSTAT_FAN_MODE_REPORT_PROPERTIES1_OFF_BIT_MASK_V4


        [Test]
        public void GetCommandClass()
        {
            var commandClass = new ThermostatFanMode();
            var classId = (byte)commandClass.GetClassId();

            Assert.That(classId, Is.EqualTo(CommandClassThermostatFanMode));
        }

        [Test]
        public void GetEvent_Parses_ReportCommand()
        {
            var commandClass = new ThermostatFanMode();
            var message = new[] { CommandClassThermostatFanMode, ThermostatFanModeReport, ThermostatFanModeSetFanModeLow };

            var nodeEvent = commandClass.GetEvent(new ZWaveNode(), message);

            Assert.That(nodeEvent, Is.Not.Null);
            Assert.That(nodeEvent.Parameter, Is.EqualTo(EventParameter.ThermostatFanMode));
            Assert.That(nodeEvent.Value, Is.EqualTo(ThermostatFanMode.FanMode.Low));
        }

        [Test]
        public void GetEvent_ReturnsNull_ForSupportedReportCommand()
        {
            var commandClass = new ThermostatFanMode();
            var message = new[] { CommandClassThermostatFanMode, ThermostatFanModeSupportedReport };

            var nodeEvent = commandClass.GetEvent(new ZWaveNode(), message);

            Assert.That(nodeEvent, Is.Null);
        }

        [Test]
        public void GetEvent_ThrowsExceptionFor_UnknownCommand()
        {
            var commandClass = new ThermostatFanMode();
            var message = new byte[] { CommandClassThermostatFanMode, 0xEE };

            Assert.That(() => commandClass.GetEvent(new ZWaveNode(), message), Throws.Exception.TypeOf<UnsupportedCommandException>());
        }

        [Test]
        [TestCase(ThermostatFanMode.FanMode.AutoLow, ThermostatFanModeSetFanModeAutoLow)]
        [TestCase(ThermostatFanMode.FanMode.Low, ThermostatFanModeSetFanModeLow)]
        [TestCase(ThermostatFanMode.FanMode.AutoHigh, ThermostatFanModeSetFanModeAutoHigh)]
        [TestCase(ThermostatFanMode.FanMode.High, ThermostatFanModeSetFanModeHigh)]
        [TestCase(ThermostatFanMode.FanMode.AutoMedium, ThermostatFanModeSetFanModeAutoMedium)]
        [TestCase(ThermostatFanMode.FanMode.Medium, ThermostatFanModeSetFanModeMedium)]
        [TestCase(ThermostatFanMode.FanMode.Circulation, ThermostatFanModeSetFanModeCirculation)]
        [TestCase(ThermostatFanMode.FanMode.Humidity, ThermostatFanModeSetFanModeHumidity)]
        [TestCase(ThermostatFanMode.FanMode.LeftRight, ThermostatFanModeSetFanModeLeftRight)]
        [TestCase(ThermostatFanMode.FanMode.UpDown, ThermostatFanModeSetFanModeUpDown)]
        [TestCase(ThermostatFanMode.FanMode.Quiet, ThermostatFanModeSetFanModeQuiet)]
        public void SetMessage(ThermostatFanMode.FanMode mode, byte modeValue)
        {
            var node = new Mock<IZWaveNode>();

            ThermostatFanMode.Set(node.Object, mode);

            var expectedMessage = new[] { CommandClassThermostatFanMode, ThermostatFanModeSet, modeValue };
            node.Verify(x => x.SendDataRequest(It.Is<byte[]>(msg => msg.SequenceEqual(expectedMessage))));
        }

        [Test]
        public void GetMessage()
        {
            var node = new Mock<IZWaveNode>();

            ThermostatFanMode.Get(node.Object);

            var expectedMessage = new[] { CommandClassThermostatFanMode, ThermostatFanModeGet };
            node.Verify(x => x.SendDataRequest(It.Is<byte[]>(msg => msg.SequenceEqual(expectedMessage))));
        }

        [Test]
        public void SupportedGetMessage()
        {
            var node = new Mock<IZWaveNode>();

            Assert.That(() => ThermostatFanMode.SupportedGet(node.Object), Throws.Exception.TypeOf<NotImplementedException>());
        }

        [Test]
        [Obsolete]
        public void ObsoleteSetMessage()
        {
            var node = new Mock<IZWaveNode>();

            ThermostatFanMode.Set(node.Object, ThermostatFanMode.Value.AutoLow);

            var expectedMessage = new[] { CommandClassThermostatFanMode, ThermostatFanModeSet, ThermostatFanModeSetFanModeAutoLow };
            node.Verify(x => x.SendDataRequest(It.Is<byte[]>(msg => msg.SequenceEqual(expectedMessage))));
        }
    }
}
