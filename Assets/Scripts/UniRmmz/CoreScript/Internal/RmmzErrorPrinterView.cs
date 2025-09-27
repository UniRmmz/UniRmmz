using TMPro;
using UnityEngine;

namespace UniRmmz
{
    /// <summary>
    /// エラー表示用のビュー
    /// </summary>
    public class RmmzErrorPrinterView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _typeText;
        [SerializeField] private TextMeshProUGUI _messageText;
        [SerializeField] private GameObject _root;

        public void Show(string type, string message)
        {
            _typeText.text = type;
            _messageText.text = message;
            _root.SetActive(true);
        }
        
        public void Hide()
        {
            _root.SetActive(false);
        }
    }
}