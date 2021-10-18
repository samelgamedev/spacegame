﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace spacegame
{
    public class Global : MonoBehaviour
    {
        public static Global instance;

        // Start is called before the first frame update
        void Awake()
        {
            alisonscript.Interpreter.RegisterFunctions();
            alisonscript.Interpreter.Run("debug_test");
            instance = this;
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
