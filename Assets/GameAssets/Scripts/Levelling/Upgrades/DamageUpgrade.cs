using System;
using Unity.Entities;
using UnityEngine;

[Serializable]
public class DamageUpgrade : BaseUpgrade
{
    [SerializeField] private float multiplier = 1.5f;

    override public void Apply()
    {
        if (!PlayerController.Instance.playerQuery.TryGetSingletonEntity<PlayerTag>(out Entity player)) return;
        var weaponData = PlayerController.Instance.entityManager.GetComponentData<WeaponData>(player);
        weaponData.projectileData.damage *= multiplier;
        PlayerController.Instance.entityManager.SetComponentData(player, weaponData);
    }
}
