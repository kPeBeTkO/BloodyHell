using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RayCasting;

namespace BloodyHell.Entities
{
    public enum Parameters // добавть статы кроме скорости 
    {
        CountDesh,
        Level,
        Speed,
        Experience,
        skillPoints
    }

    class Player : IEntity
    {
        public Dictionary<Parameters, int> playerState;
        public Vector Location { get; private set; }
        public Vector Direction { get; private set; }
        public Vector Velocity { get; private set; }

        public Player(Vector location)
        {
            Location = location;
            Velocity = Vector.Zero;
            Direction = new Vector(0, 1);
        }

        public Player(Dictionary<Parameters, int> playerState)
        {
            this.playerState = playerState;
        }

        public void AddExperience(int count)
        {
            playerState[Parameters.Experience] += count;
            if (playerState[Parameters.Experience] >= 100)
            {
                LevelUp();
                playerState[Parameters.Experience] -= 100;
            }
        }

        public void LevelUp()
        {
            playerState[Parameters.Level]++;
            playerState[Parameters.skillPoints]++;
        }

        public void DistributeSkills(Parameters state)
        {
            switch(state) // сделать больше стат 
            {
                case Parameters.Speed:
                    if (playerState[Parameters.skillPoints] > 0)
                    {
                        playerState[Parameters.Speed] += 5;
                        playerState[Parameters.skillPoints]--;
                    }
                    break;
                default:
                    // дописать
                    break;
            }
        }

        public void SetVelosity(Vector userInput, Vector interest)
        {
            Direction = (interest - Location).Normalize();
            Velocity = userInput.Rotate(Direction.Angle - Math.PI / 2) * 4;
        }

        public void MakeTurn(long timeElapsed, List<Square> walls)
        {
            //var firstWallOnWay = new Ray(Location, Velocity.Angle).FirstIntersectionOfRay(walls);
            var delta = Velocity * (timeElapsed / 1000.0f);
            var rayX = Velocity.X > 0 ? new Ray(Location, 0) : new Ray(Location, Math.PI);
            var rayY = Velocity.Y > 0 ? new Ray(Location, -Math.PI / 2) : new Ray(Location, Math.PI / 2);
            /*var intersectionX = rayX.FirstIntersectionOfRay(walls);
            var intersectionY = rayY.FirstIntersectionOfRay(walls);
            var distanceX =- Location.X + intersectionX.Item1.X;
            var distanceY =- Location.Y + intersectionY.Item1.Y;*/
            var deltaX = delta.X;
            var deltaY = delta.Y;
            Location += new Vector(deltaX, deltaY);
        }
    }
}
