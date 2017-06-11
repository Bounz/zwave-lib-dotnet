using System.Linq;
using Moq;
using NUnit.Framework;
using ZWaveLib;
using ZWaveLib.CommandClasses;
using ZWaveLib.Utilities;

namespace ZWaveLibTests.CommandClasses
{
    [TestFixture]
    public class ClimateControlScheduleTests
    {
        /* 
         * Information about the constants is taken from
         * "Sigma Designs, SDS13740-1, Software Design Specification, Z-Wave Device and Command Class Types and Defines Specification" page 56
         * Climate Control Schedule command class commands
         */

        private const byte CommandClassClimateControlSchedule = 0x46; // COMMAND_CLASS_CLIMATE_CONTROL_SCHEDULE

        /* Climate Control Schedule command class commands */
        private const byte ClimateControlScheduleVersion = 0x01;    // CLIMATE_CONTROL_SCHEDULE_VERSION
        private const byte ScheduleChangedGet = 0x04;               // SCHEDULE_CHANGED_GET
        private const byte ScheduleChangedReport = 0x05;            // SCHEDULE_CHANGED_REPORT
        private const byte ScheduleGet = 0x02;                      // SCHEDULE_GET
        private const byte ScheduleOverrideGet = 0x07;              // SCHEDULE_OVERRIDE_GET
        private const byte ScheduleOverrideReport = 0x08;           // SCHEDULE_OVERRIDE_REPORT
        private const byte ScheduleOverrideSet = 0x06;              // SCHEDULE_OVERRIDE_SET
        private const byte ScheduleReport = 0x03;                   // SCHEDULE_REPORT
        private const byte ScheduleSet = 0x01;                      // SCHEDULE_SET

        [Test]
        public void GetCommandClass()
        {
            var commandClass = new ClimateControlSchedule();
            var classId = (byte)commandClass.GetClassId();

            Assert.That(classId, Is.EqualTo(CommandClassClimateControlSchedule));
        }

        [Test]
        public void GetEvent_ParsesScheduleReportCommand()
        {
            var commandClass = new ClimateControlSchedule();
            var message = new byte[]
            {
                CommandClassClimateControlSchedule, ScheduleReport,
                0x01,
                5, 15, 20,
                5, 30, 0x80,
                5, 45, 0x79,
                6, 0, 0x7A,
                6, 15, 0x7F,
                0x00, 0x00, 0x7F,
                0x00, 0x00, 0x7F,
                0x00, 0x00, 0x7F,
                0x00, 0x00, 0x7F
            };

            var nodeEvent = commandClass.GetEvent(Mock.Of<IZWaveNode>(), message);
            var value = nodeEvent.Value as ClimateControlScheduleValue;

            Assert.That(nodeEvent.Parameter, Is.EqualTo(EventParameter.ClimateControlSchedule));
            Assert.That(value, Is.Not.Null);
            Assert.That(value.Weekday, Is.EqualTo(Weekday.Monday));

            Assert.That(value.Switchpoints[0].Hour, Is.EqualTo(5));
            Assert.That(value.Switchpoints[0].Minute, Is.EqualTo(15));
            Assert.That(value.Switchpoints[0].State.Setback, Is.Not.Null);
            Assert.That(value.Switchpoints[0].State.Setback, Is.EqualTo(20));

            Assert.That(value.Switchpoints[1].State.Setback, Is.Not.Null);
            Assert.That(value.Switchpoints[1].State.Setback, Is.EqualTo(-128));
            
            Assert.That(value.Switchpoints[2].State.Setback, Is.Null);
            Assert.That(value.Switchpoints[2].State.FrostProtectionMode, Is.True);
            Assert.That(value.Switchpoints[2].State.EnergySavingMode, Is.False);

            Assert.That(value.Switchpoints[3].State.Setback, Is.Null);
            Assert.That(value.Switchpoints[3].State.FrostProtectionMode, Is.False);
            Assert.That(value.Switchpoints[3].State.EnergySavingMode, Is.True);
            
            Assert.That(value.Switchpoints[4].State.Unused, Is.True);
            Assert.That(value.Switchpoints[4].State.FrostProtectionMode, Is.False);
            Assert.That(value.Switchpoints[4].State.EnergySavingMode, Is.False);
        }

        [Test]
        public void GetEvent_ParsesChangedReportCommand()
        {
            var commandClass = new ClimateControlSchedule();
            var message = new byte[] { CommandClassClimateControlSchedule, ScheduleChangedReport, 2 };

            var nodeEvent = commandClass.GetEvent(Mock.Of<IZWaveNode>(), message);

            Assert.That(nodeEvent.Parameter, Is.EqualTo(EventParameter.ClimateControlScheduleChanged));
            Assert.That(nodeEvent.Value, Is.Not.Null);
            Assert.That(nodeEvent.Value, Is.EqualTo(2));
        }

        [Test]
        public void GetEvent_ParsesOverrideReportCommand()
        {
            var commandClass = new ClimateControlSchedule();
            var message = new byte[]
            {
                CommandClassClimateControlSchedule, ScheduleOverrideReport,
                0x01, 50
            };

            var nodeEvent = commandClass.GetEvent(Mock.Of<IZWaveNode>(), message);
            var value = nodeEvent.Value as ClimateControlScheduleOverrideValue;

            Assert.That(nodeEvent.Parameter, Is.EqualTo(EventParameter.ClimateControlScheduleOverride));
            Assert.That(value, Is.Not.Null);
            Assert.That(value.OverrideType, Is.EqualTo(OverrideType.Temporary));
            Assert.That(value.ScheduleState.Setback, Is.EqualTo(50));
        }

        [Test]
        public void GetEvent_ThrowsExceptionFor_UnknownCommand()
        {
            var commandClass = new ClimateControlSchedule();
            var message = new byte[] { CommandClassClimateControlSchedule, 0xEE };
            
            Assert.That(() => commandClass.GetEvent(Mock.Of<IZWaveNode>(), message), Throws.TypeOf<UnsupportedCommandException>());
        }

        [Test]
        public void Set_Returns_ValidMessage()
        {
            var node = new Mock<IZWaveNode>();
            var schedule = new ClimateControlScheduleValue {Weekday = Weekday.Monday};
            schedule.Switchpoints[0].Hour = 5;
            schedule.Switchpoints[0].Minute = 15;
            schedule.Switchpoints[0].State = ScheduleStateValue.Parse(20);

            schedule.Switchpoints[1].Hour = 5;
            schedule.Switchpoints[1].Minute = 30;
            schedule.Switchpoints[1].State = ScheduleStateValue.Parse(128); //-128

            schedule.Switchpoints[2].Hour = 5;
            schedule.Switchpoints[2].Minute = 45;
            schedule.Switchpoints[2].State = ScheduleStateValue.Parse(121);

            schedule.Switchpoints[3].Hour = 6;
            schedule.Switchpoints[3].Minute = 00;
            schedule.Switchpoints[3].State = ScheduleStateValue.Parse(122);

            schedule.Switchpoints[4].Hour = 6;
            schedule.Switchpoints[4].Minute = 15;
            schedule.Switchpoints[4].State = ScheduleStateValue.Parse(127);

            schedule.Switchpoints[5].Hour = 6;
            schedule.Switchpoints[5].Minute = 15;
            schedule.Switchpoints[5].State = ScheduleStateValue.Parse(126);

            ClimateControlSchedule.Set(node.Object, schedule);

            var expectedMessage = new byte[]
            {
                CommandClassClimateControlSchedule, ScheduleSet,
                0x01,
                5, 15, 20,
                5, 30, 0x80,
                5, 45, 0x79,
                6, 0, 0x7A,
                6, 15, 0x7F,
                6, 15, 0x7E,
                0x00, 0x00, 0x7F,
                0x00, 0x00, 0x7F,
                0x00, 0x00, 0x7F
            };
            node.Verify(x => x.SendDataRequest(It.Is<byte[]>(bytes => bytes.SequenceEqual(expectedMessage))));
        }

        [Test]
        public void Get_Returns_ValidMessage()
        {
            var node = new Mock<IZWaveNode>();
            ClimateControlSchedule.Get(node.Object, Weekday.Tuesday);

            var expectedMessage = new byte[]
            {
                CommandClassClimateControlSchedule, ScheduleGet,
                0x02
            };
            node.Verify(x => x.SendDataRequest(It.Is<byte[]>(bytes => bytes.SequenceEqual(expectedMessage))));
        }

        [Test]
        public void ChangedGet_Returns_ValidMessage()
        {
            var node = new Mock<IZWaveNode>();
            ClimateControlSchedule.ChangedGet(node.Object);

            var expectedMessage = new[] {CommandClassClimateControlSchedule, ScheduleChangedGet};
            node.Verify(x => x.SendDataRequest(It.Is<byte[]>(bytes => bytes.SequenceEqual(expectedMessage))));
        }

        [Test]
        public void OverrideSet_Returns_ValidMessage_ForTemporaryOverride()
        {
            var node = new Mock<IZWaveNode>();
            var scheduleOverride = new ClimateControlScheduleOverrideValue
            {
                OverrideType = OverrideType.Temporary,
                ScheduleState = ScheduleStateValue.Parse(10)
            };
            ClimateControlSchedule.OverrideSet(node.Object, scheduleOverride);

            var expectedMessage = new byte[]
            {
                CommandClassClimateControlSchedule, ScheduleOverrideSet,
                0x01, 10
            };
            node.Verify(x => x.SendDataRequest(It.Is<byte[]>(bytes => bytes.SequenceEqual(expectedMessage))));
        }

        [Test]
        public void OverrideSet_Returns_ValidMessage_ForPermanentOverride()
        {
            var node = new Mock<IZWaveNode>();
            var scheduleOverride = new ClimateControlScheduleOverrideValue
            {
                OverrideType = OverrideType.Permenant,
                ScheduleState = ScheduleStateValue.Parse(10)
            };
            ClimateControlSchedule.OverrideSet(node.Object, scheduleOverride);

            var expectedMessage = new byte[]
            {
                CommandClassClimateControlSchedule, ScheduleOverrideSet,
                0x02, 10
            };
            node.Verify(x => x.SendDataRequest(It.Is<byte[]>(bytes => bytes.SequenceEqual(expectedMessage))));
        }

        [Test]
        public void OverrideSet_Returns_ValidMessage_ForNoOverride()
        {
            var node = new Mock<IZWaveNode>();
            var scheduleOverride = new ClimateControlScheduleOverrideValue
            {
                OverrideType = OverrideType.None,
                ScheduleState = ScheduleStateValue.Parse(10)
            };
            ClimateControlSchedule.OverrideSet(node.Object, scheduleOverride);

            var expectedMessage = new byte[]
            {
                CommandClassClimateControlSchedule, ScheduleOverrideSet,
                0x00, 10
            };
            node.Verify(x => x.SendDataRequest(It.Is<byte[]>(bytes => bytes.SequenceEqual(expectedMessage))));
        }

        [Test]
        public void OverrideGet_Returns_ValidMessage()
        {
            var node = new Mock<IZWaveNode>();
            ClimateControlSchedule.OverrideGet(node.Object);

            var expectedMessage = new[] { CommandClassClimateControlSchedule, ScheduleOverrideGet };
            node.Verify(x => x.SendDataRequest(It.Is<byte[]>(bytes => bytes.SequenceEqual(expectedMessage))));
        }

        [Test]
        public void ClimateControlScheduleValue_ToString()
        {
            var schedule = new ClimateControlScheduleValue { Weekday = Weekday.Monday };
            schedule.Switchpoints[0].Hour = 5;
            schedule.Switchpoints[0].Minute = 15;
            schedule.Switchpoints[0].State = ScheduleStateValue.Parse(20);

            var stringValue = schedule.ToString();

            var expectedMessage = "[ClimateControlScheduleValue: Weekday=Monday, Switchpoints: [" +
                                  "\r\n   Switchpoint: 0 [SwitchpointValue: Hour=5, Minute=15, State=[ScheduleStateValue: Setback=20, FrostProtectionMode=False, EnergySavingMode=False, Unused=False]]" +
                                  "\r\n   Switchpoint: 1 [SwitchpointValue: Hour=0, Minute=0, State=[ScheduleStateValue: Setback=, FrostProtectionMode=False, EnergySavingMode=False, Unused=True]]" +
                                  "\r\n   Switchpoint: 2 [SwitchpointValue: Hour=0, Minute=0, State=[ScheduleStateValue: Setback=, FrostProtectionMode=False, EnergySavingMode=False, Unused=True]]" +
                                  "\r\n   Switchpoint: 3 [SwitchpointValue: Hour=0, Minute=0, State=[ScheduleStateValue: Setback=, FrostProtectionMode=False, EnergySavingMode=False, Unused=True]]" +
                                  "\r\n   Switchpoint: 4 [SwitchpointValue: Hour=0, Minute=0, State=[ScheduleStateValue: Setback=, FrostProtectionMode=False, EnergySavingMode=False, Unused=True]]" +
                                  "\r\n   Switchpoint: 5 [SwitchpointValue: Hour=0, Minute=0, State=[ScheduleStateValue: Setback=, FrostProtectionMode=False, EnergySavingMode=False, Unused=True]]" +
                                  "\r\n   Switchpoint: 6 [SwitchpointValue: Hour=0, Minute=0, State=[ScheduleStateValue: Setback=, FrostProtectionMode=False, EnergySavingMode=False, Unused=True]]" +
                                  "\r\n   Switchpoint: 7 [SwitchpointValue: Hour=0, Minute=0, State=[ScheduleStateValue: Setback=, FrostProtectionMode=False, EnergySavingMode=False, Unused=True]]" +
                                  "\r\n   Switchpoint: 8 [SwitchpointValue: Hour=0, Minute=0, State=[ScheduleStateValue: Setback=, FrostProtectionMode=False, EnergySavingMode=False, Unused=True]]]";
            Assert.That(stringValue, Is.EqualTo(expectedMessage));
        }

        [Test]
        public void ClimateControlScheduleOverrideValue_ToString()
        {
            var scheduleOverride = new ClimateControlScheduleOverrideValue
            {
                OverrideType = OverrideType.Permenant,
                ScheduleState = ScheduleStateValue.Parse(10)
            };

            var stringValue = scheduleOverride.ToString();

            var expectedMessage = "[ClimateControlScheduleOverrideValue: OverrideType=Permenant, ScheduleState=[ScheduleStateValue: Setback=10, FrostProtectionMode=False, EnergySavingMode=False, Unused=False]]";
            Assert.That(stringValue, Is.EqualTo(expectedMessage));
        }
    }
}
