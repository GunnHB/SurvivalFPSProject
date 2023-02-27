using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandController : MonoBehaviour
{
    [Header("[Hand]")]
    [SerializeField] private Hand _currentHand;                  // 현재 장착된 Hand 타입형 무기

    private bool _isAttack = false;                              // 공격 중
    private bool _isSwing = false;                               // 팔을 휘두르는지

    private RaycastHit _hitInfo;                                 // ray 가 닿은 오브젝트의 정보

    private void Update()
    {
        TryAttack();
    }

    private void TryAttack()
    {
        if (Input.GetButton("Fire1"))
        {
            if (!_isAttack)
            {
                // 코루틴 실행
                StartCoroutine("AttackCoroutine");
            }
        }
    }

    private IEnumerator AttackCoroutine()
    {
        _isAttack = true;
        _currentHand.HandAnimator.SetTrigger("Attack");

        yield return new WaitForSeconds(_currentHand.AttackDelayA);
        _isSwing = true;

        // 공격 활성화 시점
        StartCoroutine("HitCoroutine");

        yield return new WaitForSeconds(_currentHand.AttackDelayB);
        _isSwing = false;

        yield return new WaitForSeconds(_currentHand.AttackDelay - _currentHand.AttackDelayA - _currentHand.AttackDelayB);
        _isAttack = false;
    }

    private IEnumerator HitCoroutine()
    {
        // 팔을 뻗은 시점에 닿은게 있는지 체크하기 위함
        while (_isSwing)
        {
            if (CheckObject())
            {
                // 충돌됨
                Debug.Log(_hitInfo.transform.name);
                _isSwing = false;
            }

            yield return null;
        }
    }

    private bool CheckObject()
    {
        // transform.forward == transform.TransformDirection(Vector3.forward)
        if (Physics.Raycast(transform.position, transform.forward, out _hitInfo, _currentHand.Range))
            return true;

        return false;
    }
}
