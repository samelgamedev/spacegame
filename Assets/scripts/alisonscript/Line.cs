using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;

namespace spacegame.alisonscript
{
    // dumpster fire class
    public class Line
    {
        public string contents;
        public int index;

        public bool inConditional;

        public Line(string contents, int index, bool inConditional = false)
        {
            this.contents = contents;
            this.index = index;
            this.inConditional = inConditional;
        }

        // i stole this regex lol https://stackoverflow.com/questions/49239218/get-string-between-character-using-regex-c-sharp
        // this is used in Label too and it's a bit lengthy so i made it a method
        public static string[] ArgsRegex(string input)
        {
            Regex searchForArgs = new Regex("(?<=\")(.+?)(?=\")");
            var argsCount = searchForArgs.Matches(input);

            List<string> args = new List<string>();
            for (int i = 0; i < argsCount.Count; i++)
                // only treat the argument as an argument if it's the first argument or its index in the array isn divisible by 2
                // this avoids treating the empty whitespaces between arguments as arguments,
                // e.g. ;log "hello" "dooby" logs "hello" and "dooby" instead of "hello" " " and "dooby"
                if (i == 0 || i % 2 == 0)
                    args.Add(argsCount[i].Value);

            return args.ToArray();
        }

        // this is absolutely horrible!
        public void Process(RunningScript runningScript)
        {
            /* // what was this for
            if (inConditional && !Interpreter.runningScript.inCond)
            {
                Interpreter.runningScript.IncrementIndex();
                return;
            }*/

            string line = contents;

            // remove indenting with regex to get the actual line
            Regex indenting = new Regex(@"\A\s*");
            line = indenting.Replace(line, string.Empty);

            // if the line is empty or starts with a #, treat it as a comment and return here
            if (string.IsNullOrEmpty(line) || string.IsNullOrWhiteSpace(line) || line.StartsWith("#"))
            {
                Interpreter.runningScript.IncrementIndex();
                return;
            }

            // TODO: regex and get the line starter here and put the logic in a switch

            // if the line starts with a semicolon, call a function
            if (line.StartsWith(";"))
            {
                string functionName = line.Substring(1); // get the function name by substringing the line to exclude the semicolon
                // then we just get rid of the args by cutting of the string at the first mention of a white space or quote
                functionName = new Regex("\".*?\"|\\s").Replace(functionName, string.Empty);

                if (functionName == string.Empty)
                    throw new AlisonscriptSyntaxError(Interpreter.runningScript.GetCurrentLine(), "function call expected");

                // get args from the function call
                string[] args = ArgsRegex(line);

                // if there were no args, throw an syntax error
                //if (args.Length == 0)
                //    throw new AlisonscriptSyntaxError(Interpreter.runningScript.GetCurrentLine(), "tried to call a function without arguments");

                // then call the function 
                if (Interpreter.runningScript is null) // how does this happen
                {
                    Logger.WriteLine("i guess we're done?");
                    return;
                }
                Functions.instance.Call(functionName, () => Interpreter.runningScript.IncrementIndex(), args); // increment into lineIndex as callback
            }
            // if the line starts with an @ then it's referring to an object, so set it to a value
            else if (line.StartsWith("@"))
            {
                string objectName = line;
                objectName = new Regex("\".*?\"|\\s").Replace(objectName, string.Empty); // remove everything after the whitespace

                string[] args = ArgsRegex(line); // args

                if (args.Length == 0)
                    throw new AlisonscriptSyntaxError(Interpreter.runningScript.GetCurrentLine(), "value assignment expected");

                // get the value being assigned to the object
                string objectValue = args[0];

                // add the object
                Interpreter.runningScript.AddObject(objectName, objectValue);

                Interpreter.runningScript.IncrementIndex();
            }
            // if the line starts with cond, it's a conditional
            else if (line.StartsWith("cond"))
            {
                string[] args = ArgsRegex(line);

                if (Interpreter.runningScript.inCond)
                    throw new AlisonscriptSyntaxError(Interpreter.runningScript.GetCurrentLine(), 
                        "nested conditionals haven't been added yet and will break");

                if (args.Length < 1)
                    throw new AlisonscriptSyntaxError(Interpreter.runningScript.GetCurrentLine(), 
                        $"a conditional statement expects 1 argument (given {args.Length})");

                Interpreter.runningScript.inCond = true;
                Interpreter.runningScript.condObjectName = args[0];

                Interpreter.runningScript.IncrementIndex();
            }
            // when in conditional
            else if (line.StartsWith("when"))
            {
                string[] args = ArgsRegex(line);

                if (!Interpreter.runningScript.inCond)
                    throw new AlisonscriptSyntaxError(Interpreter.runningScript.GetCurrentLine(), "stray when statement");

                if (args.Length < 1)
                    throw new AlisonscriptSyntaxError(Interpreter.runningScript.GetCurrentLine(),
                        $"a when statement expects 1 argument (given {args.Length})");

                // if the condition is true, process the rest of the in the when
                if (Interpreter.runningScript.ConditionalTrue(Interpreter.runningScript.condObjectName, args[0]))
                    Interpreter.runningScript.IncrementIndex();
                else
                    // otherwise, jump to the next occurence of when or end
                    Interpreter.runningScript.JumpToNextOccurence(Interpreter.runningScript.lineIndex, "when");
            }
            // end conditional statement
            else if (line.StartsWith("end"))
            {
                if (Interpreter.runningScript.inCond)
                    Interpreter.runningScript.inCond = false;
                else
                    throw new AlisonscriptSyntaxError(Interpreter.runningScript.GetCurrentLine(), "stray end statement");

                Interpreter.runningScript.IncrementIndex();
            }
            else
            {
                Interpreter.runningScript.IncrementIndex();
            }
        }

        public static List<Line> FromStringArray(string[] array)
        {
            if (array.Length == 0)
                throw new Exception("array is empty");
            List<Line> lines = new List<Line>();

            // initialize a line from every string in the array and add it to the list
            bool inConditional = false;
            for (int i = 0; i < array.Length; i++)
            {
                Line add = new Line(array[i], i);

                // mark future lines as in conditional 
                inConditional = add.contents.StartsWith("cond") && (!add.contents.StartsWith("end"));
                /*
                if (add.contents.StartsWith("cond"))
                    inConditional = true;
                else if (add.contents.StartsWith("end"))
                    inConditional = false;*/

                add.inConditional = inConditional;

                lines.Add(add);
            }

            // convert the list to an array and return
            return lines;
        }

        // overload string cast to just return the contents of the line for efficiency
        public static implicit operator string(Line line)
        {
            return line.contents;
        }
    }
}
