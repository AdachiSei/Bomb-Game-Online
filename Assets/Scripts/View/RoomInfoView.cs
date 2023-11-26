using TMPro;
using UnityEngine;

namespace FourthTermPresentation
{
    /// <summary>
    /// 左上のルーム名・プレイヤー名の表示を管理するコンポーネント
    /// </summary>
    [DisallowMultipleComponent]
    public class RoomInfoView : MonoBehaviour
    {
        public string RoomName => _roomNameText.text;
        public TMP_Text[] PlayerNameText => _playerNameText;

        [SerializeField]
        private TMP_Text _roomNameText = null;

        [SerializeField]
        private TMP_Text[] _playerNameText = null;


        public string SetRoomName(string name)
        {
            return _roomNameText.text = name;
        }

        public string SetPlayerNameText(string name, int index)
        {
            return _playerNameText[index].text = name;
        }
    }
}