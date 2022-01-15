using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace spacegame
{
    public class Ladder : Interactable
    {
        public override void OnInteract()
        {
            if (Player.instance.onLadder) return;
            Player.instance.onLadder = true;

            // allow vertical movement
            InputManager.fixedVerticalKeyHeld += Player.instance.VerticalMovement;
            InputManager.instance.AddEvent("verticalKeyHeld", Player.instance.UpdateParallaxesY);

            // allow leaving the ladder
            InputManager.fixedHorizontalKeyHeld += LeaveLadder;

            // set the player's gravity to 0 so they can move up the ladder
            Player.instance.SetGravity(0);
        }

        private void LeaveLadder(InputManager.KeyPressedEventArgs e)
            => LeaveLadder();

        private void LeaveLadder()
        { 
            if (!Player.instance.onLadder) return;
            Player.instance.onLadder = false;

            // disallow vertical movement and leaving the ladder
            InputManager.fixedHorizontalKeyHeld -= LeaveLadder;
            InputManager.fixedVerticalKeyHeld -= Player.instance.VerticalMovement;
            InputManager.instance.RemoveEvent("verticalKeyHeld", Player.instance.UpdateParallaxesY);

            // reset the gravity back to 1
            Player.instance.SetGravity(1);
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.gameObject.CompareTag("Player"))
                LeaveLadder();
        }
    }
}
