using Unity.Entities;

public struct ShockwaveData : IComponentData
{
    public float damage;
    public float knockback;
    public float range;
    public float fireRate;
    public float cooldown;
}
