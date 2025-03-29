using UnityEngine;
/// <summary>
/// Base entity script, any entities have to inherit from here :D
/// Default behaviours is written here ( referenced from sheep )
/// </summary>
public abstract class EntityBaseBehaviour : MonoBehaviour
{
    [SerializeField]
    private AnimalType animalData;

    // Allows us to use 1 animal script for each entity instead of writing 3 classes for each level
    [SerializeField]
    protected int level;

    // data from scriptable object
    protected int currHp;
    protected int currAtk;
    protected float currSpd;

    protected int direction;

    protected virtual void Start()
    {
        direction = 1;

        currHp = animalData.Health;
        currAtk = animalData.Damage;
        currSpd = animalData.Speed;
    }

    protected virtual void Update()
    {
        transform.position += new Vector3(0, direction * animalData.Speed, 0);
    }

    protected virtual void OnEncounterEnemy(EntityBaseBehaviour enemy)
    {
        
    }

    protected virtual void OnTakeDamage(EntityBaseBehaviour enemy, float damage)
    {

    }

    protected virtual void OnDeath()
    {

    }
    // Level transfers too incase any unit scales infinitely with level such as cotton ball of sheeps
    public virtual void Setup(int direction, int level)
    {
        this.direction = direction;
        this.level = level;

        Debug.Log("Entity level: ");
    }
}
