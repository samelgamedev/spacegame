using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using UnityEngine;

namespace spacegame.alisonscript
{
    public class RunningScript 
    {
        public readonly List<Line> lines;
        public Dictionary<string, AlisonscriptObject<object>> objects = new Dictionary<string, AlisonscriptObject<object>>();
        // keywords that currently "encapsulate" the script, such as cond
        // the encapsulation stack can be popped using the end keyword
        public Stack<IEncapsulateableKeyword> encapsulationStack = new Stack<IEncapsulateableKeyword>();
        
        private int _lineIndex;
        public int lineIndex
        {
            get
            {
                return _lineIndex;
            }
            set
            {
                if (value > lines.Count - 1)
                {
                    // done!
                    Finished();
                    return;
                }

                _lineIndex = value;
            }
        }
        public int depth;

        public bool finished;

        public RunningScript(List<Line> lines)
        {
            this.lines = lines;
        }

        // NEVER use this to get the actual line index, instead just get lineIndex
        // this is just a tidy way of getting the current line for syntax errors
        // e.g. throw new AlisonscriptSyntaxError(runningScript.GetCurrentLine(), "cometh hithereth, don the dunce hat");
        public int GetCurrentLine()
        {
            return lineIndex + 1;
        }

        public void Finished()
        {
            finished = true;
            Player.instance.canMove = true;
            Interpreter.interpreterRunning = false;
        }
        
        public Line GetLabelByName(string name)
        {
            IEnumerable<Line> labels = from line in lines where line.labelData.isLabel select line;
            
            foreach (Line line in labels)
                if (line.labelData.labelName == name)
                    return line;
            throw new Exception($"no such label with the name \"{name}\"");
        }

        public AlisonscriptObject<object> GetObject(string name)
        {
            if (!objects.ContainsKey(name))
                return null;
            return objects[name];
        }

        public bool TryGetObject(string name, out AlisonscriptObject<object> obj)
        {
            if (!objects.ContainsKey(name))
            {
                obj = null;
                return false;
            }
            obj = objects[name];
            return true;
        }

        public void AddObject(string objectName, object objectValue)
        {
            Logger.WriteLine($"adding object: {objectName} with value: {objectValue}");
            if (Interpreter.runningScript.objects.ContainsKey(objectName))
                // if the script already has an object with the name objectName, set its value
                Interpreter.runningScript.objects[objectName].value = objectValue;
            else
                // otherwise, create it and add it
                Interpreter.runningScript.objects.Add(objectName, new AlisonscriptObject<object>(objectValue));
        }
    }
}
