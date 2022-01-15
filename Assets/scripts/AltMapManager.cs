using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace spacegame
{
    public class AltMapManager : MonoBehaviour
    {
        public static AltMapManager instance;
        public bool altMapEnabled;

        private bool doingAnimation;

        private void Awake()
        {
            instance = this;
        }

        public void Toggle()
        {
            if (doingAnimation) return;
            StartCoroutine(Animation(!altMapEnabled));
        }

        private IEnumerator Animation(bool enabled)
        {
            doingAnimation = true;
            if (enabled)
            {
                SFXPlayer.instance.Play("sfx_alt_activate");
                while (MainCamera.instance.altMapMask.localScale.y <= 20)
                {
                    MainCamera.instance.altMapMask.localScale += new Vector3(0, 10 * Time.deltaTime, 0);
                    yield return new WaitForEndOfFrame();
                }
            }
            else
            {
                SFXPlayer.instance.Play("sfx_alt_deactivate");
                while (MainCamera.instance.altMapMask.localScale.y >= 1)
                {
                    MainCamera.instance.altMapMask.localScale -= new Vector3(0, 10 * Time.deltaTime, 0);
                    yield return new WaitForEndOfFrame();
                }
            }
            doingAnimation = false;

            altMapEnabled = enabled;
        }
    }
}
