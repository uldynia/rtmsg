using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; } // makes it so that other scripts cant change the singleton instance

    // Y boundaries of the field, allows units to be placed inversely via EntityTransform
    [SerializeField]
    private float topY;

    [SerializeField]
    private float bottomY;

    private void Awake()
    {
        instance = this;
    }

    // Get Function

    public Vector2 GetPlaceableBoundaryY()
    {
        return new Vector2(topY, bottomY);
    }
}
