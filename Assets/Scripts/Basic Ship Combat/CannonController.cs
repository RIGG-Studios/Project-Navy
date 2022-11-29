using System.Collections;
using Photon.Pun;
using UnityEngine;
using UnityEngine.VFX;

public class CannonController : MonoBehaviour
{
    public bool flipAxis;
    public GameObject muzzleFlash;
    public Camera camera;
    public bool occupied;
    public float maxHorizontalRotation;
    public float maxVerticalRotation;
    public Transform firePoint;
    public float cooldown;
    public float cannonBallVelocity;
    public AudioClip[] fireSounds;
    public AudioClip[] reloadSounds;
    public float reloadTime;
    public AudioSource source;

    private PlayerController _occupier;
    private float _yaw;
    private float _pitch;
    private Vector3 _occupierCameraPosition;
    private Quaternion _occupierCameraRotation;
    private float _timeSinceLastFired;
    private int _currentAmmo;
    private bool _isReloading;

    private Ship _ship;
    
    public int CannonID { get; private set; }
    
    private void Awake()
    {
        _currentAmmo = 1;
        camera.gameObject.SetActive(false);
    }

    public void Init(Ship ship, int cannonID)
    {
        _ship = ship;
        CannonID = cannonID;
    }

    public void Occupy(PlayerController player)
    {
        if(occupied) return;
        if (_currentAmmo <= 0)
        {
            return;
        }

        occupied = true;
        _occupier = player;
        _occupier.canDoAnything = false;
        _occupier.bayonetteController.canDoStuff = false;
        _occupier.musketController.canDoAnything = false;
        
        _occupier.musketController.camera.gameObject.SetActive(false);
        camera.gameObject.SetActive(true);
        
        _occupier.musketController.camera.GetChild(0).gameObject.SetActive(false);
        _occupier.musketController.stopLooking = true;
    }

    public void Reload()
    {
        if(_isReloading)
            return;
        
        StartCoroutine("ReloadRoutine");
    }

    IEnumerator ReloadRoutine()
    {
        _isReloading = true;
        source.clip = reloadSounds[UnityEngine.Random.Range(0, reloadSounds.Length)];
        source.Play();

        yield return new WaitForSeconds(reloadTime);

        _isReloading = false;
        _currentAmmo = 1;
    }

    public void UnOccupy(PlayerController player)
    {
        if (player != _occupier) return;

        _occupier.canDoAnything = true;
        _occupier.bayonetteController.canDoStuff = true;
        _occupier.musketController.canDoAnything = true;

        
        _occupier.musketController.camera.gameObject.SetActive(true);
        camera.gameObject.SetActive(false);
        _occupier.musketController.stopLooking = false;
        _occupier.musketController.camera.GetChild(0).gameObject.SetActive(true);
        
        _occupier = null;
        occupied = false;
    }

    void Update()
    {
        if(!occupied) return;

        _yaw += Input.GetAxis("Mouse Y") * 0.5f;
        _pitch += Input.GetAxis("Mouse X") * 0.5f;

        _pitch = Mathf.Clamp(_pitch, -maxHorizontalRotation, maxHorizontalRotation);
        _yaw = Mathf.Clamp(_yaw, -maxVerticalRotation / 1.5f, maxVerticalRotation);

        transform.rotation =  Quaternion.Euler(0, _pitch, flipAxis ? _yaw : -_yaw);

        if (_occupier.fire)
        {
            if (_currentAmmo <= 0)
            {
            }

            if(_timeSinceLastFired + cooldown > Time.time)
                return;
            
            _currentAmmo--;
            _timeSinceLastFired = Time.time;
    
            _ship.OnCannonFired(CannonID, firePoint.position, firePoint.rotation);
            Vector3 velocity = firePoint.forward * cannonBallVelocity;
            GameManager.Instance.SpawnProjectile(velocity, firePoint.position, firePoint.rotation, _ship.OwnerActorNumber);
        }
    }
    
}
