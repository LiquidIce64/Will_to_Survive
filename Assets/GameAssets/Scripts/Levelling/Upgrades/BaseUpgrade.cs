using UnityEngine.UI;

abstract public class BaseUpgrade
{
    public Image icon;
    public string description;

    abstract public void Apply();
}
