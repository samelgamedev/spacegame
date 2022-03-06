using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace spacegame
{
    public class Follower : NPC
    {
        private BoxCollider2D coll;
        public override void Awake()
        {
            base.Awake();
            coll = GetComponent<BoxCollider2D>();
            transform.SetParent(null);
            DontDestroyOnLoad(gameObject);
        }

        public void UpdatePosition(int index)
        {
            bool right = Player.instance.facingRight;
            Vector2 position = new Vector2(Player.instance.onLadder 
                ? Player.instance.transform.position.x : (right ? Player.instance.transform.position.x - index - 1
                : Player.instance.transform.position.x + index + 1), transform.position.y);
            // only update the y position here if the player isn't on stairs
            // because we'll update the follower's position manually inside Stairs.cs
            if (!Player.instance.onStairs)
                position = new Vector2(position.x, Player.instance.onLadder ?
                    Player.instance.transform.position.y - index - 1 : Player.instance.transform.position.y);

            PlayAnimation(walkAnimation);
            MoveTo(position, 
                // subtract the y of the player's rigidbody2d to account for gravity
                // also, go very fast on stairs
                Player.instance.onStairs ? 10 : (Player.instance.movementSpeed - Player.instance.rigidbody2d.velocity.y), 
                // we'll play and stop the walk animation ourselves
                false);
        }

        public void StopWalking() => PlayAnimation(idleAnimation);

        public static Follower FollowerFromNPC(NPC npc)
        {
            return npc.BecomeFollower();
        }
    }
}
