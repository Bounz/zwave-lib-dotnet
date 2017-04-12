using NUnit.Framework;
using ZWaveLib.Values;

namespace ZWaveLibTests.CommandClasses
{
    [TestFixture]
    public class ZWaveValueTests
    {
        private const byte Precision = 2;
        private const byte Scale = 0; // Celsius
        private const byte Size = 2;
        private const double Value = 10.25; // 1025 = 0x0401

        [Test]
        public void GetValueBytes()
        {
            var valueBytes = ZWaveValue.GetValueBytes(Value, Precision, Scale, Size);

            Assert.That(valueBytes, Is.EqualTo(new byte[] { 0x42, 0x04, 0x01 }));
        }

        [Test]
        public void ExtractValueFromBytes()
        {
            var zWaveValue = ZWaveValue.ExtractValueFromBytes(new byte[] { 0x42, 0x04, 0x01 }, 1);

            Assert.That(zWaveValue.Value, Is.EqualTo(Value));
            Assert.That(zWaveValue.Precision, Is.EqualTo(Precision));
            Assert.That(zWaveValue.Scale, Is.EqualTo(Scale));
            Assert.That(zWaveValue.Size, Is.EqualTo(Size));
        }
    }
}
