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
        #region Member Variables

        private PhotonView _photonView;

        #endregion

        #region Events

        public event Action OnReceiveStartGame;
        public event Action<int> OnCaughtSurvivor;
        public event Action OnSetIsBomber;

        #endregion

        #region Unity Methods

        private void Awake()
        {
            TryGetComponent(out _photonView);
        }

        #endregion

        #region Public Methods

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

        #endregion

        #region PunRPC Methods

        [PunRPC]
        private void StartGame()
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;
            OnReceiveStartGame?.Invoke();
            Debug.Log("Start");
        }

        [PunRPC]

        private void CaughtSurvivor(int id)
        {
            OnCaughtSurvivor?.Invoke(id);
            Debug.Log("誰かが捕まった");
        }

        [PunRPC]
        private void SetIsBomber()
        {
            OnSetIsBomber?.Invoke();
        }

        #endregion
    }

}