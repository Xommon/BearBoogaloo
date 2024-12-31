using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
public class PlayersHolder : MonoBehaviour
{
    public Color[] colours;
    /*private int numberOfPlayers;
    private int previousNumberOfPlayers;

    void Update()
    {
        numberOfPlayers = GetDirectChildren(transform).length;

        if (numberOfPlayers == previousNumberOfPlayers)
        {
            return;
        }

        for (int i = 0; i < numberOfPlayers; i++)
        {

        }
    }

    Transform[] GetDirectChildren(Transform parent)
    {
        Transform[] children = new Transform[parent.childCount];
        for (int i = 0; i < parent.childCount; i++)
        {
            children[i] = parent.GetChild(i);
        }
        return children;
    }*/
}
