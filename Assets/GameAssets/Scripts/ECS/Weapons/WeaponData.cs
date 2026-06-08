using Unity.Entities;
using Unity.Mathematics;

public struct WeaponData : IComponentData
{
    public Entity projectilePrefab;
    public ProjectileData projectileData;
    public float3 projectileVelocity;
    public float projectileLifeTime;
    public float selfKnockback;
    public float fireRate;
    public float horizontalSpread;
    public float cooldown;
}
