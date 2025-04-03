using UnityEngine;
using UnityEngine.UI;

public class ProfilePictureManager : MonoBehaviour
{
    public static Sprite myPFPSprite, opponentPFPSprite;
    [SerializeField] Image myPFP, opponentPFP;
    private void Start()
    {
        myPFP.sprite = myPFPSprite;
        opponentPFP.sprite = opponentPFPSprite;
        if(TransportManager.instance.tutorialMode)
        {
            Destroy(myPFP.gameObject);
            Destroy(opponentPFP.gameObject);
        }
    }
}
