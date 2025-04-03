using UnityEngine;
using System.Collections.Generic;
using Spine.Unity;

[CreateAssetMenu(fileName = "AnimalType", menuName = "ScriptableObjects/AnimalType")]
public class AnimalType : ScriptableObject
{
    [SerializeField] string name_animal;
    [SerializeField] string description_animal; //if we ever decide to have a hover Info for animal
    [SerializeField] bool stationary;
    [SerializeField] int hp_animal;
    [SerializeField] float speed_animal;
    [SerializeField] List<Recipe> recipes_animal;
    [SerializeField] Sprite sprite_icon_animal;
    [SerializeField] int entityID;
    [SerializeField] GameObject prefabToSpawn;
    [SerializeField] SkeletonDataAsset skeletonData;
    [SerializeField] string animationIdleName;
    [SerializeField] Vector3 skeletonUIScale;
    [SerializeField] Vector3 skeletonUIOffset;
    [SerializeField] Vector3 skeletonScale;
    [SerializeField] Vector3 skeletonOffset;
    [SerializeField]
    private int level = 1;
    public string Name { get { return name_animal; } }
    public string Description { get { return description_animal; } }
    public bool Stationary { get { return stationary; } }
    public int Health { get { return hp_animal; } }
    public float Speed { get { return speed_animal; } }
    public List<Recipe> Recipes { get { return recipes_animal; } }
    public Sprite Icon { get { return sprite_icon_animal; } }
    public int EntityID { get { return entityID; } }
    public GameObject PrefabToSpawn { get { return prefabToSpawn; } }
    public int Level {get { return level; } }

    public SkeletonDataAsset SkeletonData {get { return skeletonData; } }
    public Vector3 SkeletonScale { get { return skeletonScale; } }
    public Vector3 SkeletonOffset { get { return skeletonOffset; } }
    public string AnimationIdleName { get { return animationIdleName; } }
    public Vector3 SkeletonUIScale { get { return skeletonUIScale; } }
    public Vector3 SkeletonUIOffset { get { return skeletonUIOffset; } }

    public bool CanMergeWith(AnimalType partner_try, out AnimalType result)
    {
        result = null;
        foreach (Recipe r in recipes_animal)
        {
            if (r.Partner.entityID == partner_try.entityID)
            {
                result = r.Result;
                return true;
            }
        }
        return false;
    }

    //Normal animals default hp shldnt change, other than Cotton Ball of Sheeps
    public void AddHealth(int hp)
    {
        hp_animal += hp;
    }

    public void AddLevel(int Level)
    {
        level += Level;
    }

    public void ResetLevel()
    {
        level = 1;
    }
}