using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager m_instance = null;
    private void Awake()
    {
        m_instance = this;

        ChangeSelectedSlot(null);
    }

    [Header("Details")]
    [SerializeField] List<AnimalType> base_animal_types;
    [SerializeField] float spawn_interval;

    private float spawn_timer = 0;

    public InventorySlot[] inventoryslot;
    [HideInInspector] public InventorySlot selected_slot;

    private void Start()
    {
        EmptyAllSlots();
    }
    private void Update()
    {
        if (spawn_timer > 0)
        {
            spawn_timer -= Time.deltaTime;
        }
        else
        {
            spawn_timer = spawn_interval;
            TrySpawnNewAnimal();
        }
    }



    //In the future, the server could call this
    public void EmptyAllSlots()
    {
        for (int i = 0; i < inventoryslot.Length; ++i)
        {
            inventoryslot[i].my_inventory_item.InitialiseItem(null);
        }
    }
    //In the future, the server could call this
    public void TrySpawnNewAnimal()
    {
        //Select a random slot
        List<int> slots_empty = new List<int>();

        //Find the empty slots
        for (int i = 0;i< inventoryslot.Length;++i) 
        {
            if (inventoryslot[i].my_inventory_item.animal_type == null)
                slots_empty.Add(i);
        }

        //No need slots to spawn
        if (slots_empty.Count == 0)
            return;

        //Choose a random from base animal types
        int chosen_slot = slots_empty[Random.Range(0, slots_empty.Count)];
        inventoryslot[chosen_slot].my_inventory_item.InitialiseItem(base_animal_types[Random.Range(0, base_animal_types.Count)]);
    }

    public void ChangeSelectedSlot(InventorySlot new_slot)
    {
        if (selected_slot != null)
            selected_slot.Deselect();
        selected_slot = new_slot;
        if (selected_slot != null)
            selected_slot.Select();
    }
}
