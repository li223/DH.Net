using Newtonsoft.Json;

namespace DH.Net
{
    /// <summary>
    /// User exp info object
    /// </summary>
    public struct UserExpLevel
    {
        /// <summary>
        /// User Level
        /// </summary>
        [JsonProperty("lvl")]
        public ulong Level { get; private set; }

        /// <summary>
        /// Total Exp of the user
        /// </summary>
        [JsonProperty("total_exp")]
        public ulong TotalExp { get; private set; }

        /// <summary>
        /// The percentage of level completion
        /// </summary>
        [JsonProperty("exp_percent")]
        public ulong ExpPercent { get; private set; }
    }
}
