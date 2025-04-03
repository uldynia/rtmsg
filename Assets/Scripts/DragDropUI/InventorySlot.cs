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

    [Header("SFX")]
    [SerializeField] AudioClip merge_sfx;
    [SerializeField] AudioClip error_sfx;

    bool selected = false;
    InventoryManager inventory_manager;

    private void Awake()
    {
        my_inventory_item = transform.GetChild(0).GetComponent<InventoryItem>();
        inventory_manager = GameObject.Find("InventoryManager").GetComponent<InventoryManager>();
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
            //Deselect();
            inventory_manager.ChangeSelectedSlot(null);
        }
        //Check if smth is selected, den try merge
        else if (inventory_manager.selected_slot != null)
        {
            //Merge for tapping
            TryMerge(inventory_manager.selected_slot.my_inventory_item);

            //Deselect();
            inventory_manager.ChangeSelectedSlot(null);
        }
        else {
            //Select();
            inventory_manager.ChangeSelectedSlot(this);
        }

        Debug.Log("Triggered");
    }


    //Returns true if merged
    public bool TryMerge(InventoryItem other_inventory_item)
    {
        //Cannot merge with own tile
        if (my_inventory_item == other_inventory_item)
        {
            return false; //IGNORE
        }
        //If both empty, ignore
        if (my_inventory_item.animal_type == null && other_inventory_item.animal_type == null)
        {
            return false; //IGNORE
        }
        //If Empty, Move there
        if ((my_inventory_item.animal_type != null && other_inventory_item.animal_type == null)
            || (my_inventory_item.animal_type == null && other_inventory_item.animal_type != null))
        {
            AnimalType type = my_inventory_item.animal_type;
            my_inventory_item.InitialiseItem(other_inventory_item.animal_type);
            other_inventory_item.InitialiseItem(type);
            return false;
        }
        //If Can Merge, Merge
        else if (my_inventory_item.animal_type.CanMergeWith(other_inventory_item.animal_type, out AnimalType result))
        {
            //SPECIAL CASE
            //Check for the special case which is sheep, and create a instance of scriptable with improved stats
            bool isAdd = false;
            if (result.Name == "Cotton Ball of Sheeps")
            {
                if (!(other_inventory_item.animal_type.Level == 2 && my_inventory_item.animal_type.Level == 2))
                {
                    result.AddLevel(1);
                    if (other_inventory_item.animal_type.Level == 2 || my_inventory_item.animal_type.Level == 2)
                    {
                        result.AddLevel(1);
                        isAdd = true;
                    }
                }
                //Create a clone n alter the clone's base stats
                result = Instantiate(result);
                if (isAdd && (other_inventory_item.animal_type.Level == 2 || my_inventory_item.animal_type.Level == 2))
                {
                    result.AddHealth(2);
                }
                result.AddHealth(2);
            }
            //else if (my_inventory_item.animal_type.EntityID == other_inventory_item.animal_type.EntityID)
            //{
            //    //Reset and add level if of same type
            //    result.ResetLevel();
            //    result.AddLevel(my_inventory_item.animal_type.Level);
            //}
            other_inventory_item.MergeLerp(my_inventory_item.transform);

            if(TransportManager.instance.tutorialMode)
            {
                TutorialPlayer.instance.MergeTutorialComplete();
                TutorialPlayer.instance.MergeAnimalsTutorialComplete(my_inventory_item.animal_type, other_inventory_item.animal_type);
            }

            my_inventory_item.InitialiseItem(result);
            other_inventory_item.InitialiseItem(null);
            AudioSfxManager.m_instance.OnPlayNewAudioClip(merge_sfx);

            return true;
        }
        //If Cannot Merge, Come Back
        else
        {
            //Merge invalid n failed, display feedback
            AudioSfxManager.m_instance.OnPlayNewAudioClip(error_sfx);
            return false;
        }
    }

    //Drag and Drop
    public void OnDrop(PointerEventData eventData)
    {
        InventoryItem other_inventory_item = eventData.pointerDrag.GetComponent<InventoryItem>();
        TryMerge(other_inventory_item);

        inventory_manager.ChangeSelectedSlot(null);
    }
}
