using System;
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
    public class SensorAlarmTests
    {
        /* 
         * Information about the constants is taken from
         * "Sigma Designs, SDS13740, Software Design Specification, Z-Wave Device and Command Class Types and Defines Specification" page 81
         * Switch Binary command class commands
         */

        private const byte CommandClassSensorAlarm = 0x9C; // COMMAND_CLASS_SENSOR_ALARM

        /* Sensor Alarm command class commands */
        private const byte SensorAlarmVersion = 0x01; // SENSOR_ALARM_VERSION
        private const byte SensorAlarmGet = 0x01; // SENSOR_ALARM_GET
        private const byte SensorAlarmReport = 0x02; // SENSOR_ALARM_REPORT
        private const byte SensorAlarmSupportedGet = 0x03; // SENSOR_ALARM_SUPPORTED_GET
        private const byte SensorAlarmSupportedReport = 0x04; // SENSOR_ALARM_SUPPORTED_REPORT

        [Test]
        public void GetCommandClass()
        {
            var commandClass = new SensorAlarm();
            var classId = (byte)commandClass.GetClassId();

            Assert.That(classId, Is.EqualTo(CommandClassSensorAlarm));
        }

        [Test]
        public void GetEvent_ParsesReportCommand()
        {
            var commandClass = new SensorAlarm();
            var message = new byte[] { CommandClassSensorAlarm, SensorAlarmReport, 1, 0x01, 0x64 };

            var nodeEvent = commandClass.GetEvent(Mock.Of<IZWaveNode>(), message);

            Assert.That(nodeEvent.Parameter, Is.EqualTo(EventParameter.AlarmSmoke));
            Assert.That(nodeEvent.Value, Is.EqualTo(100));
        }

        [Test]
        public void GetEvent_ReturnsAlarmGeneric_UnknownSensorType()
        {
            var commandClass = new SensorAlarm();
            var message = new byte[] { CommandClassSensorAlarm, SensorAlarmReport, 1, 0x51, 0x64 };

            var nodeEvent = commandClass.GetEvent(Mock.Of<IZWaveNode>(), message);

            Assert.That(nodeEvent.Parameter, Is.EqualTo(EventParameter.AlarmGeneric));
            Assert.That(nodeEvent.Value, Is.EqualTo(100));
        }

        [Test]
        public void GetEvent_ThrowsExceptionFor_SupportedReportCommand()
        {
            var commandClass = new SensorAlarm();
            var message = new[] { CommandClassSensorAlarm, SensorAlarmSupportedReport };

            Assert.That(() => commandClass.GetEvent(Mock.Of<IZWaveNode>(), message), Throws.TypeOf<NotImplementedException>());
        }

        [Test]
        public void GetEvent_ThrowsExceptionFor_UnknownCommand()
        {
            var commandClass = new SensorAlarm();
            var message = new byte[] { CommandClassSensorAlarm, 0xEE };
            
            Assert.That(() => commandClass.GetEvent(Mock.Of<IZWaveNode>(), message), Throws.TypeOf<UnsupportedCommandException>());
        }

        [Test]
        public void GetMessage()
        {
            var node = new Mock<IZWaveNode>();

            SensorAlarm.Get(node.Object, ZWaveAlarmType.CarbonDioxide);

            var expectedMessage = new byte[] { CommandClassSensorAlarm, SensorAlarmGet, 0x03 };
            node.Verify(x => x.SendDataRequest(It.Is<byte[]>(bytes => bytes.SequenceEqual(expectedMessage))));
        }

        [Test]
        public void GetSupportedMessage()
        {
            var node = new Mock<IZWaveNode>();

            SensorAlarm.SupportedGet(node.Object);

            var expectedMessage = new[] { CommandClassSensorAlarm, SensorAlarmSupportedGet };
            node.Verify(x => x.SendDataRequest(It.Is<byte[]>(bytes => bytes.SequenceEqual(expectedMessage))));
        }
    }
}
