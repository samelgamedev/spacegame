using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace spacegame
{
    public class AppearCondition : MonoBehaviour
    {
        // these are both assigned in the inspector
        public string gameStateBool; // the game state bool to compare the condition to
        public bool condition; // the condition the game state bool must evaluate to for the game object to be active

        void Start()
        {
            gameObject.SetActive(GameState.GetBoolean(gameStateBool) == condition);
        }
    }
}