using UnityEngine;

[CreateAssetMenu]
public class ArchiveEntry : ScriptableObject
{
    public ArchiveElementDisplay[] mergeStages;
    public ArchiveElementDisplay[] recipe;
    public AnimalEntry[] animalEntries;
}
[System.Serializable]
public class AnimalEntry
{
    [Multiline(3)]
    public string description;
    [Multiline(3)]
    public string effect;
}