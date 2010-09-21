using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Sample.Desktop
{
	public partial class Ticker : Panel
	{
		Timer timer;
		
		int offset;
		Size textWidth;
		Bitmap buffer;

		Queue<string> feed = new Queue<string>();
		string text = "";

		public Ticker()
		{
			InitializeComponent();

			timer = new Timer
			{
				Interval = 1, 
				Enabled = false,
			};

			timer.Tick += DoScroll;
		}

		public string ScrollText
		{
			get { return text; }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					timer.Enabled = false;
					return;
				}

				if (!timer.Enabled)
				{
					text = value;
					textWidth = TextRenderer.MeasureText(text, Font);
					offset = Width;
					timer.Enabled = true;
				}
				else
				{
					feed.Enqueue(value);
				}
			}
		}

		private void DoScroll(object sender, EventArgs e)
		{
			offset -= 1;

			if (offset < -textWidth.Width)
			{
				if (feed.Count > 0)
				{
					text = feed.Dequeue();
					textWidth = TextRenderer.MeasureText(text, Font);
				}

				offset = Width;
			}

			Invalidate();
		}
		
		protected override void OnPaintBackground(PaintEventArgs e)
		{
			// Do nothing
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			if (buffer == null || buffer.Width != Width || buffer.Height != Height)
				buffer = new Bitmap(Width, Height);

			Graphics graphics = Graphics.FromImage(buffer);
			Brush backColorBrush = new SolidBrush(BackColor);
			Brush foreColorBrush = new SolidBrush(ForeColor);
			
			graphics.FillRectangle(backColorBrush, new Rectangle(0, 0, Width, Height));
			graphics.DrawString(text, Font, foreColorBrush, offset, 0);
			e.Graphics.DrawImage(buffer, 0, 0);

			backColorBrush.Dispose();
			foreColorBrush.Dispose();
			graphics.Dispose();
		}
	}
}
