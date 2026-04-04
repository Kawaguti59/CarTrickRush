using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

namespace CarTrickRush.Core
{
    /// <summary>
    /// WebGL で URP のポストプロセス／カメラ AA が最終出力を壊すケースへの回避。
    /// 音だけ聞こえて画面が真っ白になる症状の対策。
    /// </summary>
    public static class WebGlRenderingWorkaround
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Register()
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            SceneManager.sceneLoaded += OnSceneLoaded;
            ApplyToLoadedCameras();
#endif
        }

#if UNITY_WEBGL && !UNITY_EDITOR
        private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            ApplyToLoadedCameras();
        }

        private static void ApplyToLoadedCameras()
        {
            foreach (var camera in Object.FindObjectsByType<Camera>(FindObjectsSortMode.None))
            {
                var data = camera.GetUniversalAdditionalCameraData();
                if (data == null)
                {
                    continue;
                }

                data.renderPostProcessing = false;
                data.antialiasing = AntialiasingMode.None;
            }
        }
#endif
    }
}
