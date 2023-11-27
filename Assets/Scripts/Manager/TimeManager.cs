using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

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
            CountTimer().Forget();
        }

        public void StopTimer()
        {
            _isCounting = false;
        }

        public void Init()
        {
            _timer = _limitTime;
        }

        private async UniTask CountTimer()
        {
            while (_isCounting)
            {
                await UniTask.NextFrame();

                _timer -= Time.deltaTime;
                _timerText.color = _timer <= 10f ? Color.red : Color.white;
                _timerText.text = _timer <= 0f ? "0" : _timer.ToString("f0");
            }
        }
    }
}