using UnityEngine;

[System.Serializable]
public class Recipe
{
    [SerializeField] AnimalType partner_type;
    [SerializeField] AnimalType result_type;
    public AnimalType Partner { get { return partner_type; } }
    public AnimalType Result { get { return result_type; } }
}
