using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using DG.Tweening;

public class LobbyAvatarDisplay : MonoBehaviour
{
    [Header("Details")]
    [SerializeField] List<Sprite> avatar_display_sprites;

    [Header("References")]
    [SerializeField] Image local_avatar;
    [SerializeField] Image side_avatar;
    [SerializeField] Image opponent_avatar_image;


    int current_avatar_id = 0;
    const float switch_duration = 0.3f;
    bool can_switch = true;

    //private void Start()
    //{
    //    SetupAvatar(0);
    //}

    //TO be called by another function to load in player's last saved avatar
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
    }

}
