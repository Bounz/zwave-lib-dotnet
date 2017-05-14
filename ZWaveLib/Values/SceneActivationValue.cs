namespace ZWaveLib.Values
{
    public class SceneActivationValue
    {
        /// <summary>
        /// Scene ID (1…255) to be activated in the device.
        /// </summary>
        public int SceneId { get; set; }

        /// <summary>
        /// Dimming Duration in seconds
        /// </summary>
        public int DimmingDuration { get; set; }

        public static SceneActivationValue Parse(byte[] message)
        {
            var value = new SceneActivationValue
            {
                SceneId = message[2]
            };

            var duration = message[3];
            if (duration <= 0x7F)
            {
                value.DimmingDuration = duration;
            }
            else
            {
                value.DimmingDuration = (duration - 0x7F) * 60;
            }

            return value;
        }
    }
}