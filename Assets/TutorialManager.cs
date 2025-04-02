using UnityEngine;
using UnityEngine.UI;
public class TutorialManager : MonoBehaviour
{
    private void Start()
    {
        if(PlayerPrefs.GetInt("Tutorial", 0) == 1) Destroy(gameObject);
    }
    public void CancelTutorial()
    {
        PlayerPrefs.SetInt("Tutorial", 1);
        Destroy(gameObject);
    }
}
