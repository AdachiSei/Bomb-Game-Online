using Cysharp.Threading.Tasks;
using DG.Tweening;
using Photon.Pun;
using System;
using Template.Constant;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

using RBCN = UnityEngine.RigidbodyConstraints;

namespace FourthTermPresentation.GamePlayer
{
    /// <summary>
    /// プレイヤー
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerController : MonoBehaviour
    {
        public bool IsDead { get; private set; }
        public Vector3 Velocity => _rb.velocity;
        public bool IsBomber => _isBomber;
        public int ID => _photonView.ViewID;

        [SerializeField]
        [Header("ボム")]
        private GameObject _bomb = null;

        [SerializeField]
        [Header("ボムのマテリアル")]
        private Material _bombMaterial = null;

        [SerializeField]
        [Header("歩くスピード")]
        private float _walkSpeed = 3f;

        [SerializeField]
        [Header("走るスピード")]
        private float _runSpeed = 6f;

        [SerializeField]
        [Header("鬼なのか")]
        private bool _isBomber = false;

        private Rigidbody _rb = null;
        private Animator _animator = null;
        private PhotonView _photonView = null;
        private Subject<int> _idSubject = new Subject<int>();
        private IDisposable _move = null;
        private IInputPlayer _plauyerInput = new PlayerInput();

        public event Action<int> SendCaughtSurvivor;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            _animator = GetComponent<Animator>();
            _photonView = GetComponent<PhotonView>();

            _bomb.gameObject.SetActive(false);

            if (!_photonView.IsMine)
                return;

            _move = this
                .FixedUpdateAsObservable()
                .Subscribe(_ => OnMove())
                .AddTo(this);

            _idSubject
                .ThrottleFirst(TimeSpan.FromSeconds(0f))
                .Subscribe(_ => ChangeBomb(_));
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (!collision.gameObject.TryGetComponent(out PlayerController player))
                return;

            if (!_photonView.IsMine || IsDead || player.IsDead)
                return;

            if (!_isBomber && player.IsBomber)
                _idSubject.OnNext(player.ID);
        }

        private void ChangeBomb(int enemyID)
        {
            SendCaughtSurvivor?.Invoke(_photonView.ViewID);
            SendCaughtSurvivor?.Invoke(enemyID);
        }

        public void SetIsBomb(int id = 0)
        {
            if (_photonView.ViewID != id && id != 0)
                return;

            _isBomber = _isBomber ? false : true;
            _bomb.gameObject.SetActive(_isBomber);
        }

        public void DeleteBomb()
        {
            _isBomber = false;
            _bomb.gameObject.SetActive(false);
        }

        public void DeactivatePlayer()
        {
            _bomb.gameObject.SetActive(false);
            IsDead = true;
            _animator.SetBool("IsTripping", true);
            _rb.constraints = RBCN.FreezeAll;
            _move?.Dispose();
        }

        public void ChangeBomber()
        {
            if (!_photonView.IsMine || !PhotonNetwork.IsMasterClient)
                return;

            _isBomber = true;
            _bomb.gameObject.SetActive(true);
        }

        public async UniTask ChangeColor()
        {
            if (!_photonView.IsMine)
                return;

            _bombMaterial.color = Color.black;

            await UniTask.Delay(TimeSpan.FromSeconds(10f));

            _bombMaterial.DOKill();

            await _bombMaterial.DOColor(Color.yellow, 10f);

            _bombMaterial.DOKill();

            await _bombMaterial.DOColor(Color.red, 10f);

            _bombMaterial.DOKill();
            _bombMaterial.color = Color.black;
        }

        private void OnMove()
        {
            var h = _plauyerInput.H;
            var v = _plauyerInput.V;
            var y = _rb.velocity.y;

            var speed =
                !_plauyerInput.Fire3 ?
                    _walkSpeed : _runSpeed;

            if (!_plauyerInput.Fire3)
                _animator.SetBool("IsRunning", false);
            else
                _animator.SetBool("IsRunning", true);

            // カメラの方向から、X-Z平面の単位ベクトルを取得
            var forward = Camera.main.transform.forward;
            var offset = new Vector3(1, 0, 1);
            var cameraForward =
                Vector3.Scale(forward, offset).normalized;

            // 方向キーの入力値とカメラの向きから、移動方向を決定
            var moveForward =
                cameraForward * v + Camera.main.transform.right * h;

            // 移動方向にスピードを掛ける。
            // ジャンプや落下がある場合は、別途Y軸方向の速度ベクトルを足す
            var velocity = moveForward * speed + new Vector3(0, y, 0);

            _rb.velocity = velocity;

            if (moveForward != Vector3.zero)
                transform.rotation = Quaternion.LookRotation(moveForward);

            _animator.SetFloat("Speed", velocity.magnitude);
        }
    }
}