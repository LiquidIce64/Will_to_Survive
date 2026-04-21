using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    private static HealthBar instance;

    [SerializeField] private int health;
    [SerializeField] private int maxHealth;

    [SerializeField] private Sprite fullLive;
    [SerializeField] private Sprite emptyLive;
    private readonly List<Image> lives = new();

    public static HealthBar Instance => instance;

    public int Health
    {
        get => health;
        set { health = Mathf.Clamp(value, 0, maxHealth); UpdateHealth(); }
    }

    public int MaxHealth
    {
        get => maxHealth;
        set { maxHealth = Mathf.Clamp(value, 0, lives.Count); UpdateHealth(); }
    }

    private void Awake()
    {
        GetComponentsInChildren(true, lives);
        instance = this;
    }

    private void OnValidate()
    {
        GetComponentsInChildren(true, lives);
        maxHealth = Mathf.Clamp(maxHealth, 0, lives.Count);
        health = Mathf.Clamp(health, 0, maxHealth);
        UpdateHealth();
    }

    private void UpdateHealth()
    {
        if (health > maxHealth)
        {
            health = maxHealth;
        }

        for (int i = 0; i < lives.Count; i++)
        {
            lives[i].enabled = i < maxHealth;
            lives[i].sprite = (i < health) ? fullLive : emptyLive;
        }
    }
}
