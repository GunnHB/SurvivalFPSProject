using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crosshair : MonoBehaviour
{
    [Header("[Components]")]
    [SerializeField] private Animator _animator;

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

}
