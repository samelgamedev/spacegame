using System;
//using System.Speech.Synthesis;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace spacegame
{
    public static class ReadToMeManager
    {
        /*
        private static SpeechSynthesizer speech;
        private static bool enabled = true; // this is just temporary and it'll be moved elsewhere when the settings menu is added

        public static void Init()
        {
            // set speech initialize the speech synthesizer and set the output to the os's default audio device
            speech = new SpeechSynthesizer();
            speech.SetOutputToDefaultAudioDevice();
        }

        public static Prompt Say(string thingToSay)
        {
            if (!enabled)
                return null;
            if (speech is null)
                throw new Exception("speech synthesizer not initialized!");

            Prompt say = new Prompt(thingToSay);
            speech.SpeakAsync(say);
            return say;
        }

        public static Prompt SayReadable(IReadable readable)
        {
            return Say(readable.ReadSelf());
        }

        public static Prompt GetCurrentlySaying()
        {
            return speech.GetCurrentlySpokenPrompt();
        }

        public static void CancelPrompt(Prompt prompt)
        {
            if (prompt is null || prompt.IsCompleted) return;
            speech.SpeakAsyncCancel(prompt);
        }

        public static void CancelAll()
        {
            speech.SpeakAsyncCancelAll();
        }
        */
    }
}