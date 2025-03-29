using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IDropHandler
{
    [Header("References")]
    public InventoryItem my_inventory_item; //for convenient access
    public Image image;
    public Color selected_color, not_selected_color;

    bool selected = false;


    private void Awake()
    {
        my_inventory_item = transform.GetChild(0).GetComponent<InventoryItem>();
        Deselect();
    }

    public void Select() {
        image.color = selected_color;
        selected = true;
    }

    public void Deselect()
    {
        image.color = not_selected_color;
        selected = false;
    }


    public void OnClick()
    {
        if (selected) {
            Deselect();
            GameObject.Find("InventoryManager").GetComponent<InventoryManager>().ChangeSelectedSlot(null);
        }
        else {
            Select();
            GameObject.Find("InventoryManager").GetComponent<InventoryManager>().ChangeSelectedSlot(this);
        }
           
    }

    //Drag and Drop
    public void OnDrop(PointerEventData eventData)
    {
        InventoryItem other_inventory_item = eventData.pointerDrag.GetComponent<InventoryItem>();

        //Cannot merge with own tile
        if (my_inventory_item == other_inventory_item)
        {
            return; //IGNORE
        }
        //If both empty, ignore
        if (my_inventory_item.animal_type == null && other_inventory_item.animal_type == null)
        {
            return; //IGNORE
        }
        //If Empty, Move there
        if ((my_inventory_item.animal_type != null && other_inventory_item.animal_type == null)
            || (my_inventory_item.animal_type == null && other_inventory_item.animal_type != null))
        {
            AnimalType type = my_inventory_item.animal_type;
            my_inventory_item.InitialiseItem(other_inventory_item.animal_type);
            other_inventory_item.InitialiseItem(type);
        }
        //If Can Merge, Merge
        else if (my_inventory_item.animal_type.CanMergeWith(other_inventory_item.animal_type, out AnimalType result))
        {
            //SPECIAL CASE
            //Check for the special case which is sheep, and create a instance of scriptable with improved stats
            if (result.Name == "Cotton Ball of Sheeps")
            {
                //Create a clone n alter the clone's base stats
                result = Instantiate(result);
                result.AddHealth(2);
            }
            if (my_inventory_item.animal_type.EntityID.Equals(other_inventory_item.animal_type.EntityID)) // Add level if of same type
            {
                result.AddLevel();
            }
            else // Reset level if different types
            {
                result.ResetLevel();
            }
            my_inventory_item.InitialiseItem(result);
            other_inventory_item.InitialiseItem(null);
        }
        //If Cannot Merge, Come Back
        else
        {
            //Merge invalid n failed, display feedback
        }
    }
}
