using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

using DG.Tweening;
using Spine.Unity;

public class InventoryItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    //Set to public for convenience cus another script wants to access it, 
    [Header("UI")]
    public Image image_item;
    public Image image_stat_icon;
    public TextMeshProUGUI stat_tmp;
    public bool flip_image = true;
    public CanvasGroup canvasGroup;
    public SkeletonGraphic skeletonGraphic;

    [Header("References")]
    [SerializeField] Sprite attack_sprite;
    [SerializeField] Sprite defence_sprite;

    InventoryManager inventory_manager;
    DG.Tweening.Sequence animation_appear_sequence;

    private void Start()
    {
        //Expensive, can be optimised, but anyway shldnt be called more than once per game
        //Inventory Items are always there, an empty slow just has a NULL animal type
        inventory_manager = GameObject.Find("InventoryManager").GetComponent<InventoryManager>();
        parentAfterDrag = transform.parent;


        animation_appear_sequence = DOTween.Sequence();
        animation_appear_sequence.Append(transform.DOScale(new Vector3(0, 0, 1), 0));
        animation_appear_sequence.Append(transform.DOScale(new Vector3(0.85f, 0.85f, 1), 1f).SetEase(Ease.OutBounce));
        animation_appear_sequence.SetAutoKill(false);
    }

    [HideInInspector] public Transform parentAfterDrag;
    [HideInInspector] public AnimalType animal_type;


    public void InitialiseItem(AnimalType animal_type)
    {
        this.animal_type = animal_type;

        if (animal_type == null)
        {
            image_item.color = new(0, 0, 0, 0);
            skeletonGraphic.gameObject.SetActive(false);
        }
        else if (animal_type.SkeletonData != null)
        {
            image_item.color = new(0, 0, 0, 0);
            skeletonGraphic.gameObject.SetActive(true);
            skeletonGraphic.skeletonDataAsset = animal_type.SkeletonData;
            skeletonGraphic.startingAnimation = animal_type.AnimationIdleName;
            skeletonGraphic.transform.localPosition = animal_type.SkeletonUIOffset;
            skeletonGraphic.transform.localScale = animal_type.SkeletonUIScale;
            skeletonGraphic.Initialize(true);
        }
        else
        {
            image_item.color = new(1,1, 1, 1);
            skeletonGraphic.gameObject.SetActive(false);
            image_item.sprite = animal_type.Icon;
        }

        //Disable if null
        if (animal_type == null)
        {
            image_stat_icon.gameObject.SetActive(false);
        }
        //Else not null, set the stats as well
        else
        {
            image_stat_icon.gameObject.SetActive(true);
            image_stat_icon.sprite = animal_type.Stationary ? defence_sprite : attack_sprite;
            stat_tmp.text = animal_type.Health.ToString();

            AnimationAppear();
        }
    }

    //Drag and Drop
    public void OnBeginDrag(PointerEventData eventData)
    {
        image_item.raycastTarget = false;
        parentAfterDrag = transform.parent;
        transform.SetParent(transform.root);

        //Select the slot underneath
        InventorySlot slot = parentAfterDrag.GetComponent<InventorySlot>();
        inventory_manager.ChangeSelectedSlot(slot);
    }
    public void OnDrag(PointerEventData eventData)
    {
        if (animal_type == null)
            return;

        Vector2 mousepos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = new Vector3(mousepos.x, mousepos.y, 0);

        //Update event to gridmanager, position might be overriden by this to snap into place
        GridManager.instance.OnInventoryItemDrag(this);

        foreach(InventorySlot slot in inventory_manager.inventoryslot)
        {
            if (slot.my_inventory_item.animal_type == null || slot.my_inventory_item == this)
            { 
                slot.ChangeImageColor(new(1, 1, 1, 1));
            }
            else if (slot.my_inventory_item.animal_type.CanMergeWith(animal_type, out AnimalType result))
            {
                slot.ChangeImageColor(new(0, 1, 0, 1));
            }
            else
            {
                slot.ChangeImageColor(new(1, 0, 0, 1));
            }
        }
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        image_item.raycastTarget = true;
        transform.SetParent(parentAfterDrag);

        //Select the slot underneath
        InventorySlot slot = parentAfterDrag.GetComponent<InventorySlot>();
        inventory_manager.ChangeSelectedSlot(slot);

        //Update event to gridmanager
        GridManager.instance.OnInventoryItemDrop(this);

        //Set position back
        GetComponent<RectTransform>().localPosition = Vector3.zero;

        foreach (InventorySlot slot2 in inventory_manager.inventoryslot)
        {
                slot2.ChangeImageColor(new(1, 1, 1, 1));
        }
    }


    //Select
    public void OnClick()
    {
        parentAfterDrag?.GetComponent<InventorySlot>().OnClick();
    }
    //Lerping ANimation
    public void MergeLerp(Transform target)
    {
        GameObject clone = Instantiate(gameObject, transform.parent);
        clone.GetComponent<CanvasGroup>().interactable = false;
        clone.GetComponent<CanvasGroup>().blocksRaycasts = false;
        clone.transform.DOMove(target.position, 0.3f);
        Destroy(clone, 0.3f);
    }
    public void AnimationAppear()
    {
        animation_appear_sequence.Rewind();
        animation_appear_sequence.Play();
    }
}
