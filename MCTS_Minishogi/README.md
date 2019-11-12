# Monte Carlo Tree Search

One method for playing games with artificial intelligence is the [Monte Carlo Tree Search](https://en.wikipedia.org/wiki/Monte_Carlo_tree_search) (MCTS). This method is basically the minimax algorithm but for games with such a large search space that each ply can't be searched exhaustively. 

This code runs a "pure" MCTS. That is, no heuristics about the game are used in evaluating positions; games are played out by choosing moves solely at random until a win/loss is achieved. The game being played in this project is [Minishogi](https://en.wikipedia.org/wiki/Minishogi), a small variant of Japanese chess. Minishogi has a smaller board and fewer pieces than Western chess, but because captured pieces can be dropped back onto the board, the state space never shrinks towards zero. 

It turns out that a pure MCTS is not optimal for Minishogi. If the game isn't at a critical point (for instance, on the computer's first move where there isn't even an option to take a piece), most moves have a win rate close to 50%. Since my implementation of the game logic here is horrendously slow, the computer can't simulate enough moves to differentiate better positional moves. In fact, sometimes the computer just makes clearly bad moves. But if you offer up a piece for free, the computer usually takes it!

To improve the AI in Minishogi, I need faster game logic as well as redesigning the TreeNode system slightly to avoid so much memory locking, but the main idea behind MCTS here.
