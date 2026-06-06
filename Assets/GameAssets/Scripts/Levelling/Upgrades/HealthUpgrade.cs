using System;
using UnityEngine;

[Serializable]
public class HealthUpgrade : BaseUpgrade
{
    [SerializeField] private float multiplier = 2f;

    override public void Apply()
    {
        Player.Instance.maxHealth *= multiplier;
        Player.Instance.ApplyDamage(-Player.Instance.maxHealth);
    }
}
