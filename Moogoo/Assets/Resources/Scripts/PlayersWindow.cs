using UnityEngine;

public class PlayersWindow : MonoBehaviour
{
    private GameManager gameManager;

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    public void StartGame()
    {
        gameManager.SetupGame();
    }
}
