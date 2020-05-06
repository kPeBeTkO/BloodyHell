using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloodyHell
{
    public interface IEntity
    {
        void MakeTurn(long timeElapsed);
    }
}
