using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace anaclo
{
    public sealed class Form1 : Form
    {
        private IContainer components;
        private Timer _timer;

        private int _radius;
        private Point _origin;
        private readonly Dictionary<string, Pen> _pens = new Dictionary<string, Pen>();
        private Rectangle _circleBounds;

        public Form1()
        {
            InitializeComponent();
            Paint += (o, e) => DrawClock(e.Graphics);
            WinApi.MakeDraggable(this);
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            BackColor = Color.Lime;
            TransparencyKey = Color.Lime;
        }

        private void DrawClock(Graphics g)
        {
            g.SmoothingMode = SmoothingMode.HighQuality;

            g.FillEllipse(Brushes.Gainsboro, ClientRectangle);

            for (var m = 0; m < 60; m++)
                g.DrawLine(_pens["t"], GetCoords(m, 60, m % 5 == 0 ? 0.9 : 0.98), GetCoords(m, 60));

            var now = DateTime.Now;
            g.DrawLine(_pens["h"], _origin, GetCoords(now.Hour, 12, 0.85));
            g.DrawLine(_pens["m"], _origin, GetCoords(now.Minute, 60));
            g.DrawLine(_pens["s"], _origin, GetCoords(now.Second, 60));
            g.FillEllipse(Brushes.Black, _circleBounds);
        }

        private Point GetCoords(int value, int max, double scale = 1.0)
        {
            var angle = value * 2 * Math.PI / max - 2 * Math.PI / 4;
            var x = (int)(Math.Cos(angle) * _radius * scale) + _radius;
            var y = (int)(Math.Sin(angle) * _radius * scale) + _radius;
            return new Point(x, y);
        }

        private void InitializeComponent()
        {
            components = new Container();
            SuspendLayout();

            BackColor = SystemColors.ControlDarkDark;
            ClientSize = new Size(200, 200);
            DoubleBuffered = true;
            FormBorderStyle = FormBorderStyle.None;

            _timer = new Timer(components) { Enabled = true, Interval = 1000 };
            _timer.Tick += (sender, e) => Invalidate();

            Load += (o, e) =>
            {
                _radius = ClientSize.Width / 2;
                _origin = new Point(_radius, _radius);
                _pens.Add("t", Pens.Gray);
                _pens.Add("s", Pens.Red);
                _pens.Add("m", new Pen(Brushes.Black, 3));
                _pens.Add("h", new Pen(Brushes.Black, 6));
                const int r = 5;
                _circleBounds = new Rectangle(_origin.X - 5, _origin.Y - r, r * 2, r * 2);
                Invalidate();
            };
            ResumeLayout(false);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                components?.Dispose();
            base.Dispose(disposing);
        }
    }
}
