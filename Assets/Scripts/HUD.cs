using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using TMPro;

public class HUD : MonoBehaviour
{
    [Header("[Components]")]
    [SerializeField] private GunController _gunController;

    [Header("[GameObject]")]
    [SerializeField] private GameObject _objBulletHUD;

    [Header("[Texts]")]
    [SerializeField] private List<TextMeshProUGUI> _textBulletList;

    private void Update()
    {
        CheckBullet();
    }

    private void CheckBullet()
    {
        _textBulletList[0].text = _gunController.CurrentGun.CarryBulletCount.ToString();
        _textBulletList[1].text = _gunController.CurrentGun.ReloadBulletCount.ToString();
        _textBulletList[2].text = _gunController.CurrentGun.CurrentBulletCount.ToString();
    }
}
