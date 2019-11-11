using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace MCTS_Minishogi
{
    class Program
    {
        //You go first. Enter your move as follows:
        //(0,0) is the bottom-left corner. 
        //----
        //To move a piece, enter
        //XXmYYp 
        //where XX are the starting coords, and YY are the destination coords (no commas).
        //The p at the end is optional; without it, a silver general won't promote.
        //----
        //To drop a piece, enter
        //XXY
        //where XX are the drop coords, and Y is the index of the piece in your hand (starting at zero)
        //For instance, if you have a hand like "P S G", to drop the S at 1,1 you would type 111.
        //----
        //No checks are made for your move being legal.
        static void Main(string[] args)
        {
            Game game = new Game();
            MCTS ai = new MCTS();
            game.Draw();
            Console.Write("Your move: ");
            while (true)
            {
                //Get user input:
                string input = Console.ReadLine();
                if (input[2].Equals('m'))
                {
                    game = game.MakeMove(new Move(Tuple.Create(input[0] - '0', input[1] - '0'), Tuple.Create(input[3] - '0', input[4] - '0'), input[input.Length-1] == 'p'));
                }
                else
                {
                    game = game.MakeDrop(new Drop(Tuple.Create(input[0] - '0', input[1] - '0'), (Piece)game.P1Hand[input[2] - '0']));
                }
                Console.Clear();
                game.Draw();
                if(game.GetAllLegalMoves().Count == 0)
                {
                    Console.WriteLine("You checkmated black!");
                    break;
                }
                else
                {
                    Console.WriteLine("Computer is thinking...");
                }

                game = ai.ChooseMove(game, 15*1000);

                Console.Clear();
                game.Draw();
                if (game.GetAllLegalMoves().Count == 0)
                {
                    Console.WriteLine("You were checkmated...");
                    break;
                }
                else
                {
                    Console.Write("Your move: ");
                }
            }
#if DEBUG
            Console.ReadKey();
#endif
        }
    }
}
