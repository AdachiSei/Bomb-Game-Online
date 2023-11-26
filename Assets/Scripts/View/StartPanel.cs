using Cysharp.Threading.Tasks;
using FourthTermPresentation.Manager;
using Photon.Pun;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace FourthTermPresentation.View
{
    /// <summary>
    /// 最初の画面
    /// </summary>
    [DisallowMultipleComponent]
    public class StartPanel : MonoBehaviour
    {
        [SerializeField]
        private TMP_InputField _playerNameInputField = null;

        [SerializeField]
        private TMP_InputField _roomNameInputField = null;

        [SerializeField]
        private GameObject _uiRoot = null;

        [SerializeField]
        private Button _joinButton = null;

        [SerializeField]
        private ConnectionManager _connectionManager = null;

        private void Awake()
        {
            // デフォルトのプレイヤーネーム
            string playerName = 
                PlayerNameData.PlayerName != null ? 
                PlayerNameData.PlayerName : 
                $"player-{UnityEngine.Random.Range(100, 1000):D03}";

            _playerNameInputField.text = playerName;

            // デフォルトのルーム名
            //_roomNameInputField.text = $"room-{Random.Range(100, 1000):D03}";
            _roomNameInputField.text = $"room-{100:D03}";

            _joinButton.onClick.AddListener(OnStartButtonClicked);

            _uiRoot.SetActive(true);
        }

        private async void OnStartButtonClicked()
        {
            _joinButton.interactable = false;

            PlayerNameData.SetPlayerName(_playerNameInputField.text);
            var nickName = _playerNameInputField.text;
            var roomName = _roomNameInputField.text;

            _connectionManager
                .Connect
                    (nickName, roomName, Transition, Debug.LogError);

            await UniTask.Delay(TimeSpan.FromSeconds(10f));

            _joinButton.interactable = true;
        }

        private void Transition()
        {
            TransitionForUniTask().Forget();
        }

        private async UniTask TransitionForUniTask()
        {
            await UniTask.Delay(TimeSpan.FromSeconds(0.5f));

            _uiRoot.SetActive(false);
        }
    }
}