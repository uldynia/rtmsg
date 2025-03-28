using UnityEngine;
/// <summary>
/// Class that handles spawning and snapping of units to their respective positions in world position
/// </summary>
public class GridManager : MonoBehaviour
{
    public static GridManager instance { get; private set; } // makes it so that other scripts cant change the singleton instance

    // Boundaries of the field, allows units to be placed inversely via EntityTransform
    [SerializeField]
    private Vector2 Yboundary;

    [SerializeField]
    private Vector2 Xboundary;

    private void Awake()
    {
        instance = this;
    }

    // Get Function
    public Vector2 GetPlaceableBoundaryY()
    {
        return Yboundary;
    }
    public Vector2 GetPlaceableBoundaryX()
    {
        return Xboundary;
    }
}
