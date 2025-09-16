using UnityEngine;

namespace UniRmmz
{
    public class Main : MonoBehaviour
    {
        public void Start()
        {
            Rmmz.InitializeManager();
            Scene_Boot.Run();
        }
        
    }
}