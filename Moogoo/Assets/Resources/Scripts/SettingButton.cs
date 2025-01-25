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
                textDisplay.text = $"Max\nBet\n<size=48>{gameManager.maxBet}";
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
                textDisplay.text = $"Boards\n<size=48>{boardsCount}";
                break;
            case 2:
                textDisplay.text = (gameManager.betScore) ? $"Bet Score\n<size=48>On" : $"Bet Score\n<size=48>Off";
                break;
        }

        if (!gameManager.startButton.gameObject.activeInHierarchy && Application.isPlaying)
        {
            gameObject.SetActive(false);
        }
    }
}
