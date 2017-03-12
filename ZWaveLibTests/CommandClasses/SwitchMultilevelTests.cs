using System.Linq;
using Moq;
using NUnit.Framework;
using ZWaveLib;
using ZWaveLib.CommandClasses;

namespace ZWaveLibTests.CommandClasses
{
    [TestFixture]
    public class SwitchMultilevelTests
    {
        /* See "Sigma Designs, SDS13740, Software Design Specification, Z-Wave Device and Command Class Types and Defines Specification" page 99
         * Switch Multilevel command class commands         */
        private const byte CommandClassSwitchMultilevel = 0x26; // COMMAND_CLASS_SWITCH_MULTILEVEL
        private const byte SwitchMultilevelSet = 0x01;          // SWITCH_MULTILEVEL_SET
        private const byte SwitchMultilevelGet = 0x02;          // SWITCH_MULTILEVEL_GET
        private const byte SwitchMultilevelReport = 0x03;       // SWITCH_MULTILEVEL_REPORT

        [Test]
        public void GetEvent_ParsesReportCommand()
        {
            var commandClass = new SwitchMultilevel();
            var message = new byte[] { CommandClassSwitchMultilevel, SwitchMultilevelReport, 0x63};

            var zWaveEvent = commandClass.GetEvent(new ZWaveNode(), message);

            Assert.That(zWaveEvent.Parameter, Is.EqualTo(EventParameter.SwitchMultilevel));
            Assert.That(zWaveEvent.Value, Is.EqualTo(99));
        }

        [Test]
        public void GetEvent_ParsesSetCommand()
        {
            var commandClass = new SwitchMultilevel();
            var message = new byte[] { CommandClassSwitchMultilevel, SwitchMultilevelSet, 0x62 };

            var zWaveEvent = commandClass.GetEvent(new ZWaveNode(), message);

            Assert.That(zWaveEvent.Parameter, Is.EqualTo(EventParameter.SwitchMultilevel));
            Assert.That(zWaveEvent.Value, Is.EqualTo(98));
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
        public void GetCommandClass()
        {
            var commandClass = new SwitchMultilevel();
            var classId = (byte)commandClass.GetClassId();

            Assert.That(classId, Is.EqualTo(CommandClassSwitchMultilevel));
        }
    }
}
