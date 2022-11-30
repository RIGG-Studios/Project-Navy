using System.Collections;
using Photon.Pun;
using UnityEngine;
using UnityEngine.VFX;

public class CannonController : MonoBehaviour
{
    public bool flipAxis;
    public GameObject muzzleFlash;
    public Camera camera;
    public Animator cameraAnimator;
    public bool occupied;
    public float maxHorizontalRotation;
    public float maxVerticalRotation;
    public Transform firePoint;
    public float cooldown;
    public float defaultFOV;
    public float aimFOV;
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
    private CameraShake _cameraShake;
    
    public int CannonID { get; private set; }
    
    private void Awake()
    {
        _currentAmmo = 1;
        camera.gameObject.SetActive(false);
        _cameraShake = camera.GetComponent<CameraShake>();
    }

    public void Init(Ship ship, int cannonID)
    {
        _ship = ship;
        CannonID = cannonID;
    }
    
    public void Occupy(PlayerController player)
    {
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
        if (!occupied)
        {
            return;
        }
        
        _occupier.canDoAnything = true;
        _occupier.bayonetteController.canDoStuff = true;
        _occupier.musketController.canDoAnything = true;

        
        _occupier.musketController.camera.gameObject.SetActive(true);
        camera.gameObject.SetActive(false);
        camera.fieldOfView = defaultFOV;
        _occupier.musketController.stopLooking = false;
        _occupier.musketController.camera.GetChild(0).gameObject.SetActive(true);
        
        _occupier = null;
        occupied = false;
    }

    void Update()
    {
        if (_currentAmmo <= 0)
        {
        //    Reload();
        }
        
        if(!occupied) return;

        if (Input.GetMouseButton(1))
        {
            camera.fieldOfView = Mathf.Lerp(camera.fieldOfView, aimFOV, Time.deltaTime * 5f);
        }
        else
        {
            camera.fieldOfView = Mathf.Lerp(camera.fieldOfView, defaultFOV, Time.deltaTime * 5f);

        }
        
        _yaw += Input.GetAxis("Mouse Y") * 0.5f;
        _pitch += Input.GetAxis("Mouse X") * 0.5f;

        _pitch = Mathf.Clamp(_pitch, -maxHorizontalRotation, maxHorizontalRotation);
        _yaw = Mathf.Clamp(_yaw, -maxVerticalRotation / 1.5f, maxVerticalRotation);

        transform.localRotation =  Quaternion.Euler(0, _pitch, flipAxis ? _yaw : -_yaw);

        if (_occupier.fire)
        {
            if (_currentAmmo <= 0)
            {
     //           return;
            }

            if(_timeSinceLastFired + cooldown > Time.time)
                return;
            
            _currentAmmo--;
            _timeSinceLastFired = Time.time;
            _cameraShake.ShakeCamera("CannonShot");
            cameraAnimator.SetTrigger("Shoot");
    
            _ship.OnCannonFired(CannonID, firePoint.position, firePoint.rotation);
            Vector3 velocity = firePoint.forward * cannonBallVelocity;
            GameManager.Instance.SpawnProjectile(velocity, firePoint.position, firePoint.rotation, _ship.OwnerActorNumber);
        }
    }
    
}
