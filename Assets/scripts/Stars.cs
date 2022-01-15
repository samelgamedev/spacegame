using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace spacegame
{
    public class Stars : MonoBehaviour
    {
        private GameObject star;

        private void Awake()
        {
            star = PrefabManager.instance.GetPrefab("star");

            // create like 10 stars on awake without fading in
            for (int i = 0; i < 10; i++)
                CreateStar(false);

            StartCoroutine(StarsLoop());
        }

        private IEnumerator StarsLoop()
        {
            while (true)
            {
                // wait between 0.5 and 1 seconds before creating the next star
                yield return new WaitForSeconds(UnityEngine.Random.Range(0.5f, 1));

                // create 1-4 stars and fade in
                for (int i = 0; i < UnityEngine.Random.Range(1, 5); i++)
                    CreateStar(true);
            }
        }

        private void CreateStar(bool fadeIn)
        {
            // pick random position (relative to the camera)
            Vector2 position = new Vector2(
                UnityEngine.Random.Range(-5.2f, 5.3f),
                UnityEngine.Random.Range(-4f, 4f));
            // sizes
            Vector3[] sizes = new Vector3[]
            {
                new Vector3(1, 1, 1),
                new Vector3(0.5f, 0.5f, 1),
                new Vector3(0.25f, 0.25f, 1),
            };

            // instantiate
            GameObject g = Instantiate(star, position + (Vector2)MainCamera.instance.transform.position, Quaternion.identity, MainCamera.instance.transform);
            g.transform.localScale = sizes[UnityEngine.Random.Range(0, sizes.Length - 1)];

            // fade in/out the star
            // TODO: probably not so good to be using GetComponent so often
            StartCoroutine(Fade(g.GetComponent<SpriteRenderer>(), fadeIn));
        }

        private IEnumerator Fade(SpriteRenderer renderer, bool fadeIn)
        {
            Color fade = new Color(0, 0, 0, 0.1f);

            // start by fading in
            if (fadeIn)
            {
                for (int i = 0; i < 10; i++)
                {
                    renderer.color += fade;
                    yield return new WaitForSeconds(0.1f * Time.deltaTime);
                }
            }
            else
            {
                renderer.color = new Color(1, 1, 1, 1);
            }

            // wait between 0.5 and 5 seconds before fading out
            yield return new WaitForSeconds(UnityEngine.Random.Range(1, 7));

            // fade out
            for (int i = 0; i < 10; i++)
            {
                renderer.color -= fade;
                yield return new WaitForSeconds(0.1f * Time.deltaTime);
            }

            // destroy the game object
            Destroy(renderer.gameObject);
        }
    }
}
