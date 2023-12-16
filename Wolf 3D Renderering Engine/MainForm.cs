using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;

namespace Wolf_3D_Renderering_Engine
{
    public partial class MainForm : Form
    {
        private static Size raycast_window_size = new Size(800, 450);

        private Size fixedFormSize;
        private GameController gameController;

        private Bitmap frame;

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            typeof(MainForm).InvokeMember("DoubleBuffered",         //This stops the window from flickering when drawing to it
                BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic,
                null, this, new object[] { true });

            InitialiseFormSize(raycast_window_size);
            frame = new Bitmap(Width, Height);

            gameController = new GameController(this);
        }

        public void InitialiseFormSize(Size windowSize)
        {
            fixedFormSize = new Size(windowSize.Width + 16, windowSize.Height + 39);
            Size = fixedFormSize;
            Point centre = new Point(Screen.GetBounds(this).Width / 2 - Width / 2, Screen.GetBounds(this).Height / 2 - Height / 2);
            Location = centre;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.DrawImage(frame, new Point(0, 0));
            base.OnPaint(e);
        }

        public void RenderFrame(Bitmap frame)
        {
            this.frame = frame;
            Refresh();
        }

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.R)
            {
                gameController = new GameController(this);
            }
            else if (e.KeyCode == Keys.W)
            {
                gameController.player.Move(1);
            }
            else if (e.KeyCode == Keys.S)
            {
                gameController.player.Move(-1);
            }
            else if (e.KeyCode == Keys.D)
            {
                gameController.player.AdjustDirection(5);
            }
            else if (e.KeyCode == Keys.A)
            {
                gameController.player.AdjustDirection(-5);
            }

            PlayerEventArgs player = gameController.player.GetPlayerInfo();
            label1.Text = player.x + ", " + player.y + " - " + player.direction;
        }

        private void MainForm_ResizeBegin(object sender, EventArgs e)
        {
            Size = fixedFormSize;
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            Size = fixedFormSize;
        }
    }
}
