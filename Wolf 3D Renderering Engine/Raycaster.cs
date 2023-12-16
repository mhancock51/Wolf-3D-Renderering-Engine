using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Generic_Grid_System;
using TrigonometryLibrary;

namespace Wolf_3D_Renderering_Engine
{
    public class RaycastInfo
    {
        public float distance { get; private set; }
        public int face { get; private set; } //0 horizontal face, 1 = vertical face
        public int cellX { get; private set; } //the coordinate of the collision with in the grid cell
        public float x { get; private set; }
        public float y { get; private set; }

        public RaycastInfo(float distance, int face, float x, float y, int cellX)
        {
            this.distance = distance;
            this.face = face;
            this.x = x; 
            this.y = y;
            this.cellX = cellX;
        }
    }

    public class Raycaster
    {
        private static int cellSize = 64;

        public static RaycastInfo WallRenderingRaycast(Grid<Wall> wallLayout, PointF coords, float angle)
        {
            RaycastInfo horizontalRaycast;
            RaycastInfo verticalRaycast;

            horizontalRaycast = CheckForHorizontalCollisions(wallLayout, coords, angle);
            verticalRaycast = CheckForVerticalCollisions(wallLayout, coords, angle);

            if (horizontalRaycast.distance == -1 && verticalRaycast.distance > -1)
            {
                return verticalRaycast;
            }
            else if (verticalRaycast.distance == -1 && horizontalRaycast.distance > -1)
            {
                return horizontalRaycast;
            }
            else if (horizontalRaycast.distance < verticalRaycast.distance)
            {
                return horizontalRaycast;
            }
            else
            {
                return verticalRaycast;
            }
        }

        public static bool HitWall(Grid<Wall> wallLayout, PointF coords, float angle, float maxDistance)
        {
            float distance = WallRenderingRaycast(wallLayout, coords, angle).distance;
            if (distance <= maxDistance)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static RaycastInfo[] GetAllHorizontalIntercepts(Grid<Wall> wallLayout, PointF coords, float angle)
        {
            //1 means angle is facing downwards
            //-1 means angle is facing upwards
            int direction = -1;
            if (angle > 90 && angle < 270)
            {
                direction = 1;
            }

            float dY = GetCellDelta(coords.Y, direction);
            float dX = -1;
            List<RaycastInfo> horizontalIntercepts = new List<RaycastInfo>();
            bool reachedBounds = false;
            while (!reachedBounds)
            {
                dX = -Trigonometry.GetOpposite(angle, dY);
                if (coords.X + dX < wallLayout.width * cellSize && coords.X + dX > 0 && coords.Y + dY < wallLayout.height * cellSize && coords.Y + dY > 0)
                {
                    Point snappedCoords = SnapCoordsToGrid(new PointF(coords.X + dX, coords.Y + dY), cellSize);
                    float distance = Trigonometry.GetDistanceFromAxisDeltas(dX, dY);
                    int cellX = (int)Math.Round(coords.X + dX - snappedCoords.X * cellSize);
                    if (cellX < 0)
                    {
                        cellX *= -1;
                    }
                    if (cellX > 63)
                    {
                        cellX = 63;
                    }

                    horizontalIntercepts.Add(new RaycastInfo(distance, 0, coords.X + dX, coords.Y + dY, cellX));
                    dY += cellSize * direction;
                }
                else
                {
                    reachedBounds = true;
                }
            }
            return horizontalIntercepts.ToArray();
        }

        public static RaycastInfo[] GetAllVerticalIntercepts(Grid<Wall> wallLayout, PointF coords, float angle)
        {
            //1 means angle is facing right
            //-1 means angle is facing left
            int direction = 1;
            if (angle > 180 && angle < 360)
            {
                direction = -1;
            }

            angle -= 90;

            float dY = -1;
            float dX = GetCellDelta(coords.X, direction);
            List<RaycastInfo> verticalIntercepts = new List<RaycastInfo>();
            bool reachedBounds = false;
            while (!reachedBounds)
            {
                dY = Trigonometry.GetOpposite(angle, dX);
                if (coords.X + dX < wallLayout.width * cellSize && coords.X + dX > 0 && coords.Y + dY < wallLayout.height * cellSize && coords.Y + dY > 0)
                {
                    Point snappedCoords = SnapCoordsToGrid(new PointF(coords.X + dX, coords.Y + dY), cellSize);
                    float distance = Trigonometry.GetDistanceFromAxisDeltas(dX, dY);
                    int cellX = (int)Math.Round(coords.Y + dY - snappedCoords.Y * cellSize);
                    if (cellX < 0)
                    {
                        cellX *= -1;
                    }
                    if (cellX > 63)
                    {
                        cellX = 63;
                    }

                    verticalIntercepts.Add(new RaycastInfo(distance, 1, coords.X + dX, coords.Y + dY, cellX));
                    dX += cellSize * direction;
                }
                else
                {
                    reachedBounds = true;
                }
            }
            return verticalIntercepts.ToArray();

            ///
        }

        public static RaycastInfo CheckForHorizontalCollisions(Grid<Wall> wallLayout, PointF coords, float angle)
        {
            int direction = -1; //-1 means facing upwards
            if (angle > 90 && angle < 270)
            {
                direction = 0; //0 means facing downwards
            }

            RaycastInfo[] horizontalIntercepts = GetAllHorizontalIntercepts(wallLayout, coords, angle);
            //float distance = -1;
            bool foundCollision = false;
            if (horizontalIntercepts.Length == 0)
            {
                return new RaycastInfo(-1, 0, 0, 0, 0);
            }
            else
            {
                int i = 0;
                while (i < horizontalIntercepts.Length && !foundCollision)
                {
                    Point snappedCoords = SnapCoordsToGrid(new PointF(horizontalIntercepts[i].x, horizontalIntercepts[i].y), cellSize);
                    snappedCoords = new Point(snappedCoords.X, snappedCoords.Y + direction);
                    if (!wallLayout.GetCell(snappedCoords.X, snappedCoords.Y).isEmpty)
                    {
                        foundCollision = true;
                        //distance = Trigonometry.GetDistanceFromAxisDeltas(horizontalIntercepts[i].x - coords.X, horizontalIntercepts[i].y - coords.Y);
                    }
                    else
                    {
                        i++;
                    }
                }
                if (foundCollision)
                {
                    return horizontalIntercepts[i];
                }
                else
                {
                    return new RaycastInfo(-1, 0, 0, 0, 0);
                }
            }
        }

        public static RaycastInfo CheckForVerticalCollisions(Grid<Wall> wallLayout, PointF coords, float angle)
        {
            int direction = 0; //-1 means facing left
            if (angle > 180 && angle < 360)
            {
                direction = -1; //0 means facing right
            }

            RaycastInfo[] verticalIntercepts = GetAllVerticalIntercepts(wallLayout, coords, angle);
            //float distance = -1;
            bool foundCollision = false;
            if (verticalIntercepts.Length == 0)
            {
                return new RaycastInfo(-1, 0, 0, 0, 0);
            }
            else
            {
                int i = 0;
                while (i < verticalIntercepts.Length && !foundCollision)
                {
                    Point snappedCoords = SnapCoordsToGrid(new PointF(verticalIntercepts[i].x, verticalIntercepts[i].y), cellSize);
                    snappedCoords = new Point(snappedCoords.X + direction, snappedCoords.Y);
                    if (!wallLayout.GetCell(snappedCoords.X, snappedCoords.Y).isEmpty)
                    {
                        foundCollision = true;
                        //distance = Trigonometry.GetDistanceFromAxisDeltas(verticalIntercepts[i].X - coords.X, verticalIntercepts[i].Y - coords.Y);
                    }
                    else
                    {
                        i++;
                    }
                }
                if (foundCollision)
                {
                    return verticalIntercepts[i];
                }
                else
                {
                    return new RaycastInfo(-1, 0, 0, 0, 0);
                }
            }
        }

        public static Point SnapCoordsToGrid(PointF coords, int cellSize)
        {
            return new Point((int)coords.X / cellSize, (int)coords.Y / cellSize);
        }

        private static float GetCellDelta(float coord, float direction)
        {
            int snapped = (int)coord / cellSize;

            if (direction == 1)
            {
                return -(coord - (snapped + 1) * cellSize); //get bottom cellDelta
            }
            else
            {
                return -(coord - snapped * cellSize); //get top cellDelta
            }
        }
    }
}
