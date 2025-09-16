using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace UniRmmz
{
    /// <summary>
    /// RPG Maker MZスタイルのレンダリングシステムを実現するカメラコンポーネント
    /// フィルター効果を持つUI要素を適切に描画する
    /// </summary>
    [DefaultExecutionOrder(999)]
    public class RmmzCamera : MonoBehaviour
    {
        [SerializeField] private Camera sceneCamera = null;
        [SerializeField] private Camera effekseerCamera = null;
        [SerializeField] private Canvas sceneCanvas;

        private List<RenderTexture> _renderTextureCaches = new();
        private Material _blitMaterial;

        /// <summary>
        /// レンダリングパス情報を保持する内部クラス
        /// </summary>
        public class RenderingPass
        {
            public Transform Root;
            public string RootObjectName;
            public List<RenderingPass> ChildPasses = new();
            public List<IEffekseerContainer> EffekseerPasses = new();
            public RenderTexture RenderTexture;
            public bool IsTop;
            public Rect BoundingBox;
        }

        protected void Awake()
        {
            _blitMaterial = new Material(Shader.Find("UniRmmz/Blit"));
        }

        protected void OnDestroy()
        {
            GameObject.Destroy(_blitMaterial);
        }

        private void LateUpdate()
        {
            if (!Rmmz.SceneManager.CanRenderScene() || !RmmzRoot.Instance.IsGameLoopRunning)
            {
                return;
            }

            Render();    
        }

        public void Render()
        {
            // 毎フレームキャッシュをクリア
            _renderTextureCaches.Clear();

            if (sceneCanvas == null || sceneCamera == null)
                return;

            // すべてのキャンバスを一旦非表示レイヤーに設定
            SetAllCanvasesToHiddenLayer();

            // レンダリングパスを準備
            RenderingPass rootPass;
            {
                rootPass = new RenderingPass
                {
                    Root = sceneCanvas.transform,
                    RootObjectName = sceneCanvas.gameObject.name,
                    ChildPasses = new List<RenderingPass>(),
                    EffekseerPasses = new List<IEffekseerContainer>(),
                    IsTop = true,
                    RenderTexture = null,
                    BoundingBox = new Rect(0, 0, sceneCamera.pixelWidth, sceneCamera.pixelHeight)
                };

                // 深さ優先で子階層を探索
                ExploreHierarchy(sceneCanvas.transform, rootPass.ChildPasses, rootPass.EffekseerPasses);
            }
            
            if (rootPass != null)
            {
                // パスに従ってレンダリングを実行
                RenderPass(rootPass);
            }

            // 使用したレンダーテクスチャを解放
            ReleaseRenderTextures();
        }

        /// <summary>
        /// すべてのキャンバスを非表示レイヤーに設定
        /// </summary>
        private void SetAllCanvasesToHiddenLayer()
        {
            var allCanvases = sceneCanvas.transform.GetComponentsInChildren<Canvas>(includeInactive: true);
            foreach (var canvas in allCanvases)
            {
                canvas.gameObject.layer = Rmmz.RmmzDontRenderLayer;
            }
        }

        /// <summary>
        /// レンダーテクスチャのリソースを解放
        /// </summary>
        private void ReleaseRenderTextures()
        {
            foreach (var renderTexture in _renderTextureCaches)
            {
                RenderTexture.ReleaseTemporary(renderTexture);
            }

            _renderTextureCaches.Clear();
        }

        /// <summary>
        /// 階層構造からレンダリングパスを準備する
        /// </summary>
        /// <param name="root">ルートとなる RectTransform</param>
        /// <param name="isTop">最上位パスかどうか</param>
        /// <returns>レンダリングパス</returns>
        private RenderingPass PrepareRenderPass(Canvas topCanvas, IRmmzDrawable2d drawable, bool isTop = false)
        {
            Rect boundingBox = new Rect(0, 0, Graphics.Width, Graphics.Height);//drawable.ScreenRect(topCanvas);

            /*
             if (boundingBox.width == 0 || boundingBox.height == 0)
            {
                return null;
            }
            
            float sx = boundingBox.x;
            float sy = boundingBox.y;
            float ex = boundingBox.x + boundingBox.width;
            float ey = boundingBox.y + boundingBox.height;
            boundingBox.x = Mathf.Max(sx, 0);
            boundingBox.y = Mathf.Max(sy, 0);
            boundingBox.width = Mathf.Min(ex, Graphics.Width) - boundingBox.x;
            boundingBox.height = Mathf.Min(ey, Graphics.Height) - boundingBox.y;
            boundingBox.width = ExpandToMultipleOfFour(boundingBox.width);
            boundingBox.height = ExpandToMultipleOfFour(boundingBox.height);
            */

            var root = (drawable as MonoBehaviour);
            var result = new RenderingPass
            {
                Root = root.transform,
                RootObjectName = root.gameObject.name,
                ChildPasses = new List<RenderingPass>(),
                EffekseerPasses = new List<IEffekseerContainer>(),
                IsTop = isTop,
                RenderTexture = isTop ? null : CreateRenderTexture((int)boundingBox.width, (int)boundingBox.height),
                BoundingBox = boundingBox
            };
            
            // 深さ優先で子階層を探索
            ExploreHierarchy(root.transform, result.ChildPasses, result.EffekseerPasses);

            return result;
        }

        /// <summary>
        /// 階層構造を探索してフィルターを持つ子要素を見つける
        /// </summary>
        private void ExploreHierarchy(Transform parent, List<RenderingPass> childPasses, List<IEffekseerContainer> effekseerPasses)
        {
            for (int i = 0; i < parent.childCount; i++)
            {
                Transform child = parent.GetChild(i);
                if (!child.gameObject.activeSelf)
                {
                    continue;
                }
                
                // フィルターもしくはEffekseer描画可能オブジェクトを探す
                var childGraphic = child.GetComponent<Graphic>();
                if (childGraphic != null && childGraphic is IRmmzDrawable2d drawable)
                {
                    if (childGraphic is IEffekseerContainer effekseerContainer)
                    {
                        effekseerPasses.Add(effekseerContainer);
                    }
                    
                    // ここの条件は親が FilterCanvas だったら、に変えるべきかも
                    if (child.GetComponent<Canvas>() != null && child.childCount > 0)
                    {
                        // 子パスを作成して追加
                        var pass = PrepareRenderPass(sceneCanvas, drawable);
                        if (pass != null)
                        {
                            childPasses.Add(pass);    
                        }

                        continue;
                    }
                    
                }
                
                // 子階層を再帰的に探索
                ExploreHierarchy(child, childPasses, effekseerPasses);
            }
        }

        /// <summary>
        /// 新しいレンダーテクスチャを作成
        /// </summary>
        /// <returns>レンダーテクスチャ</returns>
        private RenderTexture CreateRenderTexture(int width, int height)
        {
            var renderTexture = RenderTexture.GetTemporary(
                width, height, 24, RenderTextureFormat.ARGB32);
            _renderTextureCaches.Add(renderTexture);
            return renderTexture;
        }
        
        /// <summary>
        /// 値を1以上で4の倍数の整数に拡張します
        /// </summary>
        /// <param name="value">拡張する値</param>
        /// <returns>1以上で4の倍数の整数</returns>
        private static int ExpandToMultipleOfFour(float value)
        {
            // 最低値として1を保証
            float result = Mathf.Max(1f, value);
        
            // 小数点以下を切り上げて整数にする
            int intValue = Mathf.CeilToInt(result);
        
            // 4の倍数に切り上げる
            int remainder = intValue % 4;
            if (remainder != 0)
            {
                intValue += (4 - remainder);
            }
        
            return intValue;
        }

        /// <summary>
        /// レンダリングパスに従って描画を実行
        /// </summary>
        /// <param name="pass">レンダリングパス</param>
        private void RenderPass(RenderingPass pass)
        {
            // まず子パスを処理（深さ優先）
            foreach (var childPass in pass.ChildPasses)
            {
                RenderPass(childPass);
            }

            // パスのレンダリング処理
            RenderSinglePass(pass);    
            
            foreach (var effekseer in pass.EffekseerPasses)
            {
                RenderEffekseerPass(pass.RenderTexture, effekseer);
            }
        }

        /// <summary>
        /// 単一パスのレンダリングを実行
        /// </summary>
        /// <param name="pass">レンダリングパス</param>
        private void RenderSinglePass(RenderingPass pass)
        {
            var canvas = pass.Root.GetComponent<Canvas>();
            if (canvas == null) return;

            // キャンバスを表示レイヤーに変更
            canvas.gameObject.layer = Rmmz.RmmzDefaultLayer;

            // レンダーターゲットを設定
            RenderTexture previousTarget = null;
            if (!pass.IsTop)
            {
                previousTarget = sceneCamera.targetTexture;
                sceneCamera.targetTexture = pass.RenderTexture;
            }

            try
            {
                // 子パスの準備
                PrepareChildPasses(pass.ChildPasses);

                
                if (sceneCanvas.transform.childCount == 0)
                {
                    return;
                }
                /*
                var child = sceneCanvas.transform.GetChild(0);
                var sceneTrans = child.GetComponent<RectTransform>();
                var screenOrigin = new Vector2(
                    (sceneCamera.pixelWidth - pass.BoundingBox.width) / 2,
                    (sceneCamera.pixelHeight - pass.BoundingBox.height) / 2);

                var oldPosition = sceneTrans.anchoredPosition;
                sceneTrans.anchoredPosition = screenOrigin;
                */
                
                // レンダリング実行
                if (pass.IsTop)
                {
                    sceneCamera.backgroundColor = new Color(0, 0, 0, 1);
                }
                else
                {
                    sceneCamera.backgroundColor = new Color(0, 0, 0, 0);
                }
                sceneCamera.Render();

                // 子パスの後処理
                CleanupChildPasses(pass.ChildPasses);
            }
            finally
            {
                // 設定を元に戻す
                if (!pass.IsTop)
                {
                    sceneCamera.targetTexture = previousTarget;
                }

                canvas.gameObject.layer = Rmmz.RmmzDontRenderLayer;
            }
        }

        /// <summary>
        /// 子パスのレンダリング前の準備
        /// </summary>
        /// <param name="childPasses">子パスリスト</param>
        private void PrepareChildPasses(List<RenderingPass> childPasses)
        {
            foreach (var childPass in childPasses)
            {
                //childPass.Root.gameObject.layer = Rmmz.RmmzDontRenderLayer;

                var drawable = childPass.Root.parent.GetComponent<Graphic>() as RmmzFilterCanvas;
                drawable?.BeginParentRender(childPass);

                // 子オブジェクトを一時的に非表示
                //SetChildrenActive(childPass.Root, false);
            }
        }

        /// <summary>
        /// 子パスのレンダリング後の処理
        /// </summary>
        /// <param name="childPasses">子パスリスト</param>
        private void CleanupChildPasses(List<RenderingPass> childPasses)
        {
            foreach (var childPass in childPasses)
            {
                //childPass.Root.gameObject.layer = Rmmz.RmmzDontRenderLayer;

                var drawable = childPass.Root.parent.GetComponent<Graphic>() as RmmzFilterCanvas;
                drawable?.EndParentRender();

                // 子オブジェクトの表示を元に戻す
                //SetChildrenActive(childPass.Root, true);
            }
        }
        
        /// <summary>
        /// Effekseerエフェクトのレンダリングを実行
        /// </summary>
        private void RenderEffekseerPass(RenderTexture target, IEffekseerContainer effekseer)
        {
            if (sceneCanvas.transform.childCount == 0 || !effekseer.NeedDrawEffect())
            {
                return;
            }

            var oldPixelRect = effekseerCamera.pixelRect;
            var tmpBuffer = CreateRenderTexture(Graphics.Width, Graphics.Height);
            
            try
            {
                effekseerCamera.cullingMask = 1 << effekseer.GetEffectLayer();
                effekseerCamera.targetTexture = tmpBuffer;
                effekseerCamera.Render();

                var position = effekseer.TargetPosition(effekseerCamera);
                float offsetX = position.x / Graphics.Width - 0.5f;
                float offsetY = position.y / Graphics.Height - 0.5f;
                _blitMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                _blitMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.One);
                _blitMaterial.SetInt("_SrcBlendA", (int)UnityEngine.Rendering.BlendMode.One);
                _blitMaterial.SetInt("_DstBlendA", (int)UnityEngine.Rendering.BlendMode.One);
                _blitMaterial.SetVector("_Offset", new Vector4(-offsetX, offsetY, 1, 1));
                _blitMaterial.SetInt("_IsMirror", effekseer.IsMirror() ? 1 : 0);
                UnityEngine.Graphics.Blit(tmpBuffer, target, _blitMaterial);
            }
            finally
            {
                // 設定を元に戻す
                effekseerCamera.targetTexture = null;
                effekseerCamera.ResetProjectionMatrix();
                effekseerCamera.cullingMask = 0;
                RenderTexture.ReleaseTemporary(tmpBuffer);
            }
        }

        /// <summary>
        /// 子オブジェクトのアクティブ状態を設定
        /// </summary>
        /// <param name="parent">親となる Transform</param>
        /// <param name="active">アクティブにするかどうか</param>
        private void SetChildrenActive(Transform parent, bool active)
        {
            int childCount = parent.childCount;
            for (int i = 0; i < childCount; i++)
            {
                parent.GetChild(i).gameObject.SetActive(active);
            }
        }
        
        /*
        /// <summary>
        /// RectTransformとその全子要素を含むバウンディングボックスを計算します（スケールを考慮）
        /// </summary>
        /// <param name="rectTransform">バウンディングボックスを計算するRectTransform</param>
        /// <param name="includeInactive">非アクティブなオブジェクトも含めるかどうか</param>
        /// <returns>全ての要素を含むRectの範囲</returns>
        public Rect GetBoundingBoxIncludingChildren(Canvas topCanvas)
        {
            // ワールド座標での頂点を収集
            List<Vector3> worldCorners = new List<Vector3>();
            
            // 自身の頂点を追加
            Vector3[] corners = new Vector3[4];
            rectTransform.GetWorldCorners(corners); // これはスケールを含むワールド座標
            worldCorners.AddRange(corners);
            
            // 子要素の頂点を収集
            CollectChildrenWorldCorners(rectTransform, worldCorners, false);
            
            // これですべての頂点がワールド座標で収集された
            
            // worldCornersがワールド座標のため、これをスクリーン座標に変換し、
            // 次にrectTransformローカル座標に戻す必要がある
            if (worldCorners.Count == 0)
            {
                // 頂点がない場合はデフォルト値を返す
                return new Rect(0, 0, 0, 0);
            }
            
            // rectTransformのローカル座標系に変換
            var canvasRect = topCanvas.GetComponent<RectTransform>();
            List<Vector2> localPoints = new List<Vector2>();
            foreach (Vector3 worldPoint in worldCorners)
            {
                // ワールド座標からスクリーン座標に変換
                var cam = topCanvas.worldCamera;
                Vector2 localPoint = RectTransformUtility.WorldToScreenPoint(cam, worldPoint);
                localPoints.Add(localPoint);
            }
            
            // 全ての点から最小と最大を計算
            float minX = float.MaxValue, minY = float.MaxValue;
            float maxX = float.MinValue, maxY = float.MinValue;
            
            foreach (Vector2 point in localPoints)
            {
                minX = Mathf.Min(minX, point.x);
                minY = Mathf.Min(minY, point.y);
                maxX = Mathf.Max(maxX, point.x);
                maxY = Mathf.Max(maxY, point.y);
            }
            
            // バウンディングボックスを作成
            return new Rect(minX, minY, maxX - minX, maxY - minY);
        }
        
        /// <summary>
        /// 子要素のワールド座標の頂点を再帰的に収集
        /// </summary>
        private static void CollectChildrenWorldCorners(RectTransform parent, List<Vector3> worldCorners, bool includeInactive)
        {
            // 各子要素に対して処理
            foreach (RectTransform child in parent)
            {
                // 非アクティブなオブジェクトをスキップ（オプション）
                if (!includeInactive && !child.gameObject.activeSelf)
                    continue;
                
                // 子要素の頂点をワールド座標で取得
                Vector3[] childCorners = new Vector3[4];
                child.GetWorldCorners(childCorners); // これは自動的にスケールを考慮
                worldCorners.AddRange(childCorners);
                
                // 子要素の子も再帰的に処理
                CollectChildrenWorldCorners(child, worldCorners, includeInactive);
            }
        }
        */

    }
}