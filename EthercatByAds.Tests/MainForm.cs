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
        private const int PollingTime = 100;

        // I/O variable names
        private List<string> aiInputNames = new List<string>(), diInputNames = new List<string>();

        private List<string> aoOutputNames = new List<string>(), doOutputNames = new List<string>();

        private Dictionary<string, object> dataSource = new Dictionary<string, object>();

        // Tasks
        private Task uiTask;

        public MainForm()
        {
            InitializeComponent();

            lblResourceName.Text = $"{AsmNetAddress}:{Port}";

            GetVariableNames();

            List<string> outputs = new List<string>();
            outputs.AddRange(aoOutputNames);
            outputs.AddRange(doOutputNames);
            BindingSource bs = new BindingSource
            {
                DataSource = outputs
            };
            cbxOutputNames.DataSource = bs;
            if (cbxOutputNames.Items.Count > 0)
                cbxOutputNames.SelectedIndex = 0;
        }

        /// <summary>
        /// Read the Ads variables
        /// </summary>
        /// <param name="dataSource"></param>
        private void ReadVariables()
        {
            object value;

            foreach (string inputName in aiInputNames)
            {
                if (inputName.CompareTo(string.Empty) != 0)
                {
                    Ads.Read(inputName, out double localValue);
                    value = localValue;

                    if (!dataSource.ContainsKey(inputName))
                        dataSource.Add(inputName, value);
                    else
                        dataSource[inputName] = value;
                }
            }

            foreach (string inputName in aoOutputNames)
            {
                if (inputName.CompareTo(string.Empty) != 0)
                {
                    Ads.Read(inputName, out double localValue);
                    value = localValue;

                    if (!dataSource.ContainsKey(inputName))
                        dataSource.Add(inputName, value);
                    else
                        dataSource[inputName] = value;
                }
            }

            foreach (string inputName in diInputNames)
            {
                if (inputName.CompareTo(string.Empty) != 0)
                {
                    Ads.Read(inputName, out bool localValue);
                    value = localValue;

                    if (!dataSource.ContainsKey(inputName))
                        dataSource.Add(inputName, value);
                    else
                        dataSource[inputName] = value;
                }
            }

            foreach (string inputName in doOutputNames)
            {
                if (inputName.CompareTo(string.Empty) != 0)
                {
                    Ads.Read(inputName, out bool localValue);
                    value = localValue;

                    if (!dataSource.ContainsKey(inputName))
                        dataSource.Add(inputName, value);
                    else
                        dataSource[inputName] = value;
                }
            }

            dgvInputs.DataSource = (from d in dataSource where d.Key.CompareTo(string.Empty) != 0 orderby d.Key select new { d.Key, d.Value }).ToList();
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

            string[] aoText = File.ReadAllLines(AoPath).Skip(1).ToArray();
            string[] doText = File.ReadAllLines(DoPath).Skip(1).ToArray();

            for (int i = 0; i < aiText.Length; i++)
                aiInputNames.Add(aiText[i].Split(DelimiterChar)[0].Trim());

            for (int i = 0; i < aoText.Length; i++)
                aoOutputNames.Add(aoText[i].Split(DelimiterChar)[0].Trim());

            for (int i = 0; i < diText.Length; i++)
                diInputNames.Add(diText[i].Split(DelimiterChar)[0].Trim());

            for (int i = 0; i < doText.Length; i++)
                doOutputNames.Add(doText[i].Split(DelimiterChar)[0].Trim());
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

            dgvInputs.DataSource = (from d in dataSource where d.Key.CompareTo(string.Empty) != 0 orderby d.Key select new { d.Key, d.Value }).ToList();
            dgvInputs.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dgvInputs.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvInputs.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

            ReadVariables();

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

        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            ReadVariables();
        }

        private void DataGridView_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            DataGridView grid = sender as DataGridView;
            string rowIndex = (e.RowIndex + 1).ToString();

            StringFormat centerFormat = new StringFormat()
            {
                // Right alignment might actually make more sense for numbers
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };

            Rectangle headerBounds = new Rectangle(e.RowBounds.Left, e.RowBounds.Top, grid.RowHeadersWidth, e.RowBounds.Height);
            e.Graphics.DrawString(rowIndex, Font, SystemBrushes.ControlText, headerBounds, centerFormat);
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
            => Ads.Stop();

        #endregion UI event handlers
    }
}