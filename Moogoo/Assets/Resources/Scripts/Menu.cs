using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Menu : MonoBehaviour
{
    public TMP_InputField inputField;
    public Button soloButton;
    public Button hostButton;
    public Button joinButton;

    void Update()
    {
        hostButton.enabled = inputField.text != "";
        joinButton.enabled = inputField.text != "";
    }

    public void SoloButton()
    {
        gameObject.SetActive(false);
    }
}
