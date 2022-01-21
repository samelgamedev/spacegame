using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace spacegame
{
    // REMEMBER THIS IS ATTACHED TO THE CANVAS PREFAB
    public class UIManager : MonoBehaviour
    {
        public static UIManager instance;
        public Stack<UI> inputQueue = new Stack<UI>(); // a stack is a LAST IN FIRST OUT queue
        public bool canProcessInputQueue = true;

        public GameObject canvas;

        private void Awake()
        {
            instance = this;
            canvas = gameObject;

            if (Global.debugMode)
                // create debug ui
                Instantiate(PrefabManager.instance.GetPrefab("debug ui"), instance.transform);

            // hook input manager events to process input queue
            InputManager.instance.AddEvent(Constants.Input.VERTICAL_KEY_DOWN, ProcessInputQueue);
            InputManager.instance.AddEvent(Constants.Input.SELECT_KEY_DOWN, ProcessInputQueue);
        }

        public void ProcessInputQueue(InputManager.KeyPressedEventArgs e)
        {
            // only continue if the queue isn't empty or we can process the input queue
            if (inputQueue.Count == 0 || !canProcessInputQueue) return;

            // get the first object
            UI ui = inputQueue.Peek();

            if (ui is UINavigateable && (e.key == InputManager.up || e.key == InputManager.down))
            {
                (ui as UINavigateable).Navigate(e.key == InputManager.up);
            }
            else if (e.key == InputManager.select)
            {
                ui.inputProcessedCallback.Invoke(); // invoke the callback
            }
        }

        public UI[] skadoobly;
        private void Update()
        {
            skadoobly = inputQueue.ToArray();
            if (inputQueue.Count > 0 && inputQueue.Peek().readyToDequeue)
            {
                inputQueue.Pop(); // pop
            }
        }

        public UI New(Vector2 position, Vector2 size) 
        {
            // get prefab from PrefabManager and instantiate it as a child of the canvas
            GameObject prefab = PrefabManager.instance.GetPrefab("ui");
            GameObject canvas = this.canvas;
            GameObject g = Instantiate(prefab, position + (Vector2)canvas.transform.position, Quaternion.identity, canvas.transform);

            // get ui from gameobject and call the initialize method
            UI ui = g.GetComponent<UI>();
            ui.Initialize(size);
            return ui; // return the ui
        }

        public UINavigateable NewNavigateable(Vector2 position, Vector2 size, 
            UI.PrintTextOptions customPrintTextOptions = UI.PrintTextOptions.None)
        {
            // get prefab from PrefabManager and instantiate it as a child of the canvas
            GameObject prefab = PrefabManager.instance.GetPrefab("ui navigateable");
            GameObject canvas = this.canvas;
            GameObject g = Instantiate(prefab, position + (Vector2)canvas.transform.position, Quaternion.identity, canvas.transform);

            // initialize
            UINavigateable ui = g.GetComponent<UINavigateable>();
            ui.customPrintTextOptions = customPrintTextOptions;
            ui.Initialize(size);

            return ui; // return the ui
        }
    }
}