using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using Generic_Grid_System;
using TrigonometryLibrary;
using DrawingLibrary;

namespace Wolf_3D_Renderering_Engine
{

    public class RaycastRenderingEngine
    {
        public static Grid<Color>[] textures;

        private static int cellSize = 64;
        private static int fov = 90;
        private static Size raycastWindowSize = new Size(fov * 8, (int)Math.Round(0.5625 * (fov * 8)));

        private Bitmap frame;
        private MainForm mainForm;
        private GameController gameController;

        private int columnWidth;
        private int centreY;
        private int wallHeightConstant = raycastWindowSize.Height;

        private Color floorColour = Color.DarkBlue;

        public RaycastRenderingEngine(MainForm mainForm, GameController gameController)
        {
            this.mainForm = mainForm;
            this.gameController = gameController;

            gameController.player.OnMove += RenderNewFrame;
            gameController.player.OnChangeInDirection += RenderNewFrame;

            mainForm.InitialiseFormSize(raycastWindowSize);
            columnWidth = raycastWindowSize.Width / fov;
            centreY = raycastWindowSize.Height / 2;

            RenderNewFrame(gameController.player.GetPlayerInfo());
        }

        public void RenderNewFrame(PlayerEventArgs e)
        {
            frame = new Bitmap(raycastWindowSize.Width, raycastWindowSize.Height);

            for (int i = 0; i < fov; i++)
            {
                RaycastInfo info = Raycaster.WallRenderingRaycast(gameController.wallLayout, gameController.player.GetCoords(), gameController.player.direction + i);
                float distance = CorrectDistortion(i - fov/2, info.distance);
                distance = (float)distance / cellSize;
                float wallColumnHeight = CalculateWallColumnHeight(distance);
                
                RenderWallColumn(frame, i, wallColumnHeight, info.cellX, 1, info.face);
            }

            //RenderMiniMap(frame, e);
            //DrawFPSGun(frame);
            mainForm.RenderFrame(frame);
        }

        private void DrawFPSGun(Bitmap frame)
        {
            Image gunImage = Image.FromFile(@"C:\Users\MattH\source\repos\Wolf 3D Renderering Engine\Wolf 3D Renderering Engine\ak47.png");
            Drawing.DrawImage(frame, gunImage, new Point(frame.Width / 2 - gunImage.Width / 2, (int)Math.Round(frame.Height - gunImage.Height * 1.25f)));
        }

        private static float CorrectDistortion(float angle, float distance)
        {
            return Trigonometry.GetAdjacent(angle, distance);
        }

        private void RenderMiniMap(Bitmap frame, PlayerEventArgs e)
        {
            Graphics graphics = Graphics.FromImage(frame);
            int mapCellSize = 32;
            Grid<Wall> wallLayout = gameController.wallLayout;
            for (int x = 0; x < wallLayout.width; x++)
            {
                for (int y = 0; y < wallLayout.height; y++)
                {
                    Color cellColour = Color.Gray;
                    if (!wallLayout.GetCell(x, y).isEmpty)
                    {
                        cellColour = Color.DarkGray;
                    }
                    Drawing.DrawRectangle(frame, x * mapCellSize, y * mapCellSize, mapCellSize, mapCellSize, cellColour);
                }
            }

            Drawing.DrawCircle(frame, e.x, e.y, mapCellSize / 2, Color.Yellow);

            PointF point = Trigonometry.GetDeltaCoordsOfTriangle(e.direction, mapCellSize);
            Drawing.DrawLine(frame, new PointF(e.x, e.y), new PointF(e.x + point.X, e.y + point.Y), Color.Yellow, 1);

            graphics.Dispose();
        }

        private float CalculateWallColumnHeight(float distance)
        {
            if (distance <= 0)
            {
                distance = 1;
            }
            return wallHeightConstant / distance;
        }

        private void RenderWallColumn(Bitmap frame, int x, float height, int cellX, int wallTextureIndex, int face)
        {
            Color[] textureColumn = textures[wallTextureIndex].GetColumn(cellX);
            int textureHeight = textureColumn.Length;
            float scaleMultiplier = height / textureHeight;
            for (int i = 0; i < textureColumn.Length; i++)
            {
                Color pixel = textureColumn[i];
                if (face == 1)
                {
                    pixel = ControlPaint.Light(pixel);
                }

                Drawing.DrawRectangle(frame, x * columnWidth, centreY - (float)height / 2 + scaleMultiplier * i, columnWidth, scaleMultiplier, pixel);
            }

            //Drawing.DrawRectangle(frame, x * columnWidth, centreY - (float)height / 2, columnWidth, height, wallColour);
            Drawing.DrawRectangle(frame, x * columnWidth, centreY + (float)height / 2, columnWidth, raycastWindowSize.Height - (centreY + (float)height / 2), floorColour);
        }

        public static Grid<Color> ConvertImageToGrid(Bitmap image)
        {
            Grid<Color> imageGrid = new Grid<Color>(image.Width, image.Height);
            for (int x = 0; x < imageGrid.width; x++)
            {
                for (int y = 0; y < imageGrid.height; y++)
                {
                    imageGrid.SetCell(x, y, image.GetPixel(x, y));
                }
            }
            return imageGrid;
        }

        public static Grid<Color> ConvertImageToGrid(string dir)
        {
            Image image = Image.FromFile(dir);
            return ConvertImageToGrid((Bitmap)image);
        }

        public static void LoadTexturesFromFiles()
        {
            string[] files = Directory.GetFiles(@"../assets/wall textures");
            textures = new Grid<Color>[files.Length];
            for (int i = 0; i < files.Length; i++)
            {
                Image imgTexture = Image.FromFile(files[i]);
                textures[i] = ConvertImageToGrid((Bitmap)imgTexture);
            }
        }
    }
}
