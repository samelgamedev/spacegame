﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

namespace spacegame
{
    // Project Settings > Script Execution Order for all of this mayhem
    public class Global : MonoBehaviour
    {
        public static Global instance;
        public static bool initialized;

        public static Dictionary<string, GameObject> commonObjects = new Dictionary<string, GameObject>();

        // Start is called before the first frame update
        void Awake()
        {
            instance = this;

            // initialize other stuff
            if (initialized) return;
            initialized = true;

            alisonscript.Interpreter.RegisterFunctions();

            alisonscript.Interpreter.Run("debug/test_condition");
        }

        public static GameObject GetCommonObject(string name)
        {
            foreach (string s in commonObjects.Keys)
                if (s == name)
                    return commonObjects[s];
            throw new System.Exception($"the common objects list does not have a key called {name}");
        }
    }
}
