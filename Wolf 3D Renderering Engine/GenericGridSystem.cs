using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Generic_Grid_System
{
    public class Grid<TGridObject>
    {
        public int width { get; private set; }
        public int height { get; private set; }

        public TGridObject[,] grid { get; private set; }

        public Grid(int width, int height)
        {
            this.width = width;
            this.height = height;

            grid = new TGridObject[width, height];
        }

        public void SetCell(int x, int y, TGridObject newGridObject)
        {
            grid[x, y] = newGridObject;
        }

        public TGridObject GetCell(int x, int y)
        {
            return grid[x, y];
        }

        public TGridObject[] GetColumn(int x)
        {
            TGridObject[] column = new TGridObject[height];
            for (int y = 0; y < height; y++)
            {
                column[y] = GetCell(x, y);
            }
            return column;
        }
    }
}
