using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System;

[ExecuteAlways]
public class PlayerEntry : MonoBehaviour
{
    public GameManager gameManager;

    public TextMeshProUGUI nameDisplay;
    public Image iconDisplay;
    private RectTransform iconRT;
    public TextMeshProUGUI scoreDisplay;
    private RectTransform scoreRT;
    public Image background;
    public AI ai;

    public string name;
    public int playerIndex;
    public int iconIndex;
    public int score;
    [HideInInspector]
    public int oldScore;
    public Image reaction;
    private Animator reactionAnimator;
    public List<string> hand = new List<string>();

    void Start()
    {
        // Change chip colour at beginning of game
        if (ai != null)
        {
            ChangeIcon(false);
        }

        playerIndex = (ai != null) ? ai.playerIndex : 0;
    }

    void Update()
    {
        gameManager = (gameManager == null) ? FindObjectOfType<GameManager>() : gameManager;
        scoreRT = (scoreRT == null) ? scoreDisplay.GetComponent<RectTransform>() : scoreRT;
        iconRT = (iconRT == null) ? iconDisplay.GetComponent<RectTransform>() : iconRT;
        nameDisplay.text = name;
        scoreDisplay.text = Language.language[score, gameManager.languageIndex];

        // Switch to RTL or LTR
        /*scoreRT.anchoredPosition = (gameManager.languageIndex > 1) ? new Vector2(-55, 0) : new Vector2(100, 0);
        iconRT.anchoredPosition = (gameManager.languageIndex > 1) ? new Vector2(55, -1) : new Vector2(-92, -1);
        nameDisplay.isRightToLeftText = gameManager.languageIndex > 1;
        nameDisplay.alignment = gameManager.languageIndex > 1 ? TextAlignmentOptions.Right : TextAlignmentOptions.Left;*/

        // Highlight when it's the player's turn
        background.color = (gameManager.turn == transform.GetSiblingIndex()) ? new Color(1.0f, 0.88f, 0.0f, 1.0f) : background.color = new Color(0.53f, 0.65f, 0.72f, 0.75f);
        
        iconDisplay.sprite = gameManager.icons[iconIndex];
        iconDisplay.color = Color.white;

        if (reaction != null && reactionAnimator == null)
        {
            reactionAnimator = reaction.GetComponent<Animator>();
        }
    }

    public void ChangeIcon(bool buttonClicked)
    {
        // Play sound
        if (buttonClicked)
        {
            AudioManager.Play("chip0", "chip1", "chip2");
        }

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
        
        // Save the value for player
        PlayerPrefs.SetInt($"IconIndex_{playerIndex}", iconIndex);
        PlayerPrefs.Save();
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
            int imageIndex = UnityEngine.Random.Range(0, 3);
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
            int imageIndex = UnityEngine.Random.Range(4, 7);
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
            int imageIndex = UnityEngine.Random.Range(7, 9);
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
        /*if (transform.GetSiblingIndex() == 0)
        {
            return;
        }

        ChangeIcon(false);

        // Find all active player names
        var activeNames = gameManager.players
                    .Where(player => player.gameObject.activeInHierarchy)
                    .Select(player => player.name)
                    .ToHashSet();

        // UnityEngine.Randomly assign a unique name from playerNames
        string randomName;
        do
        {
            randomName = gameManager.cpuNames[UnityEngine.Random.Range(0, gameManager.cpuNames.Length)];
        } while (activeNames.Contains(randomName));

        name = randomName;*/
    }
}
