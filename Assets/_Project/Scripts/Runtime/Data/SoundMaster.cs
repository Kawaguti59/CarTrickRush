using UnityEngine;

using System;
using System.Collections.Generic;

namespace CarTrickRush.Data
{
    [CreateAssetMenu(
        fileName = "SoundMaster",
        menuName = "CarTrickRush/Data/Sound Master")]
    public sealed class SoundMaster : ScriptableObject
    {
        /// <summary>
        /// BGMリスト.
        /// </summary>
        [SerializeField] private List<SoundData> _bgmList = new();

        /// <summary>
        /// SEリスト.
        /// </summary>
        [SerializeField] private List<SoundData> _seList = new();


        /// <summary>
        /// BGMを取得する.
        /// </summary>
        /// <param name="id">BGMのID.</param>
        /// <param name="data">BGMのデータ.</param>
        /// <returns>BGMが見つかったか.</returns>
        public bool TryGetBgm(int id, out SoundData data)
        {
            data = null;
            if (_bgmList == null) { return false; }

            for (var i = 0; i < _bgmList.Count; i++)
            {
                var d = _bgmList[i];
                if (d == null) { continue; }
                if (d.Id != id) { continue; }
                data = d;
                return true;
            }

            return false;
        }

        /// <summary>
        /// BGMを取得する.
        /// </summary>
        /// <param name="name">BGMの名前.</param>
        /// <param name="data">BGMのデータ.</param>
        /// <returns>BGMが見つかったか.</returns>
        public bool TryGetBgm(string name, out SoundData data)
        {
            data = null;
            if (_bgmList == null) { return false; }
            if (string.IsNullOrWhiteSpace(name)) { return false; }

            for (var i = 0; i < _bgmList.Count; i++)
            {
                var d = _bgmList[i];
                if (d == null) { continue; }
                if (!string.Equals(d.Name, name, StringComparison.OrdinalIgnoreCase)) { continue; }
                data = d;
                return true;
            }

            return false;
        }

        /// <summary>
        /// SEを取得する.
        /// </summary>
        /// <param name="id">SEのID.</param>
        /// <param name="data">SEのデータ.</param>
        /// <returns>SEが見つかったか.</returns>
        public bool TryGetSe(int id, out SoundData data)
        {
            data = null;
            if (_seList == null) { return false; }

            for (var i = 0; i < _seList.Count; i++)
            {
                var d = _seList[i];
                if (d == null) { continue; }
                if (d.Id != id) { continue; }
                data = d;
                return true;
            }

            return false;
        }

        /// <summary>
        /// SEを取得する.
        /// </summary>
        /// <param name="name">SEの名前.</param>
        /// <param name="data">SEのデータ.</param>
        /// <returns>SEが見つかったか.</returns>
        public bool TryGetSe(string name, out SoundData data)
        {
            data = null;
            if (_seList == null) { return false; }
            if (string.IsNullOrWhiteSpace(name)) { return false; }

            for (var i = 0; i < _seList.Count; i++)
            {
                var d = _seList[i];
                if (d == null) { continue; }
                if (!string.Equals(d.Name, name, StringComparison.OrdinalIgnoreCase)) { continue; }
                data = d;
                return true;
            }

            return false;
        }

        /// <summary>
        /// BGMまたはSEを取得する.
        /// </summary>
        /// <param name="id">BGMまたはSEのID.</param>
        /// <param name="data">BGMまたはSEのデータ.</param>
        /// <param name="isBgm">BGMかどうか.</param>
        /// <returns>BGMまたはSEが見つかったか.</returns>
        public bool TryGetAny(int id, out SoundData data, out bool isBgm)
        {
            if (TryGetBgm(id, out data))
            {
                isBgm = true;
                return true;
            }

            if (TryGetSe(id, out data))
            {
                isBgm = false;
                return true;
            }

            data = null;
            isBgm = false;
            return false;
        }

        /// <summary>
        /// BGMまたはSEを取得する.
        /// </summary>
        /// <param name="name">BGMまたはSEの名前.</param>
        /// <param name="data">BGMまたはSEのデータ.</param>
        /// <param name="isBgm">BGMかどうか.</param>
        /// <returns>BGMまたはSEが見つかったか.</returns>
        public bool TryGetAny(string name, out SoundData data, out bool isBgm)
        {
            if (TryGetBgm(name, out data))
            {
                isBgm = true;
                return true;
            }

            if (TryGetSe(name, out data))
            {
                isBgm = false;
                return true;
            }

            data = null;
            isBgm = false;
            return false;
        }
    }
}
