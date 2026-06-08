using Unity.Entities;

public struct ProjectileData : IComponentData
{
    public bool explodeOnImpact;
    public bool damageOnImpact;
    public float damage;
    public float radius;
    public float knockback;
    public bool playerOwned;
}
