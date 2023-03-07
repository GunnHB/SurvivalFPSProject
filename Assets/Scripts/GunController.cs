using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{
    [Header("[Components]")]
    [SerializeField] private Gun _currentGun;
    [SerializeField] private Camera _playerCamera;
    [SerializeField] private Crosshair _crosshair;
    private AudioSource _audioSource;

    private float _currentFireRate;

    [Header("[ObjectPool]")]
    [SerializeField] private ObjectPool _objectPool;

    private bool _isReload = false;
    private bool _isFineSightMode = false;

    private Vector3 _originPos;

    private RaycastHit _hitInfo;                    // 탄의 충돌 정보

    public bool IsFineSightMode => _isFineSightMode;
    public Gun CurrentGun => _currentGun;

    private void Start()
    {
        _objectPool.Initialize();

        _audioSource = GetComponent<AudioSource>();
        _originPos = this.transform.localPosition;
    }

    private void Update()
    {
        GunFireRateCalc();
        TryFire();
        TryReload();
        TryFineSight();
    }

    private void GunFireRateCalc()
    {
        if (_currentFireRate > 0)
            _currentFireRate -= Time.deltaTime;
    }

    private void TryFire()
    {
        if (Input.GetButton("Fire1") && _currentFireRate <= 0 && !_isReload)
            Fire();
    }

    // 트리거를 당김
    private void Fire()
    {
        if (!_isReload)
        {
            if (_currentGun.CurrentBulletCount > 0)
                Shoot();
            else
            {
                CancelFineSight();
                StartCoroutine(nameof(ReloadCoroutine));
            }
        }
    }

    // 실제 총알이 날아감
    private void Shoot()
    {
        _crosshair.FireAnimation();

        _currentGun.CurrentBulletCount--;

        // 연사 속도 재계산
        _currentFireRate = _currentGun.FireRate;

        _currentGun.MuzzleFlash.Play();
        PlaySE(_currentGun.FireSound);

        Hit();

        StopAllCoroutines();
        StartCoroutine(nameof(RetroActionCoroutine));           // 총기 반동
    }

    private void Hit()
    {
        float minAcc = -_crosshair.GetAccuracy() - _currentGun.Accuracy;
        float maxAcc = _crosshair.GetAccuracy() + _currentGun.Accuracy;

        float posX = Random.Range(minAcc, maxAcc);
        float posY = Random.Range(minAcc, maxAcc);

        Vector3 randomAccuracy = new Vector3(posX, posY, 0);

        // localPosition 으로 하면 어디있든 위치가 같아짐
        if (Physics.Raycast(_playerCamera.transform.position, _playerCamera.transform.forward + randomAccuracy, out _hitInfo, _currentGun.Range))
        {
            // // _hitInfo.normal == 충돌한 객체의 표면을 반환
            // GameObject clone = Instantiate(_ammoHitParticlePrefab, _hitInfo.point, Quaternion.LookRotation(_hitInfo.normal));
            // Destroy(clone, 2f);

            if (_objectPool._poolPrefab == null)
                return;

            _objectPool.GetObject(_hitInfo.point, Quaternion.LookRotation(_hitInfo.normal));
            // 풀에 반환은 나중에 고민하는걸로
        }
    }

    private void TryReload()
    {
        if (Input.GetKeyDown(KeyCode.R) && !_isReload && _currentGun.CurrentBulletCount < _currentGun.ReloadBulletCount)
        {
            CancelFineSight();
            StartCoroutine(nameof(ReloadCoroutine));
        }
    }

    private IEnumerator ReloadCoroutine()
    {
        if (_currentGun.CarryBulletCount > 0)
        {
            _isReload = true;
            _currentGun.GunAnimator.SetTrigger("Reload");

            // 현재 남아있는 탄알을 소유한 탄알에 더해줌
            // 탄알집에 남아있는 탄을 버리는 일을 방지하기 위함
            _currentGun.CarryBulletCount += _currentGun.CurrentBulletCount;
            _currentGun.CurrentBulletCount = 0;

            yield return new WaitForSeconds(_currentGun.ReloadTime);

            if (_currentGun.CarryBulletCount >= _currentGun.ReloadBulletCount)
            {
                _currentGun.CurrentBulletCount = _currentGun.ReloadBulletCount;
                _currentGun.CarryBulletCount -= _currentGun.ReloadBulletCount;
            }
            else
            {
                _currentGun.CurrentBulletCount = _currentGun.CarryBulletCount;
                _currentGun.CarryBulletCount = 0;
            }

            _isReload = false;
        }
        else
        {
            Debug.Log("No ammo");
        }
    }

    private void TryFineSight()
    {
        if (Input.GetButtonDown("Fire2") && !_isReload)
            FineSight();
    }

    public void CancelFineSight()
    {
        if (_isFineSightMode)
            FineSight();
    }

    private void FineSight()
    {
        _isFineSightMode = !_isFineSightMode;
        _currentGun.GunAnimator.SetBool("FineSightMode", _isFineSightMode);
        _crosshair.FineSightAnimation(_isFineSightMode);

        StopAllCoroutines();

        if (_isFineSightMode)
            StartCoroutine(nameof(FineSightActivateCoroutine));
        else
            StartCoroutine(nameof(FineSightDeactivateCoroutine));
    }

    private IEnumerator FineSightActivateCoroutine()
    {
        while (_currentGun.transform.localPosition != _currentGun.FineSightOriginPosition)
        {
            _currentGun.transform.localPosition = Vector3.Lerp(_currentGun.transform.localPosition, _currentGun.FineSightOriginPosition, .2f);

            if (IsClosedValue(_currentGun.transform.localPosition, _currentGun.FineSightOriginPosition))
            {
                _currentGun.transform.localPosition = _currentGun.FineSightOriginPosition;
                break;
            }

            yield return null;
        }
    }

    private IEnumerator FineSightDeactivateCoroutine()
    {
        while (_currentGun.transform.localPosition != _originPos)
        {
            _currentGun.transform.localPosition = Vector3.Lerp(_currentGun.transform.localPosition, _originPos, .2f);

            if (IsClosedValue(_currentGun.transform.localPosition, _originPos))
            {
                _currentGun.transform.localPosition = _originPos;
                break;
            }

            yield return null;
        }
    }

    // 무한 Lerp 방지
    private bool IsClosedValue(Vector3 first, Vector3 second)
    {
        Vector3 adjustPosition = (first - second).normalized;

        if (adjustPosition.x <= .01f && adjustPosition.y <= .01f && adjustPosition.z <= .01f)
            return true;

        return false;
    }

    private IEnumerator RetroActionCoroutine()
    {
        // 일반 상황에서 총기 반동
        Vector3 recoilBack = new Vector3(_currentGun.RetroActionForce, _originPos.y, _originPos.z);
        // 정조준 시 총기 반동
        Vector3 retroActionRecoilBack = new Vector3(_currentGun.RetroActionFineSightForce, _currentGun.FineSightOriginPosition.y, _currentGun.FineSightOriginPosition.z);

        if (!_isFineSightMode)
        {
            _currentGun.transform.localPosition = _originPos;

            // 반동 시작
            while (_currentGun.transform.localPosition.x <= _currentGun.RetroActionForce - .02f)
            {
                _currentGun.transform.localPosition = Vector3.Lerp(_currentGun.transform.localPosition, recoilBack, .4f);
                yield return null;
            }

            // 원위치
            while (_currentGun.transform.localPosition != _originPos)
            {
                _currentGun.transform.localPosition = Vector3.Lerp(_currentGun.transform.localPosition, _originPos, .1f);

                if (IsClosedValue(_currentGun.transform.localPosition, _originPos))
                {
                    _currentGun.transform.localPosition = _originPos;
                    break;
                }

                yield return null;
            }
        }
        else
        {
            _currentGun.transform.localPosition = _currentGun.FineSightOriginPosition;

            // 반동 시작
            while (_currentGun.transform.localPosition.x <= _currentGun.RetroActionFineSightForce - .02f)
            {
                _currentGun.transform.localPosition = Vector3.Lerp(_currentGun.transform.localPosition, retroActionRecoilBack, .4f);
                yield return null;
            }

            // 원위치
            while (_currentGun.transform.localPosition != _currentGun.FineSightOriginPosition)
            {
                _currentGun.transform.localPosition = Vector3.Lerp(_currentGun.transform.localPosition, _currentGun.FineSightOriginPosition, .1f);

                if (IsClosedValue(_currentGun.transform.localPosition, _currentGun.FineSightOriginPosition))
                {
                    _currentGun.transform.localPosition = _currentGun.FineSightOriginPosition;
                    break;
                }

                yield return null;
            }
        }
    }

    private void PlaySE(AudioClip _clip)
    {
        _audioSource.clip = _clip;
        _audioSource.Play();
    }
}
