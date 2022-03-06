using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace spacegame
{
    public class NPC : Interactable
    {
        public Animator anim;
        public Rigidbody2D rigidbody2d;

        public string idleAnimation = "idle";
        public string walkAnimation = "walk";

        private bool facingRight = true;

        public virtual void Awake()
        {
            rigidbody2d = GetComponent<Rigidbody2D>();
            anim = GetComponent<Animator>();
            anim.Play(idleAnimation);
        }

        public void PlayAnimation(string stateName) => anim.Play(stateName);

        public void MoveTo(Vector2 position, float maxDistanceDelta = 5, bool animate = true)
        {
            StopAllCoroutines();
            StartCoroutine(InternalMoveTo(position, maxDistanceDelta, animate));
        }

        private IEnumerator InternalMoveTo(Vector2 position, float maxDistanceDelta, bool animate)
        {
            // flip if the npc isn't facing the position 
            if ((position.x > transform.position.x && !facingRight) 
                || (position.x < transform.position.x && facingRight))
                Flip();

            if (animate) PlayAnimation(walkAnimation); // play walk animation
            // move toward position
            while ((Vector2)transform.position != position)
            {
                transform.position = Vector3.MoveTowards(transform.position, position, maxDistanceDelta * Time.deltaTime);
                yield return new WaitForEndOfFrame();
            }
            if (animate) PlayAnimation(idleAnimation); // play idle animation
        }

        private void Flip()
        {
            transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y);
            facingRight = !facingRight;
        }

        public Follower BecomeFollower()
        {
            // destroy the npc component and re-add the new follower
            Destroy(GetComponent<NPC>());
            Follower f = gameObject.AddComponent<Follower>();
            f.anim = anim;
            f.idleAnimation = idleAnimation;
            f.walkAnimation = walkAnimation;
            f.facingRight = facingRight;

            return f;
        }
    }
}
