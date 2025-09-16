using System;
using System.Collections;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEditor.Actions;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UniRmmz
{
    public class RmmzRoot : MonoBehaviour
    {
        [SerializeField] public RmmzCanvas Canvas;
        [SerializeField] public RmmzCamera Camera;
        [SerializeField] public RmmzBitmapRenderer BitmapRenderer;
        [SerializeField] public GameObject AudioRoot;
        [SerializeField] public GameObject VideoRoot;
        [SerializeField] public RawImage VideoImage;
        [SerializeField] private RectTransform _videoImageParent;
        [SerializeField] public RmmzErrorPrinterView ErrorPrinterView;
        
        public bool IsGameLoopRunning { get; set; }

        public event Action<bool> OnFocus = (bool _) => { };
        public event Action OnUnload = () => { };
        public event Action OnTick = () => { };

        public static RmmzRoot Instance = null;

        public static void Initialize()
        {
            var prefab = Resources.Load<RmmzRoot>("Prefabs/RmmzRoot");
            RmmzRoot.Instance = GameObject.Instantiate(prefab);
            if (Application.isPlaying)
            {
                UnityEngine.Object.DontDestroyOnLoad(RmmzRoot.Instance);    
            }
        }

        public static void RunCoroutine(IEnumerator routine)
        {
            if (Application.isPlaying)
            {
                Instance.StartCoroutine(routine);
            }
            else
            {
                EditorCoroutineUtility.StartCoroutineOwnerless(routine);
            }
        }

        public void Update()
        {
            if (IsGameLoopRunning)
            {
                OnTick();    
            }
        }

        /// <summary>
        /// 動画再生用の表示を切り替える
        /// </summary>
        public void SetVideoActive(bool active)
        {
            _videoImageParent.gameObject.SetActive(active);
        }

        /// <summary>
        /// 動画再生UIのサイズを設定する
        /// </summary>
        public void SetVideoResolution(int width, int height)
        {
            _videoImageParent.sizeDelta = new Vector2(width, height);
        }

        public void OnApplicationFocus(bool focus)
        {
            OnFocus(focus);
        }
        
        public void OnApplicationQuit()
        {
            OnUnload.Invoke();
            OnUnload = () => { };
            OnFocus = (bool _) => { };
            OnTick = () => { };
            Instance = null;
        }
    }
}