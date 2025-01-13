using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

[ExecuteAlways]
public class PlayerEntry : MonoBehaviour
{
    public GameManager gameManager;

    public TextMeshProUGUI nameDisplay;
    public Image iconDisplay;
    public TextMeshProUGUI scoreDisplay;
    public Image background;
    public AI ai;

    public string name;
    public int iconIndex;
    public int score;
    [HideInInspector]
    public int oldScore;
    public Image reaction;
    private Animator reactionAnimator;
    public List<string> hand = new List<string>();

    void Update()
    {
        nameDisplay.text = name;
        scoreDisplay.text = score.ToString();

        // Highlight when it's the player's turn
        if (gameManager.turn == transform.GetSiblingIndex())
        {
            background.color = new Color(background.color.r, background.color.g, background.color.b, 0.75f);
        }
        else
        {
            background.color = new Color(background.color.r, background.color.g, background.color.b, 0.25f);
        }
        
        iconDisplay.sprite = gameManager.icons[iconIndex];
        iconDisplay.color = Color.white;

        if (reaction != null && reactionAnimator == null)
        {
            reactionAnimator = reaction.GetComponent<Animator>();
        }
    }

    public void ChangeIcon()
    {
        // Set the initial value
        int newIconIndex = 0;
        if (iconIndex < gameManager.icons.Length - 1)
        {
            newIconIndex = iconIndex + 1;
        }

        // Check to make sure it doesn't conflict with another player's icon
        while ((FindObjectsOfType<PlayerEntry>().FirstOrDefault(player => player.iconIndex == newIconIndex) != null))
        {
            if (newIconIndex < gameManager.icons.Length - 1)
            {
                newIconIndex++;
            }
            else
            {
                newIconIndex = 0;
            }
        }

        // Set the final value
        iconIndex = newIconIndex;
    }

    public void ActivateAI()
    {
        StartCoroutine(ai.PlayTurn());
    }

    public void React(int index)
    {
        StartCoroutine(ReactWithDelay(index, 0.0f));
    }

    public void React(int index, float delayTime)
    {
        StartCoroutine(ReactWithDelay(index, delayTime));
    }

    IEnumerator ReactWithDelay(int index, float time)
    {
        yield return new WaitForSeconds(time);
        reactionAnimator.enabled = true;
        reactionAnimator.Play(reactionAnimator.GetCurrentAnimatorStateInfo(0).fullPathHash, -1, 0f);
        reactionAnimator.speed = 0.4f;

        if (index == 0)
        {
            int imageIndex = Random.Range(0, 3);
            reaction.sprite = gameManager.reactionImages[imageIndex];
            if (imageIndex == 2)
            {
                reactionAnimator.Play("angry");
            }
            else
            {
                reactionAnimator.Play("happy");
            }
        }
        else if (index == 1)
        {
            int imageIndex = Random.Range(4, 7);
            reaction.sprite = gameManager.reactionImages[imageIndex];
            if (imageIndex == 6)
            {
                reactionAnimator.Play("angry");
            }
            else
            {
                reactionAnimator.Play("sad");
            }
        }
        else if (index == 2)
        {
            int imageIndex = Random.Range(7, 9);
            reaction.sprite = gameManager.reactionImages[imageIndex];
            if (imageIndex == 7)
            {
                reactionAnimator.Play("sad");
            }
            else
            {
                reactionAnimator.Play("angry");
            }
        }
    }

    void OnEnable()
    {
        if (transform.GetSiblingIndex() == 0)
        {
            return;
        }

        ChangeIcon();

        // Find all active player names
        var activeNames = gameManager.players
                    .Where(player => player.gameObject.activeInHierarchy)
                    .Select(player => player.name)
                    .ToHashSet();

        // Randomly assign a unique name from playerNames
        string randomName;
        do
        {
            randomName = gameManager.cpuNames[Random.Range(0, gameManager.cpuNames.Length)];
        } while (activeNames.Contains(randomName));

        name = randomName;
    }
}
