using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class InventoryItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    //Set to public for convenience cus another script wants to access it, 
    [Header("UI")]
    public Image image_item;
    public Image image_stat_icon;
    public TextMeshProUGUI stat_tmp;
    public bool flip_image = true;

    [Header("References")]
    [SerializeField] Sprite attack_sprite;
    [SerializeField] Sprite defence_sprite;

    InventoryManager inventory_manager;
    private void Start()
    {
        //Expensive, can be optimised, but anyway shldnt be called more than once per game
        //Inventory Items are always there, an empty slow just has a NULL animal type
        inventory_manager = GameObject.Find("InventoryManager").GetComponent<InventoryManager>();
    }

    [HideInInspector] public Transform parentAfterDrag;
    [HideInInspector] public AnimalType animal_type;


    public void InitialiseItem(AnimalType animal_type)
    {
        this.animal_type = animal_type; 
        image_item.sprite = animal_type == null? null : animal_type.Icon;

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
        }
    }


    //Drag and Drop
    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("StartDrag");

        image_item.raycastTarget = false;
        parentAfterDrag = transform.parent;
        transform.SetParent(transform.root);

        //Select the slot underneath
        InventorySlot slot = parentAfterDrag.GetComponent<InventorySlot>();
        inventory_manager.ChangeSelectedSlot(slot);
    }
    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("OnDrag");

        if (animal_type == null)
            return;

        Vector2 mousepos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = new Vector3(mousepos.x, mousepos.y, 0);
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("EndDrag");

        image_item.raycastTarget = true;
        transform.SetParent(parentAfterDrag);

        //Select the slot underneath
        InventorySlot slot = parentAfterDrag.GetComponent<InventorySlot>();
        inventory_manager.ChangeSelectedSlot(slot);

        //Set position back
        GetComponent<RectTransform>().localPosition = Vector3.zero;
    }
}
