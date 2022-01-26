using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace spacegame
{
    // interface for ui elements that should be read by the read to me mode
    // the actual antics for telling the ReadToMeManager to read said ui elements still needs to be done manually,
    // but the purpose of this is more to expose methods to make the process easier
    public interface IReadable
    {
        // a sort-of alt text that's interpreted by ReadSelf
        string readerAlt { get; set; }
        // the string to return when ReadToMeManager.SayReadable is called
        string ReadSelf();
    }
}
