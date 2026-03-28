using UnityEngine;

using System;
using System.Collections.Generic;

namespace CarTrickRush.Data
{
    /// =========================================================================================
    /// <summary>
    /// シーン遷移ルールフェードのセット.
    /// </summary>
    /// =========================================================================================
    [Serializable]
    public sealed class SceneTransitionSetEntry
    {
        #region ------------------ Fields ------------------

        /// <summary>
        /// セットID.
        /// </summary>
        [SerializeField] private int _id;

        /// <summary>
        /// フェードアウト用マスク.
        /// </summary>
        [SerializeField] private Texture2D _fadeOutMask;

        /// <summary>
        /// フェードイン用マスク.
        /// </summary>
        [SerializeField] private Texture2D _fadeInMask;

        /// <summary>
        /// 覆う時間（秒）.
        /// </summary>
        [SerializeField] private float _coverDuration = 0.45f;

        /// <summary>
        /// 開く時間（秒）.
        /// </summary>
        [SerializeField] private float _revealDuration = 0.45f;

        /// <summary>
        /// マスク境界のぼかし.
        /// </summary>
        [SerializeField] private float _softness = 0.04f;

        #endregion

        #region ------------------ Properties ------------------

        /// <summary>
        /// セットID.
        /// </summary>
        public int Id => _id;

        /// <summary>
        /// フェードアウト用マスク.
        /// </summary>
        public Texture2D FadeOutMask => _fadeOutMask;

        /// <summary>
        /// フェードイン用マスク.
        /// </summary>
        public Texture2D FadeInMask => _fadeInMask;

        /// <summary>
        /// 覆う時間（秒）.
        /// </summary>
        public float CoverDuration => Mathf.Max(0.01f, _coverDuration);

        /// <summary>
        /// 開く時間（秒）.
        /// </summary>
        public float RevealDuration => Mathf.Max(0.01f, _revealDuration);

        /// <summary>
        /// マスク境界のぼかし.
        /// </summary>
        public float Softness => Mathf.Clamp(_softness, 0.001f, 0.25f);

        #endregion
    }

    /// =========================================================================================
    /// <summary>
    /// シーン遷移ルールフェードのカタログ.
    /// </summary>
    /// =========================================================================================
    [CreateAssetMenu(
        fileName = "SceneTransitionCatalog",
        menuName = "CarTrickRush/Data/Scene Transition Catalog")]
    public sealed class SceneTransitionCatalog : ScriptableObject
    {
        #region ------------------ Constants ------------------

        /// <summary>
        /// リソースアセット名.
        /// </summary>
        public const string ResourcesAssetName = "SceneTransitionCatalog";

        #endregion

        #region ------------------ Fields ------------------

        /// <summary>
        /// 遷移セット一覧.
        /// </summary>
        [SerializeField] private List<SceneTransitionSetEntry> _sets = new();

        #endregion

        #region ------------------ Public Methods ------------------

        /// <summary>
        /// 指定IDのセットを取得する.
        /// </summary>
        /// <param name="id">セットID.</param>
        /// <param name="entry">セット.</param>
        /// <returns>取得できたか.</returns>
        public bool TryGet(int id, out SceneTransitionSetEntry entry)
        {
            entry = null;
            if (id < 0)
            {
                return false;
            }

            for (var i = 0; i < _sets.Count; i++)
            {
                var s = _sets[i];
                if (s != null && s.Id == id)
                {
                    entry = s;
                    return true;
                }
            }

            return false;
        }

        #endregion

        #region ------------------ Unity Methods ------------------

        #if UNITY_EDITOR
        private void OnValidate()
        {
            var seen = new HashSet<int>();
            for (var i = 0; i < _sets.Count; i++)
            {
                var s = _sets[i];
                if (s == null)
                {
                    continue;
                }

                var id = s.Id;
                if (id < 0)
                {
                    Debug.LogWarning($"{name}: セットの ID は 0 以上にしてください（負の値は LoadScene のフェードなし指定用）.");
                }

                if (!seen.Add(id))
                {
                    Debug.LogWarning($"{name}: 重複する ID = {id} があります.");
                }
            }
        }
        #endif

        #endregion
    }
}
