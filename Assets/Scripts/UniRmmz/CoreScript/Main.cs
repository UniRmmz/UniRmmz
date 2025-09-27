using System;
using UnityEngine;

namespace UniRmmz
{
    public class Main : MonoBehaviour
    {
        public void Start()
        {
            Rmmz.InitializeUniRmmz(() => Scene_Boot.Run());
        }
    }
}