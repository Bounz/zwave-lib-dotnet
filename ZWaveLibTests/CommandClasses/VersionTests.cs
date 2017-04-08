using System.Linq;
using Moq;
using NUnit.Framework;
using ZWaveLib;
using ZWaveLib.CommandClasses;
using ZWaveLib.Values;

namespace ZWaveLibTests.CommandClasses
{
    [TestFixture]
    public class VersionTests
    {
        /* 
         * Information about the constants is taken from
         * "Sigma Designs, SDS13740, Software Design Specification, Z-Wave Device and Command Class Types and Defines Specification" page 112
         * Wake Up command class commands          */

        private const byte CommandClassVersion = 0x86;          // COMMAND_CLASS_VERSION
        private const byte VersionVersion = 0x01;               // VERSION_VERSION
        private const byte VersionCommandClassGet = 0x13;       // VERSION_COMMAND_CLASS_GET
        private const byte VersionCommandClassReport = 0x14;    // VERSION_COMMAND_CLASS_REPORT
        private const byte VersionGet = 0x11;                   // VERSION_GET
        private const byte VersionReport = 0x12;                // VERSION_REPORT
        

        [Test]
        public void GetCommandClass()
        {
            var commandClass = new Version();
            var classId = (byte) commandClass.GetClassId();

            Assert.That(classId, Is.EqualTo(CommandClassVersion));
        }

        [Test]
        public void GetEvent_ParsesReportCommand()
        {
            var commandClass = new Version();
            var message = new byte[] { CommandClassVersion, VersionReport, 0x01, 0x02, 0x03, 0x04, 0x05 };

            var nodeEvent = commandClass.GetEvent(new ZWaveNode(), message);
            var nodeVersion = nodeEvent.Value as NodeVersion;

            Assert.That(nodeEvent, Is.Not.Null);
            Assert.That(nodeVersion, Is.Not.Null);
            Assert.That(nodeVersion.LibraryType, Is.EqualTo(0x01));
            Assert.That(nodeVersion.ProtocolVersion, Is.EqualTo(0x02));
            Assert.That(nodeVersion.ProtocolSubVersion, Is.EqualTo(0x03));
            Assert.That(nodeVersion.ApplicationVersion, Is.EqualTo(0x04));
            Assert.That(nodeVersion.ApplicationSubVersion, Is.EqualTo(0x05));
        }

        [Test]
        public void GetEvent_ParsesCommandClassReportCommand()
        {
            var commandClass = new Version();
            var message = new byte[] { CommandClassVersion, VersionCommandClassReport, CommandClassVersion, 0x01 };
            var node = new Mock<IZWaveNode>();
            node.Setup(x => x.GetCommandClass(CommandClass.Version))
                .Returns(new NodeCommandClass(CommandClassVersion));

            var nodeEvent = commandClass.GetEvent(node.Object, message);
            var vesionValue = nodeEvent.Value as VersionValue;

            Assert.That(nodeEvent.Parameter, Is.EqualTo(EventParameter.VersionCommandClass));
            Assert.That(vesionValue, Is.Not.Null);
            Assert.That(vesionValue.CmdClass, Is.EqualTo(CommandClass.Version));
            Assert.That(vesionValue.Version, Is.EqualTo(0x01));
        }

        [Test]
        public void GetMessage()
        {
            var node = new Mock<IZWaveNode>();

            Version.CommandClassGet(node.Object, CommandClass.Version);

            var expectedMessage = new[] { CommandClassVersion, VersionCommandClassGet, CommandClassVersion };
            node.Verify(x => x.SendDataRequest(It.Is<byte[]>(msg => msg.SequenceEqual(expectedMessage))));
        }

        [Test]
        public void ReportMessage()
        {
            var node = new Mock<IZWaveNode>();

            Version.Get(node.Object);

            var expectedMessage = new[] { CommandClassVersion, VersionGet };
            node.Verify(x => x.SendDataRequest(It.Is<byte[]>(msg => msg.SequenceEqual(expectedMessage))));
        }
    }
}
