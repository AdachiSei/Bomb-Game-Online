using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FourthTermPresentation.Manager
{
    /// <summary>
    /// PhotonのRPC送受信を担うコンポーネント
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(PhotonView))]
    public class RPCManager : MonoBehaviour
    {
        private PhotonView _photonView;

        public event Action OnReceiveStartGame;
        public event Action<int> OnCaughtSurvivor;
        public event Action OnSetIsBomber;

        private void Awake()
        {
            TryGetComponent(out _photonView);
        }

        public void SendStartGame()
        {
            if (PhotonNetwork.PlayerList.Length <= 1) return;
            _photonView.RPC(nameof(StartGame), RpcTarget.AllViaServer);
        }

        public void SendCaughtSurvivor(int id)
        {
            _photonView.RPC(nameof(CaughtSurvivor), RpcTarget.AllViaServer, id);
        }

        public void SendSetIsBomber()
        {
            _photonView.RPC(nameof(SetIsBomber), RpcTarget.AllViaServer);
        }

        [PunRPC]
        private void StartGame()
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;
            OnReceiveStartGame?.Invoke();
#if UNITY_EDITOR
            Debug.Log("Start");
#endif
        }

        [PunRPC]

        private void CaughtSurvivor(int id)
        {
            OnCaughtSurvivor?.Invoke(id);
#if UNITY_EDITOR
            Debug.Log("誰かが捕まった");
#endif
        }

        [PunRPC]
        private void SetIsBomber()
        {
            OnSetIsBomber?.Invoke();
        }
    }

}