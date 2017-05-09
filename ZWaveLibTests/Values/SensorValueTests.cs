using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using ZWaveLib;
using ZWaveLib.Enums;
using ZWaveLib.Values;

namespace ZWaveLibTests.Values
{
    [TestFixture]
    public class SensorValueTests
    {
        private const byte Precision = 2;
        private const byte Scale = 0; // Celsius
        private const byte Size = 2;
        private const double Value = 10.25; // 1025 = 0x0401

        [Test]
        [TestCase(0x01, new byte[] { 0x42, 0x04, 0x01 }, 10.25, EventParameter.SensorTemperature, ZWaveSensorType.AirTemperature)]
        [TestCase(0x01, new byte[] { 0x4A, 0x13, 0x88 }, 10, EventParameter.SensorTemperature, ZWaveSensorType.AirTemperature)]
        public void Parse(byte type, byte[] valueMessage, double value, EventParameter eventType, ZWaveSensorType sensorType)
        {
            var messageList = new List<byte> {0x0, 0x0, type};
            messageList.AddRange(valueMessage);
            var message = messageList.ToArray();
            var sensorValue = SensorValue.Parse(message);

            Assert.That(sensorValue.EventType, Is.EqualTo(eventType));
            Assert.That(sensorValue.Parameter, Is.EqualTo(sensorType));
            Assert.That(sensorValue.Value, Is.EqualTo(value));
        }

        [Test]
        public void FahrenheitToCelsius()
        {
            var celsiusTemp = SensorValue.FahrenheitToCelsius(24.8);

            Assert.That(celsiusTemp, Is.EqualTo(-4));
        }
    }
}
