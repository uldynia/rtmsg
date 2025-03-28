using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; } // makes it so that other scripts cant change the singleton instance

    private void Awake()
    {
        instance = this;
    }
}
