using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("[Components]")]
    [SerializeField] private Camera _playerCamera;
    private Rigidbody _rigid;

    [Header("[Variables - Speed]")]
    [SerializeField] private float _walkSpeed;
    [SerializeField] private float _runSpeed;
    private float _applySpeed;

    [Header("[Vairables - Camera]")]
    [SerializeField] private float _lookSensitivity;
    [SerializeField] private float _cameraRotationLimit;
    private float _currentCameraRotationX;

    private bool isRun;

    // Start is called before the first frame update
    void Start()
    {
        _rigid = GetComponent<Rigidbody>();
        _applySpeed = _walkSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        CameraRotation();
        CharacterRotation();
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

    private void CameraRotation()
    {
        // 상하 카메라 회전
        float rotationX = Input.GetAxisRaw("Mouse Y");
        float cameraRotationX = rotationX * _lookSensitivity;

        _currentCameraRotationX -= cameraRotationX;
        _currentCameraRotationX = Mathf.Clamp(_currentCameraRotationX, -_cameraRotationLimit, _cameraRotationLimit);

        _playerCamera.transform.localEulerAngles = new Vector3(_currentCameraRotationX, 0f, 0f);
    }

    private void CharacterRotation()
    {
        // 좌우 캐릭터 회전
        float rotationY = Input.GetAxisRaw("Mouse X");
        Vector3 characterRotationY = new Vector3(0f, rotationY, 0f) * _lookSensitivity;

        _rigid.MoveRotation(_rigid.rotation * Quaternion.Euler(characterRotationY));
    }
}
