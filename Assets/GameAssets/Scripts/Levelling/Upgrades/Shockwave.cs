using Unity.Entities;
using UnityEngine;

public class Shockwave : MonoBehaviour
{
    public float damage = 1f;
    public float knockback = 1f;
    public float range = 1f;
    public float fireRate = 1f;

    protected void Start()
    {
        var entityManager = PlayerController.Instance.entityManager;
        if (!PlayerController.Instance.playerQuery.TryGetSingletonEntity<PlayerTag>(out Entity player)) return;
        if (entityManager.HasComponent<ShockwaveData>(player))
        {
            var shockwaveData = entityManager.GetComponentData<ShockwaveData>(player);
            shockwaveData.fireRate *= 2;
            entityManager.SetComponentData(player, shockwaveData);
        }
        else
        {
            entityManager.AddComponentData(player, new ShockwaveData
            {
                damage = damage,
                knockback = knockback,
                range = range,
                fireRate = fireRate,
                cooldown = 1f / fireRate,
            });
        }
        Destroy(gameObject);
    }
}
