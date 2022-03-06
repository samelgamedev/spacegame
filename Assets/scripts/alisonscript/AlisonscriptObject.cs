using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace spacegame.alisonscript
{
    public class AlisonscriptObject<T>
    {
        public T value;

        public AlisonscriptObject(T value)
        {
            this.value = value;
        }

        public static implicit operator T(AlisonscriptObject<T> input)
        {
            return input.value;
        }
    }
}
