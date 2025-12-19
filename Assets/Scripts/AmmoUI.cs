using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AmmoUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_Text weaponNameText;
    [SerializeField] private TMP_Text ammoText;
    [SerializeField] private Image weaponIcon;
    [SerializeField] private Image ammoFill;


    public void SetWeapon(WeaponStats ws)
    {
        if (weaponNameText != null)
            weaponNameText.text = ws != null ? ws.displayName : "";

        if (weaponIcon != null)
        {
            if (ws != null && ws.weaponIcon != null)
            {
                weaponIcon.enabled = true;
                weaponIcon.sprite = ws.weaponIcon;
            }
            else
            {
                weaponIcon.enabled = false;
            }
        }
    }

    public void SetAmmo(int inMag, int reserve, int magSize)
    {
        if (ammoText != null)
            ammoText.text = $"{inMag}/{(inMag + reserve)}";

        if (ammoFill != null)
        {
            float t = magSize > 0 ? (float)inMag / magSize : 0f;
            ammoFill.fillAmount = Mathf.Clamp01(t);
        }

        if (ammoPips != null && ammoPips.Length > 0)
        {
            int shown = Mathf.Clamp(inMag, 0, ammoPips.Length);
            for (int i = 0; i < ammoPips.Length; i++)
                ammoPips[i].enabled = i < shown;
        }
    }

    public void SetVisible(bool visible)
    {
        gameObject.SetActive(visible);
    }
}
