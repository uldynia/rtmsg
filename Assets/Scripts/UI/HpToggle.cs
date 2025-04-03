using UnityEngine;
using UnityEngine.UI;

public class HpToggle : MonoBehaviour
{
    public Toggle toggle;

    public static bool showHp = false;
    public void OnValueChanged(bool newCheck)
    {
        showHp = toggle.isOn;
        foreach(EntityBaseBehaviour en in GameManager.instance.entities)
        {
            en.hpBar.SetActive(showHp);
        }
    }
}
