using System;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace FourthTermPresentation.Manager
{
    /// <summary>
    /// Photonの接続周りの処理をコールバックを担うコンポーネント
    /// </summary>
    [DisallowMultipleComponent]
    public class ConnectionManager : MonoBehaviourPunCallbacks
    {
        /// <summary>
        /// プレイ人数
        /// </summary>
        byte _maxPlayers = 4;
        private string _roomName;
        private Action _onSuccess;
        private Action<string> _onError;

        public event Action<Room> OnJoinedRoomEvent;
        public event Action OnJoinedRoomGeneratePlayer;
        public event Action<Player> OnPlayerEnteredEvent;
        public event Action<Player> OnPlayerLeftEvent;

        private void OnDestroy()
        {
#if UNITY_EDITOR
            Debug.Log("Disconnect");
#endif
            PhotonNetwork.Disconnect();
        }

        /// <summary>
        /// 接続
        /// </summary>
        public void Connect
            (string nickName, string roomName, Action onSuccess, Action<string> onError)
        {
            _roomName = roomName;
            _onSuccess = onSuccess;
            _onError = onError;

            PhotonNetwork.NickName = nickName;

            if (!PhotonNetwork.IsConnected) PhotonNetwork.ConnectUsingSettings();
            else JoinOrCreateRoom();
        }

        /// <summary>
        /// マスターに接続した
        /// </summary>
        public override void OnConnectedToMaster() =>
            JoinOrCreateRoom();

        /// <summary>
        /// ルームに参加したら呼ばれる関数
        /// </summary>
        public override void OnJoinedRoom()
        {
            _onSuccess?.Invoke();
            OnJoinedRoomEvent?.Invoke(PhotonNetwork.CurrentRoom);
            OnJoinedRoomGeneratePlayer();
        }

        /// <summary>
        /// ルームの作成に失敗したら呼ばれる関数
        /// </summary>
        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            _onError?.Invoke($"CreateRoomFailed: {message} ({returnCode})");
        }

        /// <summary>
        /// ルームの参加に失敗したら呼ばれる関数
        /// </summary>
        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            _onError?.Invoke($"JoinRoomFailed: {message} ({returnCode})");
        }

        /// <summary>
        /// 切断されたら呼ばれる関数
        /// </summary>
        public override void OnDisconnected(DisconnectCause cause)
        {
            if (cause != DisconnectCause.None)
                _onError?.Invoke($"Disconnected: {cause}");
        }

        /// <summary>
        /// 部屋に入ったら呼ぶ関数
        /// </summary>
        /// <param name="newPlayer"></param>
        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            OnPlayerEnteredEvent?.Invoke(newPlayer);
        }

        /// <summary>
        /// 部屋から去ったら呼ぶ関数
        /// </summary>
        public override void OnPlayerLeftRoom(Player otherPlayer)
        { 
            OnPlayerLeftEvent?.Invoke(otherPlayer);
        }

        /// <summary>
        /// ルームに参加するか作成する関数
        /// </summary>
        private void JoinOrCreateRoom()
        {
            PhotonNetwork
                .JoinOrCreateRoom
                    (_roomName,
                        new RoomOptions { MaxPlayers = _maxPlayers },
                        TypedLobby.Default);
        }
    }
}