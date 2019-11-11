using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace MCTS_Minishogi
{ 
    class Game
    {
        public int[,] Board;
        /// <summary>
        /// Player 1 (white)'s collected pieces
        /// </summary>
        public List<int> P1Hand = new List<int>();
        /// <summary>
        /// Player 2's collected pieces
        /// </summary>
        public List<int> P2Hand = new List<int>();
        /// <summary>
        /// ply = true for white's turn
        /// </summary>
        public bool ply;

        public Game()
        {
            //(0,0) is bottom-left, white's side
            Board = new int[5, 5] { { 1, 2, 0, 0, 17 }, { 4, 0, 0, 0, 16 }, { 3, 0, 0, 0, 14 }, { 5, 0, 0, 0, 15 }, { 6, 0, 0, 13, 12 } };
            ply = true;
        }

        public List<Move> GetAllLegalMoves()
        {
            List<Move> moves = new List<Move>();
            if (ply) //White's turn
            {
                for(int x = 0; x < 5; ++x)
                {
                    for(int y = 0; y < 5; ++y)
                    {
                        //Get all moves of pieces on the board
                        switch ((Piece)Board[x, y])
                        {
                            case Piece.Empty: break;
                            case Piece.WKing: moves.AddRange(GetKingMoves(x, y)); break;
                            case Piece.WPawn: moves.AddRange(GetPawnMoves(x, y)); break;
                            case Piece.WSilver: moves.AddRange(GetSilverMoves(x, y)); break;
                            case Piece.WGold: moves.AddRange(GetGoldMoves(x, y)); break;
                            case Piece.WBishop: moves.AddRange(GetBishopMoves(x, y)); break;
                            case Piece.WRook: moves.AddRange(GetRookMoves(x, y)); break;
                            case Piece.WPawnPromo: goto case Piece.WGold;
                            case Piece.WSilverPromo: goto case Piece.WGold;
                            case Piece.WBishopPromo: moves.AddRange(GetBishopPromoMoves(x, y)); break;
                            case Piece.WRookPromo: moves.AddRange(GetRookPromoMoves(x, y)); break;
                            default: break;
                        }
                    }
                }
            }
            else
            {
                for (int x = 0; x < 5; ++x)
                {
                    for (int y = 0; y < 5; ++y)
                    {
                        //Get all moves of pieces on the board
                        switch ((Piece)Board[x, y])
                        {
                            case Piece.Empty: break;
                            case Piece.BKing: moves.AddRange(GetKingMoves(x, y)); break;
                            case Piece.BPawn: moves.AddRange(GetPawnMoves(x, y)); break;
                            case Piece.BSilver: moves.AddRange(GetSilverMoves(x, y)); break;
                            case Piece.BGold: moves.AddRange(GetGoldMoves(x, y)); break;
                            case Piece.BBishop: moves.AddRange(GetBishopMoves(x, y)); break;
                            case Piece.BRook: moves.AddRange(GetRookMoves(x, y)); break;
                            case Piece.BPawnPromo: goto case Piece.BGold;
                            case Piece.BSilverPromo: goto case Piece.BGold;
                            case Piece.BBishopPromo: moves.AddRange(GetBishopPromoMoves(x, y)); break;
                            case Piece.BRookPromo: moves.AddRange(GetRookPromoMoves(x, y)); break;
                            default: break;
                        }
                    }
                }
            }
            return moves;
        }

        /// <summary>
        /// Returns true if <paramref name="white"/> is in check.
        /// </summary>
        /// <param name="check">The board to check</param>
        /// <param name="white">True to check if white (player 1) is in check, false for black</param>
        /// <returns></returns>
        public bool InCheck(int[,] check, bool white)
        {
            //Console.WriteLine("Entering InCheck");
            //Check if white king is in check
            if (white)
            {
                var pos = check.Find((int)Piece.WKing);

                #region Orthogonal Checks
                int y = pos.Item2;
                int x;
                //Check right
                for (x = pos.Item1 + 1; x < 5; ++x)
                {
                    Piece p = (Piece)check[x, y];
                    if (x == pos.Item1 + 1)
                    {
                        if (p == Piece.BGold || p == Piece.BPawnPromo || p == Piece.BSilverPromo || p == Piece.BBishopPromo || p == Piece.BKing ||
                            p == Piece.BRook || p == Piece.BRookPromo)
                            return true;
                        else if (p != Piece.Empty)
                            break;
                    }
                    if (p == Piece.BRook || p == Piece.BRookPromo)
                        return true;
                    else if (p != Piece.Empty)
                        break;
                }
                //Check left
                for (x = pos.Item1 - 1; x >= 0; --x)
                {
                    Piece p = (Piece)check[x, y];
                    if (x == pos.Item1 - 1)
                    {
                        if (p == Piece.BGold || p == Piece.BPawnPromo || p == Piece.BSilverPromo || p == Piece.BBishopPromo || p == Piece.BKing ||
                            p == Piece.BRook || p == Piece.BRookPromo)
                            return true;
                        else if (p != Piece.Empty)
                            break;
                    }
                    if (p == Piece.BRook || p == Piece.BRookPromo)
                        return true;
                    else if (p != Piece.Empty)
                        break;
                }
                //Check down
                x = pos.Item1;
                for (y = pos.Item2 - 1; y >= 0; --y)
                {
                    Piece p = (Piece)check[x, y];
                    if (y == pos.Item2 - 1)
                    {
                        if (p == Piece.BGold || p == Piece.BPawnPromo || p == Piece.BSilverPromo || p == Piece.BBishopPromo || p == Piece.BKing ||
                            p == Piece.BRook || p == Piece.BRookPromo)
                            return true;
                        else if (p != Piece.Empty)
                            break;
                    }
                    if (p == Piece.BRook || p == Piece.BRookPromo)
                        return true;
                    else if (p != Piece.Empty)
                        break;
                }
                //Check up
                for (y = pos.Item2 + 1; y < 5; ++y)
                {
                    Piece p = (Piece)check[x, y];
                    if (y == pos.Item2 + 1)
                    {
                        if (p == Piece.BPawn || p == Piece.BSilver || p == Piece.BGold || p == Piece.BPawnPromo || p == Piece.BSilverPromo || p == Piece.BBishopPromo || p == Piece.BKing ||
                            p == Piece.BRook || p == Piece.BRookPromo)
                            return true;
                        else if (p != Piece.Empty)
                            break;
                    }
                    if (p == Piece.BRook || p == Piece.BRookPromo)
                        return true;
                    else if (p != Piece.Empty)
                        break;
                }
                #endregion
                #region Diagonal Checks
                //Check northeast
                x = pos.Item1 + 1;
                y = pos.Item2 + 1;
                while((x<5)&&(y<5))
                {
                    Piece p = (Piece)check[x, y];
                    if (y == pos.Item2 + 1)
                    {
                        if (p == Piece.BSilver || p == Piece.BGold || p == Piece.BPawnPromo || p == Piece.BSilverPromo || p == Piece.BBishopPromo || p == Piece.BKing ||
                            p == Piece.BBishop || p == Piece.BRookPromo)
                            return true;
                        else if (p != Piece.Empty)
                            break;
                    }
                    if (p == Piece.BBishop || p == Piece.BBishopPromo)
                        return true;
                    else if (p != Piece.Empty)
                        break;

                    x++; y++;
                }
                //Check northwest
                x = pos.Item1 - 1;
                y = pos.Item2 + 1;
                while ((x >= 0) && (y < 5))
                {
                    Piece p = (Piece)check[x, y];
                    if (y == pos.Item2 + 1)
                    {
                        if (p == Piece.BSilver || p == Piece.BGold || p == Piece.BPawnPromo || p == Piece.BSilverPromo || p == Piece.BBishopPromo || p == Piece.BKing ||
                            p == Piece.BBishop || p == Piece.BRookPromo)
                            return true;
                        else if (p != Piece.Empty)
                            break;
                    }
                    if (p == Piece.BBishop || p == Piece.BBishopPromo)
                        return true;
                    else if (p != Piece.Empty)
                        break;

                    x--; y++;
                }
                //Check southwest
                x = pos.Item1 - 1;
                y = pos.Item2 - 1;
                while ((x >= 0) && (y >= 0))
                {
                    Piece p = (Piece)check[x, y];
                    if (y == pos.Item2 - 1)
                    {
                        if (p == Piece.BSilver || p == Piece.BBishopPromo || p == Piece.BKing ||
                            p == Piece.BBishop || p == Piece.BRookPromo)
                            return true;
                        else if (p != Piece.Empty)
                            break;
                    }
                    if (p == Piece.BBishop || p == Piece.BBishopPromo)
                        return true;
                    else if (p != Piece.Empty)
                        break;

                    x--; y--;
                }
                //Check southeast
                x = pos.Item1 + 1;
                y = pos.Item2 - 1;
                while ((x < 5) && (y >= 0))
                {
                    Piece p = (Piece)check[x, y];
                    if (y == pos.Item2 - 1)
                    {
                        if (p == Piece.BSilver || p == Piece.BBishopPromo || p == Piece.BKing ||
                            p == Piece.BBishop || p == Piece.BRookPromo)
                            return true;
                        else if (p != Piece.Empty)
                            break;
                    }
                    if (p == Piece.BBishop || p == Piece.BBishopPromo)
                        return true;
                    else if (p != Piece.Empty)
                        break;

                    x++; y--;
                }
                #endregion
            }
            else
            {
                //var check = check.FlipVertical(); //so I don't have to rewrite all these cases...
                var pos = check.FindBlack((int)Piece.BKing);

                #region Orthogonal Checks
                int y = pos.Item2;
                int x;
                //Check right
                for (x = pos.Item1 + 1; x < 5; ++x)
                {
                    Piece p = (Piece)check[x, y];
                    if (x == pos.Item1 + 1)
                    {
                        if (p == Piece.WGold || p == Piece.WPawnPromo || p == Piece.WSilverPromo || p == Piece.WBishopPromo || p == Piece.WKing ||
                            p == Piece.WRook || p == Piece.WRookPromo)
                            return true;
                        else if (p != Piece.Empty)
                            break;
                    }
                    if (p == Piece.WRook || p == Piece.WRookPromo)
                        return true;
                    else if (p != Piece.Empty)
                        break;
                }
                //Check left
                for (x = pos.Item1 - 1; x >= 0; --x)
                {
                    Piece p = (Piece)check[x, y];
                    if (x == pos.Item1 - 1)
                    {
                        if (p == Piece.WGold || p == Piece.WPawnPromo || p == Piece.WSilverPromo || p == Piece.WBishopPromo || p == Piece.WKing ||
                            p == Piece.WRook || p == Piece.WRookPromo)
                            return true;
                        else if (p != Piece.Empty)
                            break;
                    }
                    if (p == Piece.WRook || p == Piece.WRookPromo)
                        return true;
                    else if (p != Piece.Empty)
                        break;
                }
                //Check down
                x = pos.Item1;
                for (y = pos.Item2 - 1; y >= 0; --y)
                {
                    Piece p = (Piece)check[x, y];
                    if (y == pos.Item2 - 1)
                    {
                        if (p == Piece.WPawn || p == Piece.WGold || p == Piece.WSilver || p == Piece.WPawnPromo || p == Piece.WSilverPromo || p == Piece.WBishopPromo || p == Piece.WKing ||
                            p == Piece.WRook || p == Piece.WRookPromo)
                            return true;
                        else if (p != Piece.Empty)
                            break;
                    }
                    if (p == Piece.WRook || p == Piece.WRookPromo)
                        return true;
                    else if (p != Piece.Empty)
                        break;
                }
                //Check up
                for (y = pos.Item2 + 1; y < 5; ++y)
                {
                    Piece p = (Piece)check[x, y];
                    if (y == pos.Item2 + 1)
                    {
                        if (p == Piece.WGold || p == Piece.WPawnPromo || p == Piece.WSilverPromo || p == Piece.WBishopPromo || p == Piece.WKing ||
                            p == Piece.WRook || p == Piece.WRookPromo)
                            return true;
                        else if (p != Piece.Empty)
                            break;
                    }
                    if (p == Piece.WRook || p == Piece.WRookPromo)
                        return true;
                    else if (p != Piece.Empty)
                        break;
                }
                #endregion
                #region Diagonal Checks
                //Check northeast
                x = pos.Item1 + 1;
                y = pos.Item2 + 1;
                while ((x < 5) && (y < 5))
                {
                    Piece p = (Piece)check[x, y];
                    if (y == pos.Item2 + 1)
                    {
                        if (p == Piece.WSilver || p == Piece.WBishopPromo || p == Piece.WKing ||
                            p == Piece.WBishop || p == Piece.WRookPromo)
                            return true;
                        else if (p != Piece.Empty)
                            break;
                    }
                    if (p == Piece.WBishop || p == Piece.WBishopPromo)
                        return true;
                    else if (p != Piece.Empty)
                        break;

                    x++; y++;
                }
                //Check northwest
                x = pos.Item1 - 1;
                y = pos.Item2 + 1;
                while ((x >= 0) && (y < 5))
                {
                    Piece p = (Piece)check[x, y];
                    if (y == pos.Item2 + 1)
                    {
                        if (p == Piece.WSilver || p == Piece.WBishopPromo || p == Piece.WKing ||
                            p == Piece.WBishop || p == Piece.WRookPromo)
                            return true;
                        else if (p != Piece.Empty)
                            break;
                    }
                    if (p == Piece.WBishop || p == Piece.WBishopPromo)
                        return true;
                    else if (p != Piece.Empty)
                        break;

                    x--; y++;
                }
                //Check southwest
                x = pos.Item1 - 1;
                y = pos.Item2 - 1;
                while ((x >= 0) && (y >= 0))
                {
                    Piece p = (Piece)check[x, y];
                    if (y == pos.Item2 - 1)
                    {
                        if (p == Piece.WSilver || p == Piece.WGold || p == Piece.WPawnPromo || p == Piece.WSilverPromo || p == Piece.WBishopPromo || p == Piece.WKing ||
                            p == Piece.WBishop || p == Piece.WRookPromo)
                            return true;
                        else if (p != Piece.Empty)
                            break;
                    }
                    if (p == Piece.WBishop || p == Piece.WBishopPromo)
                        return true;
                    else if (p != Piece.Empty)
                        break;

                    x--; y--;
                }
                //Check southeast
                x = pos.Item1 + 1;
                y = pos.Item2 - 1;
                while ((x < 5) && (y >= 0))
                {
                    Piece p = (Piece)check[x, y];
                    if (y == pos.Item2 - 1)
                    {
                        if (p == Piece.WSilver || p == Piece.WGold || p == Piece.WPawnPromo || p == Piece.WSilverPromo || p == Piece.WBishopPromo || p == Piece.WKing ||
                            p == Piece.WBishop || p == Piece.WRookPromo)
                            return true;
                        else if (p != Piece.Empty)
                            break;
                    }
                    if (p == Piece.WBishop || p == Piece.WBishopPromo)
                        return true;
                    else if (p != Piece.Empty)
                        break;

                    x++; y--;
                }
                #endregion
            }
            return false;
        }
        /// <summary>
        /// Returns true if <paramref name="white"/> is in checkmate. In shogi, stalemates count as checkmates
        /// </summary>
        /// <param name="check"></param>
        /// <param name="white"></param>
        /// <returns></returns>
        public bool InCheckmate(int[,] check, bool white)
        {
            //Console.WriteLine("InCheckmate");
            List<Move> attempts = new List<Move>();
            List<Drop> allDrops = new List<Drop>();
            bool tempPly = ply;
            if (white)
            {
                ply = true;
                attempts = GetAllLegalMoves();
                allDrops = GetAllLegalDrops();
                if((attempts.Count+allDrops.Count) == 0)
                {
                    ply = tempPly;
                    return true;
                }
            }
            else
            {
                ply = false;
                attempts = GetAllLegalMoves();
                allDrops = GetAllLegalDrops();
                if ((attempts.Count + allDrops.Count) == 0)
                {
                    ply = tempPly;
                    return true;
                }
            }
            ply = tempPly;
            return false;
        }

        // To avoid infinite recursion, this version doesn't consider drops,
        // because you can't block a pawn checkmate with drops
        public bool InCheckmatePawn(int[,] check, bool white)
        {
            //Console.WriteLine("InCheckmate");
            List<Move> attempts = new List<Move>();
            bool tempPly = ply;
            if (white)
            {
                ply = true;
                attempts = GetAllLegalMoves();
                if ((attempts.Count) == 0)
                {
                    ply = tempPly;
                    return true;
                }
            }
            else
            {
                ply = false;
                attempts = GetAllLegalMoves();
                if ((attempts.Count) == 0)
                {
                    ply = tempPly;
                    return true;
                }
            }
            ply = tempPly;
            return false;
        }

        public List<Move> GetKingMoves(int x, int y)
        {
            //Console.WriteLine("King {0} {1}", x, y);
            List<Move> moves = new List<Move>();
            if (ply)
            {
                for (int i = x - 1; i <= x + 1; ++i)
                {
                    for (int j = y - 1; j <= y + 1; ++j)
                    {
                        if ((i == x && j == y) || i < 0 || j < 0 || i > 4 || j > 4)
                            continue;

                        Piece p = (Piece)Board[i, j];
                        if (p > Piece.Empty && p < Piece.BKing)
                            continue;
                        else
                        {
                            Move attempt = new Move(Tuple.Create(x, y), Tuple.Create(i, j), false);
                            //see if this move will end us in check; that's illegal
                            if (!InCheck(MakeMove(attempt).Board, ply))
                                moves.Add(attempt);
                        }
                    }
                }
            }
            else
            {
                for (int i = x - 1; i <= x + 1; ++i)
                {
                    for (int j = y - 1; j <= y + 1; ++j)
                    {
                        if ((i == x && j == y) || i < 0 || j < 0 || i > 4 || j > 4)
                            continue;

                        Piece p = (Piece)Board[i, j];
                        if (p >= Piece.BKing)
                            continue;
                        else
                        {
                            Move attempt = new Move(Tuple.Create(x, y), Tuple.Create(i, j), false);
                            //see if this move will end us in check; that's illegal
                            if (!InCheck(MakeMove(attempt).Board, ply))
                                moves.Add(attempt);
                        }
                    }
                }
            }
            return moves;
        }
        public List<Move> GetPawnMoves(int x, int y)
        {
            //Console.WriteLine("Pawn {0} {1}", x, y);
            List<Move> moves = new List<Move>();
            if (ply)
            {
                Piece p = (Piece)Board[x, y + 1];
                
                if (p == Piece.Empty || p >= Piece.BKing)
                {
                    if (y < 3)
                    {

                        Move attempt = new Move(Tuple.Create(x, y), Tuple.Create(x, y + 1), false);
                        if (!InCheck(MakeMove(attempt).Board, ply))
                            moves.Add(attempt);
                    }
                    else
                    {
                        Move attempt = new Move(Tuple.Create(x, y), Tuple.Create(x, y + 1), true); //pawns must promote
                        if (!InCheck(MakeMove(attempt).Board, ply))
                            moves.Add(attempt);
                    }
                }
            }
            else
            {
                Piece p = (Piece)Board[x, y - 1];
                if (p < Piece.BKing)
                {
                    if (y > 1)
                    {

                        Move attempt = new Move(Tuple.Create(x, y), Tuple.Create(x, y - 1), false);
                        if (!InCheck(MakeMove(attempt).Board, ply))
                            moves.Add(attempt);
                    }
                    else
                    {
                        Move attempt = new Move(Tuple.Create(x, y), Tuple.Create(x, y - 1), true); //pawns must promote
                        if (!InCheck(MakeMove(attempt).Board, ply))
                            moves.Add(attempt);
                    }
                }
            }
            return moves;
        }
        public List<Move> GetSilverMoves(int x, int y)
        {
           // Console.WriteLine("Silver {0} {1}", x, y);
            List<Move> moves = new List<Move>();
            if (ply)
            {
                int[] xm = new int[5] { x - 1, x - 1, x, x + 1, x + 1 };
                int[] ym = new int[5] { y - 1, y + 1, y + 1, y + 1, y - 1 };
                for (int i = 0; i < 5; ++i)
                {
                    
                    if (xm[i] < 0 || xm[i] > 4 || ym[i] < 0 || ym[i] > 4)
                        continue;

                    Piece p = (Piece)Board[xm[i], ym[i]];
                    if (p > Piece.Empty && p < Piece.BKing)
                        continue;

                    Move attempt = new Move(Tuple.Create(x, y), Tuple.Create(xm[i], ym[i]), false);
                    if (!InCheck(MakeMove(attempt).Board, ply))
                    {
                        moves.Add(attempt);
                        //silvers have a choice to promote
                        if (ym[i] == 4 || y == 4)
                            moves.Add(new Move(Tuple.Create(x, y), Tuple.Create(xm[i], ym[i]), true));
                    }
                }
            }
            else
            {
                int[] xm = new int[5] { x - 1, x - 1, x, x + 1, x + 1 };
                int[] ym = new int[5] { y + 1, y - 1, y - 1, y - 1, y + 1 };
                for (int i = 0; i < 5; ++i)
                {
                    
                    if (xm[i] < 0 || xm[i] > 4 || ym[i] < 0 || ym[i] > 4)
                        continue;

                    Piece p = (Piece)Board[xm[i], ym[i]];
                    if (p >= Piece.BKing)
                        continue;

                    Move attempt = new Move(Tuple.Create(x, y), Tuple.Create(xm[i], ym[i]), false);
                    if (!InCheck(MakeMove(attempt).Board, ply))
                    {
                        moves.Add(attempt);
                        //silvers have a choice to promote
                        if (ym[i] == 0 || y == 0)
                            moves.Add(new Move(Tuple.Create(x, y), Tuple.Create(xm[i], ym[i]), true));
                    }
                }
            }
            return moves;
        }
        public List<Move> GetGoldMoves(int x, int y)
        {
            //Console.WriteLine("Gold {0} {1}", x, y);
            List<Move> moves = new List<Move>();
            if (ply)
            {
                int[] xm = new int[6] { x - 1, x - 1, x, x + 1, x + 1, x };
                int[] ym = new int[6] { y, y + 1, y + 1, y + 1, y, y - 1 };
                for (int i = 0; i < 6; ++i)
                {
                    if (xm[i] < 0 || xm[i] > 4 || ym[i] < 0 || ym[i] > 4)
                        continue;

                    Piece p = (Piece)Board[xm[i], ym[i]];
                    if (p > Piece.Empty && p < Piece.BKing)
                        continue;

                    Move attempt = new Move(Tuple.Create(x, y), Tuple.Create(xm[i], ym[i]), false);
                    if (!InCheck(MakeMove(attempt).Board, ply))
                    {
                        moves.Add(attempt);
                    }
                }
            }
            else
            {
                int[] xm = new int[6] { x - 1, x - 1, x, x + 1, x + 1, x };
                int[] ym = new int[6] { y, y - 1, y - 1, y - 1, y, y + 1 };
                for (int i = 0; i < 6; ++i)
                {
                    if (xm[i] < 0 || xm[i] > 4 || ym[i] < 0 || ym[i] > 4)
                        continue;

                    Piece p = (Piece)Board[xm[i], ym[i]];
                    if (p >= Piece.BKing)
                        continue;

                    Move attempt = new Move(Tuple.Create(x, y), Tuple.Create(xm[i], ym[i]), false);
                    if (!InCheck(MakeMove(attempt).Board, ply))
                    {
                        moves.Add(attempt);
                    }
                }
            }
            return moves;
        }
        public List<Move> GetBishopMoves(int x, int y)
        {
            //Console.WriteLine("Bishop {0} {1}", x, y);
            List<Move> moves = new List<Move>();
            if (ply)
            {
                //north/southeast
                int u = y;
                int d = y;
                for (int i = x + 1; i < 5; ++i)
                {
                    u++;
                    d--;
                    if (u < 5)
                    {
                        //up
                        Piece p = (Piece)Board[i, u];
                        if (p == Piece.Empty)
                        {
                            Move attempt = new Move(Tuple.Create(x, y), Tuple.Create(i, u), true); //never a reason to not promote bishop
                            if (!InCheck(MakeMove(attempt).Board, ply))
                                moves.Add(attempt);
                        }
                        else if (p >= Piece.BKing)
                        {
                            Move attempt = new Move(Tuple.Create(x, y), Tuple.Create(i, u), true);
                            if (!InCheck(MakeMove(attempt).Board, ply))
                                moves.Add(attempt);

                            u += 99; //can't pass through pieces; stop looking for up moves
                        }
                        else
                            u += 99;
                    }
                    if (d >= 0)
                    {
                        Piece p = (Piece)Board[i, d];
                        if (p == Piece.Empty)
                        {
                            Move attempt = new Move(Tuple.Create(x, y), Tuple.Create(i, d), true); //never a reason to not promote bishop
                            if (!InCheck(MakeMove(attempt).Board, ply))
                                moves.Add(attempt);
                        }
                        else if (p >= Piece.BKing)
                        {
                            Move attempt = new Move(Tuple.Create(x, y), Tuple.Create(i, d), true);
                            if (!InCheck(MakeMove(attempt).Board, ply))
                                moves.Add(attempt);

                            d -= 99; //can't pass through pieces
                        }
                        else
                            d -= 99;
                    }
                }

                u = y;
                d = y;
                //north/southwest
                for (int i = x - 1; i >= 0; --i)
                {
                    u++;
                    d--;
                    if (u < 5)
                    {
                        //up
                        Piece p = (Piece)Board[i, u];
                        if (p == Piece.Empty)
                        {
                            Move attempt = new Move(Tuple.Create(x, y), Tuple.Create(i, u), (y == 4 || u == 4) ? true : false); //never a reason to not promote bishop
                            if (!InCheck(MakeMove(attempt).Board, ply))
                                moves.Add(attempt);
                        }
                        else if (p >= Piece.BKing)
                        {
                            Move attempt = new Move(Tuple.Create(x, y), Tuple.Create(i, u), (y == 4 || u == 4) ? true : false); //never a reason to not promote bishop
                            if (!InCheck(MakeMove(attempt).Board, ply))
                                moves.Add(attempt);

                            u += 99; //can't pass through pieces; stop looking for up moves
                        }
                        else
                            u += 99;
                    }
                    if (d >= 0)
                    {
                        Piece p = (Piece)Board[i, d];
                        if (p == Piece.Empty)
                        {
                            Move attempt = new Move(Tuple.Create(x, y), Tuple.Create(i, d), (y == 4 || d == 4) ? true : false); //never a reason to not promote bishop
                            if (!InCheck(MakeMove(attempt).Board, ply))
                                moves.Add(attempt);
                        }
                        else if (p >= Piece.BKing)
                        {
                            Move attempt = new Move(Tuple.Create(x, y), Tuple.Create(i, d), (y == 4 || d == 4) ? true : false); //never a reason to not promote bishop
                            if (!InCheck(MakeMove(attempt).Board, ply))
                                moves.Add(attempt);

                            d -= 99; //can't pass through pieces
                        }
                        else
                            d -= 99;
                    }
                }
            }
            else
            {
                //north/southeast
                int u = y;
                int d = y;
                for (int i = x + 1; i < 5; ++i)
                {
                    u++;
                    d--;
                    if (u < 5)
                    {
                        //up
                        Piece p = (Piece)Board[i, u];
                        if (p == Piece.Empty)
                        {
                            Move attempt = new Move(Tuple.Create(x, y), Tuple.Create(i, u), (y == 0 || u == 0) ? true : false); //never a reason to not promote bishop
                            if (!InCheck(MakeMove(attempt).Board, ply))
                                moves.Add(attempt);
                        }
                        else if (p < Piece.BKing)
                        {
                            Move attempt = new Move(Tuple.Create(x, y), Tuple.Create(i, u), (y == 0 || u == 0) ? true : false); //never a reason to not promote bishop
                            if (!InCheck(MakeMove(attempt).Board, ply))
                                moves.Add(attempt);

                            u += 99; //can't pass through pieces; stop looking for up moves
                        }
                        else
                            u += 99;
                    }
                    if (d >= 0)
                    {
                        Piece p = (Piece)Board[i, d];
                        if (p == Piece.Empty)
                        {
                            Move attempt = new Move(Tuple.Create(x, y), Tuple.Create(i, d), (y == 0 || d == 0) ? true : false); //never a reason to not promote bishop
                            if (!InCheck(MakeMove(attempt).Board, ply))
                                moves.Add(attempt);
                        }
                        else if (p < Piece.BKing)
                        {
                            Move attempt = new Move(Tuple.Create(x, y), Tuple.Create(i, d), (y == 0 || d == 0) ? true : false); //never a reason to not promote bishop
                            if (!InCheck(MakeMove(attempt).Board, ply))
                                moves.Add(attempt);

                            d -= 99; //can't pass through pieces
                        }
                        else
                            d -= 99;
                    }
                }

                u = y;
                d = y;
                //north/southwest
                for (int i = x - 1; i >= 0; --i)
                {
                    u++;
                    d--;
                    if (u < 5)
                    {
                        //up
                        Piece p = (Piece)Board[i, u];
                        if (p == Piece.Empty)
                        {
                            Move attempt = new Move(Tuple.Create(x, y), Tuple.Create(i, u), (y == 0 || u == 0) ? true : false); //never a reason to not promote bishop
                            if (!InCheck(MakeMove(attempt).Board, ply))
                                moves.Add(attempt);
                        }
                        else if (p < Piece.BKing)
                        {
                            Move attempt = new Move(Tuple.Create(x, y), Tuple.Create(i, u), (y == 0 || u == 0) ? true : false); //never a reason to not promote bishop
                            if (!InCheck(MakeMove(attempt).Board, ply))
                                moves.Add(attempt);

                            u += 99; //can't pass through pieces; stop looking for up moves
                        }
                        else
                            u += 99;
                    }
                    if (d >= 0)
                    {
                        Piece p = (Piece)Board[i, d];
                        if (p == Piece.Empty)
                        {
                            Move attempt = new Move(Tuple.Create(x, y), Tuple.Create(i, d), (y == 0 || d == 0) ? true : false); //never a reason to not promote bishop
                            if (!InCheck(MakeMove(attempt).Board, ply))
                                moves.Add(attempt);
                        }
                        else if (p < Piece.BKing)
                        {
                            Move attempt = new Move(Tuple.Create(x, y), Tuple.Create(i, d), (y == 0 || d == 0) ? true : false); //never a reason to not promote bishop
                            if (!InCheck(MakeMove(attempt).Board, ply))
                                moves.Add(attempt);

                            d -= 99; //can't pass through pieces
                        }
                        else
                            d -= 99;
                    }
                }
            }
            return moves;
        }
        public List<Move> GetBishopPromoMoves(int x, int y)
        {
            //Console.WriteLine("Bishop+ {0} {1}", x, y);
            List<Move> moves = GetBishopMoves(x, y);
            foreach(Move mm in moves)
            {
                mm.promote = false; //bishop promo can't promote again!
            }
            moves.AddRange(GetKingMoves(x, y));
            moves = moves.Distinct().ToList();
            /*if (ply)
            {
                int[] xm = new int[4] { x - 1, x, x, x + 1 };
                int[] ym = new int[4] { y, y + 1, y, y - 1 };
                for (int i = 0; i < 4; ++i)
                {
                    Piece p = (Piece)Board[xm[i], ym[i]];
                    if (xm[i] < 0 || xm[i] > 4 || ym[i] < 0 || ym[i] > 4 || (p > Piece.Empty && p < Piece.BKing))
                        continue;

                    Move attempt = new Move(Tuple.Create(x, y), Tuple.Create(xm[i], ym[i]), false);
                    if (!InCheck(MakeMove(attempt).Board, ply))
                    {
                        moves.Add(attempt);
                    }
                }
            }
            else
            {
                int[] xm = new int[4] { x - 1, x, x, x + 1 };
                int[] ym = new int[4] { y, y + 1, y, y - 1 };
                for (int i = 0; i < 4; ++i)
                {
                    Piece p = (Piece)Board[xm[i], ym[i]];
                    if (xm[i] < 0 || xm[i] > 4 || ym[i] < 0 || ym[i] > 4 || p >= Piece.BKing)
                        continue;

                    Move attempt = new Move(Tuple.Create(x, y), Tuple.Create(xm[i], ym[i]), false);
                    if (!InCheck(MakeMove(attempt).Board, ply))
                    {
                        moves.Add(attempt);
                    }
                }
            }*/
            return moves;
        }
        public List<Move> GetRookMoves(int x, int y)
        {
           // Console.WriteLine("Rook {0} {1}", x, y);
            List<Move> moves = new List<Move>();
            if (ply)
            {
                //north/south
                for (int j = y + 1; j < 5; ++j)
                {
                    Piece p = (Piece)Board[x, j];
                    if (p == Piece.Empty)
                    {
                        Move attempt = new Move(Tuple.Create(x, y), Tuple.Create(x, j), (y == 4 || j == 4) ? true : false); //never a reason to not promote bishop
                        if (!InCheck(MakeMove(attempt).Board, ply))
                            moves.Add(attempt);
                    }
                    else if (p >= Piece.BKing)
                    {
                        Move attempt = new Move(Tuple.Create(x, y), Tuple.Create(x, j), (y == 4 || j == 4) ? true : false); //never a reason to not promote bishop
                        if (!InCheck(MakeMove(attempt).Board, ply))
                            moves.Add(attempt);

                        break; //can't pass through pieces
                    }
                    else
                        break;
                }
                for (int j = y - 1; j >= 0; --j)
                {
                    Piece p = (Piece)Board[x, j];
                    if (p == Piece.Empty)
                    {
                        Move attempt = new Move(Tuple.Create(x, y), Tuple.Create(x, j), (y == 4 || j == 4) ? true : false); //never a reason to not promote bishop
                        if (!InCheck(MakeMove(attempt).Board, ply))
                            moves.Add(attempt);
                    }
                    else if (p >= Piece.BKing)
                    {
                        Move attempt = new Move(Tuple.Create(x, y), Tuple.Create(x, j), (y == 4 || j == 4) ? true : false); //never a reason to not promote bishop
                        if (!InCheck(MakeMove(attempt).Board, ply))
                            moves.Add(attempt);

                        break; //can't pass through pieces
                    }
                    else
                        break;
                }

                //east/west

                for (int i = x + 1; i < 5; ++i)
                {
                    Piece p = (Piece)Board[i, y];
                    if (p == Piece.Empty)
                    {
                        Move attempt = new Move(Tuple.Create(x, y), Tuple.Create(i, y), (y == 4) ? true : false); //never a reason to not promote bishop
                        if (!InCheck(MakeMove(attempt).Board, ply))
                            moves.Add(attempt);
                    }
                    else if (p >= Piece.BKing)
                    {
                        Move attempt = new Move(Tuple.Create(x, y), Tuple.Create(i, y), (y == 4) ? true : false); //never a reason to not promote bishop
                        if (!InCheck(MakeMove(attempt).Board, ply))
                            moves.Add(attempt);

                        break; //can't pass through pieces
                    }
                    else
                        break;
                }
                for (int i = x - 1; i >= 0; --i)
                {
                    Piece p = (Piece)Board[i, y];
                    if (p == Piece.Empty)
                    {
                        Move attempt = new Move(Tuple.Create(x, y), Tuple.Create(i, y), (y == 4) ? true : false); //never a reason to not promote bishop
                        if (!InCheck(MakeMove(attempt).Board, ply))
                            moves.Add(attempt);
                    }
                    else if (p >= Piece.BKing)
                    {
                        Move attempt = new Move(Tuple.Create(x, y), Tuple.Create(i, y), (y == 4) ? true : false); //never a reason to not promote bishop
                        if (!InCheck(MakeMove(attempt).Board, ply))
                            moves.Add(attempt);

                        break; //can't pass through pieces
                    }
                    else
                        break;
                }
                
            }
            else
            {
                //north/south
                for (int j = y + 1; j < 5; ++j)
                {
                    Piece p = (Piece)Board[x, j];
                    if (p == Piece.Empty)
                    {
                        Move attempt = new Move(Tuple.Create(x, y), Tuple.Create(x, j), (y == 0 || j == 0) ? true : false); //never a reason to not promote bishop
                        if (!InCheck(MakeMove(attempt).Board, ply))
                            moves.Add(attempt);
                    }
                    else if (p < Piece.BKing)
                    {
                        Move attempt = new Move(Tuple.Create(x, y), Tuple.Create(x, j), (y == 0 || j == 0) ? true : false); //never a reason to not promote bishop
                        if (!InCheck(MakeMove(attempt).Board, ply))
                            moves.Add(attempt);

                        break; //can't pass through pieces
                    }
                    else
                        break;
                }
                for (int j = y - 1; j >= 0; --j)
                {
                    Piece p = (Piece)Board[x, j];
                    if (p == Piece.Empty)
                    {
                        Move attempt = new Move(Tuple.Create(x, y), Tuple.Create(x, j), true); //never a reason to not promote rook
                        if (!InCheck(MakeMove(attempt).Board, ply))
                            moves.Add(attempt);
                    }
                    else if (p < Piece.BKing)
                    {
                        Move attempt = new Move(Tuple.Create(x, y), Tuple.Create(x, j), true);
                        if (!InCheck(MakeMove(attempt).Board, ply))
                            moves.Add(attempt);

                        break; //can't pass through pieces
                    }
                    else
                        break;
                }

                //east/west

                for (int i = x + 1; i < 5; ++i)
                {
                    Piece p = (Piece)Board[i, y];
                    if (p == Piece.Empty)
                    {
                        Move attempt = new Move(Tuple.Create(x, y), Tuple.Create(i, y), true);
                        if (!InCheck(MakeMove(attempt).Board, ply))
                            moves.Add(attempt);
                    }
                    else if (p < Piece.BKing)
                    {
                        Move attempt = new Move(Tuple.Create(x, y), Tuple.Create(i, y), true); 
                        if (!InCheck(MakeMove(attempt).Board, ply))
                            moves.Add(attempt);

                        break; //can't pass through pieces
                    }
                    else
                        break;
                }
                for (int i = x - 1; i >= 0; --i)
                {
                    Piece p = (Piece)Board[i, y];
                    if (p == Piece.Empty)
                    {
                        Move attempt = new Move(Tuple.Create(x, y), Tuple.Create(i, y), (y == 0) ? true : false); //never a reason to not promote bishop
                        if (!InCheck(MakeMove(attempt).Board, ply))
                            moves.Add(attempt);
                    }
                    else if (p < Piece.BKing)
                    {
                        Move attempt = new Move(Tuple.Create(x, y), Tuple.Create(i, y), (y == 0) ? true : false); //never a reason to not promote bishop
                        if (!InCheck(MakeMove(attempt).Board, ply))
                            moves.Add(attempt);

                        break; //can't pass through pieces
                    }
                    else
                        break;
                }
            }
            return moves;

        }
        public List<Move> GetRookPromoMoves(int x, int y)
        {
           // Console.WriteLine("RookPromo {0} {1}", x, y);
            List<Move> moves = GetRookMoves(x, y);
            foreach (Move mm in moves)
            {
                mm.promote = false; //can't promote again!
            }
            moves.AddRange(GetKingMoves(x, y));
            moves = moves.Distinct().ToList();

            return moves;
        }

        public List<Drop> GetAllLegalDrops()
        {
            List<Drop> drops = new List<Drop>();
            if (ply)
            {
                foreach(int i in P1Hand)
                {
                    Piece p = (Piece)i;
                    for(int x = 0; x < 5; ++x)
                    {
                        for(int y = 0; y < 5; ++y)
                        {
                            if(Board[x,y] == 0)
                            {
                                if (!(p == Piece.WPawn))
                                {
                                    int[,] test = CloneBoard();
                                    test[x, y] = (int)p;
                                    if (!InCheck(test, true))
                                        drops.Add(new Drop(Tuple.Create(x, y), p));
                                }
                                else
                                {
                                    //Can't drop 2 pawns on the same column
                                    //nor can you drop on the back rank
                                    if ((y!=4)&&(!Board.GetColumn(x).Contains((int)Piece.WPawn)))
                                    {
                                        int[,] test = CloneBoard();
                                        test[x, y] = (int)p;
                                        //Also can't checkmate with a pawn drop
                                        if((!InCheck(test,true))&&(!InCheckmatePawn(test,false)))
                                            drops.Add(new Drop(Tuple.Create(x, y), p));
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                foreach (int i in P2Hand)
                {
                    Piece p = (Piece)i;
                    for (int x = 0; x < 5; ++x)
                    {
                        for (int y = 0; y < 5; ++y)
                        {
                            if (Board[x, y] == 0)
                            {
                                if (!(p == Piece.BPawn))
                                {
                                    int[,] test = CloneBoard();
                                    test[x, y] = (int)p;
                                    if (!InCheck(test, false))
                                        drops.Add(new Drop(Tuple.Create(x, y), p));
                                }
                                else
                                {
                                    //Can't drop 2 pawns on the same column
                                    //nor can you drop on the back rank
                                    if ((y != 0) && (!Board.GetColumn(x).Contains((int)Piece.BPawn)))
                                    {
                                        int[,] test = CloneBoard();
                                        test[x, y] = (int)p;
                                        //Also can't checkmate with a pawn drop
                                        if ((!InCheck(test, false)) && (!InCheckmatePawn(test, true)))
                                            drops.Add(new Drop(Tuple.Create(x, y), p));
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return drops;
        }

        public void SetBoard(int[,] _board)
        {
            Array.Copy(_board, Board, 25);
        }

        public int[,] CloneBoard()
        {
            int[,] clone = new int[5, 5];
            Array.Copy(Board, clone, 25);
            return clone;
        }

        /// <summary>
        /// Makes the move <paramref name="m"/> and returns a new Game state
        /// </summary>
        /// <param name="m"></param>
        public Game MakeMove(Move m)
        {
            Game newState = DeepClone();
            int xb = m.begin.Item1;
            int yb = m.begin.Item2;
            int xf = m.finish.Item1;
            int yf = m.finish.Item2;
            Piece p = (Piece)Board[xb, yb];
            newState.Board[xb, yb] = 0;
            Piece captured = (Piece)Board[xf, yf];
            //DEBUG
            /*if(captured == Piece.WKing || captured == Piece.BKing)
            {

            }*/
            //
            if (p < Piece.BKing)
            {
                //we're dealing with white's pieces
                if ((yf == 4 || yb == 4) && (m.promote) && (p == Piece.WPawn || p == Piece.WSilver || p == Piece.WBishop || p == Piece.WRook))
                    newState.Board[xf, yf] = (int)p + 5;
                else
                    newState.Board[xf, yf] = (int)p;

                if (captured != Piece.Empty)
                {
                    if (captured < Piece.BPawnPromo)
                        newState.P1Hand.Add((int)captured - 11);
                    else
                        newState.P1Hand.Add((int)captured - 16);
                }
            }
            else
            {
                //we're dealing with black's pieces
                if ((yf == 0 || yb == 0) && (m.promote) && (p == Piece.BPawn || p == Piece.BSilver || p == Piece.BBishop || p == Piece.BRook))
                    newState.Board[xf, yf] = (int)p + 5;
                else
                    newState.Board[xf, yf] = (int)p;

                if (captured != Piece.Empty)
                {
                    if (captured < Piece.WPawnPromo)
                        newState.P2Hand.Add((int)captured + 11);
                    else
                        newState.P2Hand.Add((int)captured + 6);
                }
            }
            //newState.ply = !ply; //moved this to DeepClone()
            return newState;
        }
        public Game MakeDrop(Drop d)
        {
            Game newState = DeepClone();
            //newState.ply = !ply; //moved to DeepClone
            newState.Board[d.pos.Item1, d.pos.Item2] = (int)d.piece;
            if (!newState.ply)
            {
                newState.P1Hand.Remove((int)d.piece);
            }
            else
            {
                newState.P2Hand.Remove((int)d.piece);
            }
            
            return newState;
        }

        public Game DeepClone()
        {
            Game clone = new Game();
            clone.SetBoard(Board);
            List<int> newP1Hand = new List<int>();
            List<int> newP2Hand = new List<int>();
            foreach (int i in P1Hand)
                newP1Hand.Add(i);
            foreach (int i in P2Hand)
                newP2Hand.Add(i);

            clone.P1Hand = newP1Hand;
            clone.P2Hand = newP2Hand;
            clone.ply = !ply;

            return clone;
        }

        public void Draw()
        {
            string[] pieceStrings = new string[] {"   ", " K ", " P ", " S ", " G ", " B ", " R ", " P+",
                                                  " S+", " G+", " B+", " R+", " k ", " p ", " s ", " g ",
                                                  " b ", " r ", " p+", " s+", " g+", " b+", " r+" };
            foreach(int i in P2Hand)
            {
                Console.Write(pieceStrings[i]);
                
            }
            Console.WriteLine("\n");
            for (int y = 0; y < 5; ++y)
            {
                Console.WriteLine("---------------------");
                for (int x = 0; x < 5; ++x)
                {
                    Console.Write("|" + pieceStrings[Board[x, 4-y]]);
                }
                Console.WriteLine("|");
            }
            Console.WriteLine("---------------------\n");
            foreach (int i in P1Hand)
            {
                Console.Write(pieceStrings[i]);
            }
            Console.WriteLine("\n");
        }

        public void Log(StreamWriter sw)
        {
            string[] pieceStrings = new string[] {"   ", " K ", " P ", " S ", " G ", " B ", " R ", " P+",
                                                  " S+", " G+", " B+", " R+", " k ", " p ", " s ", " g ",
                                                  " b ", " r ", " p+", " s+", " g+", " b+", " r+" };
            foreach (int i in P2Hand)
            {
                sw.Write(pieceStrings[i]);

            }
            sw.WriteLine("\n");
            for (int y = 0; y < 5; ++y)
            {
                sw.WriteLine("---------------------");
                for (int x = 0; x < 5; ++x)
                {
                    sw.Write("|" + pieceStrings[Board[x, 4 - y]]);
                }
                sw.WriteLine("|");
            }
            sw.WriteLine("---------------------\n");
            foreach (int i in P1Hand)
            {
                sw.Write(pieceStrings[i]);
            }
            sw.WriteLine("\n");
        }
    }
}
