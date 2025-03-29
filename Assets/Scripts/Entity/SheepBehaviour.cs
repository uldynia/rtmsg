using UnityEngine;

public class SheepBehaviour : EntityBaseBehaviour
{
    public override void OnStartServer()
    {
        base.OnStartServer();
        if (level > 3)
        {
            currHp = animalData.Health + (level - 2) * 2;
            ogHp = currHp;
        }

    }

    public override void Setup(int direction, int level)
    {
        base.Setup(direction, level);

        if (level > 3)
        {
            currHp = animalData.Health + (level - 2) * 2;
            ogHp = currHp;
        }
    }
}
