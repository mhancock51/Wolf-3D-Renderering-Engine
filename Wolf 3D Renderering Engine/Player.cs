using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using TrigonometryLibrary;
using Generic_Grid_System;

namespace Wolf_3D_Renderering_Engine
{
    public class PlayerEventArgs : EventArgs
    {
        public float x { get; private set; }
        public float y { get; private set; }
        public float direction { get; private set; }
        public int playerSize { get; private set; }

        public PlayerEventArgs(float x, float y, float direction, int playerSize)
        {
            this.x = x;
            this.y = y;
            this.direction = direction;
            this.playerSize = playerSize;
        }

        public PlayerEventArgs()
        {

        }
    }

    public class Player
    {
        private static int fov = 90;

        public delegate void PlayerActions(PlayerEventArgs e);
        public event PlayerActions OnMove;
        public event PlayerActions OnChangeInDirection;

        private float x;
        private float y;
        public float direction { get; private set; }
        private float movementSpeed = 2;
        private int playerSize = 32;

        private GameController gameController;

        public Player(int x, int y, GameController gameController)
        {
            this.x = x;
            this.y = y;
            direction = 0;
            this.gameController = gameController;
        }

        public PlayerEventArgs GetPlayerInfo()
        {
            return new PlayerEventArgs(x, y, direction, playerSize);
        }

        public PointF GetCoords()
        {
            return new PointF(x, y);
        }

        public void Move(int dir)
        {
            PointF newPoint = Trigonometry.GetDeltaCoordsOfTriangle(direction + fov / 2, dir * movementSpeed);
            if (GameController.IsSpaceFree(new PointF(x + newPoint.X, y + newPoint.Y), gameController.wallLayout))
            {
                x += newPoint.X;
                y += newPoint.Y;
                OnMove?.Invoke(GetPlayerInfo());
            }
        }

        public void AdjustDirection(int increment)
        {
            direction += increment;
            if (direction < 0)
            {
                direction = 360 - -increment;
            }
            else if (direction > 359)
            {
                direction = 0;
            }

            OnChangeInDirection?.Invoke(GetPlayerInfo());
        }
    }
}
