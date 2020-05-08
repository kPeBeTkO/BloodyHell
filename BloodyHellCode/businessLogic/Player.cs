using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
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

    public class Player : IEntity
    {
        public Dictionary<Parameters, int> State;
        public Vector Location { get; private set; }
        public Vector Direction { get; private set; }
        public Vector Velocity { get; private set; }
        public const float Size = 0.3f;
        public const float DefaultSpeed = 4;
        public const long HitCooldown = 200;
        public const long HitDuration = 150;
        private long time = 0;
        private long lastHit = 0;

        public bool Attacing { get; private set; } = false;
        public bool Alive = true;

        public float CurentSpeed { get { return DefaultSpeed * (1 + State[Parameters.Speed] * 0.2f); } }

        public Player(Vector location)
        {
            State = new Dictionary<Parameters, int>();
            Location = location;
            Velocity = Vector.Zero;
            Direction = new Vector(0, 1);
            State[Parameters.Speed] = 0;
        }

        public Player(Dictionary<Parameters, int> playerState)
        {
            this.State = playerState;
        }

        public void AddExperience(int count)
        {
            State[Parameters.Experience] += count;
            if (State[Parameters.Experience] >= 100)
            {
                LevelUp();
                State[Parameters.Experience] -= 100;
            }
        }

        public void LevelUp()
        {
            State[Parameters.Level]++;
            State[Parameters.skillPoints]++;
        }

        public void DistributeSkills(Parameters state)
        {
            switch(state) // сделать больше стат 
            {
                case Parameters.Speed:
                    if (State[Parameters.skillPoints] > 0)
                    {
                        State[Parameters.Speed] += 5;
                        State[Parameters.skillPoints]--;
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
            Velocity = userInput.Rotate(Direction.Angle - Math.PI / 2) * CurentSpeed;
        }

        public void Attack()
        {
            if (time - lastHit > HitDuration + HitCooldown)
            {
                Attacing = true;
                lastHit = time;
            }
        }

        public void MakeTurn(long timeElapsed, List<Square> walls)
        {
            if (!Alive)
                return;
            time += timeElapsed;
            if (time - lastHit > HitDuration)
            {
                Attacing = false;
            }
            if (walls == null || walls.Count == 0)
            {
                Location += Velocity * (timeElapsed / 1000.0f);
                return;
            }
            var firstWallOnWay = new Ray(Location, Velocity.Angle).FirstIntersectionOfRay(walls);
            var delta = Velocity * (timeElapsed / 1000.0f);
            var rayX = Velocity.X > 0 ? new Ray(Location, 0) : new Ray(Location, Math.PI);
            var rayY = Velocity.Y > 0 ? new Ray(Location, Math.PI / 2) : new Ray(Location, -Math.PI / 2);
            var intersectionX = rayX.FirstIntersectionOfRay(walls);
            var intersectionY = rayY.FirstIntersectionOfRay(walls);
            /*var distanceX = -Location.X + intersectionX.Item1.X;
            var distanceY = -Location.Y + intersectionY.Item1.Y;*/
            var deltaX = delta.X;
            var deltaY = delta.Y;
            if (firstWallOnWay.Item1 == null || (Location-firstWallOnWay.Item1).Length > Size)
            {
                Location += new Vector(deltaX, deltaY);
            }
            else
            {
                if (intersectionX.Item1 != null && Location.DistanceTo(intersectionX.Item1) < Size)
                    deltaX = 0;
                if (intersectionY.Item1 != null && Location.DistanceTo(intersectionY.Item1) < Size)
                    deltaY = 0;
                Location += new Vector(deltaX, deltaY);
            }
        }
    }
}
