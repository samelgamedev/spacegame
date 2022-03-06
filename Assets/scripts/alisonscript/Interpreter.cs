using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;
using UnityEditor;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace spacegame.alisonscript
{
    public static class Interpreter
    {
        public static bool interpreterRunning;

        private static RunningScript _runningScript;
        public static RunningScript runningScript
        {
            get
            {
                return _runningScript;
            }
            private set
            {
                if (_runningScript != null && value != null)
                    throw new Exception("shouldn't set the running script to a value while a script is running");
                _runningScript = value;
            }
        }

        public static int lineIndex
        {
            get
            {
                return runningScript.lineIndex;
            }
            set
            {
                runningScript.lineIndex = value;
            }
        }

        public static int depth
        {
            get
            {
                return runningScript.depth;
            }
            set
            {
                runningScript.depth = value;
            }
        }

        public static string[] ArgsRegex(string input)
        {
            Regex searchForArgs = new Regex("(?<=\")(.+?)(?=\")");
            var argsCount = searchForArgs.Matches(input);

            List<string> args = new List<string>();
            // TODO: allow object formatting in args using #{(object name)}
            for (int i = 0; i < argsCount.Count; i++)
                // only treat the argument as an argument if it's the first argument or its index in the array isn't divisible by 2
                // this avoids treating the empty whitespaces between arguments as arguments,
                // e.g. ;log "hello" "dooby" logs "hello" and "dooby" instead of "hello" " " and "dooby"
                if (i == 0 || i % 2 == 0)
                    args.Add(argsCount[i].Value);

            return args.ToArray();
        }

        // reflection hurts my head man
        public static void RegisterFunctions()
        {
            // iterate through methods in Functions type 
            foreach (MethodInfo m in typeof(Functions).GetMethods())
            {
                var v = m.GetCustomAttributes(typeof(FunctionAttribute));
                if (v.Count() > 0) // yeah!!! it's a function!!! let's go!!!!
                {
                    // get the name of the function from the function attribute and add it as a key to the functions dictionary
                    FunctionAttribute f = (FunctionAttribute)Attribute.GetCustomAttribute(m, typeof(FunctionAttribute));
                    string functionName = f.name;
                    Functions.instance.functions.Add(functionName, m.Name);
                    Logger.WriteLine($"registered alisonscript function: {functionName} ({m.Name})");
                }
            }
            Logger.WriteLine("finished registering alisonscript functions");
        }

        public static void RegisterKeywords()
        {
            foreach (Type t in Assembly.GetCallingAssembly().GetTypes())
            {
                // determine whether the type implements the IKeyword interface
                if (t.GetInterfaces().Contains(typeof(IKeyword)))
                {
                    // initialize an instance of the keyword and add it to the keywords list in KeywordManager
                    if (t.GetConstructors().Length == 0)
                        // selfproofing
                        throw new Exception($"add a constructor to the keyword {t.Name}!");
                    KeywordManager.keywords.Add((IKeyword)t.GetConstructors()[0].Invoke(new object[0]));
                    Logger.WriteLine($"registered alisonscript keyword: {t.Name}");
                }
            }
            Logger.WriteLine("finished registering alisonscript keywords");
        }

        public static void Run(string script, params string[] args)
        {
            interpreterRunning = true;

            // get the full file path of the script so that we can read the lines of it
            string fullScriptPath = Application.streamingAssetsPath + "/alisonscript/" + script + ".alisonscript";
            
            // error handling
            if (runningScript != null)
                throw new Exception("cannot run a new alisonscript script while the runningScript instance has a value (a script is already running)");
            if (!File.Exists(fullScriptPath))
                throw new Exception($"couldn't find alisonscript file \"{script}\"");

            // read lines of file
            string[] file = File.ReadAllLines(fullScriptPath);

            // create running script
            runningScript = new RunningScript(Line.FromStringArray(file));

            // add args to running script as objects
            for (int i = 0; i < args.Length; i++)
                runningScript.AddObject($"@in_args{i}", args[i]);

            // evaluate the first line
            EvaluateLine(runningScript.lines[0]);
        }

        public static void EvaluateLine(Line line)
        {
            if (runningScript.finished)
            {
                DisposeRunningScript();
                return;
            }

            // return if the line is empty, a comment or if we can't match the required depth
            if (string.IsNullOrEmpty(line) || string.IsNullOrWhiteSpace(line) || line.contents.StartsWith("#") 
                || depth < line.requiredDepth)
            {
                EvaluateNextLine();
                return;
            }
            // set the depth to the line's required depth if the line's required depth is less then our depth
            if (line.requiredDepth < depth) depth = line.requiredDepth;

            // try to get a keyword at this line
            if (KeywordManager.TryGetKeyword(line, out IKeyword keyword))
            {
                // get the args
                string[] args = ArgsRegex(line);
                // error handling
                if (args.Length < keyword.minimumArgs)
                    throw new AlisonscriptSyntaxError(runningScript.GetCurrentLine(), 
                        $"insufficient args required to evaluate keyword \"{keyword.name}\" (given {args.Length}, expected {keyword.minimumArgs})");
                // if we get one, call the keyword's OnCall method
                keyword.OnCall(runningScript, line, args);
            }
            // if it's invalid, that's a syntax error
            else
            {
                throw new AlisonscriptSyntaxError(runningScript.GetCurrentLine(), $"no such keyword at line: \"{line.contents}\"");
            }
        }

        // evaluate the next line in a currently running script
        public static void EvaluateNextLine()
        {
            if (runningScript is null)
                return;
            runningScript.lineIndex++;
            EvaluateLine(runningScript.lines[runningScript.lineIndex]);
        }

        public static void DisposeRunningScript()
        {
            runningScript = null;
        }
    }
}
