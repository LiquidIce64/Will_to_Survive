using Unity.Entities;

public struct EnemyData : IComponentData
{
    public uint seed;
    public NPCState state;
    public float maxAttackDistance;
    public float attackDistance;
    public float minAttackDistance;
    public float aimingTime;
    public float aimingTimeRandomness;
    public float remainingAimTime;
    public float xpDrop;
}
