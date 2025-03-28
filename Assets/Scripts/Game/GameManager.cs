using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; } // makes it so that other scripts cant change the singleton instance

    [SerializeField]
    private float topY;

    [SerializeField]
    private float bottomY;

    private void Awake()
    {
        instance = this;
    }

    public Vector2 GetPlaceableBoundaryY()
    {
        return new Vector2(topY, bottomY);
    }
}
