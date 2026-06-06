using System;
using UnityEngine;

[Serializable]
public class DamageUpgrade : BaseUpgrade
{
    [SerializeField] private float multiplier = 1.5f;

    override public void Apply()
    {
        Player.Instance.Weapon.damage *= multiplier;
    }
}
