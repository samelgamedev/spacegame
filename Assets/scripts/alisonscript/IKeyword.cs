using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace spacegame.alisonscript
{
    public interface IKeyword
    {
        // the name of the keyword as declared in an alisonscript script (eg: function call is ;, conditional is cond)
        string name { get; }
        // the minimum args required to evaluate the line containing this keyword
        int minimumArgs { get; }
        // called when the interpreter arrives at this keyword
        void OnCall(RunningScript script, Line line, string[] args);
    }
}
