using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;

namespace spacegame.alisonscript
{
    // dumpster fire class
    public class Line
    {
        // the contents of the line, not including indenting
        public string contents;
        // the full contents of the line, including indenting
        public string realContents;
        // the index of the line in the script
        public int index;
        // the script depth required to evaluate this line
        public int requiredDepth;
        // if this line is a label, this contains some basic data about it
        public LabelData labelData;

        public Line(string contents, int index)
        {
            // initialize the contents
            // "realContents" is the contents of the line, including the indenting
            // "contents" removes the indenting
            realContents = contents;
            this.contents = new Regex(@"\A\s*").Replace(realContents, string.Empty);

            // determine the required depth by first removing everything after and including a non-white space character
            // this string will contain *only* the indenting from the real contents
            string indenting = new Regex(@"(?=\S).*").Replace(realContents, string.Empty);
            // then get the number of tabs in the string with a lil baby regex, just a tiny guy, baby man, so small,
            requiredDepth = new Regex(@"\t").Matches(indenting).Count;

            this.index = index;

            // determine whether this line is a label
            // the syntax for a label is &(label name)
            // and you can go to a label with the ;goto function
            if (contents.StartsWith("&"))
                labelData = new LabelData(true, contents);
            else
                labelData = LabelData.noLabel;
        }

        public static List<Line> FromStringArray(string[] array)
        {
            if (array.Length == 0)
                throw new Exception("array is empty");
            List<Line> lines = new List<Line>();

            // initialize a line from every string in the array and add it to the list
            for (int i = 0; i < array.Length; i++)
                lines.Add(new Line(array[i], i));

            // convert the list to an array and return
            return lines;
        }

        public static implicit operator string(Line line)
        {
            return line.contents;
        }
    }
}
