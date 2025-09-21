using System;
using UnityEngine;

namespace UniRmmz
{
    public class Main : MonoBehaviour
    {
        public void Start()
        {
            Rmmz.InitializeManager();
            Rmmz.PluginManager.LoadAndSetup(() => Scene_Boot.Run());
        }
    }
}