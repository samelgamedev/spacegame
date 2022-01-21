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
            InputManager.instance.AddEvent(Constants.Input.VERTICAL_KEY_HELD, Player.instance.UpdateParallaxesY);

            // allow leaving the ladder
            InputManager.fixedHorizontalKeyHeld += LeaveLadder;

            // set the player's gravity to 0 so they can move up the ladder
            Player.instance.SetGravity(0);

            // if the player interacts with the ladder while falling and their velocity isn't reset, their falling velocity will stay despite
            // their gravity being 0, so they will ascend the ladder slower
            Player.instance.rigidbody2d.velocity = Vector2.zero;
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
            InputManager.instance.RemoveEvent(Constants.Input.VERTICAL_KEY_HELD, Player.instance.UpdateParallaxesY);

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
