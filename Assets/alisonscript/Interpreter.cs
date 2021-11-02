﻿// THIS IS HORRIBLE AND NEEDS TO BE REFACTORED
// BUT I DON'T FEEL LIKE IT! :D
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;
using UnityEditor;
using System.IO;
using System.Linq;

namespace spacegame.alisonscript
{
    public static class Interpreter
    {
        private static RunningScript _runningScript;
        public static RunningScript runningScript
        {
            get
            {
                return _runningScript;
            }
            set
            {
                if (_runningScript != null && value != null)
                    throw new Exception("shouldn't set the running script to a value while a script is running");
                _runningScript = value;
            }
        }

        // these are objects that are alive as long as the interpreter is
        public static Dictionary<string, Object> globalObjects = new Dictionary<string, Object>();

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
                    Debug.Log($"registered alisonscript function: {functionName} ({m.Name})");
                }
            }

            Debug.Log("finished registering functions");
        }

        public static void Run(string script)
        {
            if (runningScript != null)
                throw new Exception("cannot run a new alisonscript script while the runningScript instance has a value (a script is already running)");

            if (!File.Exists(Application.dataPath + "/lang/en/" + script + ".alisonscript"))
                throw new Exception($"couldn't find alisonscript file \"/lang/en/{script}\"");

            Controller.instance.canMove = false;

            // read lines of file
            string[] file = File.ReadAllLines(Application.streamingAssetsPath + "/lang/en/" + script + ".alisonscript");

            // create running script
            runningScript = new RunningScript(Line.FromStringArray(file));

            // process first line
            runningScript.lines[0].Process(runningScript); 
        }
    }
}
