using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace spacegame
{
    public class UI : MonoBehaviour, IReadable
    {
        public Text text;
        public Action inputProcessedCallback; // called when dequeued from the UIManager input queue

        public List<UI> alsoDestroy = new List<UI>(); // ui to destroy when destroying this one 

        [HideInInspector] public bool readyToDequeue;

        private string _readerAlt;
        public string readerAlt 
        {
            get => _readerAlt; 
            set => _readerAlt = value; 
        }

        public enum PrintTextOptions
        {
            // you get binary because i hate you that much <3
            None = 0b_0000_0000,
            CallbackBeforePrinting = 0b_0000_0001,
            CallbackAfterPrinting = 0b_0000_0010,
            CallbackAfterInput = 0b_0000_0100,
            DontCallback = 0b_0000_1000,
            DestroyUIAfterCallback = 0b_0001_0000,
            Instant = 0b_0010_0000,
            DontPushToInputQueue = 0b_0100_0000,
        }

        public virtual void Initialize(Vector2 size)
        {
            text = GetComponentInChildren<Text>();

            // set box scale
            RectTransform rect = GetComponent<RectTransform>();
            
            rect.sizeDelta = size;

            // set text position and scale
            Image image = GetComponent<Image>();
            RectTransform textRect = text.GetComponent<RectTransform>();
            textRect.localPosition = new Vector2(0, 0); // don't remember why this is set, try removing it later
        }

        public void SetTextAlignment(TextAnchor alignment)
        {
            text.alignment = alignment;
        }

        // TODO: tidy this up
        // also check out the stack overflow page i stole this code from: https://www.taste.com.au/recipes/collections/spaghetti-recipes
        public virtual IEnumerator PrintText(
            string text, // text input
            Action callback = null, // callback (in alisonscript this is usually just increment the line index)
            PrintTextOptions options = PrintTextOptions.CallbackAfterInput) // options
        {
            // disallow processing the input queue until the coroutine has finished
            UIManager.instance.canProcessInputQueue = false;

            // automatically skip through the text if holding left ctrl
            if (Input.GetKey(KeyCode.LeftControl))
            {
                this.text.text = text;
                yield return new WaitForEndOfFrame(); // i want it to have like. you know. that oomph. you know. the oomph
                if (callback != null)
                    callback.Invoke();
                DestroyGameObject();
                yield break;
            }

            if (options.HasFlag(PrintTextOptions.CallbackBeforePrinting))
                callback.Invoke();

            if (options.HasFlag(PrintTextOptions.Instant))
            {
                this.text.text = text;
            }
            else
            {
                // for all of the characters in the input string, append the character to the ui's text
                for (int i = 0; i < text.Length; i++)
                {
                    this.text.text += text[i];
                    yield return new WaitForSeconds(0.01f * Time.deltaTime);

                    // check for confirm or cancel key, and either is being pressed skip text
                    if (Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.X))
                    {
                        this.text.text = text;
                        break;
                    }
                }
            }

            yield return new WaitForSeconds(0.1f);

            // this is gross
            if (options.HasFlag(PrintTextOptions.CallbackAfterPrinting))
            {
                if (callback != null)
                    callback.Invoke();
                if (options.HasFlag(PrintTextOptions.DestroyUIAfterCallback)) // destroy this ui if configured to
                    DestroyGameObject();
            }
            if (options.HasFlag(PrintTextOptions.CallbackAfterInput))
            {
                // if the options has the destroy ui after callback flag, add destroy game object to the input processed callback
                if (options.HasFlag(PrintTextOptions.DestroyUIAfterCallback))
                    inputProcessedCallback += DestroyGameObject;
                // if a callback was given, add it to the input processed callback
                if (callback != null)
                    inputProcessedCallback += callback;

                /*
                // if a callback wasn't given and we're told to destroy the ui, destroy the ui
                if (callback is null && options.HasFlag(PrintTextOptions.DestroyUIAfterCallback))
                {
                    
                }
                // if a callback was given and we're told to destroy the ui, invoke the callback and destroy the ui
                else if (callback != null && options.HasFlag(PrintTextOptions.DestroyUIAfterCallback))
                {
                    inputProcessedCallback = new Action(() =>
                    {
                        callback.Invoke();
                        DestroyGameObject();
                    });
                }
                // calling it now this else is going to cause problems later
                else
                {
                    inputProcessedCallback = new Action(() =>
                    {
                        callback.Invoke();
                    });
                }*/

                // add to input queue
                if (!options.HasFlag(PrintTextOptions.DontPushToInputQueue))
                    AddToInputQueue();

                // allow processing the input queue
                UIManager.instance.canProcessInputQueue = true;
            }
        }

        public virtual string ReadSelf()
        {
            return readerAlt == string.Empty ? text.text : readerAlt;
        }

        public void AddToInputQueue()
        {
            if (!UIManager.instance.inputQueue.Contains(this))
                UIManager.instance.inputQueue.Push(this);
        }

        public void DestroyGameObject()
        {
            foreach (UI ui in alsoDestroy)
                ui?.DestroyGameObject();
            Destroy(gameObject);
        }

        private void OnDestroy()
        {
            readyToDequeue = true;
        }
    }
}
