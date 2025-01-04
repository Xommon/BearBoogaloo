using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[ExecuteAlways]
public class EndGameWindow : MonoBehaviour
{
    public GameManager gameManager;
    public ScoreEntry[] scoreEntries;
    public Dictionary<string, int> scores = new Dictionary<string, int>();

    void Update()
    {
        for (int i = 0; i < scoreEntries.Length; i++)
        {
            scoreEntries[i].gameObject.SetActive(gameManager.players[i].gameObject.activeInHierarchy);
        }
    }
    
    void OnEnable()
    {
        scores.Clear();

        // Scores from players
        foreach (PlayerEntry player in gameManager.players)
        {
            scores.Add(player.name, player.score);
        }

        // Order 
        scores = scores.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);

        // Extract keys
        List<string> keys = scores.Keys.ToList();

        // Display scores
        for (int i = 0; i < scores.Count; i++)
        {
            scoreEntries[i].name = keys[i];
            scoreEntries[i].score = scores[keys[i]];
            scoreEntries[i].icon = gameManager.icons[gameManager.players[i].iconIndex];
        }
    }
}
