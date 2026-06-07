using UnityEngine;
using UnityEngine.UI;

public class UpgradeCardUI : MonoBehaviour
{
    [SerializeField] private Image cardImage;

    private BaseUpgrade upgrade;
    private UpgradeMenuManager menuManager;

    public void SetUpgrade(BaseUpgrade newUpgrade, UpgradeMenuManager manager)
    {
        upgrade = newUpgrade;
        menuManager = manager;

        cardImage.sprite = upgrade.cardSprite;
    }

    public void OnTakeClicked()
    {
        upgrade.Apply();
        menuManager.CloseMenu();
    }
}