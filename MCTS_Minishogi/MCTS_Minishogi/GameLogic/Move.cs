using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCTS_Minishogi
{
    class Move
    {
        /// <summary>
        /// Beginning square for a piece to move
        /// </summary>
        public readonly Tuple<int, int> begin;
        /// <summary>
        /// Square the piece will move to
        /// </summary>
        public readonly Tuple<int, int> finish;
        /// <summary>
        /// If true, piece will promote
        /// </summary>
        public bool promote;
        
        public Move(Tuple<int,int> _begin, Tuple<int,int> _finish, bool _promote)
        {
            begin = _begin;
            finish = _finish;
            promote = _promote;
        }

        public override bool Equals(object obj)
        {
            bool same = true;
            Move other = obj as Move;
            if (!begin.Equals(other.begin))
                same = false;
            if (!finish.Equals(other.finish))
                same = false;
            if (promote != other.promote)
                same = false;

            return same;
        }
        public override int GetHashCode()
        {
            return begin.GetHashCode() + finish.GetHashCode() + (promote ? 17 : 0);
        }
        public override string ToString()
        {
            return begin.ToString() + ", " + finish.ToString() + ", " + promote.ToString();
        }
    }
}
