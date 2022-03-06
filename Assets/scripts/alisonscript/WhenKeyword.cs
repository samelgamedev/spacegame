using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace spacegame.alisonscript
{
    public sealed class WhenKeyword : IKeyword
    {
        public string name => "when";
        public int minimumArgs => 1;

        public void OnCall(RunningScript script, Line line, string[] args)
        {
            // error handling
            if (script.encapsulationStack.Count == 0)
                throw new AlisonscriptSyntaxError(script.GetCurrentLine(), "stray when keyword");
            // try to cast the top encapsulated keyword to a conditional keyword and throw a syntax error if we can't
            ConditionalKeyword cond = script.encapsulationStack.Peek() as ConditionalKeyword
                ?? throw new AlisonscriptSyntaxError(script.GetCurrentLine(), "stray when keyword");

            // if the object in the cond statement's value is equal to the first argument, increment the script's depth
            if (cond.Compare(args[0]))
                script.depth++;
            // either way, go ahead and evaluate the next line
            Interpreter.EvaluateNextLine();
        }
    }
}
