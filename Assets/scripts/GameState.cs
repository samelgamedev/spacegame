﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace spacegame
{
    public static class GameState
    {
        private static Dictionary<string, bool> booleans = new Dictionary<string, bool>();
        private static Dictionary<string, int> integers = new Dictionary<string, int>();

        public static bool GetBoolean(string name)
        {
            // if the booleans dictionary has a value with the key name, return it
            if (booleans.TryGetValue(name, out bool result))
                return result;
            // otherwise, create it and return false
            SetBoolean(name, false);
            return false;
        }

        public static void SetBoolean(string name, bool value)
        {
            // if the booleans dictionary has a key with name, set the value to the input
            if (booleans.ContainsKey(name))
                booleans[name] = value;
            else
                // otherwise, add it
                booleans.Add(name, value);
        }

        public static int GetInteger(string name)
        {
            // if the integers dictionary has a value with the key name, return it
            if (integers.TryGetValue(name, out int result))
                return result;
            // otherwise, create it and return 0
            SetInteger(name, 0);
            return 0;
        }
        
        public static void SetInteger(string name, int value)
        {
            // if the integers dictionary has a key with name, set the value to the input
            if (integers.ContainsKey(name))
                integers[name] = value;
            else
                // otherwise, add it
                integers.Add(name, value);
        }
    }
}