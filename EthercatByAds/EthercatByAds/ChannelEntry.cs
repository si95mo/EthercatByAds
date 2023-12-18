namespace EthercatByAds
{
    /// <summary>
    /// Describe a channel that has been read from the configuration file
    /// </summary>
    internal class ChannelEntry
    {
        /// <summary>
        /// The channel code (high level name)
        /// </summary>
        internal string ChannelCode { get; set; }

        /// <summary>
        /// The PLC variable name (low level)
        /// </summary>
        internal string VariableName { get; set; }

        /// <summary>
        /// Create a new entry of <see cref="ChannelEntry"/>
        /// </summary>
        /// <param name="channelCode">The channel code</param>
        /// <param name="variableName">The PLC variable name</param>
        internal ChannelEntry(string channelCode, string variableName)
        {
            ChannelCode = channelCode;
            VariableName = variableName;
        }

        public override string ToString()
        {
            string description = string.Format("{0} - {1}", ChannelCode, VariableName);
            return description;
        }
    }
}