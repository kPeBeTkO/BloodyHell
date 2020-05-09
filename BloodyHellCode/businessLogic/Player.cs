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
        DashCount,
        Level,
        Speed,
        Experience,
        SkillPoints
    }

    public class Player : IEntity
    {
        public const float HitRange = 2;
        public const float Size = 0.3f;
        public const float DefaultSpeed = 4;
        public const long HitCooldown = 200;
        public const long HitDuration = 150;
        public const long DashDuration = 200;
        public const int DashSpeedMultiplayer = 5;
        public const float DashHitRange = 1; 

        public Dictionary<Parameters, int> State;
        public Vector Location { get; private set; }
        public Vector Direction { get; private set; }
        public Vector Velocity { get; private set; }
        public bool InDash { get; private set; } = false;
        public int DashCount { get; private set; }
        public bool Attacing { get; private set; } = false;

        private long time = 0;
        private long lastHit = 0;
        private long lastDash = 0;
        public bool Alive = true;

        public float CurentSpeed { get { return DefaultSpeed * (1 + State[Parameters.Speed] * 0.2f); } }

        public Player(Vector location)
        {
            State = new Dictionary<Parameters, int>();
            Location = location;
            Velocity = Vector.Zero;
            Direction = new Vector(0, 1);
            State[Parameters.Speed] = 0;
            State[Parameters.SkillPoints] = 0;
            State[Parameters.Experience] = 0;
            State[Parameters.Level] = 0;
            State[Parameters.DashCount] = 10;
            DashCount = 10;
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
            State[Parameters.SkillPoints]++;
        }

        public void DistributeSkills(Parameters state)
        {
            if (State[Parameters.SkillPoints] > 0)
                switch (state)
                {
                    case Parameters.Speed:
                        State[Parameters.Speed] += 1;
                        State[Parameters.SkillPoints]--;
                        break;
                    case Parameters.DashCount:
                        State[Parameters.DashCount] += 1;
                        State[Parameters.SkillPoints]--;
                        break;
                }
        }

        public void SetVelosity(Vector userInput, Vector interest)
        {
            if (!Alive)
                return;
            Direction = (interest - Location).Normalize();
            if (!InDash)
                Velocity = userInput.Rotate(Direction.Angle - Math.PI / 2) * CurentSpeed;
        }

        public void Attack()
        {
            if (!Alive)
                return;
            if (time - lastHit > HitDuration + HitCooldown)
            {
                Attacing = true;
                lastHit = time;
            }
        }

        public void Dash()
        {
            if (DashCount <= 0)
                return;
            DashCount--;
            InDash = true;
            lastDash = time;
            Velocity = Direction * CurentSpeed * DashSpeedMultiplayer;
        }

        private void Move(long timeElapsed, List<Square> walls)
        {
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
            if (firstWallOnWay.Item1 == null || (Location - firstWallOnWay.Item1).Length > Size)
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

        public void MakeTurn(long timeElapsed, List<Square> walls)
        {
            time += timeElapsed;
            if (time - lastHit > HitDuration)
            {
                Attacing = false;
            }
            if (time - lastDash > DashDuration)
            {
                InDash = false;
            }
            if (!Alive)
                return;
            Move(timeElapsed, walls);
        }
    }
}
