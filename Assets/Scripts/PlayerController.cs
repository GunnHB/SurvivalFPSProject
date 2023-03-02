using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("[Components]")]
    [SerializeField] private Camera _playerCamera;
    private Rigidbody _rigid;
    private CapsuleCollider _collider;                  // 착지 여부를 판단하기 위함
    private GunController _gunController;
    [SerializeField] private Crosshair _crosshair;

    [Header("[Variables - Speed]")]
    [SerializeField] private float _walkSpeed;
    [SerializeField] private float _runSpeed;
    private float _applySpeed;

    [Header("[Vairables - Jump]")]
    [SerializeField] private float _jumpForce;

    [Header("[Variables - Crouch]")]
    [SerializeField] private float _crouchSpeed;
    [SerializeField] private float _crouchPosY;
    private float _originPosY;
    private float _applyCrouchPosY;

    [Header("[Vairables - Camera]")]
    [SerializeField] private float _lookSensitivity;
    [SerializeField] private float _cameraRotationLimit;
    private float _currentCameraRotationX;

    private bool _isWalk = false;
    private bool _isRun = false;
    private bool _isCrouch = false;
    private bool _isGround = true;

    // 움직임 체크 변수
    private Vector3 lastPosition;

    // Start is called before the first frame update
    void Start()
    {
        _rigid = GetComponent<Rigidbody>();
        _collider = GetComponent<CapsuleCollider>();
        _gunController = GetComponentInChildren<GunController>();

        _applySpeed = _walkSpeed;
        _originPosY = _playerCamera.transform.localPosition.y;
        _applyCrouchPosY = _originPosY;
    }

    // Update is called once per frame
    void Update()
    {
        IsGround();
        TryJump();
        TryRun();
        TryCrouch();
        Move();
        MoveCheck();
        CameraRotation();
        CharacterRotation();
    }

    // 앉기 시도
    private void TryCrouch()
    {
        if (Input.GetKeyDown(KeyCode.C))
            Crouch();
    }

    // 앉기 실행
    private void Crouch()
    {
        _isCrouch = !_isCrouch;
        _crosshair.CrouchingAnimation(_isCrouch);

        if (_isCrouch)
        {
            _applySpeed = _crouchSpeed;
            _applyCrouchPosY = _crouchPosY;
        }
        else
        {
            _applySpeed = _walkSpeed;
            _applyCrouchPosY = _originPosY;
        }

        // 앉기 계산
        StartCoroutine("CrouchCoroutine");
    }

    private IEnumerator CrouchCoroutine()
    {
        float posX = _playerCamera.transform.localPosition.x;
        float posY = _playerCamera.transform.localPosition.y;
        float posZ = _playerCamera.transform.localPosition.z;

        float first = _isCrouch ? posY : _applyCrouchPosY;
        float second = _isCrouch ? _applyCrouchPosY : posY;

        while (posY != _applyCrouchPosY)
        {
            // 높을수록 빨리 증가
            posY = Mathf.Lerp(posY, _applyCrouchPosY, .3f);
            _playerCamera.transform.localPosition = new Vector3(posX, posY, posZ);

            // 거의 같아지면 그냥 while 문 탈출
            if (first - second <= .01f)
                break;

            yield return null;
        }

        _playerCamera.transform.localPosition = new Vector3(posX, _applyCrouchPosY, posZ);
    }

    private void IsGround()
    {
        // Vector3.down -> 월드의 아래로 레이캐스트를 쏴야하기 때문
        // _collider.bounds.extents.y -> 콜라이더의 y 값의 절반만큼의 길이로 발사
        // + .1f 혹시 모를 상황에 대비하여 여유값을 추가 
        _isGround = Physics.Raycast(transform.position, Vector3.down, _collider.bounds.extents.y + .1f);
    }

    private void TryJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && _isGround)
            Jump();
    }

    private void Jump()
    {
        // 앉은 상태에서 점프하면 앉기 해제
        if (_isCrouch)
            Crouch();

        _rigid.velocity = transform.up * _jumpForce;
    }

    private void TryRun()
    {
        if (Input.GetKey(KeyCode.LeftShift))
            Running();
        if (Input.GetKeyUp(KeyCode.LeftShift))
            RunningCancel();
    }

    private void Running()
    {
        if (_isCrouch)
            Crouch();

        _isRun = true;
        _applySpeed = _runSpeed;
        _crosshair.RunningAnimation(_isRun);

        if (_gunController != null && _gunController.IsFineSightMode)
            _gunController.CancelFineSight();
    }

    private void RunningCancel()
    {
        _isRun = false;
        _applySpeed = _walkSpeed;
        _crosshair.RunningAnimation(_isRun);
    }

    private void Move()
    {
        float moveDirectionX = Input.GetAxisRaw("Horizontal");
        float moveDirectionZ = Input.GetAxisRaw("Vertical");

        Vector3 moveHorizontal = transform.right * moveDirectionX;
        Vector3 moveVertical = transform.forward * moveDirectionZ;

        Vector3 velocity = (moveHorizontal + moveVertical).normalized * _applySpeed;

        _rigid.MovePosition(transform.position + velocity * Time.deltaTime);
    }

    private void MoveCheck()
    {
        if (!_isRun && !_isCrouch)
        {
            if (Vector3.Distance(lastPosition, transform.position) >= .01f)
                _isWalk = true;
            else
                _isWalk = false;

            _crosshair.WalkingAnimation(_isWalk);
            lastPosition = transform.position;
        }
    }

    // 상하 카메라 회전
    private void CameraRotation()
    {
        float rotationX = Input.GetAxisRaw("Mouse Y");
        float cameraRotationX = rotationX * _lookSensitivity;

        _currentCameraRotationX -= cameraRotationX;
        _currentCameraRotationX = Mathf.Clamp(_currentCameraRotationX, -_cameraRotationLimit, _cameraRotationLimit);

        _playerCamera.transform.localEulerAngles = new Vector3(_currentCameraRotationX, 0f, 0f);
    }

    // 좌우 캐릭터 회전
    private void CharacterRotation()
    {
        float rotationY = Input.GetAxisRaw("Mouse X");
        Vector3 characterRotationY = new Vector3(0f, rotationY, 0f) * _lookSensitivity;

        _rigid.MoveRotation(_rigid.rotation * Quaternion.Euler(characterRotationY));
    }
}
