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
        public List<Line> lines;
        public List<Label> labels = new List<Label>();
        public Dictionary<string, Object> objects = new Dictionary<string, Object>();

        public bool inCond;
        public string condObjectName;
        
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
                lines[_lineIndex].Process(this);
            }
        }

        public RunningScript(List<Line> lines)
        {
            this.lines = lines;

            // create labels
            foreach (
                Line line in from line in lines where line // why do you call it oven when you of in the cold food of out hot eat the food
                .contents.StartsWith("&") select line)
            {
                // use the same regex in Line to just get the name of the label without the args
                string labelName = new Regex("\".*?\"|\\s").Replace(line.contents, string.Empty);

                // then get the args 
                string[] args = Line.ArgsRegex(line);

                // then create the label and add it to the labels list
                labels.Add(new Label(labelName, line.index, args));
            }
        }

        public bool ConditionalTrue(string objectName, string value)
        {
            if (!objects.ContainsKey(objectName))
                throw new AlisonscriptSyntaxError(GetCurrentLine(), $"the current script does not have an object called {objectName}");

            // compare the object's value to the inputted value
            return objects[objectName].value == value;
        }

        // NEVER use this get the actual line index, instead just get lineIndex
        // this is just a tidy way of getting the current line for syntax errors
        public int GetCurrentLine()
        {
            return lineIndex + 1;
        }

        public void IncrementIndex()
        {
            Interpreter.runningScript.lineIndex++;
        }

        public void Finished()
        {
            Player.instance.canMove = true;
            Interpreter.DisposeRunningScript();
            Interpreter.interpreterRunning = false;
        }
        
        public Label GetLabelByName(int start, string name)
        {
            for (int i = start; i < labels.Count; i++)
                if (labels[i].name == name)
                    return labels[i];
            throw new Exception($"a label with the name \"{name}\" could not be found from index position {start}");
        }

        public void JumpToNextOccurence(int start, string input)
        {
            for (int i = start + 1; i < lines.Count; i++)
            {
                // remove indenting
                Regex indenting = new Regex(@"\A\s*");
                string line = indenting.Replace(lines[i], string.Empty);

                if (line.StartsWith(input)
                    || (input == "when" && line.StartsWith("end"))) // also jump to end if the input is when to break out of conditionals
                {
                    lineIndex = i;

                    if (Interpreter.runningScript == null) // how does this happen part 2
                    {
                        Logger.WriteLine("running script is null " + lineIndex);
                        return;
                    }

                    // break out of conditional if the line isn't in one
                    if (!Interpreter.runningScript.lines[lineIndex].inConditional)
                        Interpreter.runningScript.inCond = false;
                    return;
                }
            }
            throw new Exception($"couldn't find an occurence of {input} from {start}");
        }

        public void AddObject(string objectName, string objectValue)
        {
            Logger.WriteLine($"adding object: {objectName} with value: {objectValue}");
            if (Interpreter.runningScript.objects.ContainsKey(objectName))
                // if the script already has an object with the name objectName, set its value
                Interpreter.runningScript.objects[objectName].value = objectValue;
            else
                // otherwise, create it and add it
                Interpreter.runningScript.objects.Add(objectName, new Object(objectValue));
        }
    }
}
