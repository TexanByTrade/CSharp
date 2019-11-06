using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;

namespace BF_Interpreter
{
    class Program
    {
        //Interpreter for the brainf*** language, https://esolangs.org/wiki/Brainf*** (Obviously, the name is offensive...)
        //Specifically, this interpreter uses unsigned byte-sized cells with an infinite tape
        //    
        //Usage:
        //BF_Interpreter.exe <filepath> [input] [dump (true/false)]
        //---------------------------------------------------------
        //filepath: location to brainf*** code
        //
        //input (optional): This interpreter does not pause and read input. All input is passed through the console
        //       which the program reads in sequential order with the BF "," command.
        //       Changing this behavior is simple: change line 93 to support Console.ReadLine
        //
        //dump (optional): If "true" or "t" is given as the 3rd argument, the sanitized BF code is dumped to the Console.
        //---------------------------------------------------------
        //Christopher Cormier (2019)

        static void Main(string[] args)
        {
            if(args.Length == 0)
            {
                throw new ArgumentException("Error: enter filepath to BF code.");
            }
            
            List<byte> input = new List<byte>();
            if (args.Length == 2)
                input = Encoding.ASCII.GetBytes(args[1]).ToList();

            string program = "";

            using (StreamReader sr = new StreamReader(args[0]))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    program += line;
                }
            }

            Regex junk = new Regex("[^<>+-.,\\[\\]]");
            program = junk.Replace(program, ""); //remove whitespace etc.

            //check for mismatched []
            int brackets = 0;
            for(int i = 0; i < program.Length; ++i)
            {
                if (program[i] == '[')
                    brackets++;
                else if (program[i] == ']')
                    brackets--;

                if (brackets < 0)
                    throw new ArgumentException("Error: mismatched [ or ]");
            }
            if(brackets != 0)
                throw new ArgumentException("Error: mismatched [ or ]");

            if((args.Length == 3)&&(args[2].ToLower()=="true"||args[2].ToLower()=="t")) //optional 3rd argument to dump the program to Console
            {
                Console.WriteLine(program);
            }

            List<byte> tape = Enumerable.Repeat<byte>(0, 1024).ToList();
            int dataPointer = 0;
            int programPointer = 0;
            
            while (programPointer <= (program.Length - 1))
            {
                switch (program[programPointer])
                {
                    case '>': if (dataPointer < int.MaxValue) dataPointer++; if (tape.Count <= dataPointer) tape.Add(0); break;

                    case '<': if (dataPointer > 0) dataPointer--; break;

                    case '+': tape[dataPointer]++; break;

                    case '-': tape[dataPointer]--; break;

                    case ',': if (input.Count > 0)
                        {
                            tape[dataPointer] = input[0];
                            input.RemoveAt(0);
                        }
                        else
                        {
                            tape[dataPointer] = 0;
                        }
                        break;

                    case '.': Console.Write(Encoding.ASCII.GetString(new[] { tape[dataPointer] })); break;

                    case '[': if(tape[dataPointer]==0) programPointer = FindJumpPair(program, programPointer); break;

                    case ']': if(tape[dataPointer]!=0) programPointer = FindJumpPair(program, programPointer); break;
                    //this should never be reached:
                    default: throw new InvalidProgramException("Error: illegal character in code");
                }
                programPointer++;
            }
            
        }

        static int FindJumpPair(string program, int thisInd)
        {
            if (program[thisInd] == '[')
            {
                int brackets = 1;
                for(int i = thisInd + 1; i < program.Length; ++i)
                {
                    if (program[i] == '[')
                        brackets++;
                    else if (program[i] == ']')
                        brackets--;

                    if (brackets == 0)
                        return i;
                }
            }
            else
            {
                int brackets = -1;
                for (int i = thisInd - 1; i >= 0; --i)
                {
                    if (program[i] == '[')
                        brackets++;
                    else if (program[i] == ']')
                        brackets--;

                    if (brackets == 0)
                        return i;
                }
            }
            //this should never be reached:
            throw new InvalidProgramException("Error: illegal jump.");
        }
    }
}
