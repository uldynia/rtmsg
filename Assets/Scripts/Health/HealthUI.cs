using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class HealthUI : MonoBehaviour
{
    public static HealthUI instance { get; private set; }

    [SerializeField]
    private TMP_Text playerHealth;

    [SerializeField]
    private TMP_Text enemyHealth;

    public void UpdateUI(int newPlayerHealth, int newEnemyHealth)
    {
        playerHealth.text = newPlayerHealth.ToString();
        enemyHealth.text = newEnemyHealth.ToString();
    }
}
