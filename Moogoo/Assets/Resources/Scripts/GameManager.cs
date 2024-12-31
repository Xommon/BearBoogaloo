using UnityEngine;

[ExecuteAlways]
public class GameManager : MonoBehaviour
{
    public Player[] players;

    private void Update()
    {
        if (players == FindObjectsOfType<Player>())
        {
            return;
        }

        players = FindObjectsOfType<Player>();
    }
}
