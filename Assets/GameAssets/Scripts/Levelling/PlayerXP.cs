using System;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class PlayerXP
{
    [SerializeField] private float baseRequiredXp = 200f;
    [SerializeField] private float xpPerLevel = 10f;
    [SerializeField] private float xp = 0f;
    [SerializeField] private int level = 0;
    [SerializeField] private int availableUpgrades = 0;
    public UnityEvent levelGained = new();

    public float RequiredXP => baseRequiredXp + xpPerLevel * level;
    public float XP => xp;
    public int Level => level;

    public int AvailableUpgrades
    {
        get => availableUpgrades;
        set => availableUpgrades = value;
    }

    public void AddXP(float amount)
    {
        if (amount <= 0f) return;
        xp += amount;
        float required = RequiredXP;
        if (xp >= required)
        {
            xp -= required;
            level++;
            availableUpgrades++;
            PlayerController.Instance.heal = true;
            UpgradeMenuManager.Instance.UpdateButton();
            PlayerController.Instance.OnLevelUp();
            levelGained.Invoke();
        }
        XPBar.Instance.SetValue(xp / RequiredXP);
    }
}
