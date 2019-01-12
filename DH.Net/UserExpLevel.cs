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
        public int Level { get; private set; }

        /// <summary>
        /// Total Exp of the user
        /// </summary>
        [JsonProperty("total_exp")]
        public int TotalExp { get; private set; }

        /// <summary>
        /// The percentage of level completion
        /// </summary>
        [JsonProperty("exp_percent")]
        public int ExpPercent { get; private set; }
    }
}
