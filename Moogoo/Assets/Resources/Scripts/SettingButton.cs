using UnityEngine;
using TMPro;
using UnityEngine.UI;

[ExecuteAlways]
public class SettingButton : MonoBehaviour
{
    public TextMeshProUGUI textDisplay;
    private GameManager gameManager;
    public bool betDisplay;

    // Update is called once per frame
    void Update()
    {
        if (gameManager == null)
        {
            gameManager = FindObjectOfType<GameManager>();
        }

        if (betDisplay)
        {
            textDisplay.text = $"Max\nBet\n<size=58>{gameManager.maxBet}";
        }
        else
        {
            int boardsCount = 0;
            foreach (Board board in gameManager.boards)
            {
                if (board.gameObject.activeInHierarchy)
                {
                    boardsCount++;
                }
            }
            textDisplay.text = $"Boards\n<size=48>{boardsCount}";
        }

        if (!gameManager.startButton.gameObject.activeInHierarchy && Application.isPlaying)
        {
            gameObject.SetActive(false);
        }
    }
}
