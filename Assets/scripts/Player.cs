using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace spacegame
{
    // this is sealed to futureproof my code from my own clownery
    public sealed class Player : MonoBehaviour
    {
        // speed stuff
        public float speedMultiplier = 1; // run/walk speed is always multiplied by speedMultiplier
        private float walkSpeed => 5 * speedMultiplier;
        private float runSpeed => 8 * speedMultiplier;
        public float movementSpeed => Input.GetKey(InputManager.run) ? runSpeed : walkSpeed; // actual movement speed
        public bool holdingMovementKeys => Input.GetKey(InputManager.left) || Input.GetKey(InputManager.right); // are we schmoovin?

        // other movement
        public bool canMove = true;
        [HideInInspector] public bool facingRight = true;
        [HideInInspector] public bool onLadder;
        public bool onStairs;

        // followers
        public static List<Follower> followers = new List<Follower>();

        // interaction
        public Interactable interactable; // the interactable component of the game object the player is colliding, if that game object has one
        public bool canInteract
        {
            get
            {
                return interactable != null;
            }
        }
        [HideInInspector] public Transform canInteractObj; // the game object that becomes active when the player can interact with an object

        // components
        [HideInInspector] public Animator animator;
        [HideInInspector] public BoxCollider2D coll;
        [HideInInspector] public Rigidbody2D rigidbody2d;

        // singleton
        public static Player instance;

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Public Methods
        ///////////////////////////////////////////////////////////////////////////////////////////////

        public void ToggleMovementHooks(bool add)
        {
            if (add)
            {
                InputManager.instance.AddEvent(Constants.Input.HORIZONTAL_KEY_HELD, HorizontalMoveAnimation);
                InputManager.instance.AddEvent(Constants.Input.HORIZONTAL_KEY_HELD, UpdateParallaxesX);
                InputManager.instance.AddEvent(Constants.Input.HORIZONTAL_KEY_RELEASED, StopHorizontalAnimation);
                InputManager.instance.AddEvent(Constants.Input.SELECT_KEY_DOWN, ProcessInteraction);
                InputManager.fixedHorizontalKeyHeld += HorizontalMovement;
            }
            else
            {
                InputManager.instance.RemoveEvent(Constants.Input.HORIZONTAL_KEY_HELD, HorizontalMoveAnimation);
                InputManager.instance.RemoveEvent(Constants.Input.HORIZONTAL_KEY_HELD, UpdateParallaxesX);
                InputManager.instance.RemoveEvent(Constants.Input.HORIZONTAL_KEY_RELEASED, StopHorizontalAnimation);
                InputManager.instance.RemoveEvent(Constants.Input.SELECT_KEY_DOWN, ProcessInteraction);
                InputManager.fixedHorizontalKeyHeld -= HorizontalMovement;
            }
        }

        public void SetGravity(float gravity)
        {
            rigidbody2d.gravityScale = gravity;
        }

        public void StopHorizontalAnimation()
        {
            animator.ResetTrigger("walking");
            animator.SetTrigger("idle");

            // stop x velocity
            rigidbody2d.velocity = new Vector2(0, rigidbody2d.velocity.y);

            // stop the followers from walking
            foreach (Follower f in followers)
                f.StopWalking();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Public Methods (for events)
        ///////////////////////////////////////////////////////////////////////////////////////////////

        public void HorizontalMovement(InputManager.KeyPressedEventArgs e)
        {
            if (!canMove) return;
            UpdateFollowers();

            int horizontalVelocity = e.key == InputManager.left ? -1 : 1;
            rigidbody2d.velocity = new Vector2(horizontalVelocity * movementSpeed * Time.deltaTime * 50, rigidbody2d.velocity.y);
        }

        public void VerticalMovement(InputManager.KeyPressedEventArgs e)
        {
            if (!canMove) return;
            UpdateFollowers();

            int verticalVelocity = e.key == InputManager.down ? -1 : 1;
            transform.Translate(0, verticalVelocity * movementSpeed * Time.deltaTime, 0);
        }

        public void UpdateParallaxesX(InputManager.KeyPressedEventArgs e)
        {
            foreach (Parallax p in MapData.map.parallaxObjects)
            {
                p.UpdateX(this);
            }
        }

        public void UpdateParallaxesY(InputManager.KeyPressedEventArgs e)
        {
            foreach (Parallax p in MapData.map.parallaxObjects)
            {
                p.UpdateY(this);
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Private Methods
        ///////////////////////////////////////////////////////////////////////////////////////////////

        private void Flip()
        {
            if (!canMove) return;

            // flip sprite by inverting x value of scale
            facingRight = !facingRight;
            transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
        }

        private void UpdateFollowers()
        {
            // use i as a parameter to determine the offset from the last follower
            for (int i = 0; i < followers.Count; i++)
                followers[i].UpdatePosition(i);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Private Methods (for events)
        ///////////////////////////////////////////////////////////////////////////////////////////////

        private void ProcessInteraction(InputManager.KeyPressedEventArgs e)
        {
            if (!canMove || !canInteract || alisonscript.Interpreter.interpreterRunning) return;

            interactable.OnInteract();
        }

        private void HorizontalMoveAnimation(InputManager.KeyPressedEventArgs e)
        {
            if (!canMove) return;

            // walk animation
            animator.SetTrigger("walking");

            // flip
            if (e.key == InputManager.right && !facingRight)
                Flip();
            else if (e.key == InputManager.left && facingRight)
                Flip();
        }

        private void StopHorizontalAnimation(InputManager.KeyPressedEventArgs e)
        {
            StopHorizontalAnimation();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Unity
        ///////////////////////////////////////////////////////////////////////////////////////////////

        // Start is called before the first frame update
        void Awake()
        {
            animator = GetComponent<Animator>();
            coll = GetComponent<BoxCollider2D>();
            rigidbody2d = GetComponent<Rigidbody2D>();
            canInteractObj = transform.Find("interact");
            instance = this;

            // add movement hooks
            ToggleMovementHooks(true);
        }

        private void Update()
        {
            // are we falling/has unity's physics engine gone WOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOO
            if (rigidbody2d.velocity != Vector2.zero)
                // update the parallaxes y
                UpdateParallaxesY(new InputManager.KeyPressedEventArgs(KeyCode.None));

            // open in game menu
            if (Input.GetKeyDown(InputManager.menu) && canMove)
                // open menu
                // the menu manager closes the menu here if it's already open
                InGameMenuManager.Open();

            // alt map
            if (Input.GetKeyDown(KeyCode.Q))
                AltMapManager.instance?.Toggle(); // unless it's null
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if ((collision.CompareTag("interactable") || collision.CompareTag("npc"))
                && collision.TryGetComponent(out Interactable i) && i.doOnInteract != null)
            {
                if (!i.canInteract) return;

                if (i.onTouch)
                {
                    i.OnInteract();
                }
                else
                {
                    interactable = i;
                    canInteractObj.gameObject.SetActive(true);
                }
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            // compare the object we're leaving the trigger of, and if it's the interactable, set it to null
            if (interactable != null && collision.gameObject == interactable.gameObject)
            {
                interactable = null;
                canInteractObj.gameObject.SetActive(false);
            }
        }
    }
}