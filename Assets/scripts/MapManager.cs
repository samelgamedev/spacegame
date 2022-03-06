using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace spacegame
{
    public static class MapManager
    {
        // this is used to prevent changing to a map while there is already a map change in progress
        public static bool changingMap { get; private set; }

        public static void ChangeMap(int id, int transferPoint = 0)
            => ChangeMap(SceneManager.GetSceneByBuildIndex(id).name, transferPoint);

        public static void ChangeMap(string name, int transferPoint = 0)
        {
            if (changingMap) return;
            changingMap = true;

            Logger.WriteLine($"changing map: {name}, transferPoint {transferPoint}");

            Player.instance?.ToggleMovementHooks(false);
            InputManager.instance.RemoveEvent(Constants.Input.VERTICAL_KEY_DOWN, UIManager.instance.ProcessInputQueue);
            InputManager.instance.RemoveEvent(Constants.Input.SELECT_KEY_DOWN, UIManager.instance.ProcessInputQueue);

            // load the scene
            AsyncOperation a = SceneManager.LoadSceneAsync(name);
            a.completed += new Action<AsyncOperation>((x) => 
            {
                // call map data init as callback
                MapData.map.Init(transferPoint);
                // also make sure the player's followers go back to the player's position
                if (Player.instance != null)
                    foreach (Follower f in Player.followers)
                        f.transform.position = Player.instance.transform.position;
                // and we aren't changing map anymore, so this can go back to being false
                changingMap = false;
            });

            // movement hooks are re-added in Controller.Awake
        }
    }
}
