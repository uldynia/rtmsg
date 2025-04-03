using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using DG.Tweening;
using Mirror;

public class LobbyAvatarDisplay : NetworkBehaviour
{
    [Header("Details")]
    [SerializeField] List<Sprite> avatar_display_sprites;

    [Header("References")]
    [SerializeField] Image local_avatar;
    [SerializeField] Image side_avatar;
    [SerializeField] Image opponent_avatar_image;

    public static LobbyAvatarDisplay instance;
    public int current_avatar_id
    {
        get
        {
            return PlayerPrefs.GetInt("avatarID", 0);
        }
        set
        {
            PlayerPrefs.SetInt("avatarID", value);
        }
    }
    const float switch_duration = 0.3f;
    bool can_switch = true;


    private void Start()
    {
        instance = this;
        SetupAvatar(current_avatar_id);
        AskPFPCommand();
    }
    public void SetupAvatar(int avatar_id)
    {
        current_avatar_id = avatar_id;
        local_avatar.sprite = avatar_display_sprites[avatar_id];
    }
    
    //Call this when opponent connected
    public void LoadOpponentAvatar(int avatar_id)
    {
        opponent_avatar_image.sprite = avatar_display_sprites[avatar_id];
    }

    public void SwitchAvatar(int move_amt_right)
    {
        if (!can_switch)
            return;

        can_switch = false;

        //Side
        int old_avatar_id = current_avatar_id;
        current_avatar_id += move_amt_right;
        if (current_avatar_id >= avatar_display_sprites.Count) 
            current_avatar_id -= avatar_display_sprites.Count;
        if (current_avatar_id < 0)
            current_avatar_id += avatar_display_sprites.Count;

        //DO tween animation
        local_avatar.sprite = avatar_display_sprites[old_avatar_id];
        side_avatar.sprite = avatar_display_sprites[current_avatar_id];
        side_avatar.transform.localPosition = Vector3.left * move_amt_right * 300; //Left to put it at the other side

        local_avatar.transform.DOLocalMoveX(300 * move_amt_right, switch_duration, true).SetEase(Ease.InOutSine);
        side_avatar.transform.DOLocalMoveX(0, switch_duration, true).SetEase(Ease.InOutSine).OnComplete(() => {
            local_avatar.sprite = avatar_display_sprites[current_avatar_id];
            local_avatar.transform.localPosition = Vector3.zero;
            can_switch = true;
        });
        PFP(PlayerController.localPlayer.netId, current_avatar_id);
    }
    [Command(requiresAuthority = false)]
    public void PFP(uint netID, int id)
    {
        SetPFP(netID, current_avatar_id);
    }
    [ClientRpc(includeOwner =true)]
    public void SetPFP(uint netID, int id)
    {
        if(netID == PlayerController.localPlayer.netId)
        {
            ProfilePictureManager.myPFPSprite = avatar_display_sprites[id];
        }
        else
        {
            LoadOpponentAvatar(id);
            ProfilePictureManager.opponentPFPSprite = avatar_display_sprites[id];
        }
    }
    [Command(requiresAuthority =false)]
    public void AskPFPCommand()
    {
        AskPFP();
    }
    [ClientRpc]
    public void AskPFP()
    {
        PFP(PlayerController.localPlayer.netId, current_avatar_id);
    }
}
