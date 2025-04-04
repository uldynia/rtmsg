using System.Collections.Generic;
using Spine.Unity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ArchiveManager : MonoBehaviour
{
    [SerializeField] GameObject archiveScreen;
    [SerializeField] GameObject[] stages, recipe;
    [SerializeField] TextMeshProUGUI description;
    [SerializeField]
    private GameObject arrow;
    Dictionary<string, ArchiveEntry> entries = new();
    ArchiveEntry selectedEntry;

    [SerializeField] Image description_button;
    [SerializeField] Image effect_button;
    [SerializeField] Color selected_color;
    [SerializeField] Color deselected_color;
    public static ArchiveManager instance { get; private set; }
    public enum DESCRIPTORSELECTED
    {
        DESCRIPTION,
        EFFECT,
    }
    DESCRIPTORSELECTED descriptor;
    int selectedStage = 0;
    private void Start()
    {
        instance = this;
        foreach(var entry in Resources.LoadAll<ArchiveEntry>("Resources/ArchiveEntry"))
        {
            entries.Add(entry.name, entry);
        }
    }
    public void OpenRecipe(ArchiveEntry entry, int _selectedStage)
    {
        archiveScreen.SetActive(true);
        selectedEntry = entry;
        selectedStage = _selectedStage;
        for (int i = 0; i < 3; i++)
        {
            if(stages[i].transform.childCount > 0)
            {
                if (stages[i].GetComponentInChildren<ArchiveElementDisplay>().entry == entry)
                {
                    continue;
                }
                Destroy(stages[i].transform.GetChild(0).gameObject);
            }
            Instantiate(entry.mergeStages[i].gameObject, stages[i].transform).transform.localPosition = Vector3.zero;


            //stages[i].skeletonDataAsset = entry.mergeStages[i];
            //recipe[i].skeletonDataAsset = entry.recipe[i];
        }
        for (int i = 0; i < 3; i++)
        {
            if (recipe[i].transform.childCount > 0)
            {
                if (recipe[i].GetComponentInChildren<ArchiveElementDisplay>().entry == entry.recipe[i])
                {
                    Debug.Log($"Continuing {i} {recipe[i].GetComponentInChildren<ArchiveElementDisplay>().entry.name} {entry.recipe[i].name}");
                    continue;
                }
                Destroy(recipe[i].transform.GetChild(0).gameObject);
            }
            if (entry.recipe[i]?.gameObject != null)
            {
                recipe[i].SetActive(true);
                arrow.SetActive(true);
                Instantiate(entry.recipe[i], recipe[i].transform).transform.localPosition = Vector3.zero;
            }
            else
            {
                recipe[i].SetActive(false);
                arrow.SetActive(false);
            }
        }
        SetDescriptor(1);
        UpdateDescription();
    }
    public void SetDescriptor(int selected)
    {
        descriptor = (DESCRIPTORSELECTED)selected;
        UpdateDescription();

        //Update UI
        description_button.color = descriptor == DESCRIPTORSELECTED.DESCRIPTION ? selected_color : deselected_color;
        effect_button.color = descriptor == DESCRIPTORSELECTED.EFFECT ? selected_color : deselected_color;
    }
    public void UpdateDescription()
    {
        switch (descriptor)
        {
            case DESCRIPTORSELECTED.EFFECT: // set text to effect
                description.text = selectedEntry.animalEntries[selectedStage].effect;
                break;
            case DESCRIPTORSELECTED.DESCRIPTION: // set text to effect
                description.text = selectedEntry.animalEntries[selectedStage].description;
                break;
        }
    }
}
