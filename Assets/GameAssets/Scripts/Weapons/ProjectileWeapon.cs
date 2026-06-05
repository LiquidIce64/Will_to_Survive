using UnityEngine;

public class ProjectileWeapon : BaseWeapon
{
    [SerializeField] private GameObject projectilePrefab;

    override protected void OnUse(Vector3 targetPos)
    {
        var obj = Instantiate(projectilePrefab, owner.transform.position, owner.transform.rotation);
        obj.transform.LookAt(targetPos);
        obj.transform.Rotate(transform.up, Random.Range(-horizontalSpread, horizontalSpread) / 2);

        if (!obj.TryGetComponent<Projectile>(out var projectile))
        {
            Debug.LogWarning("Projectile prefab doesn't have the required component");
            return;
        }
        
        projectile.weapon = this;
    }
}
