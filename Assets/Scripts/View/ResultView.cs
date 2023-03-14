using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FourthTermPresentation
{
    /// <summary>
    /// WIN/LOSEのアニメーション演出を管理するコンポーネント
    /// </summary>
    [DisallowMultipleComponent]
    public class ResultView : MonoBehaviour
    {
        [SerializeField]
        private Graphic _backImage = null; // GraphicクラスはImageやTMP_TextといったUIの抽象クラス

        [SerializeField]
        private Graphic _winText = null;

        [SerializeField]
        private Graphic _loseText = null;

        [SerializeField]
        private GameObject _fxWin = null;

        [SerializeField]
        private GameObject _fxLose = null;

        public void ShowWin()
        {
            _fxWin.SetActive(true);
            _backImage.gameObject.SetActive(true);
            _winText.gameObject.SetActive(true);
            //Animate(_winText, 0.5f);
        }

        public void ShowLose()
        {
            _fxLose.SetActive(true);
            _backImage.gameObject.SetActive(true);
            _loseText.gameObject.SetActive(true);
            //Animate(_loseText, 0.5f);
        }

        async private void Animate(Graphic textObject, float duration)
        {
            _backImage.gameObject.SetActive(true);
            textObject.gameObject.SetActive(true);

            var t = 0.0f;
            while (t < duration)
            {
                t = Mathf.Min(t + Time.deltaTime, duration);
                {
                    var color = textObject.color;
                    color.a = Mathf.Lerp(0, 1, t / duration);
                    textObject.color = color;
                }
                {
                    var color = _backImage.color;
                    color.a = Mathf.Lerp(0, 0.8f, t / duration);
                    _backImage.color = color;
                }
                await UniTask.NextFrame();
            }
        }
    }
}