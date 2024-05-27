using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateList : MonoBehaviour
{
    public bool jumping = false;
    public bool dashing = false;
    public bool recoilingX, recoilingY;
    public bool lookingRight;
    public bool invincible;
    public bool healing;
    public bool cutscene = false;
    public bool alive;

    [Header("Ability Unlocks")]
    //Tracking unlocked abilities
    public bool canJump;
    public bool canDoubleJump;
    public bool canDash;
}
