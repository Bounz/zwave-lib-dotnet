﻿using NUnit.Framework;
using ZWaveLib.Utilities;

namespace ZWaveLibTests
{
    [TestFixture]
    public class AesWorksTest
    {
        [Test]
        public void Test1()
        {
            /*
            AuthorizationKey: 57-83-EE-94-C4-5E-AA-DB-10-62-AD-09-00-EE-15-23
            EncryptKey 2C-86-86-B5-D1-55-8D-31-30-D1-04-55-A4-02-4C-FD
            Input Packet: 00-98-06-4B-CC-C3-04-4F-01-89-D8-34-F1-F0-E7-53-0C-34-E3
            IV AA-AA-AA-AA-AA-AA-AA-AA-C2-A9-77-CA-E7-C3-D0-7B
            encryptedPayload E3-18-DB-EA-47-5E-F7-EB-03-D9-BF-16-64-DF-25-76-0F-80-C9
            */
            var encryptionKey = new byte[] {0x2C, 0x86, 0x86, 0xB5, 0xD1, 0x55, 0x8D, 0x31, 0x30, 0xD1, 0x04, 0x55, 0xA4, 0x02, 0x4C, 0xFD};
            var iv = new byte[] {0xAA, 0xAA, 0xAA, 0xAA, 0xAA, 0xAA, 0xAA, 0xAA, 0xC2, 0xA9, 0x77, 0xCA, 0xE7, 0xC3, 0xD0, 0x7B};
            var plaintextmsg = new byte[]
                {0x00, 0x98, 0x06, 0x4B, 0xCC, 0xC3, 0x04, 0x4F, 0x01, 0x89, 0xD8, 0x34, 0xF1, 0xF0, 0xE7, 0x53, 0x0C, 0x34, 0xE3};

            var encryptedPayload = AesWork.EncryptOfbMessage(encryptionKey, iv, plaintextmsg);

            var expectedPayload = new byte[]
                {0xE3, 0x18, 0xDB, 0xEA, 0x47, 0x5E, 0xF7, 0xEB, 0x03, 0xD9, 0xBF, 0x16, 0x64, 0xDF, 0x25, 0x76, 0x0F, 0x80, 0xC9};
            Assert.That(encryptedPayload, Is.EqualTo(expectedPayload));
        }
    }
}