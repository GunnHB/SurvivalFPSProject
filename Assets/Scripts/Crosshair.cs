using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crosshair : MonoBehaviour
{
    [Header("[Components]")]
    [SerializeField] private Animator _animator;
    [SerializeField] private GunController _gunController;

    [Header("[GameObjects]")]
    [SerializeField] private GameObject _objCrosshairHUD;

    // 크로스헤어 상태에 따른 총의 정확도
    private float _gunAccuracy;

    public void WalkingAnimation(bool flag)
    {
        _animator.SetBool("Walking", flag);
    }

    public void RunningAnimation(bool flag)
    {
        _animator.SetBool("Running", flag);
    }

    public void CrouchingAnimation(bool flag)
    {
        _animator.SetBool("Crouching", flag);
    }

    public void FineSightAnimation(bool flag)
    {
        _animator.SetBool("FineSight", flag);
    }

    public void FireAnimation()
    {
        if (_animator.GetBool("Walking"))
            _animator.SetTrigger("WalkFire");
        else if (_animator.GetBool("Crouching"))
            _animator.SetTrigger("CrouchFire");
        else
            _animator.SetTrigger("IdleFire");
    }

    public float GetAccuracy()
    {
        if (_animator.GetBool("Walking"))
            _gunAccuracy = .08f;
        else if (_animator.GetBool("Crouching"))
            _gunAccuracy = .02f;
        else if (_gunController.IsFineSightMode)
            _gunAccuracy = .001f;
        else
            _gunAccuracy = .04f;

        return _gunAccuracy;
    }
}
