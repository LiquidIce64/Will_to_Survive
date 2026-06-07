using UnityEngine;

abstract public class BaseUpgrade
{
    public Sprite cardSprite;
    public string description;

    abstract public void Apply();
}
