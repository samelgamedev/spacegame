using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace spacegame.alisonscript
{
    public sealed class ConditionalKeyword : IKeyword, IEncapsulateableKeyword
    {
        public string name => "cond";
        public int minimumArgs => 1;

        public AlisonscriptObject<object> obj;

        // this method is called by WhenKeyword to determine whether to increment the depth
        public bool Compare(object value)
            => obj.value.ToString() == value.ToString();

        public void OnCall(RunningScript script, Line line, string[] args)
        {
            // try to get the object with the name specified by the first argument 
            // if we can, set it to obj so it can be used by WhenKeyword
            // if we can't, that's illegal
            if (!script.TryGetObject(args[0], out obj))
                throw new Exception($"no such object {args[0]} in the running script");
            // on we go onto the encapsulation stack
            Encapsulate();

            // increment the depth and evaluate the next line
            script.depth++;
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
