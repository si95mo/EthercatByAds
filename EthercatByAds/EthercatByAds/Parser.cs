using Diagnostic;
using IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace EthercatByAds
{
    /// <summary>
    /// Static class that parse the channel configuration file
    /// </summary>
    internal static class Parser
    {
        /// <summary>
        /// Read the channels configuration file
        /// </summary>
        /// <param name="path">The file path</param>
        /// <param name="delimiter">The delimiter <see cref="char"/></param>
        /// <returns>
        /// The (async) <see cref="Task{TResult}"/> of which the result is a <see cref="List{T}"/> with <see cref="ChannelEntry"/> values.
        /// If the file specified in <paramref name="path"/> does not exist, then the <see cref="Task"/> result will be <see langword="null"/>
        /// </returns>
        internal static async Task<List<ChannelEntry>> ReadAndParseFile(string path, char delimiter = ',')
        {
            List<ChannelEntry> channels = null;

            if (File.Exists(path))
            {
                string[] text = await ReadFileAsync(path);
                channels = ParseFile(text, delimiter);
            }
            else
                await Logger.ErrorAsync(string.Format("File specified at '{0}' does not found", path));

            return channels;
        }

        #region Helper methods

        /// <summary>
        /// Read the channels configuration file
        /// </summary>
        /// <param name="path">The file path</param>
        /// <returns>An array of <see cref="string"/> with the read text (as the result of an async <see cref="Task{TResult}"/>)</returns>
        private static async Task<string[]> ReadFileAsync(string path)
        {
            string text = await FileHandler.ReadAsync(path);
            string[] splittedText = text.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            return splittedText;
        }

        /// <summary>
        /// Parse the channels configuration file
        /// </summary>
        /// <param name="text">The read text from the file</param>
        /// <param name="delimiter">The delimiter <see cref="char"/></param>
        /// <returns>A <see cref="List{T}"/> with <see cref="ChannelEntry"/> values</returns>
        private static List<ChannelEntry> ParseFile(string[] text, char delimiter = ',')
        {
            List<ChannelEntry> channels = new List<ChannelEntry>();

            ChannelEntry channelEntry;
            string[] splittedLine;
            for (int i = 1; i < text.Length; i++)
            {
                splittedLine = text[i].Split(delimiter);
                channelEntry = new ChannelEntry(splittedLine[0].Trim(), splittedLine[1].Trim());

                if (channelEntry.ChannelCode.CompareTo(string.Empty) != 0)
                    channels.Add(channelEntry);
            }

            return channels;
        }

        #endregion Helper methods
    }
}