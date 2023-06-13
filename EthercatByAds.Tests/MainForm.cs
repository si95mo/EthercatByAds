using Core;
using Diagnostic;
using Hardware;
using Hardware.Twincat;
using System.Collections.Generic;
using System.Runtime.Remoting.Channels;
using System.Threading.Tasks;

namespace EthercatByAds
{
    /// <summary>
    /// Handle the communication with a Beckhoff PLC using Ads protocol
    /// </summary>
    public static class Ads
    {
        #region Public fields

        /// <summary>
        /// The initialized status (<see langword="true"/> if the initialization has been completed, <see langword="false"/> otherwise)
        /// </summary>
        public static bool Initialized { get; private set; }

        /// <summary>
        /// The communication status (<see langword="true"/> if the communication is in error, <see langword="false"/> otherwise)
        /// </summary>
        public static bool IsInError { get; private set; }

        /// <summary>
        /// All the digital variables read from the PLC
        /// </summary>
        public static bool[] Bits;

        /// <summary>
        /// All the analog variables read from the PLC
        /// </summary>
        public static double[] Measures;

        /// <summary>
        /// The reason of Ads communication failure
        /// </summary>
        /// <remarks>
        /// If the communication is up, the value will be <see cref="string.Empty"/>
        /// </remarks>
        public static string ReasonOfFailure { get; private set; }

        #endregion Public fields

        #region Private variables

        private static TwincatResource resource;

        #endregion Private variables

        #region Public methods

        #region Start/stop methods

        /// <summary>
        /// Initialize the communication with the PLC and create the communication channels
        /// </summary>
        /// <remarks>
        /// This method is an asynchronous <see cref="Task{TResult}"/> so, in order to get the result,
        /// it should be awaited (or <see cref="Task{TResult}.Result"/> should be used)
        /// </remarks>
        /// <param name="amsNetAddress">The PLC's AMS net address</param>
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
        public static async Task<bool> Initialize(string amsNetAddress, int port, int pollingInterval = 10, int reconnnectionInterval = 10000,
            string analogInputsPath = "analog_inputs.csv", string analogOutputsPath = "analog_outputs.csv", string digitalInputsPath = "digital_inputs.csv",
            string digitalOutputsPath = "digital_outputs.csv", char delimiter = ',')
        {
            Initialized = false;
            IsInError = false;
            ReasonOfFailure = string.Empty;

            #region Logger initialization

            Logger.Initialize();

            #endregion Logger initialization

            #region Resource initialization

            if (amsNetAddress.CompareTo(string.Empty) != 0)
                resource = new TwincatResource("TwincatByAdsResource", amsNetAddress, port, pollingInterval);
            else
                resource = new TwincatResource("TwincatByAdsResource", port, pollingInterval);

            resource.Status.ValueChanged += Status_ValueChanged;
            await Logger.InfoAsync("Ads communication initialized");

            #endregion Resource initialization

            #region Read and parse the channels configuration file

            #region Analog inputs

            List<ChannelEntry> channelEntries = await Parser.ReadAndParseFile(analogInputsPath, delimiter);
            if (channelEntries != null)
                channelEntries.ForEach((x) => new TwincatAnalogInput(x.ChannelCode, x.VariableName, resource));
            else
                HandleError("Analog inputs file not found");

            #endregion Analog inputs

            #region Analog outputs

            channelEntries = await Parser.ReadAndParseFile(analogOutputsPath, delimiter);
            if (channelEntries != null)
                channelEntries.ForEach((x) => new TwincatAnalogOutput(x.ChannelCode, x.VariableName, resource));
            else
                HandleError("Analog outputs file not found");

            #endregion Analog outputs

            #region Digital inputs

            channelEntries = await Parser.ReadAndParseFile(digitalInputsPath, delimiter);
            if (channelEntries != null)
                channelEntries.ForEach((x) => new TwincatDigitalInput(x.ChannelCode, x.VariableName, resource));
            else
                HandleError("Digital inputs file not found");

            #endregion Digital inputs

            #region Digital outputs

            channelEntries = await Parser.ReadAndParseFile(digitalOutputsPath, delimiter);
            if (channelEntries != null)
                channelEntries.ForEach((x) => new TwincatDigitalOutput(x.ChannelCode, x.VariableName, resource));
            else
                HandleError("Digital inputs file not found");

            await Logger.InfoAsync(string.Format("Channels created. A total of {0} channel(s) added", resource.Channels.Count));

            #endregion Digital outputs

            #endregion Read and parse the channels configuration file

            #region Resource start

            await resource.Start();

            #endregion Resource start

            #region Reconnecting task creation

            CreateNewReconnectingTask(reconnnectionInterval).Start();

            #endregion Reconnecting task creation

            Initialized = resource.Status.Value == ResourceStatus.Executing;
            return Initialized;
        }

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

        #endregion Start/stop methods

        #region Write methods

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
                    Logger.Error(string.Format("{0} not found in the channels list", variableName));
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
                    Logger.Error(string.Format("{0} not found in the channels list", variableName));
                }
                else // Channel found, write (digital)
                    channel.Value = value;
            }

            return returnValue;
        }

        #endregion Write methods

        #region Read methods

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
            valueRead = default(double);

            if (Initialized && resource.Channels.Count > 0) // If initialization has been done and the resource has channels
            {
                // Get the channel
                TwincatAnalogInput channel = resource.Channels.Get(variableName) as TwincatAnalogInput; // Get the channel
                if (channel == null) // Channel not found
                {
                    returnValue = false;
                    Logger.Error(string.Format("{0} not found in the channels list", variableName));
                }
                else // Channel found, read
                    valueRead = channel.Value;
            }

            return returnValue;
        }

        /// <summary>
        /// Read all the analog variables
        /// </summary>
        /// <returns>The array with the all the read values</returns>

        public static double[] ReadMeasures()
        {
            List<double> measures = new List<double>();
            if (Initialized && resource.Channels.Count > 0)
            {
                foreach (IChannel channel in resource.Channels)
                    if (channel is TwincatAnalogInput)
                        measures.Add((channel as TwincatAnalogInput).Value);
            }

            Measures = measures.ToArray();
            return Measures;
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
            valueRead = default(bool);

            if (Initialized && resource.Channels.Count > 0) // If initialization has been done and the resource has channels
            {
                // Get the channel
                TwincatDigitalInput channel = resource.Channels.Get(variableName) as TwincatDigitalInput; // Get the channel
                if (channel == null) // Channel not found
                {
                    returnValue = false;
                    Logger.Error(string.Format("{0} not found in the channels list", variableName));
                }
                else // Channel found, read
                    valueRead = channel.Value;
            }

            return returnValue;
        }

        /// <summary>
        /// Read all the digital variables
        /// </summary>
        /// <returns>The array with the all the read values</returns>

        public static bool[] ReadBits()
        {
            List<bool> bits = new List<bool>();
            if (Initialized && resource.Channels.Count > 0)
            {
                foreach (IChannel channel in resource.Channels)
                    if (channel is TwincatDigitalInput)
                        bits.Add((channel as TwincatDigitalInput).Value);
            }

            Bits = bits.ToArray();
            return Bits;
        }

        /// <summary>
        /// Read all the available variables from PLC. See <see cref="Bits"/> for the digital ones and 
        /// <see cref="Measures"/> for the analog ones
        /// </summary>
        public static void ReadAll()
        {
            ReadBits();
            ReadMeasures();
        }

        #endregion Read methods

        #endregion Public methods

        #region Helper methods

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
                        await Logger.WarnAsync(string.Format("{0} in failure. Attempting a reconnection to the PLC", resource.Code));
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
        {
            HandleStatusChange();
        }

        #endregion Helper methods
    }
}