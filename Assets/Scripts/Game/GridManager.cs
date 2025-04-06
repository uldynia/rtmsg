using UnityEngine;
using System.Collections.Generic;
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
    private Vector2Int gridMap;

    [SerializeField]
    private float gridSize;

    [SerializeField]
    private ObjectToPlace objectToPlace;

    // Ease of access
    public List<Vector2Int> coveredGrids;

    public List<Vector2Int> spawnedMilk = new();

    private void Awake()
    {
        instance = this;
        objectToPlace.gameObject.SetActive(false);
        coveredGrids = new List<Vector2Int>();
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
    public Vector2Int GetMap()
    {
        return gridMap;
    }

    public float GetGridSize()
    {
        return gridSize;
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

        // In Bounds, check for grid number ( 0,0 for bottom left )
        Vector2Int gridCoord = GetGridCoordinate(item.transform.position);

        // Check if a stationary object is there
        if (coveredGrids.Contains(gridCoord))
        {
            // Not in boundary, return
            item.canvasGroup.alpha = 1;
            objectToPlace.gameObject.SetActive(false);
            return;
        }

        // In Bounds, now snap the object
        item.canvasGroup.alpha = 0;

        // Check if there is an animation for this, if have, use it, else, do not
        if (item.animal_type.SkeletonData != null)
        {
            objectToPlace.skeletonAnimation.gameObject.SetActive(true);
            objectToPlace.sr.enabled = false;
            // Modify and apply values, finally initialising the skeleton data asset
            objectToPlace.skeletonAnimation.skeletonDataAsset = item.animal_type.SkeletonData;
            objectToPlace.skeletonAnimation.transform.localScale = item.animal_type.SkeletonScale;
            objectToPlace.skeletonAnimation.AnimationName = item.animal_type.AnimationIdleName;
            objectToPlace.skeletonAnimation.transform.localPosition = item.animal_type.SkeletonOffset;
            objectToPlace.skeletonAnimation.Initialize(true);
        }
        else
        {
            objectToPlace.skeletonAnimation.gameObject.SetActive(false);
            objectToPlace.sr.enabled = true;
            objectToPlace.sr.sprite = item.image_item.sprite;
        }
        objectToPlace.gameObject.SetActive(true);

        objectToPlace.transform.position = new Vector3(gridCoord.x + gridSize * 0.5f + Xboundary.x, gridCoord.y + gridSize * 0.5f + Yboundary.x, item.transform.position.z);
    }

    public void OnInventoryItemDrop(InventoryItem item)
    {
        if (item.animal_type == null)
        {
            return;
        }
        objectToPlace.gameObject.SetActive(false);
        item.canvasGroup.alpha = 1;
        //Check boundary ( make sure in lower half )
        if (item.transform.position.x <= Xboundary.x || item.transform.position.x >= Xboundary.y ||
            item.transform.position.y <= Yboundary.x || item.transform.position.y >= (Yboundary.y + Yboundary.x) * 0.5f)
        {
            return;
        }


        // In Bounds, check for grid number ( 0,0 for bottom left )
        Vector2Int gridCoord = GetGridCoordinate(item.transform.position);

        // Check if a stationary object is there
        if (coveredGrids.Contains(gridCoord))
        {
            return;
        }

        Vector3 spawnPosition = new Vector3(gridCoord.x + gridSize * 0.5f + Xboundary.x, gridCoord.y + gridSize * 0.5f + Yboundary.x, item.transform.position.z);

        if(TransportManager.instance.tutorialMode)
        {
            if(TutorialPlayer.instance.stage == TutorialPlayer.TUTORIALSTAGE.MERGE)
            {
                Debug.LogWarning("Attempted to deploy unit during the Merge tutorial.");
                return;
            }
            else if(TutorialPlayer.instance.stage == TutorialPlayer.TUTORIALSTAGE.DEPLOY)
            {
                TutorialPlayer.instance.DeployTutorialComplete();
            }
        }

        // THIS IS BAD! NO SECURITY TO DO THE SPAWN ON THE CLIENT SIDE!! BAD!
        PlayerController.localPlayer.SpawnEntity(item.animal_type.EntityID, spawnPosition, item.animal_type.Level);

        // Remove the item
        item.InitialiseItem(null);
    }

    public Vector2Int GetGridCoordinate(Vector3 position)
    {
        Vector2 itemOffset = position - new Vector3(Xboundary.x, Yboundary.x);

        Vector2Int gridCoord = new(Mathf.FloorToInt(itemOffset.x / gridSize), Mathf.FloorToInt(itemOffset.y / gridSize));

        return gridCoord;
    }
}
