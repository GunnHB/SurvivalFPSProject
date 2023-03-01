using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    [Header("[Components]")]
    [SerializeField] private Animator _gunAnimator;
    [SerializeField] private ParticleSystem _muzzleFlash;
    [SerializeField] private AudioClip _fireSound;

    [Header("[Variables - Guns]")]
    [SerializeField] private string _gunName;                   // 총 이름
    [SerializeField] private float _ragne;                      // 사정 거리
    [SerializeField] private float _accuracy;                   // 정확도
    [SerializeField] private float _fireRate;                   // 연사속도
    [SerializeField] private float _reloadTime;                 // 재장전 속도
    [SerializeField] private int _damage;                       // 총의 데미지

    [Header("[Variables - Bullets]")]
    [SerializeField] private int _reloadBulletCount;            // 탄알 재장전 수
    [SerializeField] private int _currentBulletCount;           // 현재 탄알집에 남아있는 총알 수
    [SerializeField] private int _maxBulletCount;               // 최대 소유 가능한 탄알 수
    [SerializeField] private int _carryBulletCount;             // 현재 소유하고 있는 탄알 수

    [Header("[Variables - Retro]")]
    [SerializeField] private float _retroActionForce;           // 반동 세기
    [SerializeField] private float _retroActionFineSightForce;  // 정조준 시 반동 세기

    [Header("[Variables - Vector]")]
    [SerializeField] private Vector3 _fineSightOriginPosition;  // 정조준 시 총의 위치 

    // Properties
    public Animator GunAnimator => _gunAnimator;
    public ParticleSystem MuzzleFlash => _muzzleFlash;
    public AudioClip FireSound => _fireSound;

    public string GunName => _gunName;
    public float Range => _ragne;
    public float Accuracy => _accuracy;
    public float FireRate => _fireRate;
    public float ReloadTime => _reloadTime;
    public int Damage => _damage;

    public int ReloadBulletCount
    {
        get { return _reloadBulletCount; }
        set { _reloadBulletCount = value; }
    }
    public int CurrentBulletCount
    {
        get { return _currentBulletCount; }
        set { _currentBulletCount = value; }
    }
    public int MaxBulletCount
    {
        get { return _maxBulletCount; }
        set { _maxBulletCount = value; }
    }
    public int CarryBulletCount
    {
        get { return _carryBulletCount; }
        set { _carryBulletCount = value; }
    }

    public float RetroActionForce => _retroActionForce;
    public float RetroActionFineSightForce => _retroActionFineSightForce;

    public Vector3 FineSightOriginPosition => _fineSightOriginPosition;
}
