using System;
using System.Collections;
using Unity.Mathematics;
using UnityEngine;

//After the prototype phase is over, delete this script and start from scratch, nothing in here is worth preserving
public class MusketController : MonoBehaviour
{
    public Transform camera;
    public float sensitivity;
    public float range;
    public string playerTag;
    public Transform firePoint;
    public int maxAmmo;
    public float reloadTime;
    
    private float _yaw;
    private float _pitch;
    private int _currentAmmo;
    private bool _isReloading;

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        _currentAmmo = maxAmmo;
    }

    void Update()
    {
        _yaw += Input.GetAxis ("Mouse X") * sensitivity;
        _pitch += -Input.GetAxis ("Mouse Y") * sensitivity;

        _pitch = Mathf.Clamp(_pitch, -80.0f, 80.0f);
        
        transform.eulerAngles = transform.up * (_yaw);
        camera.localEulerAngles = new Vector3(_pitch, 0, 0);

        if (Input.GetMouseButtonDown(0))
        {
            if (_currentAmmo <= 0)
                return;

            _currentAmmo--;
            RaycastHit hit;
            if (Physics.Raycast(firePoint.position, firePoint.forward, out hit))
            {
                if (!((hit.transform.position - firePoint.transform.position).magnitude <= range))
                    return;
                if(!hit.collider.gameObject.CompareTag(playerTag))
                    return;
                Debug.Log("Do damage here");
            }
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            if(_isReloading) 
                return;
            
            StartCoroutine("ReloadRoutine");
        }
    }

    IEnumerator ReloadRoutine()
    {
        _isReloading = true;
        yield return new WaitForSeconds(reloadTime);
        _currentAmmo = maxAmmo;
        _isReloading = false;
    }
}
