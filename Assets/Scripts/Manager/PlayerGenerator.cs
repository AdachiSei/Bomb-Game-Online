using FourthTermPresentation.GamePlayer;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Template.Extension;
using UnityEngine;
using Cinemachine;

namespace FourthTermPresentation.Manager
{
    /// <summary>
    /// ジェネレーター
    /// </summary>
    public class PlayerGenerator : MonoBehaviour
    {
        [SerializeField]
        private ConnectionManager _connectionManager = null;

        [SerializeField]
        private RPCManager _rpcManager = null;

        [SerializeField]
        [Header("プレイヤーのプレファブ")]
        private PlayerController _playerPrefab;

        [SerializeField]
        [Header("メインカメラ")]
        private CinemachineFreeLook _freeLook = null;

        private void Awake()
        {
            _connectionManager.OnJoinedRoomGeneratePlayer += Generate;
        }

        private void Generate()
        {
            var prefabName = _playerPrefab.name;
            var pos = _playerPrefab.transform.position;
            var rot = Quaternion.identity;
            var player =  PhotonNetwork.Instantiate(prefabName, pos, rot);
            player.name = PhotonNetwork.NickName;
            _freeLook.Follow = player.transform;
            _freeLook.LookAt = player.transform;
        }
    }
}