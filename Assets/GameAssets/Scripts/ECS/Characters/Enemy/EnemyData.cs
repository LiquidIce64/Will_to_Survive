using Unity.Entities;

public struct EnemyData : IComponentData
{
    public NPCState state;
    public float maxAttackDistance;
    public float attackDistance;
    public float minAttackDistance;
    public float aimingTime;
    public float remainingAimTime;
    public float xpDrop;
}
