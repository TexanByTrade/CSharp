using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCTS_Minishogi
{
    class Drop
    {
        /// <summary>
        /// Position to drop piece at
        /// </summary>
        public Tuple<int, int> pos;
        public Piece piece;

        public Drop(Tuple<int,int> _pos, Piece _p)
        {
            pos = _pos;
            piece = _p;
        }
    }
}
