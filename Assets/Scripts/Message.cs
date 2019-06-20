using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;

public class Message : MonoBehaviour
{
    #region SINGLETON PATTERN
    private static Message _instance;
    public static Message Instance
    {
        get {
            if (_instance != null) return _instance;
            _instance = FindObjectOfType<Message>();

            if (_instance != null) return _instance;
            GameObject container = new GameObject("Message");
            _instance = container.AddComponent<Message>();

            return _instance;
        }
    }
    #endregion

    [SerializeField] private RectTransform messagePanel;
    [SerializeField] private Text messageText;

    public void DisableText()
    {
        messagePanel.gameObject.SetActive(false);
    }

    public void SetMessage(string message)
    {
        messageText.text = message;
        messagePanel.gameObject.SetActive(true);
    }
}
