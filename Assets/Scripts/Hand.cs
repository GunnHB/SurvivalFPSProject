using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour
{
    [Header("[Components]")]
    [SerializeField] private Animator _handAnimator;       // 애니메이션

    [Header("[Variables]")]
    [SerializeField] private string _handName;             // 너클이나 맨손 구분
    [SerializeField] private float _range;                 // 공격 범위
    [SerializeField] private int _damage;                  // 공격력
    [SerializeField] private float _workSpeed;             // 작업 속도
    [SerializeField] private float _attackDelay;           // 공격 딜레이
    [SerializeField] private float _attackDelayA;          // 공격 활성화 시점 (주먹이 나가는 시점)
    [SerializeField] private float _attackDelayB;          // 공격 비활성화 시점 (주먹이 들어오는 시점)

    // Properties
    public Animator HandAnimator => _handAnimator;
    public string HandName => _handName;
    public float Range => _range;
    public int Damage => _damage;
    public float WorkSpeed => _workSpeed;
    public float AttackDelay => _attackDelay;
    public float AttackDelayA => _attackDelayA;
    public float AttackDelayB => _attackDelayB;
}
