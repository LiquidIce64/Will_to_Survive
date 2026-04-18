using System;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class PlayerXP
{
    [SerializeField] private float baseRequiredXp = 100f;
    [SerializeField] private float xpPerLevel = 50f;
    [SerializeField] private float xp = 0f;
    [SerializeField] private int level = 0;
    [SerializeField] private UnityEvent levelGained = new();

    public float RequiredXP => baseRequiredXp + xpPerLevel * level;
    public float XP => xp;
    public int Level => level;

    public void AddXP(float amount)
    {
        xp += amount;
        float required = RequiredXP;
        if (xp >= required)
        {
            xp -= required;
            level++;
            levelGained.Invoke();
        }
    }
}
