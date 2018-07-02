namespace BioMetrixCore
{
    partial class Master
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Master));
            this.lblStatus = new System.Windows.Forms.Label();
            this.btnBeep = new System.Windows.Forms.Button();
            this.pnlHeader = new System.Windows.Forms.Panel();
            this.dgvRecords = new System.Windows.Forms.DataGridView();
            this.btnPowerOff = new System.Windows.Forms.Button();
            this.btnRestartDevice = new System.Windows.Forms.Button();
            this.btnGetDeviceTime = new System.Windows.Forms.Button();
            this.btnEnableDevice = new System.Windows.Forms.Button();
            this.btnDisableDevice = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.btnPullData = new System.Windows.Forms.Button();
            this.btnUploadUserInfo = new System.Windows.Forms.Button();
            this.lblDeviceInfo = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgvRecords)).BeginInit();
            this.panel1.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblStatus
            // 
            this.lblStatus.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lblStatus.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblStatus.Location = new System.Drawing.Point(0, 483);
            this.lblStatus.Margin = new System.Windows.Forms.Padding(3, 5, 3, 0);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Padding = new System.Windows.Forms.Padding(0, 5, 0, 5);
            this.lblStatus.Size = new System.Drawing.Size(815, 25);
            this.lblStatus.TabIndex = 6;
            this.lblStatus.Text = "label3";
            // 
            // btnBeep
            // 
            this.btnBeep.Location = new System.Drawing.Point(173, 3);
            this.btnBeep.Name = "btnBeep";
            this.btnBeep.Size = new System.Drawing.Size(59, 48);
            this.btnBeep.TabIndex = 5;
            this.btnBeep.Text = "Beep";
            this.btnBeep.UseVisualStyleBackColor = true;
           // this.btnBeep.Click += new System.EventHandler(this.btnBeep_Click);
            // 
            // pnlHeader
            // 
            this.pnlHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(236)))), ((int)(((byte)(236)))), ((int)(((byte)(236)))));
            this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(815, 37);
            this.pnlHeader.TabIndex = 712;
            this.pnlHeader.Paint += new System.Windows.Forms.PaintEventHandler(this.pnlHeader_Paint);
            // 
            // dgvRecords
            // 
            this.dgvRecords.AllowUserToAddRows = false;
            this.dgvRecords.AllowUserToDeleteRows = false;
            this.dgvRecords.AllowUserToOrderColumns = true;
            this.dgvRecords.AllowUserToResizeColumns = false;
            this.dgvRecords.AllowUserToResizeRows = false;
            this.dgvRecords.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvRecords.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvRecords.Location = new System.Drawing.Point(0, 54);
            this.dgvRecords.Name = "dgvRecords";
            this.dgvRecords.Size = new System.Drawing.Size(791, 360);
            this.dgvRecords.TabIndex = 883;
            // 
            // btnPowerOff
            // 
            this.btnPowerOff.Location = new System.Drawing.Point(451, 3);
            this.btnPowerOff.Name = "btnPowerOff";
            this.btnPowerOff.Size = new System.Drawing.Size(65, 48);
            this.btnPowerOff.TabIndex = 885;
            this.btnPowerOff.Text = "Power Off";
            this.btnPowerOff.UseVisualStyleBackColor = true;
            //this.btnPowerOff.Click += new System.EventHandler(this.btnPowerOff_Click);
            // 
            // btnRestartDevice
            // 
            this.btnRestartDevice.Location = new System.Drawing.Point(380, 3);
            this.btnRestartDevice.Name = "btnRestartDevice";
            this.btnRestartDevice.Size = new System.Drawing.Size(65, 48);
            this.btnRestartDevice.TabIndex = 886;
            this.btnRestartDevice.Text = "Restart Device";
            this.btnRestartDevice.UseVisualStyleBackColor = true;
           // this.btnRestartDevice.Click += new System.EventHandler(this.btnRestartDevice_Click);
            // 
            // btnGetDeviceTime
            // 
            this.btnGetDeviceTime.Location = new System.Drawing.Point(89, 3);
            this.btnGetDeviceTime.Name = "btnGetDeviceTime";
            this.btnGetDeviceTime.Size = new System.Drawing.Size(78, 48);
            this.btnGetDeviceTime.TabIndex = 887;
            this.btnGetDeviceTime.Text = "Get Device Time";
            this.btnGetDeviceTime.UseVisualStyleBackColor = true;
           // this.btnGetDeviceTime.Click += new System.EventHandler(this.btnGetDeviceTime_Click);
            // 
            // btnEnableDevice
            // 
            this.btnEnableDevice.Location = new System.Drawing.Point(238, 3);
            this.btnEnableDevice.Name = "btnEnableDevice";
            this.btnEnableDevice.Size = new System.Drawing.Size(65, 48);
            this.btnEnableDevice.TabIndex = 889;
            this.btnEnableDevice.Text = "Enable Device";
            this.btnEnableDevice.UseVisualStyleBackColor = true;
           // this.btnEnableDevice.Click += new System.EventHandler(this.btnEnableDevice_Click);
            // 
            // btnDisableDevice
            // 
            this.btnDisableDevice.Location = new System.Drawing.Point(309, 3);
            this.btnDisableDevice.Name = "btnDisableDevice";
            this.btnDisableDevice.Size = new System.Drawing.Size(65, 48);
            this.btnDisableDevice.TabIndex = 890;
            this.btnDisableDevice.Text = "Disable Device";
            this.btnDisableDevice.UseVisualStyleBackColor = true;
            //this.btnDisableDevice.Click += new System.EventHandler(this.btnDisableDevice_Click);
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Controls.Add(this.dgvRecords);
            this.panel1.Controls.Add(this.flowLayoutPanel1);
            this.panel1.Location = new System.Drawing.Point(12, 68);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(791, 414);
            this.panel1.TabIndex = 891;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.AutoSize = true;
            this.flowLayoutPanel1.Controls.Add(this.btnPullData);
            this.flowLayoutPanel1.Controls.Add(this.btnGetDeviceTime);
            this.flowLayoutPanel1.Controls.Add(this.btnBeep);
            this.flowLayoutPanel1.Controls.Add(this.btnEnableDevice);
            this.flowLayoutPanel1.Controls.Add(this.btnDisableDevice);
            this.flowLayoutPanel1.Controls.Add(this.btnRestartDevice);
            this.flowLayoutPanel1.Controls.Add(this.btnPowerOff);
            this.flowLayoutPanel1.Controls.Add(this.btnUploadUserInfo);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(791, 54);
            this.flowLayoutPanel1.TabIndex = 0;
            // 
            // btnPullData
            // 
            this.btnPullData.Location = new System.Drawing.Point(3, 3);
            this.btnPullData.Name = "btnPullData";
            this.btnPullData.Size = new System.Drawing.Size(80, 48);
            this.btnPullData.TabIndex = 10;
            this.btnPullData.Text = "Get Log Data";
            this.btnPullData.UseVisualStyleBackColor = true;
            // 
            // btnUploadUserInfo
            // 
            this.btnUploadUserInfo.Location = new System.Drawing.Point(522, 3);
            this.btnUploadUserInfo.Name = "btnUploadUserInfo";
            this.btnUploadUserInfo.Size = new System.Drawing.Size(65, 48);
            this.btnUploadUserInfo.TabIndex = 893;
            this.btnUploadUserInfo.Text = "Upload User Info";
            this.btnUploadUserInfo.UseVisualStyleBackColor = true;
            // 
            // lblDeviceInfo
            // 
            this.lblDeviceInfo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblDeviceInfo.Location = new System.Drawing.Point(11, 45);
            this.lblDeviceInfo.Name = "lblDeviceInfo";
            this.lblDeviceInfo.Size = new System.Drawing.Size(792, 19);
            this.lblDeviceInfo.TabIndex = 892;
            this.lblDeviceInfo.Text = "Device Info : --";
            // 
            // Master
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(815, 508);
            this.Controls.Add(this.lblDeviceInfo);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.pnlHeader);
            this.Controls.Add(this.lblStatus);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(615, 425);
            this.Name = "Master";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Biometric Device Demo";
            ((System.ComponentModel.ISupportInitialize)(this.dgvRecords)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Button btnBeep;
        private System.Windows.Forms.Panel pnlHeader;
        private System.Windows.Forms.DataGridView dgvRecords;
        private System.Windows.Forms.Button btnPowerOff;
        private System.Windows.Forms.Button btnRestartDevice;
        private System.Windows.Forms.Button btnGetDeviceTime;
        private System.Windows.Forms.Button btnEnableDevice;
        private System.Windows.Forms.Button btnDisableDevice;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Label lblDeviceInfo;
        private System.Windows.Forms.Button btnUploadUserInfo;
        private System.Windows.Forms.Button btnPullData;
    }
}

