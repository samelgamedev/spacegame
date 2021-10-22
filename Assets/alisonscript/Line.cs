﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;

namespace spacegame.alisonscript
{
    public class Line
    {
        public string contents;

        public Line(string contents)
        {
            this.contents = contents;
        }

        public void Process(RunningScript runningScript)
        {
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

            // if the line starts with a semicolon, call a function
            if (line.StartsWith(";"))
            {
                // this is absolutely horrible!
                string functionName = line.Substring(1); // get the function name by substringing the line to exclude the semicolon
                // then we just get rid of the args by cutting of the string at the first mention of a white space or quote
                functionName = new Regex("\".*?\"|\\s").Replace(functionName, string.Empty);

                if (functionName == string.Empty)
                    throw new AlisonscriptSyntaxError(Interpreter.runningScript.GetCurrentLine(), "function call expected");

                // i stole this regex lol https://stackoverflow.com/questions/49239218/get-string-between-character-using-regex-c-sharp

                // TODO: this regex picks up whitespaces between arguments,
                // so additional arguments need to be passed like ';thing "an argument""another argument"'
                // instead of 'thing "an argument" "another argument"'
                // nevermind i just tested it again this is completely broken with more than one argument
                Regex searchForArgs = new Regex("(?<=\")(.+?)(?=\")");
                var argsCount = searchForArgs.Matches(line);

                // if there were no args, throw an syntax error
                if (argsCount.Count == 0)
                    throw new AlisonscriptSyntaxError(Interpreter.runningScript.GetCurrentLine(), "tried to call a function without arguments");

                string[] args = new string[argsCount.Count];
                for (int i = 0; i < argsCount.Count; i++)
                    args[i] = argsCount[i].Value;

                // then call the function 
                if (Interpreter.runningScript is null)
                {
                    Debug.Log("i guess we're done?");
                    return;
                }
                Functions.instance.Call(functionName, () => Interpreter.runningScript.IncrementIndex(), args); // increment into lineIndex as callback
            }
        }

        public static List<Line> FromStringArray(string[] array)
        {
            if (array.Length == 0)
                throw new Exception("array is empty");
            List<Line> lines = new List<Line>();

            // initialize a line from every string in the array and add it to the list
            foreach (string s in array)
                lines.Add(new Line(s));

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
