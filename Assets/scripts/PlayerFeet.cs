using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace spacegame 
{ 
    public class PlayerFeet : MonoBehaviour
    {
        [HideInInspector] public bool onGround;

        private bool IsGround(Collider2D collision)
            => collision.CompareTag("ground");

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (IsGround(collision))
                onGround = true;
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (IsGround(collision))
                onGround = false;
        }
    }
}
