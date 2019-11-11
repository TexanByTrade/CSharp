using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCTS_Minishogi
{
    public enum GameResult
    {
        LOSS,
        DRAW,
        WIN
    }

    public enum Player
    {
        P1,
        WHITE = P1,
        P2,
        BLACK = P2
    }

    public enum Piece
    {
        Empty = 0,
        WKing,
        WPawn,
        WSilver,
        WGold,
        WBishop,
        WRook,
        WPawnPromo,
        WSilverPromo,
        WGoldPromo, //there is no gold promo in shogi, but makes promoting easier in code
        WBishopPromo,
        WRookPromo,
        BKing,
        BPawn,
        BSilver,
        BGold,
        BBishop,
        BRook,
        BPawnPromo,
        BSilverPromo,
        BGoldPromo,
        BBishopPromo,
        BRookPromo
    }

    public static class ExtensionMethods
    {
        public static Tuple<int, int> Find<T>(this T[,] matrix, T value)
        {
            for(int x = 0; x < 5; ++x)
            {
                for(int y = 0; y < 5; ++y)
                {
                    if (matrix[x, y].Equals(value))
                        return Tuple.Create(x, y);
                }
            }

            return Tuple.Create(-1, -1);
        }

        //faster to find black king by starting at 4,4
        public static Tuple<int, int> FindBlack<T>(this T[,] matrix, T value)
        {
            for (int x = 4; x >= 0; --x)
            {
                for (int y = 4; y >= 0; --y)
                {
                    if (matrix[x, y].Equals(value))
                        return Tuple.Create(x, y);
                }
            }

            return Tuple.Create(-1, -1);
        }

        public static T[,] FlipVertical<T>(this T[,] matrix)
        {
            int w = matrix.GetLength(0);
            int h = matrix.GetLength(1);
            T[,] flipped = new T[w, h];
            for (int y = 0; y < h; ++y)
            {
                for (int x = 0; x < w; ++x)
                {
                    flipped[x, h - y - 1] = matrix[x, y];
                }
            }
            return flipped;
        }

        public static T[] GetColumn<T>(this T[,] matrix, int x)
        {
            T[] col = new T[matrix.GetLength(1)];
            for(int i = 0; i < matrix.GetLength(1); ++i)
            {
                col[i] = matrix[x, i];
            }
            return col;
        }
    }
}
