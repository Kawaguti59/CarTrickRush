using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

namespace CarTrickRush.Core
{
    /// =========================================================================================   
    /// <summary>
    /// インスペクターで編集可能なMapコレクション.
    /// </summary>
    /// =========================================================================================
    [Serializable]
    public sealed class InspectableMap<TKey, TValue> :
        ISerializationCallbackReceiver,
        IEnumerable<KeyValuePair<TKey, TValue>>
    {
        #region ------------------ Fields ------------------

        /// <summary>
        /// ペアのリスト.
        /// </summary>
        [SerializeField] private List<InspectablePair> _pairs = new();

        /// <summary>
        /// キャッシュ.
        /// </summary>
        [NonSerialized] private Dictionary<TKey, TValue> _cache;

        /// <summary>
        /// キャッシュがDirtyかどうか.
        /// </summary>
        [NonSerialized] private bool _cacheDirty = true;

        #endregion

        #region ------------------ Properties ------------------

        /// <summary>
        /// インスペクターおよびアセットに保存される順序のペア一覧.
        /// </summary>
        public IReadOnlyList<InspectablePair> Pairs => _pairs;

        /// <summary>
        /// 要素数.
        /// </summary>
        public int Count
        {
            get
            {
                EnsureCache();
                return _cache.Count;
            }
        }

        /// <summary>
        /// キーに対応するバリューを取得または設定する.
        /// </summary>
        /// <param name="key">キー.</param>
        /// <returns>バリュー.</returns>
        public TValue this[TKey key]
        {
            get
            {
                if (!TryGetValue(key, out var value))
                {
                    throw new KeyNotFoundException();
                }
                return value;
            }
            set
            {
                _cacheDirty = true;
                for (var index = 0; index < _pairs.Count; index++)
                {
                    if (KeyEquals(_pairs[index].Key, key))
                    {
                        _pairs[index] = new InspectablePair(key, value);
                        return;
                    }
                }

                _pairs.Add(new InspectablePair(key, value));
            }
        }

        #endregion

        #region ------------------ Interface Methods ------------------

        /// <summary>
        /// 列挙子を取得する.
        /// </summary>
        /// <returns>列挙子.</returns>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary>
        /// シリアライズ前に呼ばれる.
        /// </summary>
        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
        }

        /// <summary>
        /// シリアライズ後に呼ばれる.
        /// </summary>
        void ISerializationCallbackReceiver.OnAfterDeserialize() => _cacheDirty = true;

        #endregion

        #region ------------------ Public Methods ------------------

        /// <summary>
        /// キーに対応するバリューを取得する.
        /// </summary>
        /// <param name="key">キー.</param>
        /// <param name="value">バリュー.</param>
        /// <returns>バリューが取得できたかどうか.</returns>
        public bool TryGetValue(TKey key, out TValue value)
        {
            EnsureCache();
            return _cache.TryGetValue(key, out value);
        }

        /// <summary>
        /// キーが存在するかどうかを判断する.
        /// </summary>
        /// <param name="key">キー.</param>
        /// <returns>キーが存在するかどうか.</returns>
        public bool ContainsKey(TKey key)
        {
            EnsureCache();
            return _cache.ContainsKey(key);
        }

        /// <summary>
        /// バリューが存在するかどうかを判断する.
        /// </summary>
        /// <param name="searchValue">検索するバリュー.</param>
        /// <returns>バリューが存在するかどうか.</returns>
        public bool ContainsValue(TValue searchValue)
        {
            EnsureCache();
            foreach (var value in _cache.Values)
            {
                if (ValueEquals(value, searchValue))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// キーとバリューを追加する.
        /// </summary>
        /// <param name="key">キー.</param>
        /// <param name="value">バリュー.</param>
        public void Add(TKey key, TValue value)
        {
            if (ContainsKey(key))
            {
                throw new ArgumentException("An item with the same key has already been added.", nameof(key));
            }

            _cacheDirty = true;
            _pairs.Add(new InspectablePair(key, value));
        }

        /// <summary>
        /// キーに対応するバリューを削除する.
        /// </summary>
        /// <param name="key">キー.</param>
        /// <returns>削除できたかどうか.</returns>
        public bool Remove(TKey key)
        {
            _cacheDirty = true;
            var removed = false;
            for (var index = _pairs.Count - 1; index >= 0; index--)
            {
                if (KeyEquals(_pairs[index].Key, key))
                {
                    _pairs.RemoveAt(index);
                    removed = true;
                }
            }

            return removed;
        }

        /// <summary>
        /// すべてのキーとバリューを削除する.
        /// </summary>
        public void Clear()
        {
            _cacheDirty = true;
            _pairs.Clear();
            _cache?.Clear();
        }

        /// <summary>
        /// マップを Dictionary に変換する.
        /// </summary>
        /// <returns>Dictionary.</returns>
        public Dictionary<TKey, TValue> ToDictionary()
        {
            EnsureCache();
            return new Dictionary<TKey, TValue>(_cache, EqualityComparer<TKey>.Default);
        }

        /// <summary>
        /// すべてのキーを取得する.
        /// </summary>
        /// <returns>キーのコレクション.</returns>
        public Dictionary<TKey, TValue>.KeyCollection Keys
        {
            get
            {
                EnsureCache();
                return _cache.Keys;
            }
        }

        /// <summary>
        /// すべてのバリューを取得する.
        /// </summary>
        /// <returns>バリューのコレクション.</returns>
        public Dictionary<TKey, TValue>.ValueCollection Values
        {
            get
            {
                EnsureCache();
                return _cache.Values;
            }
        }

        /// <summary>
        /// 列挙子を取得する.
        /// </summary>
        /// <returns>列挙子.</returns>
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            EnsureCache();
            return _cache.GetEnumerator();
        }

        #endregion

        #region ------------------ Private Methods ------------------

        /// <summary>
        /// キャッシュを更新する.
        /// </summary>
        private void EnsureCache()
        {
            if (!_cacheDirty && _cache != null)
                return;

            _cache ??= new Dictionary<TKey, TValue>(EqualityComparer<TKey>.Default);
            _cache.Clear();

            for (var index = 0; index < _pairs.Count; index++)
            {
                var pair = _pairs[index];
                _cache[pair.Key] = pair.Value;
            }

            _cacheDirty = false;
        }

        /// <summary>
        /// キーが等しいかどうかを判断する.
        /// </summary>
        /// <param name="a">キーA.</param>
        /// <param name="b">キーB.</param>
        /// <returns>キーが等しいかどうか.</returns>
        private static bool KeyEquals(TKey a, TKey b) => EqualityComparer<TKey>.Default.Equals(a, b);

        /// <summary>
        /// バリューが等しいかどうかを判断する.
        /// </summary>
        /// <param name="a">バリューA.</param>
        /// <param name="b">バリューB.</param>
        /// <returns>バリューが等しいかどうか.</returns>
        private static bool ValueEquals(TValue a, TValue b) => EqualityComparer<TValue>.Default.Equals(a, b);

        #endregion
        
        #region ------------------ Nested Class ------------------

        /// =====================================================================================
        /// <summary>
        /// キーとバリューのペア.
        /// </summary> 
        /// =====================================================================================
        [Serializable]
        public sealed class InspectablePair
        {
            #region ------------------ Fields ------------------

            /// <summary>
            /// キー.
            /// </summary>
            [SerializeField] private TKey _key;

            /// <summary>
            /// バリュー.
            /// </summary>
            [SerializeField] private TValue _value;

            #endregion

            #region ------------------ Properties ------------------

            /// <summary>
            /// キー.
            /// </summary>
            public TKey Key => _key;

            /// <summary>
            /// バリュー.
            /// </summary>
            public TValue Value => _value;

            #endregion

            #region ------------------ Public Methods ------------------

            /// <summary>
            /// コンストラクタ.
            /// </summary>
            public InspectablePair()
            {
            }

            /// <summary>
            /// コンストラクタ.
            /// </summary>
            /// <param name="key">キー.</param>
            /// <param name="value">バリュー.</param>
            public InspectablePair(TKey key, TValue value)
            {
                _key = key;
                _value = value;
            }

            #endregion
        }

        #endregion
    }
}
