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
    public class ThermostatOperatingStateTests
    {
        /* 
         * Information about the constants is taken from
         * "Sigma Designs, SDS13740, Software Design Specification, Z-Wave Device and Command Class Types and Defines Specification" page 107-108
         * Thermostat Operating State command class commands
         */

        private const byte CommandClassThermostatOperatingState = 0x42; // COMMAND_CLASS_THERMOSTAT_OPERATING_STATE

        /* Thermostat Operating State command class commands */
        private const byte ThermostatOperatingStateVersionV2 = 0x02;    // THERMOSTAT_OPERATING_STATE_VERSION_V2

        private const byte ThermostatOperatingStateGet = 0x02;                  // THERMOSTAT_OPERATING_STATE_GET_V2
        private const byte ThermostatOperatingStateReport = 0x03;               // THERMOSTAT_OPERATING_STATE_REPORT_V2
        private const byte ThermostatOperatingStateLoggingSupportedGet = 0x01;  // THERMOSTAT_OPERATING_STATE_LOGGING_SUPPORTED_GET_V2
        private const byte ThermostatOperatingLoggingSupportedReport = 0x04;    // THERMOSTAT_OPERATING_LOGGING_SUPPORTED_REPORT_V2
        private const byte ThermostatOperatingStateLoggingGet = 0x05;           // THERMOSTAT_OPERATING_STATE_LOGGING_GET_V2
        private const byte ThermostatOperatingStateLoggingReport = 0x06;        // THERMOSTAT_OPERATING_STATE_LOGGING_REPORT_V2

        /* Values used for Thermostat Operating State Report command */
        private const byte ThermostatOperatingStateReportProperties1OperatingStateMask = 0x0F; //THERMOSTAT_OPERATING_STATE_REPORT_PROPERTIES1_OPERATING_STATE_MASK_V2
        private const byte ThermostatOperatingStateReportOperatingStateIdle = 0x00; //THERMOSTAT_OPERATING_STATE_REPORT_OPERATING_STATE_IDLE_V2
        private const byte ThermostatOperatingStateReportOperatingStateHeating = 0x01; //THERMOSTAT_OPERATING_STATE_REPORT_OPERATING_STATE_HEATING_V2
        private const byte ThermostatOperatingStateReportOperatingStateCooling = 0x02; //THERMOSTAT_OPERATING_STATE_REPORT_OPERATING_STATE_COOLING_V2
        private const byte ThermostatOperatingStateReportOperatingStateFanOnly = 0x03; //THERMOSTAT_OPERATING_STATE_REPORT_OPERATING_STATE_FAN_ONLY_V2
        private const byte ThermostatOperatingStateReportOperatingStatePendingHeat = 0x04; //THERMOSTAT_OPERATING_STATE_REPORT_OPERATING_STATE_PENDING_HEAT_V2
        private const byte ThermostatOperatingStateReportOperatingStatePendingCool = 0x05; //THERMOSTAT_OPERATING_STATE_REPORT_OPERATING_STATE_PENDING_COOL_V2
        private const byte ThermostatOperatingStateReportOperatingStateVentEconomizer = 0x06; //THERMOSTAT_OPERATING_STATE_REPORT_OPERATING_STATE_VENT_ECONOMIZER_V2
        private const byte ThermostatOperatingStateReportOperatingStateAuxHeating = 0x07; //THERMOSTAT_OPERATING_STATE_REPORT_OPERATING_STATE_AUX_HEATING_V2
        private const byte ThermostatOperatingStateReportOperatingState_2NdStageHeating = 0x08; //THERMOSTAT_OPERATING_STATE_REPORT_OPERATING_STATE_2ND_STAGE_HEATING_V2
        private const byte ThermostatOperatingStateReportOperatingState_2NdStageCooling = 0x09; //THERMOSTAT_OPERATING_STATE_REPORT_OPERATING_STATE_2ND_STAGE_COOLING_V2
        private const byte ThermostatOperatingStateReportOperatingState_2NdStageAuxHeat = 0x0A; //THERMOSTAT_OPERATING_STATE_REPORT_OPERATING_STATE_2ND_STAGE_AUX_HEAT_V2
        private const byte ThermostatOperatingStateReportOperatingState_3RdStageAuxHeat = 0x0B; //THERMOSTAT_OPERATING_STATE_REPORT_OPERATING_STATE_3RD_STAGE_AUX_HEAT_V2
        private const byte ThermostatOperatingStateReportProperties1ReservedMaskV2 = 0xF0; //THERMOSTAT_OPERATING_STATE_REPORT_PROPERTIES1_RESERVED_MASK_V2
        private const byte ThermostatOperatingStateReportProperties1ReservedShiftV2 = 0x04; //THERMOSTAT_OPERATING_STATE_REPORT_PROPERTIES1_RESERVED_SHIFT_V2

        [Test]
        public void GetCommandClass()
        {
            var commandClass = new ThermostatOperatingState();
            var classId = (byte) commandClass.GetClassId();

            Assert.That(classId, Is.EqualTo(CommandClassThermostatOperatingState));
        }

        [Test]
        public void GetEvent_Parses_ReportCommand()
        {
            var commandClass = new ThermostatOperatingState();
            var message = new[] {CommandClassThermostatOperatingState, ThermostatOperatingStateReport, ThermostatOperatingStateReportOperatingStateCooling};

            var nodeEvent = commandClass.GetEvent(new ZWaveNode(), message);
           
            Assert.That(nodeEvent, Is.Not.Null);
            Assert.That(nodeEvent.Parameter, Is.EqualTo(EventParameter.ThermostatOperatingState));
            Assert.That(nodeEvent.Value, Is.EqualTo(ThermostatOperatingState.OperatingState.Cooling));
        }

        [Test]
        public void GetEvent_ReturnsNullFor_LoggingSupportedReportCommand()
        {
            var commandClass = new ThermostatOperatingState();
            var message = new[] { CommandClassThermostatOperatingState, ThermostatOperatingLoggingSupportedReport };
            
            var nodeEvent = commandClass.GetEvent(new ZWaveNode(), message);

            Assert.That(nodeEvent, Is.Null);
        }

        [Test]
        public void GetEvent_ReturnsNullFor_LoggingReportCommand()
        {
            var commandClass = new ThermostatOperatingState();
            var message = new[] { CommandClassThermostatOperatingState, ThermostatOperatingStateLoggingReport };

            var nodeEvent = commandClass.GetEvent(new ZWaveNode(), message);

            Assert.That(nodeEvent, Is.Null);
        }

        [Test]
        public void GetEvent_ThrowsExceptionFor_UnknownCommand()
        {
            var commandClass = new ThermostatOperatingState();
            var message = new byte[] { CommandClassThermostatOperatingState, 0xEE };
            
            Assert.That(() => commandClass.GetEvent(new ZWaveNode(), message), Throws.Exception.TypeOf<UnsupportedCommandException>());
        }
        
        [Test]
        public void GetMessage()
        {
            var node = new Mock<IZWaveNode>();
            
            ThermostatOperatingState.Get(node.Object);

            var expectedMessage = new[] {CommandClassThermostatOperatingState, ThermostatOperatingStateGet};
            node.Verify(x => x.SendDataRequest(It.Is<byte[]>(msg => msg.SequenceEqual(expectedMessage))));
        }

        [Test]
        public void LoggingSupportedGetMessage()
        {
            var node = new Mock<IZWaveNode>();
            
            Assert.That(() => ThermostatOperatingState.LoggingSupportedGet(node.Object), Throws.Exception.TypeOf<NotImplementedException>());
        }

        [Test]
        public void LoggingGetMessage()
        {
            var node = new Mock<IZWaveNode>();

            Assert.That(() => ThermostatOperatingState.LoggingGet(node.Object), Throws.Exception.TypeOf<NotImplementedException>());
        }

        [Test]
        [Obsolete]
        public void GetOperatingStateMessage()
        {
            var node = new Mock<IZWaveNode>();

            ThermostatOperatingState.GetOperatingState(node.Object);

            var expectedMessage = new[] { CommandClassThermostatOperatingState, ThermostatOperatingStateGet };
            node.Verify(x => x.SendDataRequest(It.Is<byte[]>(msg => msg.SequenceEqual(expectedMessage))));
        }
    }
}
