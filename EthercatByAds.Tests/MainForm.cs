using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EthercatByAds.Tests
{
    public partial class MainForm : Form
    {
        // Twincat connection
        private const string AsmNetAddress = "5.112.56.172.1.1";

        private const int Port = 851;

        // Config files path
        private const string AiPath = "files//analog_inputs.csv";

        private const string AoPath = "files//analog_outputs.csv";
        private const string DiPath = "files//digital_inputs.csv";
        private const string DoPath = "files//digital_outputs.csv";

        // For parsing
        private const char DelimiterChar = ',';

        // Polling time
        private const int PollingTime = 10;

        // I/O variable names
        private List<string> inputNames = new List<string>();

        private List<string> outputNames = new List<string>();

        // Tasks
        private Task updateTask;
        private Task uiTask;
        private object lockObject = new object();

        public MainForm()
        {
            InitializeComponent();

            lblResourceName.Text = $"{AsmNetAddress}:{Port}";

            GetVariableNames();

            BindingSource bs = new BindingSource
            {
                DataSource = outputNames
            };
            cbxOutputNames.DataSource = bs;
            cbxOutputNames.SelectedIndex = 0;

            // Started inside load event
            updateTask = new Task(async () =>
                {
                    Dictionary<string, object> dataSource = new Dictionary<string, object>();
                    object value;
                    Action initializeDataGridView = new Action(() =>
                        {

                            dgvInputs.DataSource = (from d in dataSource orderby d.Key select new { d.Key, d.Value }).ToList();
                            dgvInputs.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                            dgvInputs.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                            dgvInputs.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                        }
                    );

                    if (!InvokeRequired)
                        initializeDataGridView();
                    else
                        BeginInvoke(initializeDataGridView);

                    while (true)
                    {
                        foreach (string inputName in inputNames)
                        {
                            if (inputName.Contains("Digital"))
                            {
                                Ads.Read(inputName, out bool localValue);
                                value = localValue;
                            }
                            else
                            {
                                Ads.Read(inputName, out double localValue);
                                value = localValue;
                            }

                            if (!dataSource.ContainsKey(inputName))
                                dataSource.Add(inputName, value);
                            else
                                dataSource[inputName] = value;
                        }

                        //if (!InvokeRequired)
                        //    UpdateVariablesValue(dataSource);
                        //else
                        //    BeginInvoke(new Action(() => UpdateVariablesValue(dataSource)));

                        await Task.Delay(2000);
                    }
                }
            );
        }

        #region Methods

        /// <summary>
        /// Get the variable names from the config files
        /// and populate the relative collection
        /// </summary>
        private void GetVariableNames()
        {
            string[] aiText = File.ReadAllLines(AiPath).Skip(1).ToArray(); // Remove headers
            string[] diText = File.ReadAllLines(DiPath).Skip(1).ToArray();
            string[] inputs = aiText.Concat(diText).ToArray();

            string[] aoText = File.ReadAllLines(AoPath).Skip(1).ToArray();
            string[] doText = File.ReadAllLines(DoPath).Skip(1).ToArray();
            string[] outputs = aoText.Concat(doText).ToArray();

            for (int i = 0; i < inputs.Length; i++)
                inputNames.Add(inputs[i].Split(DelimiterChar)[0].Trim());

            for (int i = 0; i < outputs.Length; i++)
                outputNames.Add(outputs[i].Split(DelimiterChar)[0].Trim());
        }

        /// <summary>
        /// Update the variables value related UI
        /// </summary>
        /// <param name="dataSource">The data source from which get the values</param>
        private void UpdateVariablesValue(Dictionary<string, object> dataSource)
        {
            //dgvInputs.Items.Clear();

            //lock (lockObject)
            //{
            //    foreach (KeyValuePair<string, object> x in dataSource)
            //        dgvInputs.Items.Add($"{x.Key}\t{x.Value}");
            //}
        }

        /// <summary>
        /// Update the Ads status related UI
        /// </summary>
        private void UpdateAdsStatus()
        {
            if (Ads.Initialized)
            {
                if (!Ads.IsInError) // Ads initialized and not in error
                {
                    lblResourceStatus.Text = "Communication up";
                    lblReasonOfFailure.Text = "";

                    ledStatus.StopBlink();
                    ledStatus.Color = Color.Green;
                    ledStatus.Blink(500);
                }
                else // Ads initializaed and in error
                {
                    lblResourceStatus.Text = $"Communication in error";
                    lblReasonOfFailure.Text = Ads.ReasonOfFailure;

                    ledStatus.StopBlink();
                    ledStatus.Color = Color.Red;
                    ledStatus.Blink(500);
                }
            }
            else // Ads not initialized
            {
                lblResourceStatus.Text = "Initialization failed";
                lblReasonOfFailure.Text = "";

                ledStatus.StopBlink();
                ledStatus.Color = Color.Yellow;
                ledStatus.Blink(500);
            }
        }

        #endregion Methods

        #region UI event handlers

        private async void MainForm_Load(object sender, EventArgs e)
        {
            await Ads.Initialize(AsmNetAddress, Port, PollingTime, 10000, AiPath, AoPath, DiPath, DoPath, DelimiterChar);

            updateTask.Start();
            uiTask = new Task(async () =>
                {
                    while (true)
                    {
                        if (!InvokeRequired)
                            UpdateAdsStatus();
                        else
                            BeginInvoke(new Action(() => UpdateAdsStatus()));

                        await Task.Delay(1000);
                    }
                }
            );
            uiTask.Start();
        }

        private void BtnWrite_Click(object sender, EventArgs e)
        {
            if (Ads.Initialized)
            {
                string channelName = cbxOutputNames.SelectedItem.ToString();

                if (channelName.Contains("Digital"))
                    Ads.Write(channelName, chbValue.Checked);
                else
                    Ads.Write(channelName, (double)nudAnalogValue.Value);
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
            => Ads.Stop();

        #endregion UI event handlers
    }
}