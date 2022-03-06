using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace spacegame.alisonscript
{
    public struct LabelData
    {
        public static readonly LabelData noLabel = new LabelData(false, string.Empty);
        public bool isLabel;
        public string labelName;

        public LabelData(bool isLabel, string labelName)
        {
            this.isLabel = isLabel;
            this.labelName = labelName;
        }
    }
}
