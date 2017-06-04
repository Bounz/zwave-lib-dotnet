using System;
using NUnit.Framework;
using ZWaveLib.Values;

namespace ZWaveLibTests.Values
{
    [TestFixture]
    public class ZWaveValueTests
    {
        private const byte Precision = 2;
        private const byte Scale = 0; // Celsius
        private const byte Size = 2;
        private const double Value = 10.25; // 1025 = 0x0401

        [Test]
        public void GetValueBytes_Celsius()
        {
            var valueBytes = ZWaveValue.GetValueBytes(Value, Precision, Scale, Size);

            Assert.That(valueBytes, Is.EqualTo(new byte[] { 0x42, 0x04, 0x01 }));
        }

        [Test]
        public void GetValueBytes_Fahrenheit()
        {
            var valueBytes = ZWaveValue.GetValueBytes(50, Precision, 1, Size);

            Assert.That(valueBytes, Is.EqualTo(new byte[] { 0x4A, 0x13, 0x88 }));
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

        [Test]
        [TestCase(50, 1)]
        [TestCase(28000, 2)]
        [TestCase(33000, 4)]
        [TestCase(2147483000, 4)]
        public void GetValueBytes_SelectsRightSize(double value, int size)
        {
            var valueBytes = ZWaveValue.GetValueBytes(value, 0x00);

            Assert.That(valueBytes.Length, Is.EqualTo(size + 1));
        }

        [Test]
        public void GetValueBytes_ThrowsException_ValueIsTooBig()
        {
            Assert.That(()=> ZWaveValue.GetValueBytes(4147483000, 0x00), Throws.TypeOf<ArgumentOutOfRangeException>());
        }

        [Test]
        [TestCase(50, 0)]
        [TestCase(280.18, 2)]
        [TestCase(33000.5895, 4)]
        [TestCase(214.7483001, 7)]
        public void GetValueBytes_SelectsRightPrecision(double value, int precision)
        {
            var valueBytes = ZWaveValue.GetValueBytes(value, 0x00);
            var zWaveValue = ZWaveValue.ExtractValueFromBytes(valueBytes, 1);

            Assert.That(zWaveValue.Precision, Is.EqualTo(precision));
        }

        [Test]
        public void GetValueBytes_ThrowsException_PrecisionIsTooHigh()
        {
            Assert.That(() => ZWaveValue.GetValueBytes(1.12345678, 0x00), Throws.TypeOf<ArgumentOutOfRangeException>());
        }
    }
}
