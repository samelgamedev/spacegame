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
        public static void ChangeMap(int id, int transferPoint = 0)
            => ChangeMap(SceneManager.GetSceneByBuildIndex(id).name, transferPoint);

        public static void ChangeMap(string name, int transferPoint = 0)
        {
            Logger.WriteLine($"changing map: {name}, transferPoint {transferPoint}");

            Player.instance?.ToggleMovementHooks(false);
            InputManager.instance.RemoveEvent(Constants.Input.VERTICAL_KEY_DOWN, UIManager.instance.ProcessInputQueue);
            InputManager.instance.RemoveEvent(Constants.Input.SELECT_KEY_DOWN, UIManager.instance.ProcessInputQueue);

            // load the scene
            AsyncOperation a = SceneManager.LoadSceneAsync(name);
            a.completed += new Action<AsyncOperation>(
                // call map data init as callback
                x => MapData.map.Init(transferPoint));

            // movement hooks are re-added in Controller.Awake
        }
    }
}
