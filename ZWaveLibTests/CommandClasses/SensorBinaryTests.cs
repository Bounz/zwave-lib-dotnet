using System;
using System.Linq;
using Moq;
using NUnit.Framework;
using ZWaveLib;
using ZWaveLib.CommandClasses;
using ZWaveLib.Utilities;

namespace ZWaveLibTests.CommandClasses
{
    [TestFixture]
    public class SensorBinaryTests
    {
        /* 
         * Information about the constants is taken from
         * "Sigma Designs, SDS13740, Software Design Specification, Z-Wave Device and Command Class Types and Defines Specification" page 81
         * Switch Binary command class commands
         */

        private const byte CommandClassSensorBinary = 0x30; // COMMAND_CLASS_SENSOR_BINARY

        /* Sensor Binary command class commands */
        private const byte SensorBinaryVersionV2 = 0x02; // SENSOR_BINARY_VERSION_V2
        private const byte SensorBinaryGetV2 = 0x02; // SENSOR_BINARY_GET_V2
        private const byte SensorBinaryReportV2 = 0x03; // SENSOR_BINARY_REPORT_V2
        private const byte SensorBinarySupportedGetSensorV2 = 0x01; // SENSOR_BINARY_SUPPORTED_GET_SENSOR_V2
        private const byte SensorBinarySupportedSensorReportV2 = 0x04; // SENSOR_BINARY_SUPPORTED_SENSOR_REPORT_V2
        /* Values used for Sensor Binary Report command */
        private const byte SensorBinaryReportIdleV2 = 0x00; // SENSOR_BINARY_REPORT_IDLE_V2
        private const byte SensorBinaryReportDetectedAnEventV2 = 0xFF; // SENSOR_BINARY_REPORT_DETECTED_AN_EVENT_V2

        [Test]
        public void GetCommandClass()
        {
            var commandClass = new SensorBinary();
            var classId = (byte)commandClass.GetClassId();

            Assert.That(classId, Is.EqualTo(CommandClassSensorBinary));
        }

        [Test]
        public void GetEvent_ParsesReportCommandV1()
        {
            var commandClass = new SensorBinary();
            var message = new[] { CommandClassSensorBinary, SensorBinaryReportV2, SensorBinaryReportIdleV2 };

            var nodeEvent = commandClass.GetEvent(Mock.Of<IZWaveNode>(), message);

            Assert.That(nodeEvent.Parameter, Is.EqualTo(EventParameter.SensorGeneric));
            Assert.That(nodeEvent.Value, Is.EqualTo(0));
        }

        [Test]
        [TestCase(0x01, EventParameter.SensorGeneric)]
        [TestCase(0x07, EventParameter.SensorGeneric)]
        [TestCase(0x09, EventParameter.SensorGeneric)]
        [TestCase(0x0B, EventParameter.SensorGeneric)]
        [TestCase(0x0D, EventParameter.SensorGeneric)]
        [TestCase(0x02, EventParameter.AlarmSmoke)]
        [TestCase(0x03, EventParameter.AlarmCarbonMonoxide)]
        [TestCase(0x04, EventParameter.AlarmCarbonDioxide)]
        [TestCase(0x05, EventParameter.AlarmHeat)]
        [TestCase(0x06, EventParameter.AlarmFlood)]
        [TestCase(0x08, EventParameter.AlarmTampered)]
        [TestCase(0x0A, EventParameter.AlarmDoorWindow)]
        [TestCase(0x0C, EventParameter.SensorMotion)]
        public void GetEvent_ParsesReportCommandV2(byte sensorType, EventParameter alarmType)
        {
            var commandClass = new SensorBinary();
            var message = new[] { CommandClassSensorBinary, SensorBinaryReportV2, SensorBinaryReportDetectedAnEventV2, sensorType };

            var nodeEvent = commandClass.GetEvent(Mock.Of<IZWaveNode>(), message);

            Assert.That(nodeEvent.Parameter, Is.EqualTo(alarmType));
            Assert.That(nodeEvent.Value, Is.EqualTo(255));
        }

        [Test]
        public void GetEvent_ReturnsSensorGeneric_UnknownSensorType()
        {
            var commandClass = new SensorBinary();
            var message = new byte[] { CommandClassSensorBinary, SensorBinaryReportV2, SensorBinaryReportDetectedAnEventV2, 0x53 };

            var nodeEvent = commandClass.GetEvent(Mock.Of<IZWaveNode>(), message);

            Assert.That(nodeEvent.Parameter, Is.EqualTo(EventParameter.SensorGeneric));
            Assert.That(nodeEvent.Value, Is.EqualTo(255));
        }

        [Test]
        public void GetEvent_ThrowsExceptionFor_SupportedReportCommand()
        {
            var commandClass = new SensorBinary();
            var message = new[] { CommandClassSensorBinary, SensorBinarySupportedSensorReportV2 };

            Assert.That(() => commandClass.GetEvent(Mock.Of<IZWaveNode>(), message), Throws.TypeOf<NotImplementedException>());
        }

        [Test]
        public void GetEvent_ThrowsExceptionFor_UnknownCommand()
        {
            var commandClass = new SensorBinary();
            var message = new byte[] { CommandClassSensorBinary, 0xEE };
            
            Assert.That(() => commandClass.GetEvent(Mock.Of<IZWaveNode>(), message), Throws.TypeOf<UnsupportedCommandException>());
        }

        [Test]
        public void GetMessage()
        {
            var node = new Mock<IZWaveNode>();

            SensorBinary.Get(node.Object);

            var expectedMessage = new[] { CommandClassSensorBinary, SensorBinaryGetV2 };
            node.Verify(x => x.SendDataRequest(It.Is<byte[]>(bytes => bytes.SequenceEqual(expectedMessage))));
        }

        [Test]
        public void GetSupportedMessage()
        {
            var node = new Mock<IZWaveNode>();

            SensorBinary.SupportedGet(node.Object);

            var expectedMessage = new[] { CommandClassSensorBinary, SensorBinarySupportedGetSensorV2 };
            node.Verify(x => x.SendDataRequest(It.Is<byte[]>(bytes => bytes.SequenceEqual(expectedMessage))));
        }
    }
}
