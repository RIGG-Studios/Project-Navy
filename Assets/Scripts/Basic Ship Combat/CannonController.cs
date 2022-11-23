using System;
using UnityEngine;

public class CannonController : MonoBehaviour
{
    public bool occupied;
    public float maxHorizontalRotation;
    public float maxVerticalRotation;
    public GameObject cannonBall;
    public Transform newCameraTransform;
    public Transform firePoint;
    public float cooldown;
    public float cannonBallVelocity;

    private PlayerController _occupier;
    private float _yaw;
    private float _pitch;
    private Vector3 _occupierCameraPosition;
    private Quaternion _occupierCameraRotation;
    private float _timeSinceLastFired;
    private int _currentAmmo;

    private void Awake()
    {
        _currentAmmo = 1;
    }

    public void Occupy(PlayerController player)
    {
        if(occupied) return;

        occupied = true;
        _occupier = player;
        _occupier.canDoAnything = false;
        _occupier.bayonetteController.canDoStuff = false;
        _occupier.musketController.canDoAnything = false;
        
        _occupierCameraPosition = _occupier.musketController.camera.position;
        _occupierCameraRotation = _occupier.musketController.camera.rotation;

        _occupier.musketController.camera.position = newCameraTransform.position;
        _occupier.musketController.camera.rotation = newCameraTransform.rotation;
        _occupier.musketController.camera.parent = newCameraTransform;
        _occupier.musketController.camera.GetChild(0).gameObject.SetActive(false);
        _occupier.musketController.stopLooking = true;
    }

    public void UnOccupy(PlayerController player)
    {
        if (player != _occupier) return;

        _occupier.canDoAnything = true;
        _occupier.bayonetteController.canDoStuff = true;
        _occupier.musketController.canDoAnything = true;

        _occupier.musketController.camera.position = _occupierCameraPosition;
        _occupier.musketController.camera.rotation = _occupierCameraRotation;
        _occupier.musketController.stopLooking = false;
        _occupier.musketController.camera.parent = _occupier.transform;
        _occupier.musketController.camera.GetChild(0).gameObject.SetActive(true);
        
        _occupier = null;
        occupied = false;
    }

    void Update()
    {
        if(!occupied) return;

        _pitch += _occupier.moveDirection.y;
        _yaw += _occupier.moveDirection.x;

        _pitch = Mathf.Clamp(_pitch, -maxHorizontalRotation, maxHorizontalRotation);
        _yaw = Mathf.Clamp(_yaw, 0, maxVerticalRotation);

        transform.eulerAngles = new Vector3(0, _pitch, -_yaw);

        if (_occupier.fire)
        {
            if(_currentAmmo <= 0)
                return;

            if(_timeSinceLastFired + cooldown > Time.time)
                return;
            
            _currentAmmo--;
            _timeSinceLastFired = Time.time;
            
            GameObject instantiatedCannonBall = Instantiate(cannonBall);
            instantiatedCannonBall.transform.position = firePoint.position;
            instantiatedCannonBall.GetComponent<Rigidbody>().AddForce(firePoint.forward * cannonBallVelocity, ForceMode.Impulse);
        }
    }
}
