using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace spacegame
{
    public class SpaceshipDoor : MonoBehaviour
    {
        [SerializeField] private bool opening;
        [SerializeField] private bool closing;

        private BoxCollider2D trigger;

        private void Start()
        {
            trigger = GetComponent<BoxCollider2D>();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (closing || opening) return;

            if (collision.CompareTag("Player"))
            {
                StartCoroutine(ToggleDoor(true));
            }
        }

        private IEnumerator ToggleDoor(bool open)
        {
            SFXPlayer.instance.Play("sfx_spaceship_door_open");

            if (open)
            {
                opening = true;
                transform.localScale = new Vector3(1, 1, 1); // reset local scale
                while (transform.localScale.y >= 0)
                {
                    transform.localScale -= new Vector3(0, 0.05f);
                    yield return new WaitForSeconds(0.001f * Time.deltaTime);
                }
                opening = false;

                // check that the player is still in the collider, which they probably aren't, and if they aren't close it
                if (!trigger.IsTouching(Player.instance.coll))
                {
                    yield return new WaitForSeconds(0.5f);
                    StartCoroutine(ToggleDoor(false));
                }
            }
            else
            {
                closing = true;
                transform.localScale = new Vector3(1, 0, 1); // reset local scale y back to 0 (do not know why this needs to happen)
                while (transform.localScale.y <= 1)
                {
                    transform.localScale += new Vector3(0, 0.05f);
                    yield return new WaitForSeconds(0.001f * Time.deltaTime);
                }
                closing = false;

                // check that the player is still in the collider, and if they are reopen it
                if (trigger.IsTouching(Player.instance.coll))
                {
                    yield return new WaitForSeconds(0.5f);
                    StartCoroutine(ToggleDoor(true));
                }
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (closing || opening) return;

            if (collision.CompareTag("Player"))
            {
                StartCoroutine(ToggleDoor(false));
            }
        }
    }

}
