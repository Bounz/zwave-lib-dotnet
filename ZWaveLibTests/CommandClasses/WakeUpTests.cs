using System.Linq;
using Moq;
using NUnit.Framework;
using ZWaveLib;
using ZWaveLib.CommandClasses;

namespace ZWaveLibTests.CommandClasses
{
    [TestFixture]
    public class WakeUpTests
    {
        /* 
         * Information about the constants is taken from
         * "Sigma Designs, SDS13740, Software Design Specification, Z-Wave Device and Command Class Types and Defines Specification" page 112
         * Wake Up command class commands          */

        private const byte CommandClassWakeUp = 0x84;       // COMMAND_CLASS_WAKE_UP
        private const byte WakeUpIntervalGet = 0x05;        // WAKE_UP_INTERVAL_GET
        private const byte WakeUpIntervalReport = 0x06;     // WAKE_UP_INTERVAL_REPORT
        private const byte WakeUpIntervalSet = 0x04;        // WAKE_UP_INTERVAL_SET
        private const byte WakeUpNoMoreInformation = 0x08;  // WAKE_UP_NO_MORE_INFORMATION
        private const byte WakeUpNotification = 0x07;       // WAKE_UP_NOTIFICATION

        private const byte ControllerNodeId = 0x01;

        [Test]
        public void GetCommandClass()
        {
            var commandClass = new WakeUp();
            var classId = (byte)commandClass.GetClassId();

            Assert.That(classId, Is.EqualTo(CommandClassWakeUp));
        }

        [Test]
        public void GetEvent_ParsesIntervalReportCommand_MalformedMessage()
        {
            var commandClass = new WakeUp();
            var message = new[] { CommandClassWakeUp, WakeUpIntervalReport };

            var nodeEvent = commandClass.GetEvent(new ZWaveNode(), message);

            Assert.That(nodeEvent, Is.Null);
        }

        [Test]
        public void GetEvent_ParsesIntervalReportCommand()
        {
            var commandClass = new WakeUp();
            var message = new byte[] { CommandClassWakeUp, WakeUpIntervalReport, 0x00, 0x00, 0x04 };

            var nodeEvent = commandClass.GetEvent(new ZWaveNode(), message);

            Assert.That(nodeEvent.Parameter, Is.EqualTo(EventParameter.WakeUpInterval));
            Assert.That(nodeEvent.Value, Is.EqualTo(4));
        }

        [Test]
        public void GetEvent_ParsesNotificationCommand()
        {
            var commandClass = new WakeUp();
            var message = new[] { CommandClassWakeUp, WakeUpNotification };
            var node = new Mock<IZWaveNode>();
            node.Setup(x => x.GetData("WakeUpStatus", null))
                .Returns(new NodeData("WakeUpStatus", new WakeUpStatus {IsSleeping = true}));

            var nodeEvent = commandClass.GetEvent(node.Object, message);

            Assert.That(nodeEvent.Parameter, Is.EqualTo(EventParameter.WakeUpNotify));
            Assert.That(nodeEvent.Value, Is.EqualTo(1));
            node.Verify(x => x.ResendQueuedMessages());
        }

        [Test]
        public void GetMessage()
        {
            var node = new Mock<IZWaveNode>();

            WakeUp.Get(node.Object);

            var expectedMessage = new[] { CommandClassWakeUp, WakeUpIntervalGet };
            node.Verify(x => x.SendDataRequest(It.Is<byte[]>(msg => msg.SequenceEqual(expectedMessage))));
        }

        [Test]
        public void SetMessage()
        {
            var node = new Mock<IZWaveNode>();

            WakeUp.Set(node.Object, 1000);

            var expectedMessage = new byte[] { CommandClassWakeUp, WakeUpIntervalSet, 0x00, 0x03, 0xe8, ControllerNodeId };
            node.Verify(x => x.SendDataRequest(It.Is<byte[]>(msg => msg.SequenceEqual(expectedMessage))));
        }

        [Test]
        public void SendToSleepMessage()
        {
            var node = new Mock<IZWaveNode>();
            node.Setup(x => x.GetData("WakeUpStatus", It.IsAny<object>()))
                .Returns(new NodeData("WakeUpStatus", new WakeUpStatus { IsSleeping = false }));

            node.Setup(x => x.SendDataRequest(It.IsAny<byte[]>()))
                .Returns(new ZWaveMessage(new byte[]{0x00, 0x00, 0x00}));

            WakeUp.SendToSleep(node.Object);

            var expectedMessage = new byte[] { CommandClassWakeUp, WakeUpNoMoreInformation, 0x25 };
            node.Verify(x => x.SendDataRequest(It.Is<byte[]>(msg => msg.SequenceEqual(expectedMessage))));
        }
    }
}
