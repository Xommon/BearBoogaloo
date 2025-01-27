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
                textDisplay.text = $"Max Bet\n<size=40>{gameManager.maxBet}";
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
                textDisplay.text = $"Boards\n<size=40>{boardsCount}";
                break;
            case 2:
                textDisplay.text = (gameManager.betScore) ? $"Score Type\n<size=40>1" : $"Score Type\n<size=40>2";
                break;
        }

        if (!gameManager.startButton.gameObject.activeInHierarchy && Application.isPlaying)
        {
            gameObject.SetActive(false);
        }
    }
}
