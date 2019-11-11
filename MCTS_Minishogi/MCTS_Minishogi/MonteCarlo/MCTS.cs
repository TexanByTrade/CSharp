using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;

namespace MCTS_Minishogi
{
    class MCTS
    {
        private readonly Random r = new Random();
        private readonly object locker = new object(); //for thread-safe random
        
        private const int MAX_NODES = 1000000;

        public MCTS()
        {
        }
        
        public Game ChooseMove(Game currentGame, int timeout)
        {
            Stopwatch clock = new Stopwatch();
            clock.Start();
            TreeNode TreeRoot = new TreeNode(null, currentGame);
            
            //List<TreeNode> allNodes = new List<TreeNode>();
            //allNodes.Add(TreeRoot);

            while (clock.ElapsedMilliseconds < timeout)
            {
                TreeNode pick = TreeRoot;
                List<TreeNode> children;

                //Descend down the tree, picking the best moves for each side
                while((children = pick.GetChildren()).Count != 0)
                {
                    pick = children.Aggregate((x, y) => x.score >= y.score ? x : y); //OrderByDescending(x => x.score).First();
                }

                //if (allNodes.Count < MAX_NODES)
                    ExpandNode(pick);

                {
                    GameResult winner = SimulateMove(pick);
                    switch (winner)
                    {
                        case GameResult.WIN: pick.ReportWin(); break;
                        case GameResult.LOSS: pick.ReportLoss(); break;
                        case GameResult.DRAW: pick.ReportDraw(); break;
                    }
                }
                Parallel.ForEach(pick.GetChildren(), node =>
                {
                    for(int i = 0; i < 5; ++i)
                    {
                        GameResult winner = SimulateMove(node);
                        switch (winner)
                        {
                            case GameResult.WIN: node.ReportWin(); break;
                            case GameResult.LOSS: node.ReportLoss(); break;
                            case GameResult.DRAW: node.ReportDraw(); break;
                        }
                    }
                });
                TreeRoot.UpdateScore(); //cascades down to all nodes

            }
            TreeRoot.GetChildren().Sort((x, y) => y.visits.CompareTo(x.visits));//y.score.CompareTo(x.score));
            TreeNode bestChild = TreeRoot.GetChildren()[0];
            return bestChild.GameState;
        }
        
        private void ExpandNode(TreeNode node)
        {
            Game currentGame = node.GameState;
            List<Move> moves = currentGame.GetAllLegalMoves();
            List<Drop> drops = currentGame.GetAllLegalDrops();
            foreach(Move m in moves)
            {
                TreeNode ntn = new TreeNode(node, currentGame.MakeMove(m));
                node.AddChild(ntn);
                //allNodes.Add(ntn);
            }
            foreach(Drop d in drops)
            {
                TreeNode ntn = new TreeNode(node, currentGame.MakeDrop(d));
                node.AddChild(ntn);
                //allNodes.Add(ntn);
            }
        }
 
        private GameResult SimulateMove(TreeNode start)
        {
            int c = 0;
            Game nextState = start.GameState.DeepClone();
            nextState.ply = !nextState.ply; //reset ply
            while (true)
            {
                if ((++c) > 500) //experimentally, games seem to end with random moves in 75-150 moves?
                    return GameResult.DRAW;

                //Console.Clear();
                List<Move> moves = nextState.GetAllLegalMoves();
                List<Drop> drops = nextState.GetAllLegalDrops();
                int total = moves.Count + drops.Count;
                
                if (total == 0)
                    return (start.GameState.ply == nextState.ply) ? GameResult.WIN : GameResult.LOSS;

                int ri;
                lock (locker)
                {
                    ri = r.Next(total);
                }

                if (ri < moves.Count)
                {
                    nextState = nextState.MakeMove(moves[ri]);
                }
                else
                {
                    nextState = nextState.MakeDrop(drops[ri-moves.Count]);
                }
                
            }
        }
        
    }
}
