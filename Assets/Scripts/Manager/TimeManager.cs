using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace FourthTermPresentation.Manager
{
    /// <summary>
    /// 時間を管理するマネージャー
    /// </summary>
    public class TimeManager : MonoBehaviour
    {
        #region Public Property

        public float Timer => _timer;

        #endregion

        #region Inspector Member

        [SerializeField]
        private RPCManager _rpcManager = null;

        [SerializeField]
        [Header("タイマーのテキスト")]
        TMP_Text _timerText = null;

        [SerializeField]
        [Header("制限時間")]
        private float _limitTime = 30f;

        [SerializeField]
        [Header("タイマー")]
        private float _timer = 0f;

        #endregion

        #region Private Member

        private bool _isCounting = false;

        #endregion

        #region Unity Method

        private void Awake()
        {
            Init();
            _rpcManager.OnReceiveStartGame += StartTimer;
        }

        #endregion

        #region Public Methods

        public void StartTimer()
        {
            if (_isCounting) return;
            _isCounting = true;
            CountTimer();
        }

        public void StopTimer()
        {
            _isCounting = false;
        }

        public void Init()
        {
            _timer = _limitTime;
        }

        #endregion

        #region Private Method

        async private void CountTimer()
        {
            while (_isCounting)
            {
                await UniTask.NextFrame();
                _timer -= Time.deltaTime;
                if (_timer <= 0f) _timerText.text = "0";
                else _timerText.text = _timer.ToString("f0");
            }
        }

        #endregion
    }
}