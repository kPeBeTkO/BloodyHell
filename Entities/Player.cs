using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloodyHell.Entities
{
    public enum state // добавть статы кроме скорости 
    {
        CountDesh,
        Level,
        Speed,
        Experience,
        skillPoints
    }

    class Player : IEntity
    {
        public Dictionary<state, int> playerState;

        public Player(Dictionary<state, int> playerState)
        {
            this.playerState = playerState;
        }

        public void AddExperience(int count)
        {
            playerState[state.Experience] += count;
            if (playerState[state.Experience] >= 100)
            {
                LevelUp();
                playerState[state.Experience] -= 100;
            }
        }

        public void LevelUp()
        {
            playerState[state.Level]++;
            playerState[state.skillPoints]++;
        }

        public void DistributeSkills(state state)
        {
            switch(state) // сделать больше стат 
            {
                case state.Speed:
                    if (playerState[state.skillPoints] > 0)
                    {
                        playerState[state.Speed] += 5;
                        playerState[state.skillPoints]--;
                    }
                    break;
                default:
                    // дописать
                    break;
            }
        }

        public void MakeTurn()
        {
            throw new NotImplementedException();
        }
    }
}
