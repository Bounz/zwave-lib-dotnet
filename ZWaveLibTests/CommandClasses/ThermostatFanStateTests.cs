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
    public class ThermostatFanStateTests
    {
        /* 
         * Information about the constants is taken from
         * "Sigma Designs, SDS13740, Software Design Specification, Z-Wave Device and Command Class Types and Defines Specification" page 103
         * Thermostat Fan State command class commands
         */

        private const byte CommandClassThermostatFanState = 0x45; // COMMAND_CLASS_THERMOSTAT_FAN_STATE

        /* Thermostat Fan State command class commands */
        private const byte ThermostatFanStateVersionV2 = 0x02;  // THERMOSTAT_FAN_STATE_VERSION_V2
        private const byte ThermostatFanStateGet = 0x02;      // THERMOSTAT_FAN_STATE_GET_V2
        private const byte ThermostatFanStateReport = 0x03;   // THERMOSTAT_FAN_STATE_REPORT_V2
        /* Values used for Thermostat Fan State Report command */
        private const byte ThermostatFanStateReportLevelFanOperatingStateMaskV2 = 0x0F;             // THERMOSTAT_FAN_STATE_REPORT_LEVEL_FAN_OPERATING_STATE_MASK_V2
        private const byte ThermostatFanStateReportFanOperatingStateIdleV2 = 0x00;                  // THERMOSTAT_FAN_STATE_REPORT_FAN_OPERATING_STATE_IDLE_V2
        private const byte ThermostatFanStateReportFanOperatingStateRunningV2 = 0x01;               // THERMOSTAT_FAN_STATE_REPORT_FAN_OPERATING_STATE_RUNNING_V2
        private const byte ThermostatFanStateReportFanOperatingStateRunningHighV2 = 0x02;           // THERMOSTAT_FAN_STATE_REPORT_FAN_OPERATING_STATE_RUNNING_HIGH_V2
        private const byte ThermostatFanStateReportFanOperatingStateRunningMediumV2 = 0x03;         // THERMOSTAT_FAN_STATE_REPORT_FAN_OPERATING_STATE_RUNNING_MEDIUM_V2
        private const byte ThermostatFanStateReportFanOperatingStateCirculationV2 = 0x04;           // THERMOSTAT_FAN_STATE_REPORT_FAN_OPERATING_STATE_CIRCULATION_V2
        private const byte ThermostatFanStateReportFanOperatingStateHumidityCirculationV2 = 0x05;   // THERMOSTAT_FAN_STATE_REPORT_FAN_OPERATING_STATE_HUMIDITY_CIRCULATION_V2
        private const byte ThermostatFanStateReportFanOperatingStateRightLeftCirculationV2 = 0x06;  // THERMOSTAT_FAN_STATE_REPORT_FAN_OPERATING_STATE_RIGHT_LEFT_CIRCULATION_V2
        private const byte ThermostatFanStateReportFanOperatingStateUpDownCirculationV2 = 0x07;     // THERMOSTAT_FAN_STATE_REPORT_FAN_OPERATING_STATE_UP_DOWN_CIRCULATION_V2
        private const byte ThermostatFanStateReportFanOperatingStateQuietCirculationV2 = 0x08;      // THERMOSTAT_FAN_STATE_REPORT_FAN_OPERATING_STATE_QUIET_CIRCULATION_V2
        private const byte ThermostatFanStateReportLevelReservedMaskV2 = 0xF0;                      // THERMOSTAT_FAN_STATE_REPORT_LEVEL_RESERVED_MASK_V2
        private const byte ThermostatFanStateReportLevelReservedShiftV2 = 0x04;                     // THERMOSTAT_FAN_STATE_REPORT_LEVEL_RESERVED_SHIFT_V2


        [Test]
        public void GetCommandClass()
        {
            var commandClass = new ThermostatFanState();
            var classId = (byte)commandClass.GetClassId();

            Assert.That(classId, Is.EqualTo(CommandClassThermostatFanState));
        }

        [Test]
        public void GetEvent_Parses_ReportCommand()
        {
            var commandClass = new ThermostatFanState();
            var message = new[] { CommandClassThermostatFanState, ThermostatFanStateReport, ThermostatFanStateReportFanOperatingStateRunningV2 };

            var nodeEvent = commandClass.GetEvent(new ZWaveNode(), message);

            Assert.That(nodeEvent, Is.Not.Null);
            Assert.That(nodeEvent.Parameter, Is.EqualTo(EventParameter.ThermostatFanState));
            Assert.That(nodeEvent.Value, Is.EqualTo(ThermostatFanState.FanState.Running));
        }

        [Test]
        public void GetEvent_ThrowsExceptionFor_UnknownCommand()
        {
            var commandClass = new ThermostatFanState();
            var message = new byte[] { CommandClassThermostatFanState, 0xEE };

            Assert.That(() => commandClass.GetEvent(new ZWaveNode(), message), Throws.Exception.TypeOf<UnsupportedCommandException>());
        }

        [Test]
        public void GetMessage()
        {
            var node = new Mock<IZWaveNode>();

            ThermostatFanState.Get(node.Object);

            var expectedMessage = new[] { CommandClassThermostatFanState, ThermostatFanStateGet };
            node.Verify(x => x.SendDataRequest(It.Is<byte[]>(msg => msg.SequenceEqual(expectedMessage))));
        }
        
    }
}
