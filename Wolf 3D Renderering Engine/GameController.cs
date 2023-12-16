using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Generic_Grid_System;

namespace Wolf_3D_Renderering_Engine
{
    public class Wall
    {
        public int wallTextureIndex { get; private set; }
        public bool isEmpty { get; private set; }

        public Wall(int wallTextureIndex, bool isEmpty)
        {
            this.isEmpty = isEmpty;
            this.wallTextureIndex = wallTextureIndex;
        }

        public Wall(int wallTextureIndexe)
        {
            isEmpty = true;
            this.wallTextureIndex = wallTextureIndex;
        }
    }

    public enum RenderingMode { TopDown, Raycast }
    public class GameController
    {
        private static int cellSize = 64;

        public Player player { get; private set; }
        public RaycastRenderingEngine raycastRenderingEngine { get; private set; }
        public TopDownRenderingEngine topDownRenderingEngine { get; private set; }

        public Grid<Wall> wallLayout { get; private set; }

        public GameController(MainForm mainForm)
        {
            RaycastRenderingEngine.LoadTexturesFromFiles();
            InitialiseWallLayout();
            player = new Player(cellSize * 2, cellSize * 2, this);

            //topDownRenderingEngine = new TopDownRenderingEngine(mainForm, this);
            raycastRenderingEngine = new RaycastRenderingEngine(mainForm, this);
        }

        private void InitialiseWallLayout()
        {
            wallLayout = new Grid<Wall>(10, 10);

            for (int x = 0; x < wallLayout.width; x++)
            {
                for (int y = 0; y < wallLayout.height; y++)
                {
                    wallLayout.SetCell(x, y, new Wall(0, true));
                }
            }

            for (int i = 0; i < wallLayout.width; i++)
            {
                wallLayout.SetCell(0, i, new Wall(0, false));
                wallLayout.SetCell(wallLayout.width - 1, i, new Wall(0, false));
                wallLayout.SetCell(i, 0, new Wall(0, false));
                wallLayout.SetCell(i, wallLayout.height - 1, new Wall(0, false));
            }

            Random random = new Random();
            for (int i = 0; i < 5; i++)
            {
                wallLayout.SetCell(random.Next(1, wallLayout.width - 1), random.Next(1, wallLayout.height - 1), new Wall(0, false));
            }
        }

        public static bool IsSpaceFree(PointF newPosition, Grid<Wall> WallLayout)
        {
            Point gridCoords = Raycaster.SnapCoordsToGrid(newPosition, cellSize);
            if (WallLayout.GetCell(gridCoords.X, gridCoords.Y).isEmpty)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
