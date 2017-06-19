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
    public class CentralSceneTests
    {
        /* 
         * Information about the constants is taken from
         * "Sigma Designs, SDS13740-1, Software Design Specification, Z-Wave Device and Command Class Types and Defines Specification" page 56
         * Climate Control Schedule command class commands
         */

        private const byte CommandClassCentralScene = 0x5B;         // COMMAND_CLASS_CENTRAL_SCENE

        /* Central Scene command class commands */
        private const byte CentralSceneVersionV2 = 0x02;            // CENTRAL_SCENE_VERSION_V2
        private const byte CentralSceneSupportedGetV2 = 0x01;       // CENTRAL_SCENE_SUPPORTED_GET_V2
        private const byte CentralSceneSupportedReportV2 = 0x02;    // CENTRAL_SCENE_SUPPORTED_REPORT_V2
        private const byte CentralSceneNotificationV2 = 0x03;       // CENTRAL_SCENE_NOTIFICATION_V2

        [Test]
        public void GetCommandClass()
        {
            var commandClass = new CentralScene();
            var classId = (byte)commandClass.GetClassId();

            Assert.That(classId, Is.EqualTo(CommandClassCentralScene));
        }
        
        [Test]
        public void GetEvent_ParsesNotificationCommand()
        {
            var commandClass = new CentralScene();
            var message = new byte[]
            {
                CommandClassCentralScene, CentralSceneNotificationV2,
                0x02, 0x06, 0x0A
            };

            var nodeEvent = commandClass.GetEvent(Mock.Of<IZWaveNode>(), message);
            var value = nodeEvent.Value as CentralSceneValue;

            Assert.That(nodeEvent.Parameter, Is.EqualTo(EventParameter.CentralSceneNotification));
            Assert.That(value, Is.Not.Null);
            Assert.That(value.SequenceNumber, Is.EqualTo(2));
            Assert.That(value.PressType, Is.EqualTo(CentralScenePressType.Pressed5Times));
            Assert.That(value.SceneNumber, Is.EqualTo(10));
        }

        [Test]
        public void GetEvent_ParsesSupportedReportCommand()
        {
            var commandClass = new CentralScene();
            var message = new byte[] { CommandClassCentralScene, CentralSceneSupportedReportV2, 0x0A };

            var nodeEvent = commandClass.GetEvent(Mock.Of<IZWaveNode>(), message);

            Assert.That(nodeEvent.Parameter, Is.EqualTo(EventParameter.CentralSceneSupportedReport));
            Assert.That(nodeEvent.Value, Is.Not.Null);
            Assert.That(nodeEvent.Value, Is.EqualTo(10));
        }

        [Test]
        public void GetEvent_ThrowsExceptionFor_UnknownCommand()
        {
            var commandClass = new CentralScene();
            var message = new byte[] { CommandClassCentralScene, 0xEE };
            
            Assert.That(() => commandClass.GetEvent(Mock.Of<IZWaveNode>(), message), Throws.TypeOf<UnsupportedCommandException>());
        }
        
        [Test]
        public void SuppoertedGet_Returns_ValidMessage()
        {
            var node = new Mock<IZWaveNode>();
            CentralScene.SupportedGet(node.Object);

            var expectedMessage = new[]
            {
                CommandClassCentralScene, CentralSceneSupportedGetV2
            };
            node.Verify(x => x.SendDataRequest(It.Is<byte[]>(bytes => bytes.SequenceEqual(expectedMessage))));
        }

        [Test]
        public void CentralSceneValue_ToString()
        {
            var value = new CentralSceneValue
            {
                SequenceNumber = 1,
                PressType = CentralScenePressType.HeldDown,
                SceneNumber = 3
            };

            var expectedMessage = "SceneId: 3, Level: 1, PressType: HeldDown";

            Assert.That(value.ToString(), Is.EqualTo(expectedMessage));
        }
    }
}
