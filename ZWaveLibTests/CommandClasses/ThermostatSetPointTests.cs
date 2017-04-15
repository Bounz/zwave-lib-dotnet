using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using ZWaveLib;
using ZWaveLib.CommandClasses;
using ZWaveLib.Utilities;
using ZWaveLib.Values;

namespace ZWaveLibTests.CommandClasses
{
    [TestFixture]
    public class ThermostatSetpointTests
    {
        /* 
         * Information about the constants is taken from
         * "Sigma Designs, SDS13740, Software Design Specification, Z-Wave Device and Command Class Types and Defines Specification" page 107-110
         * Thermostat Setpoint command class commands
         */

        private const byte CommandClassThermostatSetpoint = 0x43;     // COMMAND_CLASS_THERMOSTAT_SETPOINT
        private const byte ThermostatSetpointVersion = 0x01;          // THERMOSTAT_SETPOINT_VERSION
        private const byte ThermostatSetpointGet = 0x02;              // THERMOSTAT_SETPOINT_GET
        private const byte ThermostatSetpointReport = 0x03;           // THERMOSTAT_SETPOINT_REPORT
        private const byte ThermostatSetpointSet = 0x01;              // THERMOSTAT_SETPOINT_SET
        private const byte ThermostatSetpointSupportedGet = 0x04;     // THERMOSTAT_SETPOINT_SUPPORTED_GET
        private const byte ThermostatSetpointSupportedReport = 0x05;  // THERMOSTAT_SETPOINT_SUPPORTED_REPORT

        private const double Tempreture = 10.25;
        private const byte Precision = 2;
        private const byte Scale = 0; // Celsius
        private const byte Size = 2;

        [Test]
        public void GetCommandClass()
        {
            var commandClass = new ThermostatSetPoint();
            var classId = (byte) commandClass.GetClassId();

            Assert.That(classId, Is.EqualTo(CommandClassThermostatSetpoint));
        }

        [Test]
        public void GetEvent_Parses_ReportCommand()
        {
            var commandClass = new ThermostatSetPoint();
            var valueBytes = ZWaveValue.GetValueBytes(Tempreture, Precision, Scale, Size);
            var messageBytes = new List<byte> { CommandClassThermostatSetpoint, ThermostatSetpointReport, 0x01 };
            messageBytes.AddRange(valueBytes);
            var message = messageBytes.ToArray();

            var nodeEvent = commandClass.GetEvent(new ZWaveNode(), message);
            var setpointValue = nodeEvent.Value as SetpointValue;

            Assert.That(nodeEvent, Is.Not.Null);
            Assert.That(setpointValue, Is.Not.Null);
            Assert.That(setpointValue.Type, Is.EqualTo(ThermostatSetPoint.SetpointType.Heating));
            Assert.That(setpointValue.Value, Is.EqualTo(Tempreture));
        }

        [Test]
        public void GetEvent_ReturnsNullFor_SupportedReportCommand()
        {
            var commandClass = new ThermostatSetPoint();
            var message = new[] { CommandClassThermostatSetpoint, ThermostatSetpointSupportedReport };
            
            var nodeEvent = commandClass.GetEvent(new ZWaveNode(), message);

            Assert.That(nodeEvent, Is.Null);
        }

        [Test]
        public void GetEvent_ThrowsExceptionFor_UnknownCommand()
        {
            var commandClass = new ThermostatSetPoint();
            var message = new byte[] { CommandClassThermostatSetpoint, 0xEE };

            Assert.That(() => commandClass.GetEvent(new ZWaveNode(), message), Throws.Exception.TypeOf<UnsupportedCommandException>());
        }

        [Test]
        public void SetMessage()
        {
            var node = new Mock<IZWaveNode>();
            node.Setup(x => x.GetData("SetPoint", It.IsAny<object>()))
                .Returns(new NodeData("SetPoint", new ZWaveValue(5, Precision, Scale, Size)));

            ThermostatSetPoint.Set(node.Object, ThermostatSetPoint.SetpointType.Heating, 10.25);

            var valueBytes = ZWaveValue.GetValueBytes(Tempreture, Precision, Scale, Size);
            var expectedMessage = new List<byte> { CommandClassThermostatSetpoint, ThermostatSetpointSet, 0x01 };
            expectedMessage.AddRange(valueBytes);

            node.Verify(x => x.SendDataRequest(It.Is<byte[]>(msg => msg.SequenceEqual(expectedMessage))));
        }

        [Test]
        public void GetMessage()
        {
            var node = new Mock<IZWaveNode>();
            
            ThermostatSetPoint.Get(node.Object, ThermostatSetPoint.SetpointType.Heating);

            var expectedMessage = new byte[] {CommandClassThermostatSetpoint, ThermostatSetpointGet, 0x01};
            node.Verify(x => x.SendDataRequest(It.Is<byte[]>(msg => msg.SequenceEqual(expectedMessage))));
        }

        [Test]
        public void SupportedGetMessage()
        {
            var node = new Mock<IZWaveNode>();
            
            Assert.That(() => ThermostatSetPoint.SupportedGet(node.Object), Throws.Exception.TypeOf<NotImplementedException>());
        }
    }
}
