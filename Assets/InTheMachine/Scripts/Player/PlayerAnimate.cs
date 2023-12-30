using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimate : AgentAnimator
{
   
    [SerializeField] private SpriteRenderer pakSprite;
    [SerializeField] private int pakAimUpYOffset;
    [SerializeField] private float pakXOffset;
    private Player myPlayer => myAgent as Player;
    private PlayerGun myGun;
    private PixelAligner pakPixelAligner;
    private float pakOffsetBase;
    private float flashTime;

    public Action<bool> onPlayerFlip;
    public int PakAimUpOffset => pakAimUpYOffset;

    public Sprite CurrentPlayerSprite => spriteRenderer.sprite;

    #region Singleton + Awake
    private static PlayerAnimate _singleton;
    public static PlayerAnimate main
    {
        get => _singleton;
        private set
        {
            if (_singleton == null)
            {
                _singleton = value;
            }
            else if (_singleton != value)
            {
                Debug.LogWarning("PlayerAnimate instance already exists, destroy duplicate!");
                Destroy(value);
            }
        }
    }

    private void Awake()
    {
        main = this;
    }
    #endregion


    public Vector2 FacingDirection
    {
        get
        {
            return spriteRenderer.flipX ? Vector2.left : Vector2.right;
        }
    }

    protected override void Start()
    {
        base.Start();
        myGun = GetComponent<PlayerGun>();
        pakPixelAligner = pakSprite.GetComponent<PixelAligner>();
        pakXOffset = Mathf.Abs(pakSprite.transform.localPosition.x);
        pakOffsetBase = pakXOffset;
        myPlayer.onIdleEnter += () => { animator.ResetTrigger("Rising"); animator.ResetTrigger("Falling"); };
        myPlayer.onAscendEnter += () => animator.SetTrigger("Rising");
        myPlayer.onDescendEnter += () => animator.SetTrigger("Falling");
        myPlayer.onFlyEnter += () => AnimatePakFlip(true);
        myPlayer.onFlyExit += () => { if (myPlayer.CurrentState != Player.PlayerState.Boost) AnimatePakFlip(false); };
        myPlayer.onUltraBoostExit += () => AnimatePakFlip(false);
        myPlayer.onWalkEnter += () => animator.SetBool("IsWalking", true);
        myPlayer.onWalkExit += () => animator.SetBool("IsWalking", false);
        myPlayer.onStunEnter += () => { animator.SetBool("IsStunned", true); pakPixelAligner.SetOffset(Vector2.zero); };
        myPlayer.onStunExit += () =>
        {
            animator.SetBool("IsStunned", false);
        };

        myPlayer.onHealEnter += () =>
        {
            animator.SetBool("IsHealing", true);
        };
        myPlayer.onHealExit += () =>
        {
            animator.SetBool("IsHealing", false);
        };

        myPlayer.PowerMeter.onMin += () => pakSprite.color = Color.gray;
        myPlayer.PowerMeter.onMax += () => pakSprite.color = Color.white;

        myGun.onShoot += () => animator.SetTrigger("Shoot");
        AnimatePakForward(true);

        if (!Player.main.HasAbility(Player.Ability.Gun))
        pakSprite.enabled = false;
    }

    public void EnablePak()
    {
        pakSprite.enabled = true;
    }

    private void Update()
    {
        animator.SetBool("TractorActive", myPlayer.BeamActive);
        animator.SetBool("IsGrounded", myPlayer.IsGrounded);
        if (myPlayer.IsStunned)
        {
            return;
        }

        bool updateHor = myPlayer.UserInputDir.x != 0;
        AnimatePlayerDirection(updateHor, myPlayer.UserInputDir.x < 0);
        AnimatePakForward(updateHor);
        AnimatePakAimUp(myPlayer.UserInputDir.y > 0.5 && !myPlayer.IsFlying);
        if (myPlayer.IFramesActive)
        {
            flashTime += Time.deltaTime;
            if (flashTime > 0.08f)
            {
                flashTime = 0;
                spriteRenderer.color = spriteRenderer.color == myBaseColor ? Color.clear : myBaseColor;
            }
        }
        else
        {
            flashTime = 0;
            spriteRenderer.color = myBaseColor;
        }
    }

    
    private void AnimatePakAimUp(bool aimUp)
    {
        if (aimUp)
        {
            pakSprite.transform.localPosition = new(0, PixelAligner.PixelsToWidth(pakAimUpYOffset), pakSprite.transform.localPosition.z);
            pakPixelAligner.SetOffset(new(0, PixelAligner.PixelsToWidth(pakAimUpYOffset)));
        }
        else
            AnimatePakForward(animator.GetBool("AimUp"));   //triggers only when the player releases up

        animator.SetBool("AimUp", aimUp);
    }

    private void AnimatePakFlip(bool flying)
    {
        animator.SetBool("IsFlying", flying);
        pakXOffset = flying ? -pakOffsetBase : pakOffsetBase;
        AnimatePakForward(true);
    }

    private void AnimatePakForward(bool update)
    {
        if (update)
        {
            pakSprite.flipX = spriteRenderer.flipX;
            pakSprite.transform.localPosition = new(FacingDirection.x * pakXOffset, 0, pakSprite.transform.localPosition.z);
            pakPixelAligner.SetOffset(new(pakXOffset, 0));
        }
    }

    private void AnimatePlayerDirection(bool update, bool left)
    {
        if (update)
        {
            spriteRenderer.flipX = left;
            onPlayerFlip?.Invoke(left);
        }
    }
}
