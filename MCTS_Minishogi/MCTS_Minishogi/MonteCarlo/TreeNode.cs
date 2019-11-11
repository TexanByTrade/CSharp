using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace MCTS_Minishogi
{
    class TreeNode
    {
        public const double C = 1.4142/2.0;

        public TreeNode parent;
        private List<TreeNode> children = new List<TreeNode>();

        public int visits = 0;
        public int wins = 0;
        public double score = 0.0;

        public Game GameState;

        public TreeNode(TreeNode _parent, Game _state)
        {
            parent = _parent;
            GameState = _state;
            UpdateScore();
        }

        //Alternate wins/losses back up the tree
        public void ReportWin()
        {
            Interlocked.Increment(ref wins);
            Interlocked.Increment(ref visits);
            if (parent != null)
            {
                parent.ReportLoss();
            }
        }
        public void ReportLoss()
        {
            Interlocked.Increment(ref visits);
            if (parent != null)
            {
                parent.ReportWin();
            }
        }
        public void ReportDraw()
        {
            Interlocked.Increment(ref visits);
            if (parent != null)
            {
                parent.ReportDraw();
            }
        }

        public void UpdateScore()
        {
            int parentVisits = parent != null ? parent.visits : visits;
            score = wins / (visits+1e-8) + C * Math.Sqrt(Math.Log(parentVisits+1) / (visits+1e-8));
            foreach(TreeNode child in children)
            {
                child.UpdateScore();
            }
        }

        public void AddChild(TreeNode child)
        {
            lock (children)
            {
                children.Add(child);
            }
        }
        public void AddChildren(List<TreeNode> children)
        {
            lock (children)
            {
                children.AddRange(children);
            }
        }
        public List<TreeNode> GetChildren()
        {
            return children;
        }
        
    }
}
