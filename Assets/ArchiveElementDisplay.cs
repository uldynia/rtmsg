using UnityEngine;

public class ArchiveElementDisplay : MonoBehaviour
{
    public ArchiveEntry entry;
    public int index;
    public void OnClick()
    {
        ArchiveManager.instance.OpenRecipe(entry, index);
    }
}
