namespace SEP490G69
{
    using UnityEngine;
    using System;

    [Serializable]
    public class PreLoadPrefab
    {
        [SerializeField] private GameObject prefab;
        [SerializeField] private int preLoadCount;

        public GameObject Prefab => prefab;
        public int PreLoadCount => preLoadCount;
    }
}