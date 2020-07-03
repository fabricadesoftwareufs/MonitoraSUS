using System;

namespace Util
{
    public class RecaptchaModel
    {
        public bool Success { get; set; }
        public DateTime Chanllenge_ts { get; set; }
        public string HostName { get; set; }
        public float Score { get; set; }
        public string Action { get; set; }
        public Array ErrorCodes { get; set; }
    }
}
