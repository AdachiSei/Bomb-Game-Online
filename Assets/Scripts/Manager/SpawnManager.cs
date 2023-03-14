using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FourthTermPresentation
{
    /// <summary>
    /// スポーン位置を決める
    /// </summary>
    public class SpawnManager : MonoBehaviour
    {
        [SerializeField]
        [Header("スポーン位置")]
        List<Transform> _spawnPos = new();

        private int _currentIndex = -1;

        public Vector3 SpawnPos()
        {
            _currentIndex++;
            if (_currentIndex > _spawnPos.Count) _currentIndex = 0;
            return _spawnPos[_currentIndex].position;
        }
    }
}
