using System;
using System.Linq;
using Moq;
using NUnit.Framework;
using ZWaveLib;
using ZWaveLib.CommandClasses;
using ZWaveLib.Values;

namespace ZWaveLibTests.CommandClasses
{
    [TestFixture]
    public class UserCodeTests
    {
        /* 
         * Information about the constants is taken from
         * "Sigma Designs, SDS13740, Software Design Specification, Z-Wave Device and Command Class Types and Defines Specification" page 111
         * User Code command class commands
         */

        private const byte CommandClassUserCode = 0x63;     // COMMAND_CLASS_VERSION
        private const byte UserCodeVersion = 0x01;          // USER_CODE_VERSION
        private const byte UserCodeGet = 0x02;              // USER_CODE_GET
        private const byte UserCodeReport = 0x03;           // USER_CODE_REPORT
        private const byte UserCodeSet = 0x01;              // USER_CODE_SET
        private const byte UsersNumberGet = 0x04;           // USERS_NUMBER_GET
        private const byte UsersNumberReport = 0x05;        // USERS_NUMBER_REPORT

        /* 
         * Values used for User Code Report command 
         * #define USER_CODE_REPORT_AVAILABLE_NOT_SET 0x00
         * #define USER_CODE_REPORT_OCCUPIED 0x01
         * #define USER_CODE_REPORT_RESERVED_BY_ADMINISTRATOR 0x02
         * #define USER_CODE_REPORT_STATUS_NOT_AVAILABLE 0xFE
         * Values used for User Code Set command
         * #define USER_CODE_SET_AVAILABLE_NOT_SET 0x00
         * #define USER_CODE_SET_OCCUPIED 0x01
         * #define USER_CODE_SET_RESERVED_BY_ADMINISTRATOR 0x02
         * #define USER_CODE_SET_STATUS_NOT_AVAILABLE 0xFE
         */

        private readonly byte[] _tagCode = new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0A };

        [Test]
        public void GetCommandClass()
        {
            var commandClass = new UserCode();
            var classId = (byte) commandClass.GetClassId();

            Assert.That(classId, Is.EqualTo(CommandClassUserCode));
        }

        [Test]
        public void GetEvent_Parses_ReportCommand()
        {
            var commandClass = new UserCode();
            var message = new byte[] { CommandClassUserCode, UserCodeReport, 0x01, 0x01, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0A };

            var nodeEvent = commandClass.GetEvent(new ZWaveNode(), message);
            var userCode = nodeEvent.Value as UserCodeValue;

            Assert.That(nodeEvent, Is.Not.Null);
            Assert.That(userCode, Is.Not.Null);
            Assert.That(userCode.UserId, Is.EqualTo(0x01));
            Assert.That(userCode.UserIdStatus, Is.EqualTo(0x01));
            Assert.That(userCode.TagCode, Is.EqualTo(_tagCode));
        }

        [Test]
        public void GetEvent_ReturnsNullFor_UsersNumberReportCommand()
        {
            var commandClass = new UserCode();
            var message = new[] { CommandClassUserCode, UsersNumberReport};
            var node = new Mock<IZWaveNode>();
            node.Setup(x => x.GetCommandClass(CommandClass.Version))
                .Returns(new NodeCommandClass(CommandClassUserCode));

            var nodeEvent = commandClass.GetEvent(node.Object, message);

            Assert.That(nodeEvent, Is.Null);
        }

        [Test]
        public void SetMessage()
        {
            var node = new Mock<IZWaveNode>();
            node.Setup(x => x.GetData("UserCode", It.IsAny<object>()))
                .Returns(new NodeData("UserCode", new UserCodeValue()));
            var userCode = new UserCodeValue(0x02, 0x01, _tagCode);

            UserCode.Set(node.Object, userCode);

            var expectedMessage = new byte[] {CommandClassUserCode, UserCodeSet, 0x02, 0x01, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0A};
            node.Verify(x => x.SendDataRequest(It.Is<byte[]>(msg => msg.SequenceEqual(expectedMessage))));
        }

        [Test]
        public void GetUserCode_ReturnsDataStoredInNode()
        {
            var node = new Mock<IZWaveNode>();
            var userCode = new UserCodeValue(0x02, 0x01, _tagCode);
            node.Setup(x => x.GetData("UserCode", It.IsAny<object>()))
                .Returns(new NodeData("UserCode", userCode));
            
            var actualUserCode = UserCode.GetUserCode(node.Object);

            Assert.That(actualUserCode, Is.EqualTo(userCode));
        }

        [Test]
        public void UsersNumberGetMessage()
        {
            var node = new Mock<IZWaveNode>();

            Assert.That(() => UserCode.UsersNumberGet(node.Object), Throws.Exception.TypeOf<NotImplementedException>());
        }
    }
}
