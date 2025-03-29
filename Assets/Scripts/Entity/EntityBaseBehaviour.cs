using UnityEngine;
using Mirror;
/// <summary>
/// Base entity script, any entities have to inherit from here :D
/// Default behaviours is written here ( referenced from sheep )
/// </summary>
public abstract class EntityBaseBehaviour : NetworkBehaviour
{
    [SerializeField]
    protected AnimalType animalData;

    // Allows us to use 1 animal script for each entity instead of writing 3 classes for each level
    [SerializeField]
    protected int level;

    // data from scriptable object
    protected int currHp;
    // This is so the current entity can damage to
    protected int ogHp;
    protected float currSpd;

    protected int direction;

    public override void OnStartServer()
    {
        base.OnStartServer();

        currHp = animalData.Health;
        ogHp = animalData.Health;
        currSpd = animalData.Speed;
    }

    protected virtual void Update()
    {
        if (isServer)
        {
            UpdateServer();
        }
    }
    protected virtual void UpdateServer()
    {
        transform.position += new Vector3(0, direction * currSpd * Time.deltaTime, 0);
    }

    protected virtual void OnEncounterEnemy(EntityBaseBehaviour enemy)
    {
        if (direction > 0) // Make sure that entities that are positive on the server side deal dmg
        {
            OnTakeDamage(enemy);
            enemy.OnTakeDamage(this);
            ogHp = currHp;
            enemy.ogHp = enemy.currHp;
        }
    }

    protected virtual void OnTakeDamage(EntityBaseBehaviour enemy)
    {
        currHp -= enemy.ogHp;
        if (currHp <= 0)
        {
            OnDeath();
        }
    }

    protected virtual void OnDeath()
    {
        NetworkServer.Destroy(gameObject);
    }
    // Level transfers too incase any unit scales infinitely with level such as cotton ball of sheeps
    public virtual void Setup(int direction, int level)
    {
        this.direction = direction;
        this.level = level;
    }
    
    public AnimalType GetData()
    {
        return animalData;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isServer)
        {
            EntityBaseBehaviour enemy = collision.gameObject.GetComponent<EntityBaseBehaviour>();
            if (enemy != null && enemy.direction != direction)
            {
                OnEncounterEnemy(enemy);
            }
        }
    }
}
