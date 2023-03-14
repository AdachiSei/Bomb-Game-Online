using Photon.Pun;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using Cysharp.Threading.Tasks;
using TMPro;
using FourthTermPresentation.Manager;
using FourthTermPresentation.GamePlayer;
using System;

namespace FourthTermPresentation
{
    /// <summary>
    /// ゲームの全体的な管理を担うコンポーネント
    /// </summary>
    [DisallowMultipleComponent]
    public class GameManager : MonoBehaviour
    {
        public PlayerComponentHolder[] PlayerComponentHolders => _playerHolders;

        private static readonly int ScoreToWin = 5;

        [SerializeField]
        private RoomInfoView _roomInfoView = null;

        [SerializeField]
        private Button _startGameButton = null;

        [SerializeField]
        private ConnectionManager _connectionManager = null;

        [SerializeField]
        private PhotonView _photonView = null;

        [SerializeField]
        private RPCManager _rpcManager = null;

        [SerializeField]
        private SpawnManager _spawnManager = null;

        [SerializeField]
        TimeManager _timeManager = null;

        [SerializeField]
        private TMP_Text _winText;

        [SerializeField]
        private PlayerComponentHolder[] _playerHolders = null;

        [SerializeField]
        private ResultView _resultView = null;

        private void Start()
        {
            _connectionManager.OnJoinedRoomEvent += room =>
            {
                _roomInfoView.SetRoomName(room.Name);
                UpdatePlayerList();

                if (PhotonNetwork.IsMasterClient)
                {
                    _startGameButton.gameObject.SetActive(true);
                    _startGameButton.onClick.AddListener(_rpcManager.SendStartGame);
                }
            };
            _connectionManager.OnPlayerEnteredEvent += player => UpdatePlayerList();
            _connectionManager.OnPlayerLeftEvent += player => UpdatePlayerList();

            _rpcManager.OnReceiveStartGame += StartGame;
        }

        #region Private Methods

        private void UpdatePlayerList()
        {
            foreach (var holder in _playerHolders)
            {
                holder.SetPlayerNameText("");
                holder.SetJobText("");
            }

            int count = 0;

            for (int i = 0; i < _playerHolders.Length; i++)
            {
                if (PhotonNetwork.PlayerList.Length == i) break;
                var player = PhotonNetwork.PlayerList[i];
                if (player == null) continue;
                _roomInfoView.SetPlayerNameText(player.NickName, count);
                if (player.IsMasterClient) _playerHolders[i].SetJobText("Bomber");
                count++;
            }
        }

        async private void StartGame()
        {
            _startGameButton.gameObject.SetActive(false);
            SetPlayer();
            foreach (var holder in _playerHolders)
            {
                await UniTask.WaitUntil(() => holder.Player != null);
                PlayerPresenterAndKill(holder);
            }
            Judg();
        }

        private void SetPlayer()
        {
            var photonViews = FindObjectsOfType<PhotonView>();
            var players = photonViews
                            .OrderBy(player => player.ViewID)
                            .Select(playes => playes.GetComponent<PlayerController>());
            var count = -2;
            foreach (var player in players)
            {
                count++;
                if (count == -1) continue;
                player.transform.position = _spawnManager.SpawnPos();
                _playerHolders[count].SetPlayer(player);
            }
        }

        async private void PlayerPresenterAndKill(PlayerComponentHolder holder)
        {
            _rpcManager.OnSetIsBomber += holder.Player.SetIsBomber;
            await UniTask.Delay(TimeSpan.FromSeconds(1f));
            _rpcManager.SendSetIsBomber();

            _rpcManager.OnCaughtSurvivor += holder.Player.SetBomb;
            holder.Player.SendCaughtSurvivor += _rpcManager.SendCaughtSurvivor;

            holder.Player
                .ObserveEveryValueChanged(player => player.IsBomber)
                .Subscribe(x =>
                {
                    if (x == true) holder.SetJobText("Bomber");
                    else holder.SetJobText("");
                })
                .AddTo(this);

            float limitTime = 30f;

            while (true)
            {
                await UniTask.WaitUntil(() => _timeManager.Timer < limitTime);

                if (holder.Player.IsBomber == true)
                {
                    holder.Player.DeactivatePlayer();
                    holder.SetJobText("");
                    await UniTask.NextFrame();
                    Judg();
                    return;
                }
                else
                {
                    limitTime -= 15;
                    if (limitTime < 0) return;
                }
            }
        }

        private void Judg()
        {
            int tripCount = 0;
            int dontTripCount = 0;
            foreach (var holder in _playerHolders)
            {
                if (holder.Player?.IsTripping == false) dontTripCount++;
                else tripCount++;
            }

            if (dontTripCount == 1)
            {
                foreach (var holder in _playerHolders)
                {
                    if (holder.Player.IsTripping == false)
                    {
                        Debug.Log("おめでとう");
                        if (_winText.text == "") _winText.text = $"{holder.PlayerName} Win";
                        return;
                    }
                }
            }
            else
            {
                foreach (var holder in _playerHolders)
                {
                    if (holder.Player?.IsTripping == false)
                    {
                        holder.SetJobText("Bomber");
                        holder.Player.SetBomb();
                        break;
                    }
                }
            }

        }
        #endregion
    }
}