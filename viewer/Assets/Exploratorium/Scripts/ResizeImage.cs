using System;
using System.IO;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Networking;
using UnityEngine.Rendering;

namespace Exploratorium
{
    public class ResizeImage
    {
        private static Texture2D Result { get; set; }

        private static Material ResizeMaterial
        {
            get
            {
                if (_material != null)
                    return _material;

                switch (GetCurrentRenderPipelineInUse())
                {
                    case RenderPipeline.URP:
                        var shader = Shader.Find(URPShader);
                        if (shader == null)
                            throw new Exception($"Could not find shader {URPShader}");
                        else
                            Debug.Log($"Using {shader.name}");
                        _material = new Material(shader);
                        break;
                    case RenderPipeline.Legacy:
                    case RenderPipeline.HDRP:
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                return _material;
            }
        }

        private static Material _material;
        private const string URPShader = "Universal Render Pipeline/Unlit";

        [CanBeNull]
        public static async UniTask<Texture2D> GetResized(
            [NotNull] string sourceFilePath,
            int longEdge
        )
        {
            try
            {
                /*var tx = new Texture2D(longEdge, longEdge, TextureFormat.RGBA32, false);
                var bytes = File.ReadAllBytes(sourceFilePath);
                tx.LoadImage(bytes);*/

                // load resized
                UnityWebRequest webRequest = UnityWebRequestTexture.GetTexture(new Uri(sourceFilePath));
                webRequest.certificateHandler = AcceptAllCertificates.Handler;
                await webRequest.SendWebRequest().ToUniTask();
                Texture2D tx = DownloadHandlerTexture.GetContent(webRequest);
                tx.filterMode = FilterMode.Trilinear;
                tx.name = Path.GetFileNameWithoutExtension(sourceFilePath);

                var scale = longEdge / (float)Mathf.Max(tx.width, tx.height);
                var materialRef = ResizeMaterial;
                Texture2D result;

                result = RenderWithMaterial(
                    ref tx,
                    ref materialRef,
                    new Vector2Int((int)(tx.width * scale), (int)(tx.height * scale))
                );
                return result;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return null;
            }
        }

        [CanBeNull]
        public static async UniTask<Texture2D> GetResized(
            [NotNull] string sourceFilePath,
            Vector2Int cropTo
        )
        {
            try
            {
                // load resized
                UnityWebRequest webRequest = UnityWebRequestTexture.GetTexture(new Uri(sourceFilePath));
                webRequest.certificateHandler = AcceptAllCertificates.Handler;
                await webRequest.SendWebRequest().ToUniTask();
                Texture2D tx = DownloadHandlerTexture.GetContent(webRequest);
                tx.filterMode = FilterMode.Trilinear;
                tx.name = Path.GetFileNameWithoutExtension(sourceFilePath);

                int requiredResolution = Mathf.Max(cropTo.x, cropTo.y);
                var requiredScale = requiredResolution / (float)Mathf.Min(tx.width, tx.height);
                var materialRef = ResizeMaterial;
                Texture2D result = RenderWithMaterial(
                    ref tx,
                    ref materialRef,
                    new Vector2Int(Mathf.CeilToInt(tx.width * requiredScale),
                        Mathf.CeilToInt(tx.height * requiredScale)),
                    cropTo
                );
                return result;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return null;
            }
        }

        /// <remarks>Threadsafe</remarks>
        public static string Save(
            [NotNull] byte[] txRawBytes, GraphicsFormat format, uint width, uint height,
            [NotNull] string filePath
        )
        {
            if (string.IsNullOrWhiteSpace(filePath))
                return null;

            string jpgPath = Path.ChangeExtension(filePath, "jpg");
            byte[] jpgBytes = ImageConversion.EncodeArrayToJPG(txRawBytes, format, width, height);
            File.WriteAllBytes(jpgPath, jpgBytes);
            return jpgPath;
        }

        public static string Save([NotNull] Texture2D tx, [NotNull] string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                return null;

            string jpgPath = Path.ChangeExtension(filePath, "jpg");
            byte[] jpgBytes = ImageConversion.EncodeArrayToJPG(tx.GetRawTextureData(), tx.graphicsFormat,
                (uint)tx.width, (uint)tx.height);
            File.WriteAllBytes(jpgPath, jpgBytes);
            return jpgPath;
        }

        public static Texture2D RenderWithMaterial(
            [NotNull] ref Texture2D sourceTx,
            [NotNull] ref Material material,
            Vector2Int resizeTo,
            [NotNull] string filePath = ""
        ) => RenderWithMaterial(ref sourceTx, ref material, resizeTo, resizeTo, filePath);

        public static Texture2D RenderWithMaterial(
            [NotNull] ref Texture2D sourceTx,
            [NotNull] ref Material material,
            Vector2Int resizeTo,
            Vector2Int cropTo,
            [NotNull] string filePath = ""
        )
        {
            RenderTexture rt = RenderTexture.GetTemporary(resizeTo.x, resizeTo.y);

            // sets dest as the render target, sets source _MainTex property on the material, and draws a full-screen quad.
            Graphics.Blit(sourceTx, rt);

            Texture2D resultTx = new Texture2D(cropTo.x, cropTo.y, TextureFormat.RGBA32, false)
            {
                filterMode = FilterMode.Bilinear,
                wrapMode = TextureWrapMode.Clamp,
                name = sourceTx.name
            };
            RenderTexture.active = rt;
            int xPos = Mathf.Max(0, resizeTo.x - cropTo.x) / 2;
            int yPos = Mathf.Max(0, resizeTo.y - cropTo.y) / 2;
            resultTx.ReadPixels(new Rect(new Vector2(xPos, yPos), cropTo), 0, 0);
            resultTx.Apply();

            RenderTexture.active = null;
            RenderTexture.ReleaseTemporary(rt);

            if (filePath.Length != 0)
                Save(resultTx, filePath);
            return resultTx;
        }

        internal static RenderPipeline GetCurrentRenderPipelineInUse()
        {
            RenderPipelineAsset rpa = GraphicsSettings.renderPipelineAsset;
            if (rpa != null)
            {
                switch (rpa.GetType().Name)
                {
                    case "UniversalRenderPipelineAsset": return RenderPipeline.URP;
                    case "HDRenderPipelineAsset": return RenderPipeline.HDRP;
                }
            }

            return RenderPipeline.Legacy;
        }

        internal enum RenderPipeline
        {
            Legacy,
            URP,
            HDRP
        }

        public static Sprite CreateFilledSprite(float spriteAspect, Texture2D tx)
        {
            float txAspect = tx.width / (float)tx.height;
            Rect spriteRect;


            // portrait

            if (spriteAspect > txAspect)
            {
                // crop top&bottom
                float cropToHeight = spriteAspect > 1f ? tx.width / spriteAspect : tx.width * spriteAspect;
                spriteRect = new Rect(
                    x: 0,
                    y: (tx.height - cropToHeight) * 0.5f,
                    width: tx.width,
                    height: cropToHeight
                );
            }
            else
            {
                // crop left&right
                float cropToWidth = spriteAspect > 1f ? tx.height / spriteAspect : tx.height * spriteAspect;
                spriteRect = new Rect(
                    x: (tx.width - cropToWidth) * 0.5f,
                    y: 0,
                    width: cropToWidth,
                    height: tx.height
                );
            }


            Sprite sprite = Sprite.Create(texture: tx, rect: spriteRect, pivot: spriteRect.center);
            return sprite;
        }

        public static Rect FitRect(Rect source, Rect target, FitMode mode = FitMode.Contain)
        {
            float sw = target.width / source.width;
            float sh = target.height / source.height;
            float scale = mode switch
            {
                FitMode.Contain => Mathf.Min(sw, sh),
                FitMode.Cover => Mathf.Max(sw, sh),
                _ => 1.0f
            };

            return new Rect(
                target.x + (target.width - source.width * scale) / 2.0f,
                target.y + (target.height - source.height * scale) / 2.0f,
                source.width * scale,
                source.height * scale
            );
        }

        public enum FitMode
        {
            Cover,
            Contain
        }
    }
}