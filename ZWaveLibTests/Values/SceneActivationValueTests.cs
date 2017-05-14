using NUnit.Framework;
using ZWaveLib.Values;

namespace ZWaveLibTests.Values
{
    [TestFixture]
    public class SceneActivationValueTests
    {
        [Test]
        public void Parse_Parses_Seconds()
        {
            const byte durationSeconds = 127;
            var message = new byte[] {0x0, 0x0, 2, durationSeconds};

            var parsedValue = SceneActivationValue.Parse(message);
            Assert.That(parsedValue.SceneId, Is.EqualTo(2));
            Assert.That(parsedValue.DimmingDuration, Is.EqualTo(durationSeconds));
        }

        [Test]
        public void Parse_Parses_Minutes()
        {
            const byte durationMinutes = 5;
            var message = new byte[] {0x0, 0x0, 2, 0x7F + durationMinutes};

            var parsedValue = SceneActivationValue.Parse(message);
            Assert.That(parsedValue.SceneId, Is.EqualTo(2));
            Assert.That(parsedValue.DimmingDuration, Is.EqualTo(durationMinutes * 60));
        }
    }
}
