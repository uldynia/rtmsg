using System.Collections.Generic;
using Spine.Unity;
using TMPro;
using UnityEngine;

public class ArchiveManager : MonoBehaviour
{
    [SerializeField] GameObject archiveScreen;
    [SerializeField] GameObject[] stages, recipe;
    [SerializeField] TextMeshProUGUI description;
    Dictionary<string, ArchiveEntry> entries = new();
    ArchiveEntry selectedEntry;
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
                Destroy(stages[i].transform.GetChild(0).gameObject);
            if (recipe[i].transform.childCount > 0)
                Destroy(recipe[i].transform.GetChild(0).gameObject);
            Instantiate(entry.mergeStages[i].gameObject, stages[i].transform).transform.localPosition = Vector3.zero;
            if (entry.recipe[i]?.gameObject != null)
                Instantiate(entry.recipe[i], recipe[i].transform).transform.localPosition = Vector3.zero;

            //stages[i].skeletonDataAsset = entry.mergeStages[i];
            //recipe[i].skeletonDataAsset = entry.recipe[i];
        }
        UpdateDescription();
    }
    public void SetDescriptor(int selected)
    {
        descriptor = (DESCRIPTORSELECTED)selected;
        UpdateDescription();
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
