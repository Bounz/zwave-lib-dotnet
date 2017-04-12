using System;
using Moq;
using NUnit.Framework;
using ZWaveLib;
using ZWaveLib.CommandClasses;

namespace ZWaveLibTests.CommandClasses
{
    [TestFixture]
    public class ThermostatSetbackTests
    {
        /*
         * Information about the constants is taken from
         * "Sigma Designs, SDS13740, Software Design Specification, Z-Wave Device and Command Class Types and Defines Specification" page 107
         * Thermostat Setback command class commands
         */

        private const byte CommandClassThermostatSetback = 0x47;   // COMMAND_CLASS_THERMOSTAT_SETBACK
        private const byte ThermostatSetbackVersion = 0x01;        // THERMOSTAT_SETBACK_VERSION
        private const byte ThermostatSetbackGet = 0x02;            // THERMOSTAT_SETBACK_GET
        private const byte ThermostatSetbackReport = 0x03;         // THERMOSTAT_SETBACK_REPORT
        private const byte ThermostatSetbackSet = 0x01;            // THERMOSTAT_SETBACK_SET


        [Test]
        public void GetCommandClass()
        {
            var commandClass = new ThermostatSetBack();
            var classId = (byte) commandClass.GetClassId();

            Assert.That(classId, Is.EqualTo(CommandClassThermostatSetback));
        }

        [Test]
        public void GetEvent_Parses_ReportCommand()
        {
            var commandClass = new ThermostatSetBack();
            var message = new byte[] { CommandClassThermostatSetback, ThermostatSetbackReport, 0x01, 0x02 };

            var nodeEvent = commandClass.GetEvent(new ZWaveNode(), message);

            Assert.That(nodeEvent, Is.Not.Null);
            Assert.That(nodeEvent.Parameter, Is.EqualTo(EventParameter.ThermostatSetBack));
            Assert.That(nodeEvent.Value, Is.EqualTo(0x01));
        }

        [Test]
        public void SetMessage()
        {
            var node = new Mock<IZWaveNode>();

            Assert.That(() => ThermostatSetBack.Set(node.Object), Throws.Exception.TypeOf<NotImplementedException>());

        }

        [Test]
        public void GetMessage()
        {
            var node = new Mock<IZWaveNode>();

            Assert.That(() => ThermostatSetBack.Set(node.Object), Throws.Exception.TypeOf<NotImplementedException>());
        }
    }
}
