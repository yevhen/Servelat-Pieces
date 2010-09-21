namespace Sample.Desktop
{
	partial class NewsForm
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
			this.ticker = new Sample.Desktop.Ticker();
			this.SuspendLayout();
			// 
			// ticker
			// 
			this.ticker.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.ticker.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ticker.ForeColor = System.Drawing.SystemColors.InactiveCaptionText;
			this.ticker.Location = new System.Drawing.Point(0, 11);
			this.ticker.Name = "ticker";
			this.ticker.ScrollText = "";
			this.ticker.Size = new System.Drawing.Size(486, 36);
			this.ticker.TabIndex = 0;
			// 
			// NewsForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(486, 175);
			this.Controls.Add(this.ticker);
			this.Name = "NewsForm";
			this.Text = "NewsForm";
			this.Load += new System.EventHandler(this.NewsForm_Load);
			this.ResumeLayout(false);

		}

		#endregion

		private Ticker ticker;
	}
}

