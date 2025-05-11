using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerStats playerStats;
    
    [Header("UI Elements")]
    [SerializeField] private Image healthBar;
    [SerializeField] private Image staminaBar;
    [SerializeField] private Image playerAvatar;
    [SerializeField] private Image weaponIcon;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private TextMeshProUGUI staminaText;

    private void Update()
    {
        UpdateUI();
    }

    private void UpdateUI()
    {
        // Update health bar
        healthBar.fillAmount = playerStats.GetHealthPercentage();
        healthText.text = $"{(playerStats.GetHealthPercentage() * 100):F0}%";

        // Update stamina bar
        staminaBar.fillAmount = playerStats.GetStaminaPercentage();
        staminaText.text = $"{(playerStats.GetStaminaPercentage() * 100):F0}%";

        // Update weapon icon if weapon changes
        WeaponData currentWeapon = playerStats.GetCurrentWeapon();
        if (currentWeapon != null && weaponIcon != null)
        {
            weaponIcon.sprite = currentWeapon.weaponIcon;
        }
    }
}
