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
    public class ThermostatModeTests
    {
        /* 
         * Information about the constants is taken from
         * "Sigma Designs, SDS13740, Software Design Specification, Z-Wave Device and Command Class Types and Defines Specification" page 104-105
         * Thermostat Mode command class commands
         */

        private const byte CommandClassThermostatMode = 0x40; // COMMAND_CLASS_THERMOSTAT_OPERATING_STATE

        /* Thermostat Mode command class commands */
        private const byte ThermostatModeVersion = 0x01;

        private const byte ThermostatModeGet = 0x02;                // THERMOSTAT_MODE_GET
        private const byte ThermostatModeReport = 0x03;             // THERMOSTAT_MODE_REPORT
        private const byte ThermostatModeSet = 0x01;                // THERMOSTAT_MODE_SET
        private const byte ThermostatModeSupportedGet = 0x04;       // THERMOSTAT_MODE_SUPPORTED_GET
        private const byte ThermostatModeSupportedReport = 0x05;    // THERMOSTAT_MODE_SUPPORTED_REPORT

        /* Values used for Thermostat Mode Report command */
        private const byte ThermostatModeReportLevelModeMask = 0x1F;        // THERMOSTAT_MODE_REPORT_LEVEL_MODE_MASK
        private const byte ThermostatModeReportModeOff = 0x00;              // THERMOSTAT_MODE_REPORT_MODE_OFF
        private const byte ThermostatModeReportModeHeat = 0x01;             // THERMOSTAT_MODE_REPORT_MODE_HEAT
        private const byte ThermostatModeReportModeCool = 0x02;             // THERMOSTAT_MODE_REPORT_MODE_COOL
        private const byte ThermostatModeReportModeAuto = 0x03;             // THERMOSTAT_MODE_REPORT_MODE_AUTO
        private const byte ThermostatModeReportModeAuxiliaryHeat = 0x04;    // THERMOSTAT_MODE_REPORT_MODE_AUXILIARY_HEAT
        private const byte ThermostatModeReportModeResume = 0x05;           // THERMOSTAT_MODE_REPORT_MODE_RESUME
        private const byte ThermostatModeReportModeFanOnly = 0x06;          // THERMOSTAT_MODE_REPORT_MODE_FAN_ONLY
        private const byte ThermostatModeReportModeFurnace = 0x07;          // THERMOSTAT_MODE_REPORT_MODE_FURNACE
        private const byte ThermostatModeReportModeDryAir = 0x08;           // THERMOSTAT_MODE_REPORT_MODE_DRY_AIR
        private const byte ThermostatModeReportModeMoistAir = 0x09;         // THERMOSTAT_MODE_REPORT_MODE_MOIST_AIR
        private const byte ThermostatModeReportModeAutoChangeover = 0x0A;   // THERMOSTAT_MODE_REPORT_MODE_AUTO_CHANGEOVER
        private const byte ThermostatModeReportLevelReservedMask = 0xE0;    // THERMOSTAT_MODE_REPORT_LEVEL_RESERVED_MASK
        private const byte ThermostatModeReportLevelReservedShift = 0x05;   // THERMOSTAT_MODE_REPORT_LEVEL_RESERVED_SHIFT
        /* V3 */
        private const byte ThermostatModeReportModeEnergySaveHeatV3 = 0x0B; // THERMOSTAT_MODE_REPORT_MODE_ENERGY_SAVE_HEAT_V3
        private const byte ThermostatModeReportModeEnergySaveCoolV3 = 0x0C; // THERMOSTAT_MODE_REPORT_MODE_ENERGY_SAVE_COOL_V3
        private const byte ThermostatModeReportModeAwayV3 = 0x0D;           // THERMOSTAT_MODE_REPORT_MODE_AWAY_V3

        /* Values used for Thermostat Mode Set command */
        private const byte ThermostatModeSetLevelModeMask = 0x1F;           // THERMOSTAT_MODE_SET_LEVEL_MODE_MASK
        private const byte ThermostatModeSetModeOff = 0x00;                 // THERMOSTAT_MODE_SET_MODE_OFF
        private const byte ThermostatModeSetModeHeat = 0x01;                // THERMOSTAT_MODE_SET_MODE_HEAT
        private const byte ThermostatModeSetModeCool = 0x02;                // THERMOSTAT_MODE_SET_MODE_COOL
        private const byte ThermostatModeSetModeAuto = 0x03;                // THERMOSTAT_MODE_SET_MODE_AUTO
        private const byte ThermostatModeSetModeAuxiliaryHeat = 0x04;       // THERMOSTAT_MODE_SET_MODE_AUXILIARY_HEAT
        private const byte ThermostatModeSetModeResume = 0x05;              // THERMOSTAT_MODE_SET_MODE_RESUME
        private const byte ThermostatModeSetModeFanOnly = 0x06;             // THERMOSTAT_MODE_SET_MODE_FAN_ONLY
        private const byte ThermostatModeSetModeFurnace = 0x07;             // THERMOSTAT_MODE_SET_MODE_FURNACE
        private const byte ThermostatModeSetModeDryAir = 0x08;              // THERMOSTAT_MODE_SET_MODE_DRY_AIR
        private const byte ThermostatModeSetModeMoistAir = 0x09;            // THERMOSTAT_MODE_SET_MODE_MOIST_AIR
        private const byte ThermostatModeSetModeAutoChangeover = 0x0A;      // THERMOSTAT_MODE_SET_MODE_AUTO_CHANGEOVER
        private const byte ThermostatModeSetLevelReservedMask = 0xE0;       // THERMOSTAT_MODE_SET_LEVEL_RESERVED_MASK
        private const byte ThermostatModeSetLevelReservedShift = 0x05;      // THERMOSTAT_MODE_SET_LEVEL_RESERVED_SHIFT
        /* V3 */
        private const byte ThermostatModeSetModeEnergySaveHeatV3 = 0x0B;    // THERMOSTAT_MODE_SET_MODE_ENERGY_SAVE_HEAT_V3
        private const byte ThermostatModeSetModeEnergySaveCoolV3 = 0x0C;    // THERMOSTAT_MODE_SET_MODE_ENERGY_SAVE_COOL_V3
        private const byte ThermostatModeSetModeAwayV3 = 0x0D;              // THERMOSTAT_MODE_SET_MODE_AWAY_V3


        [Test]
        public void GetCommandClass()
        {
            var commandClass = new ThermostatMode();
            var classId = (byte) commandClass.GetClassId();

            Assert.That(classId, Is.EqualTo(CommandClassThermostatMode));
        }

        [Test]
        public void GetEvent_Parses_ReportCommand()
        {
            var commandClass = new ThermostatMode();
            var message = new[] {CommandClassThermostatMode, ThermostatModeReport, ThermostatModeReportModeAuto };

            var nodeEvent = commandClass.GetEvent(new ZWaveNode(), message);
           
            Assert.That(nodeEvent, Is.Not.Null);
            Assert.That(nodeEvent.Parameter, Is.EqualTo(EventParameter.ThermostatMode));
            Assert.That(nodeEvent.Value, Is.EqualTo(ThermostatMode.Mode.Auto));
        }

        [Test]
        public void GetEvent_ReturnsNull_ForSupportedReportCommand()
        {
            var commandClass = new ThermostatMode();
            var message = new[] { CommandClassThermostatMode, ThermostatModeSupportedReport };
            
            var nodeEvent = commandClass.GetEvent(new ZWaveNode(), message);

            Assert.That(nodeEvent, Is.Null);
        }

        [Test]
        public void GetEvent_ThrowsExceptionFor_UnknownCommand()
        {
            var commandClass = new ThermostatMode();
            var message = new byte[] { CommandClassThermostatMode, 0xEE };
            
            Assert.That(() => commandClass.GetEvent(new ZWaveNode(), message), Throws.Exception.TypeOf<UnsupportedCommandException>());
        }

        [Test]
        [TestCase(ThermostatMode.Mode.Off, ThermostatModeSetModeOff)]
        [TestCase(ThermostatMode.Mode.Heat, ThermostatModeSetModeHeat)]
        [TestCase(ThermostatMode.Mode.Cool, ThermostatModeSetModeCool)]
        [TestCase(ThermostatMode.Mode.Auto, ThermostatModeSetModeAuto)]
        [TestCase(ThermostatMode.Mode.AuxHeat, ThermostatModeSetModeAuxiliaryHeat)]
        [TestCase(ThermostatMode.Mode.Resume, ThermostatModeSetModeResume)]
        [TestCase(ThermostatMode.Mode.FanOnly, ThermostatModeSetModeFanOnly)]
        [TestCase(ThermostatMode.Mode.Furnace, ThermostatModeSetModeFurnace)]
        [TestCase(ThermostatMode.Mode.DryAir, ThermostatModeSetModeDryAir)]
        [TestCase(ThermostatMode.Mode.MoistAir, ThermostatModeSetModeMoistAir)]
        [TestCase(ThermostatMode.Mode.AutoChangeover, ThermostatModeSetModeAutoChangeover)]
        [TestCase(ThermostatMode.Mode.HeatEconomy, ThermostatModeSetModeEnergySaveHeatV3)]
        [TestCase(ThermostatMode.Mode.CoolEconomy, ThermostatModeSetModeEnergySaveCoolV3)]
        [TestCase(ThermostatMode.Mode.Away, ThermostatModeSetModeAwayV3)]
        public void SetMessage(ThermostatMode.Mode mode, byte modeValue)
        {
            var node = new Mock<IZWaveNode>();

            ThermostatMode.Set(node.Object, mode);

            var expectedMessage = new[] { CommandClassThermostatMode, ThermostatModeSet, modeValue };
            node.Verify(x => x.SendDataRequest(It.Is<byte[]>(msg => msg.SequenceEqual(expectedMessage))));
        }

        [Test]
        public void GetMessage()
        {
            var node = new Mock<IZWaveNode>();
            
            ThermostatMode.Get(node.Object);

            var expectedMessage = new[] {CommandClassThermostatMode, ThermostatModeGet};
            node.Verify(x => x.SendDataRequest(It.Is<byte[]>(msg => msg.SequenceEqual(expectedMessage))));
        }

        [Test]
        public void SupportedGetMessage()
        {
            var node = new Mock<IZWaveNode>();

            Assert.That(() => ThermostatMode.SupportedGet(node.Object), Throws.Exception.TypeOf<NotImplementedException>());
        }
    }
}
