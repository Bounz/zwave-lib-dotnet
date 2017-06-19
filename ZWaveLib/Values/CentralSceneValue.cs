namespace ZWaveLib.Values
{
    public enum CentralScenePressType
    {
        Pressed1Time = 0x00,
        Released = 0x01,
        HeldDown = 0x02,
        Pressed2Times = 0x03,
        Pressed3Times = 0x04,
        Pressed4Times = 0x05,
        Pressed5Times = 0x06,
        Reserved = 0x07,
    }
    
    public class CentralSceneValue
    {
        public byte SequenceNumber;
        public CentralScenePressType PressType;
        public byte SceneNumber;

        public static CentralSceneValue Parse(byte[] message)
        {
            return new CentralSceneValue()
            {
                SequenceNumber = message[2],
                PressType = (CentralScenePressType)(message[3] & 0x07),
                SceneNumber = message[4]
            };
        }

        public override string ToString()
        {
            return $"SceneId: {SceneNumber}, Level: {SequenceNumber}, PressType: {PressType}";
        }
    }
}
