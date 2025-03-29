using UnityEngine;
using Mirror;
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

    [SerializeField]
    private Vector2 gridMap;

    [SerializeField]
    private float gridSize;

    [SerializeField]
    private SpriteRenderer objectToPlace;
    private void Awake()
    {
        instance = this;
        objectToPlace.gameObject.SetActive(false);
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

    public void OnInventoryItemDrag(InventoryItem item)
    {
        //Check boundary ( make sure in lower half )
        if (item.transform.position.x <= Xboundary.x || item.transform.position.x >= Xboundary.y ||
            item.transform.position.y <= Yboundary.x || item.transform.position.y >= (Yboundary.y + Yboundary.x) * 0.5f)
        {
            // Not in boundary, return
            item.canvasGroup.alpha = 1;
            objectToPlace.gameObject.SetActive(false);
            return;
        }
        item.canvasGroup.alpha = 0;
        objectToPlace.sprite = item.image_item.sprite;
        objectToPlace.gameObject.SetActive(true);
        // In Bounds, check for grid number ( 0,0 for bottom left )
        Vector2 itemOffset = item.transform.position - new Vector3(Xboundary.x, Yboundary.x);

        Vector2 gridCoord = new(Mathf.FloorToInt(itemOffset.x / gridSize), Mathf.FloorToInt(itemOffset.y / gridSize));

        objectToPlace.transform.position = new Vector3(gridCoord.x + gridSize * 0.5f + Xboundary.x, gridCoord.y + gridSize * 0.5f + Yboundary.x, item.transform.position.z);
    }

    public void OnInventoryItemDrop(InventoryItem item)
    {
        objectToPlace.gameObject.SetActive(false);
        item.canvasGroup.alpha = 1;
        //Check boundary ( make sure in lower half )
        if (item.transform.position.x <= Xboundary.x || item.transform.position.x >= Xboundary.y ||
            item.transform.position.y <= Yboundary.x || item.transform.position.y >= (Yboundary.y + Yboundary.x) * 0.5f)
        {
            return;
        }
        

        // In Bounds, check for grid number ( 0,0 for bottom left )
        Vector2 itemOffset = item.transform.position - new Vector3(Xboundary.x, Yboundary.x);

        Vector2 gridCoord = new(Mathf.FloorToInt(itemOffset.x / gridSize), Mathf.FloorToInt(itemOffset.y / gridSize));

        Vector3 spawnPosition = new Vector3(gridCoord.x + gridSize * 0.5f + Xboundary.x, gridCoord.y + gridSize * 0.5f + Yboundary.x, item.transform.position.z);

        // THIS IS BAD! NO SECURITY TO DO THE SPAWN ON THE CLIENT SIDE!! BAD!

        PlayerController.localPlayer.SpawnEntity(item.animal_type.EntityID, spawnPosition, item.animal_type.Level);

        // Remove the item
        item.InitialiseItem(null);
    }
}
