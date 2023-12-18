
namespace EthercatByAds.Tests
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing && (components != null))
        //    {
        //        components.Dispose();
        //    }
        //    base.Dispose();
        //}

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lblResourceStatus = new UserInterface.Controls.LabelControl();
            this.lblResourceName = new UserInterface.Controls.LabelControl();
            this.ledStatus = new UserInterface.Controls.LedControl();
            this.lblReasonOfFailure = new UserInterface.Controls.LabelControl();
            this.dgvInputs = new System.Windows.Forms.DataGridView();
            this.cbxOutputNames = new System.Windows.Forms.ComboBox();
            this.btnWrite = new UserInterface.Controls.ButtonControl();
            this.nudAnalogValue = new System.Windows.Forms.NumericUpDown();
            this.labelControl1 = new UserInterface.Controls.LabelControl();
            this.labelControl2 = new UserInterface.Controls.LabelControl();
            this.chbValue = new System.Windows.Forms.CheckBox();
            this.btnRefresh = new UserInterface.Controls.ButtonControl();
            ((System.ComponentModel.ISupportInitialize)(this.dgvInputs)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudAnalogValue)).BeginInit();
            this.SuspendLayout();
            // 
            // lblResourceStatus
            // 
            this.lblResourceStatus.AutoSize = true;
            this.lblResourceStatus.Font = new System.Drawing.Font("Lucida Sans Unicode", 12F);
            this.lblResourceStatus.Location = new System.Drawing.Point(223, 9);
            this.lblResourceStatus.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.lblResourceStatus.Name = "lblResourceStatus";
            this.lblResourceStatus.Size = new System.Drawing.Size(45, 20);
            this.lblResourceStatus.TabIndex = 2;
            this.lblResourceStatus.Text = "----";
            // 
            // lblResourceName
            // 
            this.lblResourceName.AutoSize = true;
            this.lblResourceName.Font = new System.Drawing.Font("Lucida Sans Unicode", 12F);
            this.lblResourceName.Location = new System.Drawing.Point(8, 9);
            this.lblResourceName.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.lblResourceName.Name = "lblResourceName";
            this.lblResourceName.Size = new System.Drawing.Size(179, 20);
            this.lblResourceName.TabIndex = 1;
            this.lblResourceName.Text = "5.112.56.172.1.1:851";
            // 
            // ledStatus
            // 
            this.ledStatus.Location = new System.Drawing.Point(758, 7);
            this.ledStatus.Margin = new System.Windows.Forms.Padding(5);
            this.ledStatus.Name = "ledStatus";
            this.ledStatus.On = true;
            this.ledStatus.Size = new System.Drawing.Size(32, 32);
            this.ledStatus.TabIndex = 0;
            this.ledStatus.Text = "ledControl1";
            // 
            // lblReasonOfFailure
            // 
            this.lblReasonOfFailure.AutoSize = true;
            this.lblReasonOfFailure.Font = new System.Drawing.Font("Lucida Sans Unicode", 8F);
            this.lblReasonOfFailure.Location = new System.Drawing.Point(224, 29);
            this.lblReasonOfFailure.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.lblReasonOfFailure.Name = "lblReasonOfFailure";
            this.lblReasonOfFailure.Size = new System.Drawing.Size(0, 15);
            this.lblReasonOfFailure.TabIndex = 3;
            // 
            // dgvInputs
            // 
            this.dgvInputs.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvInputs.Location = new System.Drawing.Point(12, 86);
            this.dgvInputs.Name = "dgvInputs";
            this.dgvInputs.Size = new System.Drawing.Size(778, 455);
            this.dgvInputs.TabIndex = 13;
            this.dgvInputs.RowPostPaint += new System.Windows.Forms.DataGridViewRowPostPaintEventHandler(this.DataGridView_RowPostPaint);
            // 
            // cbxOutputNames
            // 
            this.cbxOutputNames.FormattingEnabled = true;
            this.cbxOutputNames.Location = new System.Drawing.Point(12, 47);
            this.cbxOutputNames.Name = "cbxOutputNames";
            this.cbxOutputNames.Size = new System.Drawing.Size(239, 28);
            this.cbxOutputNames.TabIndex = 7;
            // 
            // btnWrite
            // 
            this.btnWrite.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.btnWrite.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.btnWrite.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnWrite.Font = new System.Drawing.Font("Lucida Sans Unicode", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnWrite.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.btnWrite.Location = new System.Drawing.Point(644, 44);
            this.btnWrite.Name = "btnWrite";
            this.btnWrite.Size = new System.Drawing.Size(146, 32);
            this.btnWrite.TabIndex = 8;
            this.btnWrite.Text = "Write to PLC";
            this.btnWrite.UseVisualStyleBackColor = false;
            this.btnWrite.Click += new System.EventHandler(this.BtnWrite_Click);
            // 
            // nudAnalogValue
            // 
            this.nudAnalogValue.DecimalPlaces = 3;
            this.nudAnalogValue.Location = new System.Drawing.Point(374, 48);
            this.nudAnalogValue.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.nudAnalogValue.Name = "nudAnalogValue";
            this.nudAnalogValue.Size = new System.Drawing.Size(126, 32);
            this.nudAnalogValue.TabIndex = 9;
            // 
            // labelControl1
            // 
            this.labelControl1.AutoSize = true;
            this.labelControl1.Font = new System.Drawing.Font("Lucida Sans Unicode", 12F);
            this.labelControl1.Location = new System.Drawing.Point(257, 50);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(111, 20);
            this.labelControl1.TabIndex = 10;
            this.labelControl1.Text = "Analog value";
            // 
            // labelControl2
            // 
            this.labelControl2.AutoSize = true;
            this.labelControl2.Font = new System.Drawing.Font("Lucida Sans Unicode", 12F);
            this.labelControl2.Location = new System.Drawing.Point(506, 50);
            this.labelControl2.Name = "labelControl2";
            this.labelControl2.Size = new System.Drawing.Size(108, 20);
            this.labelControl2.TabIndex = 11;
            this.labelControl2.Text = "Digital value";
            // 
            // chbValue
            // 
            this.chbValue.AutoSize = true;
            this.chbValue.Location = new System.Drawing.Point(623, 56);
            this.chbValue.Name = "chbValue";
            this.chbValue.Size = new System.Drawing.Size(15, 14);
            this.chbValue.TabIndex = 12;
            this.chbValue.UseVisualStyleBackColor = true;
            // 
            // btnRefresh
            // 
            this.btnRefresh.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.btnRefresh.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.btnRefresh.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRefresh.Font = new System.Drawing.Font("Lucida Sans Unicode", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRefresh.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.btnRefresh.Location = new System.Drawing.Point(644, 547);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(146, 32);
            this.btnRefresh.TabIndex = 14;
            this.btnRefresh.Text = "Refresh";
            this.btnRefresh.UseVisualStyleBackColor = false;
            this.btnRefresh.Click += new System.EventHandler(this.BtnRefresh_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(804, 591);
            this.Controls.Add(this.btnRefresh);
            this.Controls.Add(this.dgvInputs);
            this.Controls.Add(this.chbValue);
            this.Controls.Add(this.labelControl2);
            this.Controls.Add(this.labelControl1);
            this.Controls.Add(this.nudAnalogValue);
            this.Controls.Add(this.btnWrite);
            this.Controls.Add(this.cbxOutputNames);
            this.Controls.Add(this.lblReasonOfFailure);
            this.Controls.Add(this.lblResourceStatus);
            this.Controls.Add(this.lblResourceName);
            this.Controls.Add(this.ledStatus);
            this.Font = new System.Drawing.Font("Lucida Sans Unicode", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Margin = new System.Windows.Forms.Padding(5);
            this.Name = "MainForm";
            this.Text = "EtherCAT test application";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvInputs)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudAnalogValue)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private UserInterface.Controls.LedControl ledStatus;
        private UserInterface.Controls.LabelControl lblResourceName;
        private UserInterface.Controls.LabelControl lblResourceStatus;
        private UserInterface.Controls.LabelControl lblReasonOfFailure;
        private System.Windows.Forms.DataGridView dgvInputs;
        private System.Windows.Forms.ComboBox cbxOutputNames;
        private UserInterface.Controls.ButtonControl btnWrite;
        private System.Windows.Forms.NumericUpDown nudAnalogValue;
        private UserInterface.Controls.LabelControl labelControl1;
        private UserInterface.Controls.LabelControl labelControl2;
        private System.Windows.Forms.CheckBox chbValue;
        private UserInterface.Controls.ButtonControl btnRefresh;
    }
}

