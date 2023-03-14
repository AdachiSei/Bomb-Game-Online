using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FourthTermPresentation
{
    /// <summary>
    /// �X�|�[���ʒu�����߂�
    /// </summary>
    public class SpawnManager : MonoBehaviour
    {
        [SerializeField]
        [Header("�X�|�[���ʒu")]
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
