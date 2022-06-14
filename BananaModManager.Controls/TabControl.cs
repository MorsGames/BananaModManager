using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace BananaModManager.Controls
{
    public partial class TabControl : System.Windows.Forms.TabControl
    {
        public TabControl(IContainer container)
        {
            container.Add(this);

            //InitializeComponent();

            SetStyle(
                ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.ResizeRedraw |
                ControlStyles.OptimizedDoubleBuffer, true);
        }

        [Description("The offset of the label.")]
        public Point Offset { get; set; } = new Point(4, 2);

        [Description("The color of the background thing!")]
        [Category("Color")]
        public Color TabBackColor { get; set; }

        [Description("The color of the thing!")]
        [Category("Color")]
        public Color TabColor { get; set; }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;

            g.Clear(TabBackColor);

            for (var i = 0; i <= TabCount - 1; i++)
            {
                var header = new Rectangle(new Point(GetTabRect(i).Location.X, GetTabRect(i).Location.Y),
                    new Size(GetTabRect(i).Width, GetTabRect(i).Height));
                var textSize = new Rectangle(header.Location.X + Offset.X, header.Location.Y + Offset.Y, header.Width,
                    header.Height);

                if (i == SelectedIndex)
                {
                    g.FillRectangle(new SolidBrush(TabColor), header);

                    var labelColor = TabColor.GetBrightness() < 0.5 ? SystemColors.Window : SystemColors.ControlText;

                    g.DrawString(TabPages[i].Text, Font, new SolidBrush(labelColor), textSize);
                }
                else
                {
                    g.DrawString(TabPages[i].Text, Font, new SolidBrush(SystemColors.ControlText), textSize);
                }
            }

            g.FillRectangle(new SolidBrush(TabColor), new Rectangle(0, ItemSize.Height, Width, 4));
        }
    }
}