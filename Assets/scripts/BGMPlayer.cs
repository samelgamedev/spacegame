using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace spacegame
{
    public class BGMPlayer : MonoBehaviour
    {
        [HideInInspector] public AudioSource aud;
        public static BGMPlayer instance;
        public AudioClip playingClip
        {
            get
            {
                return aud.clip;
            }
        }

        // all bgm/sfx files are in Resources/ and are loaded in Init.Initialization
        public AudioClip[] bgm;

        // Start is called before the first frame update
        void Awake()
        {
            aud = GetComponent<AudioSource>();
            instance = this;
        }

        public void Play(string name)
        {
            foreach (AudioClip a in bgm)
            {
                if (a.name == name)
                {
                    aud.clip = a;
                    aud.Play();
                    return;
                }
            }
            throw new Exception($"no AudioClip called {name} in bgm array");
        }

        public void Stop()
        {
            aud.Stop();
            aud.clip = null;
        }
    }
}