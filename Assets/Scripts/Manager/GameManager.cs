using Cysharp.Threading.Tasks;
using FourthTermPresentation.Extension;
using FourthTermPresentation.GamePlayer;
using FourthTermPresentation.Manager;
using Photon.Pun;
using System;
using System.Linq;
using Template.Manager;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

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
        private TimeManager _timeManager = null;

        [SerializeField]
        private TMP_Text _winText;

        [SerializeField]
        private PlayerComponentHolder[] _playerHolders = null;

        [SerializeField]
        private SceneLoader _sceneLoader = null;

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

        /// <summary>
        /// ゲームを始めると呼ばれる関数
        /// </summary>
        async private void StartGame()
        {
            _startGameButton.gameObject.SetActive(false);
            await FindPlayer();
            _playerHolders.ForEach(x => PlayerKill(x));
            Judg();
            Battle();
        }

        async private UniTask FindPlayer()
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

            await UniTask.NextFrame();
        }

        async private void PlayerKill(PlayerComponentHolder holder)
        {
            Debug.Log("Kill");
            if (holder.Player == null) return;
            _rpcManager.OnSetIsBomber += holder.Player.ChangeBomber;
            await UniTask.NextFrame();

            _rpcManager.SendSetIsBomber();
            _rpcManager.OnCaughtSurvivor += holder.Player.SetIsBomb;
            holder.Player.SendCaughtSurvivor += _rpcManager.SendCaughtSurvivor;
        }

        /// <summary>
        /// バトルをサポートする関数
        /// </summary>
        async private void Battle()
        {
            while (true)
            {
                await UniTask.WaitUntil(() => _timeManager.Timer <= 0);

                var holder =
                    _playerHolders.FirstOrDefault(x => x.Player.IsBomber == true);

                holder.Player.DeactivatePlayer();
                holder.Player.DeleteBomb();
                holder.SetJobText("");

                await UniTask.NextFrame();

                if (Judg())
                {
                    _timeManager.StopTimer();
                    await UniTask.Delay(TimeSpan.FromSeconds(5f));
                    _sceneLoader.LoadScene(_sceneLoader.CurrentScene);
                    return;
                }
                _timeManager.Init();
                Debug.Log("Battle");
            }
        }

        /// <summary>
        /// 勝敗が決まったか続行するかを決める関数
        /// </summary>
        /// <returns></returns>
        private bool Judg()
        {
            int surviveCount = 0;
            _playerHolders
                .ForEach(x => { if (x.Player?.IsDead == false) surviveCount++; });

            //生きているプレイヤーをとってくる
            var holder = _playerHolders.FirstOrDefault(x => x.Player.IsDead == false);
            if (surviveCount == 1)
            {
                _winText.text = $"{holder.PlayerName} Win";
                return true;
            }


            holder.SetJobText("Bomber");
            holder.Player.SetIsBomb();

            _playerHolders
                .ForEach(x => x.Player.ChangeColor().Forget());
            return false;
        }

        #endregion
    }
}