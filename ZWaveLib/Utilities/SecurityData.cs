using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ZWaveLib.Utilities
{
    public class SecurityData
    {
        private byte[] _encryptionKey = null;
        private byte[] _controllerCurrentNonce = null;
        private byte[] _privateNetworkKey = null;
        //private byte[] privateNetworkKey = new byte[] { 0x0F, 0x1E, 0x2D, 0x3C, 0x4B, 0x5A, 0x69, 0x78, 0x87, 0x96, 0xA5, 0xB4, 0xC3, 0xD2, 0xE1, 0xF0 };
        //private byte[] privateNetworkKey = new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F, 0x10 };
        private readonly ZWaveNode _parentNode = null;

        public byte[] DeviceCurrentNonce = null;

        public Stopwatch ControllerNonceTimer = new Stopwatch();
        public Stopwatch DeviceNonceTimer = new Stopwatch();

        public bool SchemeAgreed = false;
        public bool IsWaitingForNonce = false;
        public bool IsAddingNode = false;
        public bool IsNetworkKeySet = false;

        public SecurityData(IZWaveNode node)
        {
            _parentNode = (ZWaveNode)node;
        }

        public void SetPrivateNetworkKey(byte[] key)
        {
            _privateNetworkKey = key;
        }

        public byte[] GetPrivateNetworkKey()
        {
            return _privateNetworkKey;
        }

        public byte[] GeneratePrivateNetworkKey()
        {
            _privateNetworkKey = new byte[16];
            var rnd = new Random();
            rnd.NextBytes(_privateNetworkKey);

            // notify the controller that the privateNetworkKey was generated so that it can save it
            var keyGenEvent = new NodeEvent(_parentNode, EventParameter.SecurityGeneratedKey, 0, 0);
            _parentNode.OnNodeUpdated(keyGenEvent);

            return _privateNetworkKey;
        }

        public void GenerateControllerCurrentNonce()
        {
            var localControllerCurrentNonce = new byte[8];

            if (_controllerCurrentNonce == null)
            {
                _controllerCurrentNonce = new byte[8];
                // we needs to generate one and save it for the next call;
                var rnd = new Random();
                rnd.NextBytes(_controllerCurrentNonce);
            }

            Utility.Logger.Info("ControllerCurrentNonce: " + BitConverter.ToString(localControllerCurrentNonce));
            //return localControllerCurrentNonce;
        }

        public byte[] GetControllerCurrentNonce(bool clearNonce = false)
        {
            var localControllerCurrentNonce = new byte[8];

            // safety check - don't try to copy if the source array is null
            if (_controllerCurrentNonce == null)
            {
                Utility.Logger.Error("Controller Current Nonce is NULL");
                return _controllerCurrentNonce;
            }

            Array.Copy(_controllerCurrentNonce, localControllerCurrentNonce, 8);

            if (clearNonce)
            {
                // Ideally we should get in here ONLY when decrypting a message from the device.
                // This would cause the controllerCurrentNonce to be re-generated which would
                // give us the most secure communication, if we see issues we just don't pass
                // the clearNonce argument

                // since for now we don't regenerate it let's not set it to null
                //controllerCurrentNonce = null;
            }

            return localControllerCurrentNonce;
        }

        public byte[] EncryptionKey
        {
            get
            {
                // the EncryptionKey needs to be generated in two cases:
                // 1 - encryptionKey is null
                // 2 - once the PrivateNetworkKey was sent to the device

                // these three arrays seems to be this like this
                var initialNetworkKey = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
                var encryptPassword = new byte[] { 0xAA, 0xAA, 0xAA, 0xAA, 0xAA, 0xAA, 0xAA, 0xAA, 0xAA, 0xAA, 0xAA, 0xAA, 0xAA, 0xAA, 0xAA, 0xAA };
                var authPassword = new byte[] { 0x55, 0x55, 0x55, 0x55, 0x55, 0x55, 0x55, 0x55, 0x55, 0x55, 0x55, 0x55, 0x55, 0x55, 0x55, 0x55 };

                byte[] networkKey;

                if (IsAddingNode && !IsNetworkKeySet)
                {
                    networkKey = initialNetworkKey;
                    Utility.Logger.Info("In SetupNetworkKey  - in node inclusion mode.");
                }
                else
                {
                    if (_privateNetworkKey == null)
                        GeneratePrivateNetworkKey();
                    networkKey = _privateNetworkKey;
                }

                _encryptionKey = AesWork.EncryptEcbMessage(networkKey, encryptPassword);
                AuthorizationKey = AesWork.EncryptEcbMessage(networkKey, authPassword);

                Utility.Logger.Info("      networkKey: " + BitConverter.ToString(networkKey));
                Utility.Logger.Info("   encryptionKey: " + BitConverter.ToString(_encryptionKey));
                Utility.Logger.Info("AuthorizationKey: " + BitConverter.ToString(AuthorizationKey));

                return _encryptionKey;
            }
        }

        public byte[] AuthorizationKey = null;

        public List<SecutiryPayload> SecurePayload = new List<SecutiryPayload>();
        public int SequenceCounter = 0;
    }
}