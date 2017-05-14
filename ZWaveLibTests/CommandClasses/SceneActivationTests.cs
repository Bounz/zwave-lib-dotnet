using Moq;
using NUnit.Framework;
using ZWaveLib;
using ZWaveLib.CommandClasses;
using ZWaveLib.Utilities;
using ZWaveLib.Values;

namespace ZWaveLibTests.CommandClasses
{
    [TestFixture]
    public class SceneActivationTests
    {
        /* 
         * Information about the constants is taken from
         * "Sigma Designs, SDS13740, Software Design Specification, Z-Wave Device and Command Class Types and Defines Specification" page 78
         * Scene Activation command class commands
         */

        private const byte CommandClassSceneActivation = 0x2B; // COMMAND_CLASS_SCENE_ACTIVATION

        /* Scene Activation command class commands */
        private const byte SceneActivationVersion = 0x01; // SCENE_ACTIVATION_VERSION
        private const byte SceneActivationSet = 0x01; // SCENE_ACTIVATION_SET

        [Test]
        public void GetCommandClass()
        {
            var commandClass = new SceneActivation();
            var classId = (byte)commandClass.GetClassId();

            Assert.That(classId, Is.EqualTo(CommandClassSceneActivation));
        }

        [Test]
        public void GetEvent_ParsesSetCommand()
        {
            var commandClass = new SceneActivation();
            var message = new byte[] { CommandClassSceneActivation, SceneActivationSet, 2, 50 };

            var nodeEvent = commandClass.GetEvent(Mock.Of<IZWaveNode>(), message);
            var value = nodeEvent.Value as SceneActivationValue;

            Assert.That(nodeEvent.Parameter, Is.EqualTo(EventParameter.SceneActivation));
            Assert.That(value, Is.Not.Null);
            Assert.That(value.SceneId, Is.EqualTo(2));
            Assert.That(value.DimmingDuration, Is.EqualTo(50));
        }

        [Test]
        public void GetEvent_ThrowsExceptionFor_UnknownCommand()
        {
            var commandClass = new SceneActivation();
            var message = new byte[] { CommandClassSceneActivation, 0xEE };
            
            Assert.That(() => commandClass.GetEvent(Mock.Of<IZWaveNode>(), message), Throws.TypeOf<UnsupportedCommandException>());
        }
    }
}
