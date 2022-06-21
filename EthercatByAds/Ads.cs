using Core;
using Diagnostic;
using Hardware;
using Hardware.Twincat;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EthercatByAds
{
    /// <summary>
    /// Handle the communication with a Beckhoff PLC using Ads protocol
    /// </summary>
    public static class Ads
    {
        /// <summary>
        /// The initialized status (<see langword="true"/> if the initialization has been completed, <see langword="false"/> otherwise)
        /// </summary>
        public static bool Initialized { get; private set; } = false;

        /// <summary>
        /// The communication status (<see langword="true"/> if the communication is in error, <see langword="false"/> otherwise)
        /// </summary>
        public static bool IsInError { get; private set; } = false;

        /// <summary>
        /// The reason of Ads communication failure
        /// </summary>
        /// <remarks>
        /// If the communication is up, the value will be <see cref="string.Empty"/>
        /// </remarks>
        public static string ReasonOfFailure { get; private set; } = "";

        private static TwincatResource resource;

        /// <summary>
        /// Initialize the communication with the PLC and create the communication channels
        /// </summary>
        /// <remarks>
        /// This method is an asynchronous <see cref="Task{TResult}"/> so, in order to get the result,
        /// it should be awaited (or <see cref="Task{TResult}.Result"/> should be used)
        /// </remarks>
        /// <param name="asmNetAddress">The PLC's ASM net address</param>
        /// <param name="port">The port number</param>
        /// <param name="pollingInterval">The polling interval (in milliseconds)</param>
        /// <param name="reconnnectionInterval">The reconnection interval between each attempt (in milliseconds)</param>
        /// <param name="analogInputsPath">The analog input channels (csv) file path</param>
        /// <param name="analogOutputsPath">The analog output channels (csv) file path</param>
        /// <param name="digitalInputsPath">The digital input channels (csv) file path</param>
        /// <param name="digitalOutputsPath">The digital output channels (csv) file path</param>
        /// <param name="delimiter">The (csv) channels file delimiter <see cref="char"/></param>
        /// <returns>
        /// The (async) initialization <see cref="Task{TResult}"/> (in which the <see cref="Task"/>'s result
        /// will be <see langword="true"/> if the initialization completed successfully, <see langword="false"/> otherwise)
        /// </returns>
        public static async Task<bool> Initialize(string asmNetAddress, int port, int pollingInterval = 100, int reconnnectionInterval = 10000,
            string analogInputsPath = "analog_inputs.csv", string analogOutputsPath = "analog_outputs.csv", string digitalInputsPath = "digital_inputs.csv",
            string digitalOutputsPath = "digital_outputs.csv", char delimiter = ',')
        {
            // **** Logger initialization
            Logger.Initialize();

            // **** Resource initialization
            resource = new TwincatResource("TwincatByAdsResource", asmNetAddress, port, pollingInterval);
            resource.Status.ValueChanged += Status_ValueChanged;
            await Logger.InfoAsync($"Ads communication initialized");

            // **** Read and parse the channels configuration file
            // Analog inputs
            List<ChannelEntry> channelEntries = await Parser.ReadAndParseFile(analogInputsPath, delimiter);
            if (channelEntries != null)
                channelEntries.ForEach((x) => new TwincatAnalogInput(x.ChannelCode, x.VariableName, resource));
            else
                HandleError("Analog inputs file not found");

            // Analog outputs
            channelEntries = await Parser.ReadAndParseFile(analogOutputsPath, delimiter);
            if (channelEntries != null)
                channelEntries.ForEach((x) => new TwincatAnalogOutput(x.ChannelCode, x.VariableName, resource));
            else
                HandleError("Analog outputs file not found");

            // Digital inputs
            channelEntries = await Parser.ReadAndParseFile(digitalInputsPath, delimiter);
            if (channelEntries != null)
                channelEntries.ForEach((x) => new TwincatDigitalInput(x.ChannelCode, x.VariableName, resource));
            else
                HandleError("Digital inputs file not found");

            // Digital outputs
            channelEntries = await Parser.ReadAndParseFile(digitalOutputsPath, delimiter);
            if (channelEntries != null)
                channelEntries.ForEach((x) => new TwincatDigitalOutput(x.ChannelCode, x.VariableName, resource));
            else
                HandleError("Digital inputs file not found");

            await Logger.InfoAsync($"Channels created. A total of {resource.Channels.Count} channel(s) added");

            // **** Resource start
            await resource.Start();

            // **** Reconnecting task creation
            CreateNewReconnectingTask(reconnnectionInterval).Start();

            Initialized = resource.Status.Value == ResourceStatus.Executing;
            return Initialized;
        }

        /// <summary>
        /// Handle an error status
        /// </summary>
        /// <param name="reasonOfFailure">The reason of failure</param>
        private static void HandleError(string reasonOfFailure)
        {
            IsInError = true;
            ReasonOfFailure = reasonOfFailure;
        }

        /// <summary>
        /// Create an async <see cref="Task"/> that attempt to reconnect the
        /// <see cref="TwincatResource"/> to the PLC
        /// </summary>
        /// <param name="reconnectionInterval">The reconnection interval (in milliseconds)</param>
        /// <returns>The async <see cref="Task"/></returns>
        private static Task CreateNewReconnectingTask(int reconnectionInterval = 10000)
        {
            // Create the reconnecting task
            Task reconnectingTask = new Task(async () =>
                {
                    while (true)
                    {
                        if (resource.Status.Value == ResourceStatus.Failure) // If the resource is in failure
                        {
                            await Logger.WarnAsync($"{resource.Code} in failure. Attempting a reconnection to the PLC");
                            await resource.Start(); // Attempt to start it
                        }

                        // Wait for <reconnectionInterval> milliseconds
                        await Task.Delay(reconnectionInterval);
                    }
                }
            );

            return reconnectingTask;
        }

        /// <summary>
        /// Update the <see cref="ReasonOfFailure"/> and <see cref="IsInError"/> values
        /// </summary>
        private static void HandleStatusChange()
        {
            if (resource.Status.Value != ResourceStatus.Starting && resource.Status.Value != ResourceStatus.Stopping)
                IsInError = resource.Status.Value == ResourceStatus.Failure;

            if (!IsInError) // The resource is not in error
                ReasonOfFailure = "";
            else // The resource is in error
            {
                if (resource.LastFailure.Description.CompareTo(string.Empty) != 0)
                {
                    // Get the error description
                    ReasonOfFailure = resource.LastFailure.Description;

                    // And parse it (remove the resource code and the string " - ")
                    ReasonOfFailure = ReasonOfFailure.Replace(resource.Code, string.Empty);
                    ReasonOfFailure = ReasonOfFailure.Remove(0, 3);
                    ReasonOfFailure = ReasonOfFailure.Trim();
                }
            }
        }

        /// <summary>
        /// Update the <see cref="ReasonOfFailure"/> value
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">The <see cref="ValueChangedEventArgs"/></param>
        private static void Status_ValueChanged(object sender, ValueChangedEventArgs e)
            => HandleStatusChange();

        /// <summary>
        /// Stop the Ads communication with the PLC
        /// </summary>
        /// <returns><see langword="true"/> if the communication stopped successfully, <see langword="false"/> otherwise</returns>
        public static bool Stop()
        {
            if (Initialized) // Stop the resource only if it has been initialized
                resource.Stop();

            bool returnValue = resource.Status.Value == ResourceStatus.Stopped;
            return returnValue;
        }

        /// <summary>
        /// Write a new value to the PLC's variable
        /// </summary>
        /// <remarks>
        /// If the <see cref="Initialize(string, int, int, int, string, string, string, string, char)"/> method has not been called, the
        /// return value will be <see langword="default"/> (<see cref="double"/> or <see cref="bool"/>)!
        /// </remarks>
        /// <param name="variableName">The variable name</param>
        /// <param name="value">The value to write</param>
        /// <returns>
        /// <see langword="true"/> if the resource is running and the operation completed,
        /// <see langword="false"/> if a communication error occurred or <paramref name="variableName"/> is not found
        /// </returns>
        public static bool Write(string variableName, double value)
        {
            bool returnValue = resource.Status.Value == ResourceStatus.Executing; // True if the resource is executing, false otherwise

            if (Initialized && resource.Channels.Count > 0) // If initialization has been done and the resource has channels
            {
                // Get the channel
                TwincatAnalogOutput channel = resource.Channels.Get(variableName) as TwincatAnalogOutput;
                if (channel == null) // Channel not found
                {
                    returnValue = false;
                    Logger.Error($"{variableName} not found in the channels list");
                }
                else // Channel found, write (analog)
                    channel.Value = value;
            }

            return returnValue;
        }

        /// <summary>
        /// Write a new value to the PLC's variable
        /// </summary>
        /// <remarks>
        /// If the <see cref="Initialize(string, int, int, int, string, string, string, string, char)"/> method has not been called, the
        /// return value will be <see langword="default"/> (<see cref="double"/> or <see cref="bool"/>)!
        /// </remarks>
        /// <param name="variableName">The variable name</param>
        /// <param name="value">The value to write</param>
        /// <returns>
        /// <see langword="true"/> if the resource is running and the operation completed,
        /// <see langword="false"/> if a communication error occurred or <paramref name="variableName"/> is not found
        /// </returns>
        public static bool Write(string variableName, bool value)
        {
            bool returnValue = resource.Status.Value == ResourceStatus.Executing; // True if the resource is executing, false otherwise

            if (Initialized && resource.Channels.Count > 0) // If initialization has been done and the resource has channels
            {
                // Get the channel
                TwincatDigitalOutput channel = resource.Channels.Get(variableName) as TwincatDigitalOutput;
                if (channel == null) // Channel not found
                {
                    returnValue = false;
                    Logger.Error($"{variableName} not found in the channels list");
                }
                else // Channel found, write (digital)
                    channel.Value = value;
            }

            return returnValue;
        }

        /// <summary>
        /// Read a variable from the PLC
        /// </summary>
        /// <remarks>
        /// If the <see cref="Initialize(string, int, int, int, string, string, string, string, char)"/> method has not been called, the
        /// return value will be <see cref="double"/>'s <see langword="default"/>!
        /// </remarks>
        /// <param name="variableName">The variable name</param>
        /// <param name="valueRead">The value read from the PLC</param>
        /// <returns>
        /// <see langword="true"/> if the resource is running and the operation completed,
        /// <see langword="false"/> if a communication error occurred or <paramref name="variableName"/> is not found
        /// </returns>
        public static bool Read(string variableName, out double valueRead)
        {
            bool returnValue = resource.Status.Value == ResourceStatus.Executing;
            valueRead = 0;

            if (Initialized && resource.Channels.Count > 0) // If initialization has been done and the resource has channels
            {
                // Get the channel
                TwincatAnalogInput channel = resource.Channels.Get(variableName) as TwincatAnalogInput; // Get the channel
                if (channel == null) // Channel not found
                {
                    returnValue = false;
                    Logger.Error($"{variableName} not found in the channels list");
                }
                else // Channel found, read
                    valueRead = channel.Value;
            }

            return returnValue;
        }

        /// <summary>
        /// Read a variable from the PLC
        /// </summary>
        /// <remarks>
        /// If the <see cref="Initialize(string, int, int, int, string, string, string, string, char)"/> method has not been called, the
        /// return value will be <see cref="bool"/>'s <see langword="default"/>!
        /// </remarks>
        /// <param name="variableName">The variable name</param>
        /// <param name="valueRead">The value read from the PLC</param>
        /// <returns>
        /// <see langword="true"/> if the resource is running and the operation completed,
        /// <see langword="false"/> if a communication error occurred or <paramref name="variableName"/> is not found
        /// </returns>
        public static bool Read(string variableName, out bool valueRead)
        {
            bool returnValue = resource.Status.Value == ResourceStatus.Executing;
            valueRead = false;

            if (Initialized && resource.Channels.Count > 0) // If initialization has been done and the resource has channels
            {
                // Get the channel
                TwincatDigitalInput channel = resource.Channels.Get(variableName) as TwincatDigitalInput; // Get the channel
                if (channel == null) // Channel not found
                {
                    returnValue = false;
                    Logger.Error($"{variableName} not found in the channels list");
                }
                else // Channel found, read
                    valueRead = channel.Value;
            }

            return returnValue;
        }
    }
}