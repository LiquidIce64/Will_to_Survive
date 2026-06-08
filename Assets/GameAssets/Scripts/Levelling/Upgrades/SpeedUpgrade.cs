using System;
using UnityEngine;

[Serializable]
public class SpeedUpgrade : BaseUpgrade
{
    [SerializeField] private float multiplier = 1.5f;

    override public void Apply()
    {
        PlayerController.Instance.speed *= multiplier;
    }
}
