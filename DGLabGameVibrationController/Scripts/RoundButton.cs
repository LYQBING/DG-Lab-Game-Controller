using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace DGLabGameVibrationController
{
	public class RoundButton : Button
	{
		public int CornerRadius { get; set; } = 12;
		protected override void OnPaint(PaintEventArgs pevent)
		{
			base.OnPaint(pevent);
			Rectangle rect = this.ClientRectangle;
			using (GraphicsPath path = new GraphicsPath())
			{
				path.AddArc(rect.X, rect.Y, CornerRadius, CornerRadius, 180, 90);
				path.AddArc(rect.Right - CornerRadius, rect.Y, CornerRadius, CornerRadius, 270, 90);
				path.AddArc(rect.Right - CornerRadius, rect.Bottom - CornerRadius, CornerRadius, CornerRadius, 0, 90);
				path.AddArc(rect.X, rect.Bottom - CornerRadius, CornerRadius, CornerRadius, 90, 90);
				path.CloseAllFigures();
				this.Region = new Region(path);
			}
		}
	}
}
