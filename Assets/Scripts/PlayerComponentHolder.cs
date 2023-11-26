using Cysharp.Threading.Tasks;
using FourthTermPresentation.GamePlayer;
using TMPro;
using UniRx;
using UnityEngine;

namespace FourthTermPresentation
{
    /// <summary>
    /// 1チームあたりのコンポーネントをまとめるためのコンポーネント
    /// </summary>
    [DisallowMultipleComponent]
    public class PlayerComponentHolder : MonoBehaviour
    {
        public string PlayerName => _playerNameText.text;
        public string JobText => _jobText.text;
        public PlayerController Player => _player;

        [SerializeField]
        private TMP_Text _playerNameText = null;

        [SerializeField]
        private TMP_Text _jobText = null;

        [SerializeField]
        private PlayerController _player = null;

        private void Awake()
        {
            PlayerPresenter();
        }

        public void SetPlayerNameText(string name)
        {
            _playerNameText.text = name;
        }

        public void SetJobText(string job)
        {
            _jobText.text = job;
        }

        public void SetPlayer(PlayerController player)
        {
            _player = player;
        }

        private async void PlayerPresenter()
        {
            await UniTask.WaitUntil(() => _player != null);

            _player
                .ObserveEveryValueChanged(player => player.IsBomber)
                .Subscribe(x =>
                {
                    if (x == true) SetJobText("Bomber");
                    else SetJobText("");
                })
                .AddTo(this);
        }
    }
}