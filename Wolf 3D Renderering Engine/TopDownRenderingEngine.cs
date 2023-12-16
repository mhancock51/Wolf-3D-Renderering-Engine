using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;
using Generic_Grid_System;
using TrigonometryLibrary;
using DrawingLibrary;

namespace Wolf_3D_Renderering_Engine
{
    public class TopDownRenderingEngine
    {
        private static int cellSize = 64;
        private static int fov = 90;

        private Bitmap wallbmp;
        private Size levelSize;
        private MainForm mainForm;
        private GameController gameController;
        private Grid<Wall> wall_layout;

        public TopDownRenderingEngine(MainForm mainForm, GameController gameController)
        {
            wall_layout = gameController.wallLayout;
            this.mainForm = mainForm;
            this.gameController = gameController;
            levelSize = new Size(wall_layout.width * cellSize, wall_layout.height * cellSize);

            gameController.player.OnMove += RenderNewFrame;
            gameController.player.OnChangeInDirection += RenderNewFrame;

            mainForm.InitialiseFormSize(levelSize);
            InitialiseWallbmp();

            RenderNewFrame(gameController.player.GetPlayerInfo());
        }

        private void RenderNewFrame(PlayerEventArgs e)
        {
            Bitmap frame = wallbmp.Clone(new Rectangle(0, 0, wallbmp.Width, wallbmp.Height), wallbmp.PixelFormat);
            DrawRaycasts(frame, e);
            DrawPlayer(frame, e);  
            mainForm.RenderFrame(frame);
        }

        private void InitialiseWallbmp()
        {
            wallbmp = new Bitmap(levelSize.Width, levelSize.Height);
            Color fillColour = Color.DarkGray;
            Color outlineColour = Color.Black;

            for (int x = 0; x < wall_layout.width; x++)
            {
                for (int y = 0; y < wall_layout.height; y++)
                {
                    if (!wall_layout.GetCell(x, y).isEmpty)
                    {
                        Drawing.DrawRectangle(wallbmp, x * cellSize, y * cellSize, cellSize, cellSize, fillColour);
                        Drawing.DrawRectangleOutline(wallbmp, x * cellSize, y * cellSize, cellSize, cellSize, outlineColour);
                    }
                }
            }
        }

        private void DrawRaycasts(Bitmap frame, PlayerEventArgs e)
        {
            PointF playerCoords = new PointF(e.x, e.y);
            float angle = e.direction;

            for (int i = 0; i < fov; i++)
            {
                RaycastInfo info = Raycaster.WallRenderingRaycast(wall_layout, playerCoords, angle + i);
                float distance = info.distance;
                distance = Trigonometry.GetAdjacent(i - fov / 2, distance);

                PointF endPoint = Trigonometry.GetDeltaCoordsOfTriangle(angle + i, distance);
                endPoint = new PointF(playerCoords.X + endPoint.X, playerCoords.Y + endPoint.Y);
                Drawing.DrawLine(frame, playerCoords, endPoint, Color.Red, 1);
                //Drawing.DrawCircle(frame, Raycaster.SnapCoordsToGrid(new PointF(info.x, info.y), cellSize).X * cellSize + info.cellX, info.y, 4, Color.Red);
            }
        }

        private void DrawPlayer(Bitmap frame, PlayerEventArgs e)
        {
            //Draw Player Circle
            Drawing.DrawCircle(frame, e.x - (float)e.playerSize / 2, e.y - (float)e.playerSize / 2, e.playerSize, Color.Blue);
            Drawing.DrawCircleOutline(frame, e.x - (float)e.playerSize / 2, e.y - (float)e.playerSize / 2, e.playerSize, Color.Black);

            //Draw Player Direction Line
            PointF lineEndPoint = Trigonometry.GetDeltaCoordsOfTriangle(e.direction + fov / 2, e.playerSize);
            lineEndPoint = new PointF(e.x + lineEndPoint.X, e.y + lineEndPoint.Y);
            Drawing.DrawLine(frame, new PointF(e.x, e.y), lineEndPoint, Color.Blue, e.playerSize / 8);
        }
    }
}
