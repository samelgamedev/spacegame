using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace spacegame.alisonscript
{
    public static class KeywordManager
    {
        public static List<IKeyword> keywords = new List<IKeyword>();

        public static bool TryGetKeyword(Line line, out IKeyword result)
        {
            foreach (IKeyword keyword in keywords)
            {
                if (new Regex($"^{keyword.name}").IsMatch(line.contents))
                {
                    result = keyword;
                    return true;
                }
            }
            result = null;
            return false;
        }
    }
}
