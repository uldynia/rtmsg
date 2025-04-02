using UnityEngine;
using Mirror;
using System.Collections.Generic;
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

    protected float targetLaneXPos;
    protected float currLaneXPos;
    protected bool isChangingLane;
    [SerializeField]
    protected float laneChangeSpeed;

    protected float currLaneSpeed;

    protected List<Buff> buffs = new();

    [Header("SFX References")]
    [SerializeField] AudioClip deploy_sfx;
    [SerializeField] List<AudioClip> fight_sfx;

    public override void OnStartServer()
    {
        base.OnStartServer();

        currHp = animalData.Health;
        ogHp = animalData.Health;
        currSpd = animalData.Speed;
    }
    private void Start()
    {
        AudioSfxManager.m_instance.OnPlayNewAudioClip(deploy_sfx);
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
        for (int BuffNo = 0; BuffNo < buffs.Count; BuffNo++)
        {
            Buff buff = buffs[BuffNo];
            if (buff.timeLeft < 99999)
            {
                buff.timeLeft -= Time.deltaTime;
                if (buff.timeLeft <= 0)
                {
                    RemoveBuff(buff);
                    BuffNo--;
                }
            }
        }
        if (isChangingLane)
        {
            transform.position = new(Mathf.Lerp(currLaneXPos, targetLaneXPos, currLaneSpeed), transform.position.y + direction * currSpd * Time.deltaTime, transform.position.z);
            currLaneSpeed += Time.deltaTime * laneChangeSpeed;
            if (currLaneSpeed >= 1)
            {
                isChangingLane = false;
                transform.position = new(targetLaneXPos, transform.position.y, transform.position.z);
                OnFinishedLaneChange();
            }
        }
        else
        {
            transform.position += new Vector3(0, direction * currSpd * Time.deltaTime, 0);
        }
    }
    protected virtual void OnFinishedLaneChange()
    {

    }
    public virtual void ChangeLane(int direction)
    {
        
        currLaneXPos = transform.position.x;
        targetLaneXPos = transform.position.x + GridManager.instance.GetGridSize() * direction;
        // Stationary units should not be going into each other
        if (currSpd != 0)
        {
            isChangingLane = true;
        }
        // OLD CODE THAT MAKES STATIONARY ENTITIES MOVE INTO EMPTY SQAURES ( NOT WORKING )
        //else
        //{
        //    if (!GridManager.instance.coveredGrids.Contains(GridManager.instance.GetGridCoordinate(new(targetLaneXPos, transform.position.y))))
        //    {
        //        isChangingLane = true;
        //        PlayerController.localPlayer.RegisterStationaryObject(GridManager.instance.GetGridCoordinate(new(targetLaneXPos, transform.position.y)));
        //        PlayerController.localPlayer.UnregisterStationaryObject(GridManager.instance.GetGridCoordinate(new(transform.position.x, transform.position.y)));
        //    }
        //}
        currLaneSpeed = 0;
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
        PlayFight();
        PlayDeploy();
    }
    [ClientRpc]
    private void PlayFight()
    {
        AudioSfxManager.m_instance.OnPlayNewAudioClip(fight_sfx[Random.Range(0, fight_sfx.Count)]);
    }
    [ClientRpc]
    private void PlayDeploy()
    {
        AudioSfxManager.m_instance.OnPlayNewAudioClip(deploy_sfx);
    }

    protected virtual void OnTakeDamage(EntityBaseBehaviour enemy)
    {
        currHp -= enemy.ogHp;
        if (currHp <= 0)
        {
            OnDeath();
        }
    }

    public virtual void OnDeath()
    {
        GameManager.instance.entities.Remove(this);
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

    public void ChangeDirection(int newDir)
    {
        direction = newDir;
    }
    public int GetDirection()
    {
        return direction;
    }
    public void ChangeLevel(int newLevel)
    {
        level = newLevel;
    }
    public void ChangeData(AnimalType newData)
    {
        animalData = newData;
    }
    public int GetHealth()
    {
        return currHp;
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
    public virtual bool HasBuff(Buff buff)
    {
        return buffs.Contains(buff);
    }
    public virtual void ApplyBuff(Buff buff)
    {
        buffs.Add(buff);

        // Apply buff effects
        // H - HP, S - Speed
        // This is bad and hardcoded but im sooo lazyyyyy

        switch (buff.buffName)
        {
            case "H":
                currHp += (int)buff.buffValue;
                ogHp = currHp;
                break;
            case "S":
                if (currSpd != 0) // Do not give speed to stationary
                {
                    currSpd += buff.buffValue;
                }
                break;
        }
    }

    public virtual void RemoveBuff(Buff buff)
    {
        if (buffs.Contains(buff))
        {
            buffs.Remove(buff);
        }

        switch (buff.buffName)
        {
            case "H":
                currHp -= (int)buff.buffValue;
                if (currHp <= 0)
                {
                    currHp = 1;
                }
                ogHp = currHp;
                break;
            case "S":
                if (currSpd != 0) // Do not remove speed from stationary
                {
                    currSpd -= buff.buffValue;
                }
                break;
        }
        // Remove buff effects
        // H - HP, S - Speed
        // This is bad and hardcoded but im sooo lazyyyyy
    }
}

[System.Serializable]
public class Buff
{
    public EntityBaseBehaviour entity;
    public string buffName;
    public float timeLeft; // Any time above 99999 is considered infinite
    public float buffValue;
}
