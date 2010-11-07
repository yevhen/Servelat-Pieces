namespace Sample.Custom
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
			this.downloadButton = new System.Windows.Forms.Button();
			this.queueListBox = new System.Windows.Forms.ListBox();
			this.urlTextBox = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// downloadButton
			// 
			this.downloadButton.Location = new System.Drawing.Point(370, 24);
			this.downloadButton.Name = "downloadButton";
			this.downloadButton.Size = new System.Drawing.Size(75, 23);
			this.downloadButton.TabIndex = 0;
			this.downloadButton.Text = "Download";
			this.downloadButton.UseVisualStyleBackColor = true;
			this.downloadButton.Click += new System.EventHandler(this.downloadButton_Click);
			// 
			// queueListBox
			// 
			this.queueListBox.FormattingEnabled = true;
			this.queueListBox.Location = new System.Drawing.Point(12, 52);
			this.queueListBox.Name = "queueListBox";
			this.queueListBox.Size = new System.Drawing.Size(352, 186);
			this.queueListBox.TabIndex = 1;
			// 
			// urlTextBox
			// 
			this.urlTextBox.Location = new System.Drawing.Point(12, 26);
			this.urlTextBox.Name = "urlTextBox";
			this.urlTextBox.Size = new System.Drawing.Size(352, 20);
			this.urlTextBox.TabIndex = 2;
			this.urlTextBox.Text = "http://www.msn.com";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(12, 10);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(23, 13);
			this.label1.TabIndex = 3;
			this.label1.Text = "Url:";
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(456, 258);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.urlTextBox);
			this.Controls.Add(this.queueListBox);
			this.Controls.Add(this.downloadButton);
			this.Name = "MainForm";
			this.Text = "MainForm";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button downloadButton;
		private System.Windows.Forms.ListBox queueListBox;
		private System.Windows.Forms.TextBox urlTextBox;
		private System.Windows.Forms.Label label1;
	}
}