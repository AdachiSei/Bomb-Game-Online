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
        public float Timer => _timer;

        [SerializeField]
        private RPCManager _rpcManager = null;

        [SerializeField]
        [Header("タイマーのテキスト")]
        private TMP_Text _timerText = null;

        [SerializeField]
        [Header("制限時間")]
        private float _limitTime = 30f;

        [SerializeField]
        [Header("タイマー")]
        private float _timer = 0f;

        private bool _isCounting = false;

        private void Awake()
        {
            Init();
            _rpcManager.OnReceiveStartGame += StartTimer;
        }

        public void StartTimer()
        {
            if (_isCounting)
                return;
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

        async private void CountTimer()
        {
            while (_isCounting)
            {
                await UniTask.NextFrame();

                _timer -= Time.deltaTime;
                if (_timer <= 0f)
                    _timerText.text = "0";
                else
                    _timerText.text = _timer.ToString("f0");
            }
        }
    }
}