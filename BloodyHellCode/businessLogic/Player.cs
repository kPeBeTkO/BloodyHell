using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading.Tasks;
using RayCasting;

namespace BloodyHell.Entities
{
    public enum Parameters
    {
        DashCount,
        Level,
        Speed,
        Experience,
        SkillPoints
    }

    public class Player : Entity
    {
        public const float HitRange = 2;
        public const float Size = 0.4f;
        public const float DefaultSpeed = 4;
        public const long HitCooldown = 200;
        public const long HitDuration = 150;
        public const long DashDuration = 200;
        public const int DashSpeedMultiplayer = 7;
        public const float DashHitRange = 1; 

        public Dictionary<Parameters, int> Stats;
        public Vector Direction { get; private set; }
        public bool InDash { get; private set; } = false;
        public int DashCount { get { return Stats[Parameters.DashCount]; } private set { Stats[Parameters.DashCount] = value; } }
        public bool Attacing { get; private set; } = false;
        public bool InWalk { get; private set; } = false;

        private long time = 0;
        private long lastHit = 0;
        private long lastDash = 0;
        private long lastStep = 0;

        public float CurentSpeed { get { return DefaultSpeed * (1 + Stats[Parameters.Speed] * 0.2f); } }

        public Player(Vector location)
        {
            Stats = new Dictionary<Parameters, int>();
            Location = location;
            Velocity = Vector.Zero;
            Direction = new Vector(0, 1);
            Stats[Parameters.Speed] = 0;
            Stats[Parameters.SkillPoints] = 0;
            Stats[Parameters.Experience] = 0;
            Stats[Parameters.Level] = 0;
            Stats[Parameters.DashCount] = 10;
        }

        public Player(Dictionary<Parameters, int> playerState)
        {
            Stats = playerState;
        }

        public void AddExperience(int count)
        {
            Stats[Parameters.Experience] += count;
            if (Stats[Parameters.Experience] >= 100)
            {
                LevelUp();
                Stats[Parameters.Experience] -= 100;
            }
        }

        public void LevelUp()
        {
            Stats[Parameters.Level]++;
            Stats[Parameters.SkillPoints]++;
        }

        public void DistributeSkills(Parameters state)
        {
            if (Stats[Parameters.SkillPoints] > 0)
            {
                switch (state)
                {
                    case Parameters.Speed:
                        Stats[Parameters.Speed]++;
                        break;
                    case Parameters.DashCount:
                        Stats[Parameters.DashCount]++;
                        break;
                }
                Stats[Parameters.SkillPoints]--;
            }
        }

        public void SetVelosity(Vector userInput, Vector interest)
        {
            if (!Alive)
                return;
            Direction = interest.Normalize();
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
            var intersectionX = rayX.FirstIntersectionOfRay(walls).Item1;
            var intersectionY = rayY.FirstIntersectionOfRay(walls).Item1;
            var deltaX = delta.X;
            var deltaY = delta.Y;
            if (intersectionX != null && Location.DistanceTo(intersectionX) - Size < Math.Abs(deltaX))
                deltaX = Velocity.X > 0 ? intersectionX.X - Location.X - Size : intersectionX.X - Location.X + Size;
            if (intersectionY != null && Location.DistanceTo(intersectionY) - Size < Math.Abs(deltaY))
                deltaY = Velocity.Y > 0 ? intersectionY.Y - Location.Y - Size : intersectionY.Y - Location.Y + Size;
            if ((intersectionX == null || Location.DistanceTo(intersectionX) - Size * 1.01 > Math.Abs(deltaX)) &&
                (intersectionY == null || Location.DistanceTo(intersectionY) - Size * 1.01 > Math.Abs(deltaY)) &&
                firstWallOnWay.Item1 != null && firstWallOnWay.Item1.DistanceTo(Location) - Size < delta.Length)
            {
                var corner = firstWallOnWay.Item2.GetClosestCornerToPoint(Location);
                var point = firstWallOnWay.Item1;
                if (point.X - corner.X < 0.01)
                    deltaX = Velocity.X > 0 ? corner.X - Location.X - Size : corner.X - Location.X + Size;
                else
                    deltaY = Velocity.Y > 0 ? corner.Y - Location.Y - Size : corner.Y - Location.Y + Size;
            }
            Location += new Vector(deltaX, deltaY);
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

            if (Velocity.X != 0 || Velocity.Y != 0)
                InWalk = true;
            else
                InWalk = false;

            if (!Alive)
                return;
            Move(timeElapsed, walls);
        }
    }
}
