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
    //public Dictionary<string, int> scores = new Dictionary<string, int>();
    
    void OnEnable()
    {
        for (int i = 0; i < scoreEntries.Length; i++)
        {
            scoreEntries[i].gameObject.SetActive(gameManager.players[i].gameObject.activeInHierarchy);

            if (scoreEntries[i].gameObject.activeInHierarchy)
            {
                scoreEntries[i].name = gameManager.players[i].name;
                scoreEntries[i].score = gameManager.players[i].score;
                scoreEntries[i].icon = gameManager.players[i].iconIndex;
            }
        }

        // Sort and rearrange the ScoreEntries by score (highest to lowest)
        var sortedEntries = scoreEntries.OrderByDescending(entry => entry.score).ToList();

        for (int i = 0; i < sortedEntries.Count; i++)
        {
            sortedEntries[i].transform.SetSiblingIndex(i);
        }
    }
}
