using System.Linq;
using Moq;
using NUnit.Framework;
using ZWaveLib;
using ZWaveLib.CommandClasses;
using ZWaveLib.Utilities;

namespace ZWaveLibTests.CommandClasses
{
    [TestFixture]
    public class SwitchMultilevelTests
    {
        /* 
         * Information about the constants is taken from
         * "Sigma Designs, SDS13740, Software Design Specification, Z-Wave Device and Command Class Types and Defines Specification" page 99
         * Switch Multilevel command class commands
         */

        private const byte CommandClassSwitchMultilevel = 0x26; // COMMAND_CLASS_SWITCH_MULTILEVEL
        private const byte SwitchMultilevelSet = 0x01;          // SWITCH_MULTILEVEL_SET
        private const byte SwitchMultilevelGet = 0x02;          // SWITCH_MULTILEVEL_GET
        private const byte SwitchMultilevelReport = 0x03;       // SWITCH_MULTILEVEL_REPORT
        private const byte SwitchMultilevelStartLevelChange = 0x04; // SWITCH_MULTILEVEL_START_LEVEL_CHANGE
        private const byte SwitchMultilevelStopLevelChange = 0x05;	// SWITCH_MULTILEVEL_STOP_LEVEL_CHANGE

        [Test]
        public void GetCommandClass()
        {
            var commandClass = new SwitchMultilevel();
            var classId = (byte)commandClass.GetClassId();

            Assert.That(classId, Is.EqualTo(CommandClassSwitchMultilevel));
        }

        [Test]
        public void GetEvent_ReportCommand_ReturnsNodeEvent()
        {
            var commandClass = new SwitchMultilevel();
            var message = new byte[] { CommandClassSwitchMultilevel, SwitchMultilevelReport, 0x63};

            var nodeEvent = commandClass.GetEvent(new ZWaveNode(), message);

            Assert.That(nodeEvent.Parameter, Is.EqualTo(EventParameter.SwitchMultilevel));
            Assert.That(nodeEvent.Value, Is.EqualTo(99));
        }

        [Test]
        public void GetEvent_SetCommand_ReturnsNodeEvent()
        {
            var commandClass = new SwitchMultilevel();
            var message = new byte[] { CommandClassSwitchMultilevel, SwitchMultilevelSet, 0x62 };

            var nodeEvent = commandClass.GetEvent(new ZWaveNode(), message);

            Assert.That(nodeEvent.Parameter, Is.EqualTo(EventParameter.SwitchMultilevel));
            Assert.That(nodeEvent.Value, Is.EqualTo(98));
        }

        [Test]
        public void GetEvent_UnknownCommand_ThrowsException()
        {
            var commandClass = new SwitchMultilevel();
            var message = new byte[] { CommandClassSwitchMultilevel, 0xEE };

            Assert.That(() => commandClass.GetEvent(Mock.Of<IZWaveNode>(), message), Throws.TypeOf<UnsupportedCommandException>());
        }

        [Test]
        public void SetMessage()
        {
            var node = new Mock<IZWaveNode>();

            SwitchMultilevel.Set(node.Object, 50);

            var expectedMessage = new byte[] { CommandClassSwitchMultilevel, SwitchMultilevelSet, 50};
            node.Verify(x => x.SendDataRequest(It.Is<byte[]>(bytes => bytes.SequenceEqual(expectedMessage))));
        }

        [Test]
        public void GetMessage()
        {
            var node = new Mock<IZWaveNode>();

            SwitchMultilevel.Get(node.Object);

            var expectedMessage = new[] { CommandClassSwitchMultilevel, SwitchMultilevelGet };
            node.Verify(x => x.SendDataRequest(It.Is<byte[]>(bytes => bytes.SequenceEqual(expectedMessage))));
        }

        [Test]
        public void StartLevelChange_Message()
        {
            var node = new Mock<IZWaveNode>();

            SwitchMultilevel.StartLevelChange(node.Object, true, true, 0x80);

            var expectedMessage = new byte[] { CommandClassSwitchMultilevel, SwitchMultilevelStartLevelChange, 0x60, 0x80 };
            node.Verify(x => x.SendDataRequest(It.Is<byte[]>(bytes => bytes.SequenceEqual(expectedMessage))));
        }

        [Test]
        public void StopLevelChange_Message()
        {
            var node = new Mock<IZWaveNode>();

            SwitchMultilevel.StopLevelChange(node.Object);

            var expectedMessage = new[] { CommandClassSwitchMultilevel, SwitchMultilevelStopLevelChange };
            node.Verify(x => x.SendDataRequest(It.Is<byte[]>(bytes => bytes.SequenceEqual(expectedMessage))));
        }
    }
}
