using System;
using UnityEngine;

[Serializable]
public class FireRateUpgrade : BaseUpgrade
{
    [SerializeField] private float multiplier = 1.25f;

    override public void Apply()
    {
        Player.Instance.Weapon.fireRate *= multiplier;
    }
}
