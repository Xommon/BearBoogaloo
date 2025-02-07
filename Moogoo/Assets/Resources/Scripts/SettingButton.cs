using UnityEngine;
using TMPro;
using UnityEngine.UI;

[ExecuteAlways]
public class SettingButton : MonoBehaviour
{
    public TextMeshProUGUI textDisplay;
    private GameManager gameManager;
    public int settingIndex;

    // Update is called once per frame
    void Update()
    {
        if (gameManager == null)
        {
            gameManager = FindObjectOfType<GameManager>();
        }

        switch (settingIndex)
        {
            case 0:
                textDisplay.text = $"{Language.language[11, gameManager.languageIndex]}\n<size=40>{Language.language[gameManager.maxBet, gameManager.languageIndex]}";
                break;
            case 1:
                int boardsCount = 0;
                foreach (Board board in gameManager.boards)
                {
                    if (board.gameObject.activeInHierarchy)
                    {
                        boardsCount++;
                    }
                }
                textDisplay.text = $"{Language.language[12, gameManager.languageIndex]}\n<size=40>{Language.language[boardsCount, gameManager.languageIndex]}";
                break;
            case 2:
                textDisplay.text = gameManager.betScore ? $"{Language.language[13, gameManager.languageIndex]}\n<size=40>{Language.language[1, gameManager.languageIndex]}" : $"{Language.language[13, gameManager.languageIndex]}\n<size=40>{Language.language[2, gameManager.languageIndex]}";
                break;
            case 3:
                textDisplay.text = $"{Language.language[14, gameManager.languageIndex]}\n\n ";
                break;
        }

        if (!gameManager.startButton.gameObject.activeInHierarchy && Application.isPlaying)
        {
            gameObject.SetActive(false);
        }
    }
}
