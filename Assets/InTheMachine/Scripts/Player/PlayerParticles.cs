using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Player))]
public class PlayerParticles : MonoBehaviour
{
    private Player player;
    private PlayerAnimate animPlayer;
    [SerializeField] private ParticleSystem _psysFly;
    [SerializeField] private ParticleSystem _psysBoost;
    [SerializeField] private ParticleSystem _psysUltraBoostActivate, _psysUltraBoostStay;
    [SerializeField] private ParticleSystem _psysFlameGun;
    [SerializeField] private Color boostColor, ultraBoostColor;

    private ParticleSystem.ShapeModule flameGunShape;

    private Vector3 _psysFlyPositionBase;
    // Start is called before the first frame update
    void Start()
    {
        player = GetComponent<Player>();
        animPlayer = GetComponent<PlayerAnimate>();
        player.onFlyEnter += () => _psysFly.Play();
        player.onFlyExit += () => _psysFly.Stop();
        player.onBoostEnter += () =>
        {
            var main = _psysBoost.main;
            main.startColor = boostColor;
            var texSheet = _psysBoost.textureSheetAnimation;
            texSheet.SetSprite(0, animPlayer.CurrentPlayerSprite);
            _psysBoost.Play();
        };
        player.onBoostExit += () =>
        {
            if (player.CurrentState != Player.PlayerState.UltraBoost)
                _psysBoost.Stop();
        };

        player.onUltraBoostEnter += () =>
        {
            var main = _psysBoost.main;
            main.startColor = ultraBoostColor;
            _psysUltraBoostActivate.Play();
            _psysUltraBoostStay.Play();
        };
        player.onUltraBoostExit += () =>
        {
            _psysBoost.Stop();
            _psysUltraBoostStay.Stop();
        };

        animPlayer.onPlayerFlip += FlipParticlePositions;

        player.shoot.onPress += () =>
        {
            if (PlayerGun.main.CurrentProfile == GunProfileType.Fire && !Player.main.OutOfPower)
            {
                _psysFlameGun.Play();
            }
        };
        player.shoot.onRelease += () =>
        {
            _psysFlameGun.Stop();
        };
        player.PowerMeter.onMin += () =>
        {
            _psysFlameGun.Stop();
        };

        _psysFly.Stop();
        _psysFlyPositionBase = _psysFly.transform.localPosition;

        flameGunShape = _psysFlameGun.shape;
    }

    private void FixedUpdate()
    {
        _psysFlameGun.transform.position = PlayerGun.main.SpawnPosition;
        if (PlayerGun.main.Direction.y != 0)
        {
            flameGunShape.rotation = new Vector3(0,0,90 - (2.5f));
        }
        else
        {
            flameGunShape.rotation = new Vector3(0, 0, (PlayerGun.main.Direction.x > 0 ? 0 : 180) - (2.5f));
        }
    }

    private void FlipParticlePositions(bool flip)
    {
        float dir = flip ? -1 : 1;

        _psysFly.transform.localPosition = new Vector3(_psysFlyPositionBase.x * dir, _psysFly.transform.localPosition.y, _psysFly.transform.localPosition.z);

        var main = _psysBoost.main;
        main.startRotationY = flip ? 180 * Mathf.Deg2Rad : 0;

        var shape = _psysUltraBoostActivate.shape;
        shape.rotation = new(0, 0, flip ? 5 : 130);

        shape = _psysUltraBoostStay.shape;
        shape.rotation = new(0, 0, flip ? -22 : 157);
    }

}
