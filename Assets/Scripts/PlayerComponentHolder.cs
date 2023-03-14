using Cysharp.Threading.Tasks;
using FourthTermPresentation.GamePlayer;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UniRx;
using FourthTermPresentation.Manager;

namespace FourthTermPresentation
{
    /// <summary>
    /// 1チームあたりのコンポーネントをまとめるためのコンポーネント
    /// </summary>
    [DisallowMultipleComponent]
    public class PlayerComponentHolder : MonoBehaviour
    {
        public string PlayerName => _playerNameText.text;
        public string JobText => _jobText.text;
        public PlayerController Player => _player;

        [SerializeField]
        private TMP_Text _playerNameText = null;

        [SerializeField]
        private TMP_Text _jobText = null;

        [SerializeField]
        private PlayerController _player = null;

        public void SetPlayerNameText(string name) =>
            _playerNameText.text = name;

        public void SetJobText(string job) =>
            _jobText.text = job;

        public void SetPlayer(PlayerController player) =>
            _player = player;
    }
}