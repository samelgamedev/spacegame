using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace spacegame
{
    public class Interactable : MonoBehaviour
    {
        public Event doOnInteract;
        public bool onTouch;
        public bool destroyAfter;

        public virtual void OnInteract()
        {
            if (destroyAfter) Destroy(gameObject);
            EventManager.ProcessEvent(doOnInteract);
        }

        private void OnDestroy()
        {
            if (MapManager.changingMap) return;

            if (Player.instance.interactable == this)
            {
                Player.instance.interactable = null;
                Player.instance.canInteractObj.gameObject.SetActive(false);
            }
        }
    }
}
