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
            var texSheet = _psysBoost.textureSheetAnimation;
            texSheet.SetSprite(0, animPlayer.CurrentPlayerSprite);
            _psysBoost.Play();
        };
        animPlayer.onPlayerFlip += FlipParticlePositions;

        _psysFly.Stop();
        _psysFlyPositionBase = _psysFly.transform.localPosition;
    }

    private void FlipParticlePositions(bool flip)
    {
        float dir = flip ? -1 : 1;

        _psysFly.transform.localPosition = new Vector3(_psysFlyPositionBase.x * dir, _psysFly.transform.localPosition.y, _psysFly.transform.localPosition.z);
        var main = _psysBoost.main;
        main.startRotationY = flip ? 180*Mathf.Deg2Rad : 0;
        
    }

}
