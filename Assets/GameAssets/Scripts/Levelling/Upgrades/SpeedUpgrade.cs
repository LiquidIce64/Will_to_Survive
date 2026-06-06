using System;
using UnityEngine;

[Serializable]
public class SpeedUpgrade : BaseUpgrade
{
    [SerializeField] private float multiplier = 1.5f;

    override public void Apply()
    {
        Player.Instance.speed *= multiplier;
    }
}
