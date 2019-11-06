# brainf*** Interpreter

Brainf*** (unfortunately I can't change the offensive name) is a simple programming langauge designed to somewhat resemble the classic Turing machine. An infinite tape contains cells that can be read or incremented. For instructions, see https://esolangs.org/wiki/Brainf***.

Call BF_Interpreter.exe from the command line with at least one argument specifying the path to the bf code. Pass true as the 3rd argument to dump the sanitized program to console.

> BF_Interpreter.exe <filepath> [input string] [dump (T/F)]
