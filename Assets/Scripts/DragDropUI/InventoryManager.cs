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
    public float spawn_interval;

    private float spawn_timer = 0;

    public InventorySlot[] inventoryslot;
    [HideInInspector] public InventorySlot selected_slot;

    private void Start()
    {
        EmptyAllSlots();

        // modified by xavier to take tutorial mode into account
        if(!TransportManager.instance.tutorialMode)
        InvokeRepeating("TrySpawnNewAnimal", 1.5F, spawn_interval);
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
    // from Xavier: moved the randomness to this function so that tutorial can fix the animals that are spawned
    int previousIndex = -1;
    int repeatedCount = 0;
    public int maxConsecutiveRepeats = 3;
    public void TrySpawnNewAnimal()
    {
        if (base_animal_types == null || base_animal_types.Count == 0)
        {
            Debug.LogError("base_animal_types list is null or empty.");
            return;
        }

        int newIndex;
        do
        {
            newIndex = Random.Range(0, base_animal_types.Count);
            if (newIndex == previousIndex)
            {
                repeatedCount++;
            }
            else
            {
                Debug.Log($"You would've gotten {repeatedCount} units in a row had it not been for divine intervention.");
                repeatedCount = 0;
            }
        } while (repeatedCount > maxConsecutiveRepeats - 1 && (Random.Range(0f, 1f) < 0.3f));

        previousIndex = newIndex;
        SpawnNewAnimal(base_animal_types[newIndex]);
    }
    public void SpawnNewAnimal(int id)
    {
        SpawnNewAnimal(base_animal_types[id]);
    }
    public void SpawnNewAnimal(AnimalType animalType)
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
        inventoryslot[chosen_slot].my_inventory_item.InitialiseItem(animalType);
        inventoryslot[chosen_slot].my_inventory_item.animal_type.ResetLevel();

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
