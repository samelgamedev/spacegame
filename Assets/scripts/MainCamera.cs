using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace spacegame
{
    public class MainCamera : MonoBehaviour
    {
        public GameObject player;
        public Transform altMapMask;
        public bool followPlayer = true;

        public static MainCamera instance;

        // Start is called before the first frame update
        void Awake()
        {
            instance = this;
            if (player == null) player = GameObject.Find("alison");
            altMapMask = transform.Find("alt mask");
        }

        // Update is called once per frame
        void Update()
        {
            if (followPlayer && player != null) transform.position = player.transform.position - new Vector3(0, 0, 10);
        }
    }
}
