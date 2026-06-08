using System;
using UnityEngine;

[Serializable]
public class HealthUpgrade : BaseUpgrade
{
    [SerializeField] private float multiplier = 2f;

    override public void Apply()
    {
        PlayerController.Instance.maxHealth *= multiplier;
        PlayerController.Instance.heal = true;
    }
}
