
namespace View
{
    partial class TankWars
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
            this.IPTextBox = new System.Windows.Forms.TextBox();
            this.playerNameTextBox = new System.Windows.Forms.TextBox();
            this.connectButton = new System.Windows.Forms.Button();
            this.AddressLabel = new System.Windows.Forms.Label();
            this.playerNameLabel = new System.Windows.Forms.Label();
            this.eventLog1 = new System.Diagnostics.EventLog();
            ((System.ComponentModel.ISupportInitialize)(this.eventLog1)).BeginInit();
            this.SuspendLayout();
            // 
            // IPTextBox
            // 
            this.IPTextBox.Location = new System.Drawing.Point(13, 33);
            this.IPTextBox.Name = "IPTextBox";
            this.IPTextBox.Size = new System.Drawing.Size(100, 20);
            this.IPTextBox.TabIndex = 0;
            this.IPTextBox.Text = "localhost";
            // 
            // playerNameTextBox
            // 
            this.playerNameTextBox.Location = new System.Drawing.Point(120, 33);
            this.playerNameTextBox.MaxLength = 16;
            this.playerNameTextBox.Name = "playerNameTextBox";
            this.playerNameTextBox.Size = new System.Drawing.Size(100, 20);
            this.playerNameTextBox.TabIndex = 1;
            // 
            // connectButton
            // 
            this.connectButton.Location = new System.Drawing.Point(226, 33);
            this.connectButton.Name = "connectButton";
            this.connectButton.Size = new System.Drawing.Size(70, 20);
            this.connectButton.TabIndex = 2;
            this.connectButton.Text = "Connect";
            this.connectButton.UseVisualStyleBackColor = true;
            this.connectButton.Click += new System.EventHandler(this.connectButton_Click);
            // 
            // AddressLabel
            // 
            this.AddressLabel.AutoSize = true;
            this.AddressLabel.Location = new System.Drawing.Point(13, 14);
            this.AddressLabel.Name = "AddressLabel";
            this.AddressLabel.Size = new System.Drawing.Size(82, 13);
            this.AddressLabel.TabIndex = 3;
            this.AddressLabel.Text = "Server Address:";
            // 
            // playerNameLabel
            // 
            this.playerNameLabel.AutoSize = true;
            this.playerNameLabel.Location = new System.Drawing.Point(120, 14);
            this.playerNameLabel.Name = "playerNameLabel";
            this.playerNameLabel.Size = new System.Drawing.Size(70, 13);
            this.playerNameLabel.TabIndex = 4;
            this.playerNameLabel.Text = "Player Name:";
            // 
            // eventLog1
            // 
            this.eventLog1.SynchronizingObject = this;
            // 
            // TankWars
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(884, 921);
            this.Controls.Add(this.playerNameLabel);
            this.Controls.Add(this.AddressLabel);
            this.Controls.Add(this.connectButton);
            this.Controls.Add(this.playerNameTextBox);
            this.Controls.Add(this.IPTextBox);
            this.Name = "TankWars";
            this.Text = "TankWars";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.TankWars_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.eventLog1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox IPTextBox;
        private System.Windows.Forms.TextBox playerNameTextBox;
        private System.Windows.Forms.Button connectButton;
        private System.Windows.Forms.Label AddressLabel;
        private System.Windows.Forms.Label playerNameLabel;
        private System.Diagnostics.EventLog eventLog1;
    }
}

