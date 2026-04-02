using UnityEngine;

using System;

namespace CarTrickRush.Data
{
    [Serializable]
    public sealed class SoundData
    {
        [SerializeField] private int _id = 0;
        [SerializeField] private string _name = default;
        [SerializeField] private string _useTag = default;
        [SerializeField] private AudioClip _clip = default;

        public int Id => _id;
        public string Name => _name;
        public string UseTag => _useTag;
        public AudioClip Clip => _clip;
    }
}
