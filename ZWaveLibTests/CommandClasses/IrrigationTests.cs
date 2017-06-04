using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using ZWaveLib;
using ZWaveLib.CommandClasses.Irrigation;
using ZWaveLib.Utilities;
using ZWaveLib.Values;

namespace ZWaveLibTests.CommandClasses
{
    [TestFixture]
    public class IrrigationTests
    {
        /* 
         * Information about the constants is taken from
         * "Sigma Designs, SDS13740, Software Design Specification, Z-Wave Device and Command Class Types and Defines Specification" page 125
         * Irrigation command class commands
         */

        private const byte CommandClassIrrigation = 0x6B; // COMMAND_CLASS_IRRIGATION

        /* Irrigation command class commands */
        private const byte IrrigationSystemInfoGet = 0x01; // IRRIGATION_SYSTEM_INFO_GET
        private const byte IrrigationSystemInfoReport = 0x02; // IRRIGATION_SYSTEM_INFO_REPORT
        private const byte IrrigationSystemStatusGet = 0x03; // IRRIGATION_SYSTEM_STATUS_GET
        private const byte IrrigationSystemStatusReport = 0x04; // IRRIGATION_SYSTEM_STATUS_REPORT
        private const byte IrrigationSystemConfigSet = 0x05; // IRRIGATION_SYSTEM_CONFIG_SET
        private const byte IrrigationSystemConfigGet = 0x06; // IRRIGATION_SYSTEM_CONFIG_GET
        private const byte IrrigationSystemConfigReport = 0x07; // IRRIGATION_SYSTEM_CONFIG_REPORT
        private const byte IrrigationValveInfoGet = 0x08; // IRRIGATION_VALVE_INFO_GET
        private const byte IrrigationValveInfoReport = 0x09; // IRRIGATION_VALVE_INFO_REPORT
        private const byte IrrigationValveConfigSet = 0x0A; // IRRIGATION_VALVE_CONFIG_SET
        private const byte IrrigationValveConfigGet = 0x0B; // IRRIGATION_VALVE_CONFIG_GET
        private const byte IrrigationValveConfigReport = 0x0C; // IRRIGATION_VALVE_CONFIG_REPORT
        private const byte IrrigationValveRun = 0x0D; // IRRIGATION_VALVE_RUN
        private const byte IrrigationValveTableSet = 0x0E; // IRRIGATION_VALVE_TABLE_SET
        private const byte IrrigationValveTableGet = 0x0F; // IRRIGATION_VALVE_TABLE_GET
        private const byte IrrigationValveTableReport = 0x10; // IRRIGATION_VALVE_TABLE_REPORT
        private const byte IrrigationValveTableRun = 0x11; // IRRIGATION_VALVE_TABLE_RUN
        private const byte IrrigationSystemShutoff = 0x12; // IRRIGATION_SYSTEM_SHUTOFF

        [Test]
        public void GetCommandClass()
        {
            var commandClass = new Irrigation();
            var classId = (byte) commandClass.GetClassId();

            Assert.That(classId, Is.EqualTo(CommandClassIrrigation));
        }

        [Test]
        public void GetEvent_Parses_SystemInfoReport()
        {
            var commandClass = new Irrigation();
            var message = new byte[] { CommandClassIrrigation, IrrigationSystemInfoReport, 0x01, 0x10, 0x05, 0x2 };

            var nodeEvent = commandClass.GetEvent(Mock.Of<IZWaveNode>(), message);
            var value = nodeEvent.Value as IrrigationSystemInfoReport;

            Assert.That(nodeEvent.Parameter, Is.EqualTo(EventParameter.IrrigationSystemInfoReport));
            Assert.That(value, Is.Not.Null);
            Assert.That(value.IsMasterValueSupported, Is.True);
            Assert.That(value.TotalNumberOfValves, Is.EqualTo(16));
            Assert.That(value.TotalNumberOfValveTables, Is.EqualTo(5));
            Assert.That(value.ValveTableMaxSize, Is.EqualTo(2));
        }

        [Test]
        public void GetEvent_Parses_SystemStatusReport()
        {
            const double flow = 5.25; // l/h
            const double pressure = 1.5; // bar

            var commandClass = new Irrigation();
            var commandBytes = new List<byte>
            {
                CommandClassIrrigation,
                IrrigationSystemStatusReport,
                110,
                0x03
            };
            var flowValueBytes = ZWaveValue.GetValueBytes(flow, 2, 0, 2);
            commandBytes.AddRange(flowValueBytes);
            var pressureValueBytes = ZWaveValue.GetValueBytes(pressure, 1, 0, 1);
            commandBytes.AddRange(pressureValueBytes);
            commandBytes.AddRange(new byte[] {0x02, 0x03, 0x00, 0x05});
            var message = commandBytes.ToArray();

            var nodeEvent = commandClass.GetEvent(Mock.Of<IZWaveNode>(), message);
            var value = nodeEvent.Value as IrrigationSystemStatusReport;

            Assert.That(nodeEvent.Parameter, Is.EqualTo(EventParameter.IrrigationSystemStatusReport));
            Assert.That(value, Is.Not.Null);
            Assert.That(value.SystemVoltage, Is.EqualTo(110));
            Assert.That(value.SensorStatus, Is.EqualTo(IrrigationSystemSensorStatusMask.PressureSensorActive | IrrigationSystemSensorStatusMask.FlowSensorActive));
            Assert.That(value.Flow, Is.EqualTo(flow));
            Assert.That(value.Pressure, Is.EqualTo(pressure));
            Assert.That(value.ShutoffDuration, Is.EqualTo(2));
            Assert.That(value.SystemErrorStatus, Is.EqualTo(IrrigationSystemErrorMask.NotProgrammed | IrrigationSystemErrorMask.EmergencyShutdows));
            Assert.That(value.IsMasterValveOpen, Is.False);
            Assert.That(value.ValveId, Is.EqualTo(5));
        }

        [Test]
        public void GetEvent_Parses_SystemConfigReport()
        {
            const double highThreshold = 7.9;
            const double lowThreshold = 1.23;

            var commandClass = new Irrigation();
            var commandBytes = new List<byte>
            {
                CommandClassIrrigation,
                IrrigationSystemConfigReport,
                3
            };
            var highThresholdValueBytes = ZWaveValue.GetValueBytes(highThreshold, 1, 0, 1);
            commandBytes.AddRange(highThresholdValueBytes);
            var lowThresholdValueBytes = ZWaveValue.GetValueBytes(lowThreshold, 2, 0, 1);
            commandBytes.AddRange(lowThresholdValueBytes);
            commandBytes.Add(0x00);
            var message = commandBytes.ToArray();

            var nodeEvent = commandClass.GetEvent(Mock.Of<IZWaveNode>(), message);
            var value = nodeEvent.Value as IrrigationSystemConfig;

            Assert.That(nodeEvent.Parameter, Is.EqualTo(EventParameter.IrrigationSystemConfigReport));
            Assert.That(value, Is.Not.Null);
            Assert.That(value.MasterValveDelay, Is.EqualTo(3));
            Assert.That(value.PressureHighThreshold, Is.EqualTo(highThreshold));
            Assert.That(value.PressureLowThreshold, Is.EqualTo(lowThreshold));
            Assert.That(value.SensorPolarity, Is.EqualTo(IrrigationSensorPolarityMask.None));
        }

        [Test]
        public void GetEvent_Parses_ValveInfoReport()
        {
            var commandClass = new Irrigation();
            var message = new byte[] { CommandClassIrrigation, IrrigationValveInfoReport, 0x02, 0x12, 0x05, 0x00 };

            var nodeEvent = commandClass.GetEvent(Mock.Of<IZWaveNode>(), message);
            var value = nodeEvent.Value as IrrigationValveInfo;

            Assert.That(nodeEvent.Parameter, Is.EqualTo(EventParameter.IrrigationValveInfoReport));
            Assert.That(value, Is.Not.Null);
            Assert.That(value.IsMasterValve, Is.False);
            Assert.That(value.IsConnected, Is.True);
            Assert.That(value.ValveId, Is.EqualTo(18));
            Assert.That(value.NominalCurrent, Is.EqualTo(5));
            Assert.That(value.ErrorStatus, Is.EqualTo(IrrigationValveErrorStatusMask.None));
        }

        [Test]
        public void GetEvent_Parses_ValveConfigReport()
        {
            const double maxFlow = 15.0;
            const double highThreshold = 7.9;
            const double lowThreshold = 1.23;

            var commandClass = new Irrigation();
            var commandBytes = new List<byte>
            {
                CommandClassIrrigation,
                IrrigationValveConfigReport,
                0x00,
                0x05,
                230,
                100
            };
            var maxFlowValueBytes = ZWaveValue.GetValueBytes(maxFlow, 0, 0, 1);
            commandBytes.AddRange(maxFlowValueBytes);
            var highThresholdValueBytes = ZWaveValue.GetValueBytes(highThreshold, 1, 0, 1);
            commandBytes.AddRange(highThresholdValueBytes);
            var lowThresholdValueBytes = ZWaveValue.GetValueBytes(lowThreshold, 2, 0, 1);
            commandBytes.AddRange(lowThresholdValueBytes);
            commandBytes.Add(0x02);
            var message = commandBytes.ToArray();

            var nodeEvent = commandClass.GetEvent(Mock.Of<IZWaveNode>(), message);
            var value = nodeEvent.Value as IrrigationValveConfig;

            Assert.That(nodeEvent.Parameter, Is.EqualTo(EventParameter.IrrigationValveConfigReport));
            Assert.That(value, Is.Not.Null);
            Assert.That(value.UseMasterValve, Is.False);
            Assert.That(value.ValveId, Is.EqualTo(5));
            Assert.That(value.NominalCurrentHighThreshold, Is.EqualTo(230));
            Assert.That(value.NominalCurrentLowThreshold, Is.EqualTo(100));
            Assert.That(value.MaximumFlow, Is.EqualTo(maxFlow));
            Assert.That(value.FlowHighThreshold, Is.EqualTo(highThreshold));
            Assert.That(value.FlowLowThreshold, Is.EqualTo(lowThreshold));
            Assert.That(value.UseRainSensor, Is.False);
            Assert.That(value.UseMoistureSensor, Is.True);
        }

        [Test]
        public void GetEvent_Parses_ValveTableReport()
        {
            const int duration1 = 54321;    // 0xD4 0x31
            const int duration2 = 123;      // 0x00 0x7B
            var commandClass = new Irrigation();
            
            var message = new byte[] { CommandClassIrrigation, IrrigationValveTableReport, 0x01,
                0x10, 0xD4, 0x31,
                0x11, 0x00, 0x7B };

            var nodeEvent = commandClass.GetEvent(Mock.Of<IZWaveNode>(), message);
            var value = nodeEvent.Value as IrrigationValveTable;

            Assert.That(nodeEvent.Parameter, Is.EqualTo(EventParameter.IrrigationValveTableReport));
            Assert.That(value, Is.Not.Null);
            Assert.That(value.TableId, Is.EqualTo(1));
            Assert.That(value.Items.Count, Is.EqualTo(2));
            Assert.That(value.Items[0].ValveId, Is.EqualTo(16));
            Assert.That(value.Items[0].Duration, Is.EqualTo(duration1));
            Assert.That(value.Items[1].ValveId, Is.EqualTo(17));
            Assert.That(value.Items[1].Duration, Is.EqualTo(duration2));
        }

        [Test]
        public void GetEvent_ThrowsExceptionFor_UnknownCommand()
        {
            var commandClass = new Irrigation();
            var message = new byte[] { CommandClassIrrigation, 0xEE };
            
            Assert.That(() => commandClass.GetEvent(Mock.Of<IZWaveNode>(), message), Throws.TypeOf<UnsupportedCommandException>());
        }

        [Test]
        public void SystemInfoGet_Returns_ValidMessage()
        {
            var node = new Mock<IZWaveNode>();

            Irrigation.SystemInfoGet(node.Object);

            var expectedMessage = new[] { CommandClassIrrigation, IrrigationSystemInfoGet };
            node.Verify(x => x.SendDataRequest(It.Is<byte[]>(bytes => bytes.SequenceEqual(expectedMessage))));
        }

        [Test]
        public void SystemStatusGet_Returns_ValidMessage()
        {
            var node = new Mock<IZWaveNode>();

            Irrigation.SystemStatusGet(node.Object);

            var expectedMessage = new[] { CommandClassIrrigation, IrrigationSystemStatusGet };
            node.Verify(x => x.SendDataRequest(It.Is<byte[]>(bytes => bytes.SequenceEqual(expectedMessage))));
        }

        [Test]
        public void SystemConfigSet_Returns_ValidMessage()
        {
            var node = new Mock<IZWaveNode>();
            var config = new IrrigationSystemConfig
            {
                MasterValveDelay = 15,
                MoistureSensorPolarity = 0,
                PressureHighThreshold = 7.9,
                PressureLowThreshold = 1.23,
                SensorPolarity = IrrigationSensorPolarityMask.MoistureSensorPolarity | IrrigationSensorPolarityMask.Valid
            };

            Irrigation.SystemConfigSet(node.Object, config);

            var expectedMessage = new byte[]
            {
                CommandClassIrrigation,
                IrrigationSystemConfigSet,
                0x0F,
                0x21, 0x4F,
                0x41, 0x7B,
                0x82
            };
            node.Verify(x => x.SendDataRequest(It.Is<byte[]>(bytes => bytes.SequenceEqual(expectedMessage))));
        }

        [Test]
        public void SystemConfigGet_Returns_ValidMessage()
        {
            var node = new Mock<IZWaveNode>();

            Irrigation.SystemConfigGet(node.Object);

            var expectedMessage = new[] { CommandClassIrrigation, IrrigationSystemConfigGet };
            node.Verify(x => x.SendDataRequest(It.Is<byte[]>(bytes => bytes.SequenceEqual(expectedMessage))));
        }

        [Test]
        public void ValveInfoGet_MasterValve_Returns_ValidMessage()
        {
            var node = new Mock<IZWaveNode>();

            Irrigation.ValveInfoGet(node.Object, 2, true);

            var expectedMessage = new byte[]
            {
                CommandClassIrrigation, IrrigationValveInfoGet,
                0x01, 0x01
            };
            node.Verify(x => x.SendDataRequest(It.Is<byte[]>(bytes => bytes.SequenceEqual(expectedMessage))));
        }

        [Test]
        public void ValveInfoGet_NonMasterValve_Returns_ValidMessage()
        {
            var node = new Mock<IZWaveNode>();

            Irrigation.ValveInfoGet(node.Object, 2);

            var expectedMessage = new byte[]
            {
                CommandClassIrrigation, IrrigationValveInfoGet,
                0x00, 0x02
            };
            node.Verify(x => x.SendDataRequest(It.Is<byte[]>(bytes => bytes.SequenceEqual(expectedMessage))));
        }

        [Test]
        public void ValveConfigSet_Returns_ValidMessage()
        {
            var node = new Mock<IZWaveNode>();
            var config = new IrrigationValveConfig
            {
                UseMasterValve = true,
                ValveId = 10,
                NominalCurrentHighThreshold = 230,
                NominalCurrentLowThreshold = 100,
                MaximumFlow = 7.9,
                FlowHighThreshold = 7.9,
                FlowLowThreshold = 1.23,
                UseRainSensor = true,
                UseMoistureSensor = true
            };

            Irrigation.ValveConfigSet(node.Object, config);

            var expectedMessage = new byte[]
            {
                CommandClassIrrigation,
                IrrigationValveConfigSet,
                0x01,
                0x01,
                0xE6,
                0x64,
                0x21, 0x4F,
                0x21, 0x4F,
                0x41, 0x7B,
                0x03
            };
            node.Verify(x => x.SendDataRequest(It.Is<byte[]>(bytes => bytes.SequenceEqual(expectedMessage))));
        }

        [Test]
        public void ValveConfigGet_MasterValve_Returns_ValidMessage()
        {
            var node = new Mock<IZWaveNode>();

            Irrigation.ValveConfigGet(node.Object, 3, true);

            var expectedMessage = new byte[]
            {
                CommandClassIrrigation, IrrigationValveConfigGet,
                0x01, 0x01
            };
            node.Verify(x => x.SendDataRequest(It.Is<byte[]>(bytes => bytes.SequenceEqual(expectedMessage))));
        }
        
        [Test]
        public void ValveRun_MasterValve_Returns_ValidMessage()
        {
            var node = new Mock<IZWaveNode>();

            Irrigation.ValveRun(node.Object, 3, true, 120);

            var expectedMessage = new byte[]
            {
                CommandClassIrrigation, IrrigationValveRun,
                0x01, 0x01, 0x00, 0x78
            };
            node.Verify(x => x.SendDataRequest(It.Is<byte[]>(bytes => bytes.SequenceEqual(expectedMessage))));
        }

        [Test]
        public void ValveTableSet_Returns_ValidMessage()
        {
            var node = new Mock<IZWaveNode>();
            var table = new IrrigationValveTable
            {
                TableId = 2,
                Items = new List<IrrigationValveTableItem>
                {
                    new IrrigationValveTableItem{ValveId = 1, Duration = 54321},
                    new IrrigationValveTableItem{ValveId = 2, Duration = 123}
                }
            };

            Irrigation.ValveTableSet(node.Object, table);

            var expectedMessage = new byte[]
            {
                CommandClassIrrigation,
                IrrigationValveTableSet,
                0x02,
                0x01, 0xD4, 0x31,
                0x02, 0x00, 0x7B
            };
            node.Verify(x => x.SendDataRequest(It.Is<byte[]>(bytes => bytes.SequenceEqual(expectedMessage))));
        }

        [Test]
        public void ValveTableGet_Returns_ValidMessage()
        {
            var node = new Mock<IZWaveNode>();

            Irrigation.ValveTableGet(node.Object, 2);

            var expectedMessage = new byte[]
            {
                CommandClassIrrigation, IrrigationValveTableGet, 0x02
            };
            node.Verify(x => x.SendDataRequest(It.Is<byte[]>(bytes => bytes.SequenceEqual(expectedMessage))));
        }

        [Test]
        public void ValveTableRun_Returns_ValidMessage()
        {
            var node = new Mock<IZWaveNode>();

            Irrigation.ValveTableRun(node.Object, new byte[] {1, 2, 3});

            var expectedMessage = new byte[]
            {
                CommandClassIrrigation, IrrigationValveTableRun,
                0x01, 0x02, 0x03
            };
            node.Verify(x => x.SendDataRequest(It.Is<byte[]>(bytes => bytes.SequenceEqual(expectedMessage))));
        }

        [Test]
        public void SystemShutoff_Returns_ValidMessage()
        {
            var node = new Mock<IZWaveNode>();

            Irrigation.SystemShutoff(node.Object, 5);

            var expectedMessage = new byte[]
            {
                CommandClassIrrigation, IrrigationSystemShutoff, 0x05
            };
            node.Verify(x => x.SendDataRequest(It.Is<byte[]>(bytes => bytes.SequenceEqual(expectedMessage))));
        }
    }
}
