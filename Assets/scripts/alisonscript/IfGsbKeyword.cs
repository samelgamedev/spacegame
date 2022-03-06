using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace spacegame.alisonscript
{
    public sealed class IfGsbKeyword : IKeyword, IEncapsulateableKeyword
    {
        public string name => "ifgsb";
        public int minimumArgs => 1;

        public void OnCall(RunningScript script, Line line, string[] args)
        {
            // if a second argument is given to ifgsb, parse the argument to a bool and treat that as the required condition
            // that GetGameStateBoolean needs to return
            bool condition = true;
            if (args.Length > 1 && !bool.TryParse(args[1], out condition))
                throw new Exception($"failed to parse {args[1]} to a boolean");

            if (GameState.GetBoolean(args[0]) == condition)
                script.depth++;
            Encapsulate();
            Interpreter.EvaluateNextLine();
        }

        public void Encapsulate()
        {
            Interpreter.runningScript.encapsulationStack.Push(this);
        }

        public void EndBehaviour()
        {
            Interpreter.runningScript.encapsulationStack.Pop();
        }
    }
}
