using UnityEngine;
using UnityEngine.UI;
using TMPro;

[ExecuteAlways]
public class Slidershow : MonoBehaviour
{
    [Range(0,7)]
    public int slideshowIndex;
    public Image image;
    public Sprite[] screenshots;
    public TextMeshProUGUI textDisplay;
    private GameManager gameManager;

    void Update()
    {
        gameManager = gameManager == null ? FindFirstObjectByType<GameManager>() : gameManager;
        image.sprite = screenshots[slideshowIndex];
        textDisplay.text = Language.language[29 + slideshowIndex, gameManager.languageIndex];

        if ((Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D)) && slideshowIndex < screenshots.Length - 1)
        {
            slideshowIndex++;
        }
        else if ((Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A)) && slideshowIndex > 0)
        {
            slideshowIndex--;
        }
    }

    void OnDisable()
    {
        slideshowIndex = 0;
    }
}
