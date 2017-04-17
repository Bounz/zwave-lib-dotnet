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
    public class SwitchBinaryTests
    {
        /* 
         * Information about the constants is taken from
         * "Sigma Designs, SDS13740, Software Design Specification, Z-Wave Device and Command Class Types and Defines Specification" page 99
         * Switch Binary command class commands
         */

        private const byte CommandClassSwitchBinary = 0x25; // COMMAND_CLASS_SWITCH_MULTILEVEL

        /* Switch Binary command class commands */
        private const byte SwitchBinaryVersionV2 = 0x02; // SWITCH_BINARY_VERSION_V2
        private const byte SwitchBinaryGet = 0x02; // SWITCH_BINARY_GET_V2
        private const byte SwitchBinaryReport = 0x03;  // SWITCH_BINARY_REPORT_V2
        private const byte SwitchBinarySet = 0x01; // SWITCH_BINARY_SET_V2
        /* Values used for Switch Binary Report command */
        private const byte SwitchBinaryReportOffDisableV2 = 0x00;  // SWITCH_BINARY_REPORT_OFF_DISABLE_V2
        private const byte SwitchBinaryReportOnEnableV2 = 0xFF;    // SWITCH_BINARY_REPORT_ON_ENABLE_V2
        private const byte SwitchBinaryReportAlreadyAtTheTargetValueV2 = 0x00;  // SWITCH_BINARY_REPORT_ALREADY_AT_THE_TARGET_VALUE_V2
        private const byte SwitchBinaryReportUnknownDurationV2 = 0xFE; // SWITCH_BINARY_REPORT_UNKNOWN_DURATION_V2
        private const byte SwitchBinaryReportReservedV2 = 0xFF; // SWITCH_BINARY_REPORT_RESERVED_V2
        /* Values used for Switch Binary Set command */
        private const byte SwitchBinarySetOffDisableV2 = 0x00; // SWITCH_BINARY_SET_OFF_DISABLE_V2
        private const byte SwitchBinarySetOnEnableV2 = 0xFF;   // SWITCH_BINARY_SET_ON_ENABLE_V2
        private const byte SwitchBinarySetInstantlyV2 = 0x00;   // SWITCH_BINARY_SET_INSTANTLY_V2
        private const byte SwitchBinarySetDefaultV2 = 0xFF;	// SWITCH_BINARY_SET_DEFAULT_V2

        [Test]
        public void GetCommandClass()
        {
            var commandClass = new SwitchBinary();
            var classId = (byte)commandClass.GetClassId();

            Assert.That(classId, Is.EqualTo(CommandClassSwitchBinary));
        }

        [Test]
        public void GetEvent_ParsesReportCommand()
        {
            var commandClass = new SwitchBinary();
            var message = new byte[] { CommandClassSwitchBinary, SwitchBinaryReport, 0x63 };

            var nodeEvent = commandClass.GetEvent(Mock.Of<IZWaveNode>(), message);

            Assert.That(nodeEvent.Parameter, Is.EqualTo(EventParameter.SwitchBinary));
            Assert.That(nodeEvent.Value, Is.EqualTo(99));
        }

        [Test]
        public void GetEvent_ParsesSetCommand()
        {
            var commandClass = new SwitchBinary();
            var message = new byte[] { CommandClassSwitchBinary, SwitchBinarySet, 0x62 };

            var nodeEvent = commandClass.GetEvent(Mock.Of<IZWaveNode>(), message);

            Assert.That(nodeEvent.Parameter, Is.EqualTo(EventParameter.SwitchBinary));
            Assert.That(nodeEvent.Value, Is.EqualTo(98));
        }

        [Test]
        public void GetEvent_ThrowsExceptionFor_UnknownCommand()
        {
            var commandClass = new SwitchBinary();
            var message = new byte[] { CommandClassSwitchBinary, 0xEE };
            
            Assert.That(() => commandClass.GetEvent(Mock.Of<IZWaveNode>(), message), Throws.TypeOf<UnsupportedCommandException>());
        }

        [Test]
        public void Set_ValidValue_ReturnsMessage()
        {
            var node = new Mock<IZWaveNode>();

            SwitchBinary.Set(node.Object, 50);

            var expectedMessage = new byte[] { CommandClassSwitchBinary, SwitchBinarySet, 50 };
            node.Verify(x => x.SendDataRequest(It.Is<byte[]>(bytes => bytes.SequenceEqual(expectedMessage))));
        }

        [Test]
        public void Set_InvalidValue_ThrowsException()
        {
            Assert.That(() => SwitchBinary.Set(Mock.Of<IZWaveNode>(), 155), Throws.TypeOf<ArgumentOutOfRangeException>());
        }

        [Test]
        public void GetMessage()
        {
            var node = new Mock<IZWaveNode>();

            SwitchBinary.Get(node.Object);

            var expectedMessage = new[] { CommandClassSwitchBinary, SwitchBinaryGet };
            node.Verify(x => x.SendDataRequest(It.Is<byte[]>(bytes => bytes.SequenceEqual(expectedMessage))));
        }
    }
}
