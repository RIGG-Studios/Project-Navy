using System;
using System.Collections;
using Photon.Pun;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.VFX;
using Random = System.Random;

//After the prototype phase is over, delete this script and start from scratch, nothing in here is worth preserving
public class MusketController : MonoBehaviourPun
{
    public Transform camera;
    public Transform gunHolder;
    public float sensitivity;
    public float range;
    public string playerTag;
    public Transform firePoint;
    public int maxAmmo;
    public float reloadTime;
    public float swayAmount;
    public float swaySpeed;
    public float aimedSwayAmount;
    public float aimedSwaySpeed;
    public float swayClamp;
    public float recoilAmount;
    public float kickbackAmount;
    public float recoilDecreaseRate;
    public float damage;
    public AudioClip[] fireSounds;
    public AudioClip[] hitImpactSounds;
    public AudioClip[] hitPlayerSounds;
    public AudioClip[] aimSounds;
    public AudioClip musketDryFire;
    public GameObject hitImpactPrefab;
    public Animator animator;
    public Animator musketAnimator;
    public GameObject muzzleFlash;
    public Animator walkingAnimator;
    public BayonetteController controller;
    public bool canDoAnything;
    public bool isAiming;
    public bool stopLooking;
    
    private float _yaw;
    private float _pitch;
    private int _currentAmmo;
    private bool _isReloading;
    private Vector3 musketPivotOriginalPosition;

    private Vector3 _weaponSway;
    private float _recoilAngle;
    private AudioSource _fireSource;
    private Player _player;

    private float _currentSwayAmount;
    private float _currentSwaySpeed;

    private float _xRot;

    private void Awake()
    {
        _player = GetComponent<Player>();
        
        Cursor.lockState = CursorLockMode.Locked;
        _currentAmmo = maxAmmo;
        musketPivotOriginalPosition = gunHolder.transform.localPosition;
        _fireSource = firePoint.GetComponent<AudioSource>();

        _currentSwayAmount = swayAmount;
        _currentSwaySpeed = swaySpeed;
        canDoAnything = true;
    }

    Vector3 finalRotation;
    
    void Update()
    {
        _yaw += Input.GetAxis ("Mouse X") * sensitivity;
        _pitch += -Input.GetAxis ("Mouse Y") * sensitivity;

        _weaponSway.x -= Input.GetAxis("Mouse Y") * _currentSwayAmount;
        _weaponSway.y += Input.GetAxis("Mouse X") * _currentSwayAmount;
        _weaponSway.z += Input.GetAxis("Mouse X") * _currentSwayAmount;

        _weaponSway.x = Mathf.Clamp(_weaponSway.x, -swayClamp, swayClamp);
        _weaponSway.y = Mathf.Clamp(_weaponSway.y, -swayClamp, swayClamp);
        _weaponSway.z = Mathf.Clamp(_weaponSway.z, -swayClamp, swayClamp);

        _recoilAngle = Mathf.Lerp(_recoilAngle, 0, Time.deltaTime * recoilDecreaseRate);
        

        if (!stopLooking)
        {
            float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;

            _xRot -= mouseY;
            _xRot = Mathf.Clamp(_xRot, -90f, 90f);
        
            camera.localRotation = Quaternion.Euler(_xRot - _recoilAngle, 0f, 0f);
            transform.Rotate(Vector3.up * mouseX);
            
            /*/
            transform.eulerAngles = transform.up * (_yaw);
            camera.localEulerAngles = new Vector3(finalAngle, 0, 0);
/*/
            _weaponSway = Vector3.Lerp(_weaponSway, Vector3.zero, Time.deltaTime * _currentSwaySpeed);
            finalRotation = Vector3.Lerp(finalRotation, _weaponSway, Time.deltaTime * _currentSwaySpeed);

            gunHolder.localEulerAngles = finalRotation;
            gunHolder.localPosition = musketPivotOriginalPosition - new Vector3(0, 0, _recoilAngle * kickbackAmount);
        }

        if(!canDoAnything)
            return;
        
        if (Input.GetMouseButtonDown(0))
        {
            if (_currentAmmo <= 0)
            {
                _fireSource.clip = musketDryFire;
                _fireSource.Play();
                return;
            }

            _currentAmmo--;
            musketAnimator.SetTrigger("Fire");
            
            photonView.RPC("RPCEffects", RpcTarget.All);
            
            _recoilAngle += recoilAmount;
            
            RaycastHit hit;
            if (Physics.Raycast(firePoint.position, firePoint.forward, out hit))
            {
                GameObject instantiatedHitImpact = Instantiate(hitImpactPrefab);
                instantiatedHitImpact.transform.position = hit.point;
                AudioClip clipToPlay;
                DoHitImpacts impactEffects = instantiatedHitImpact.GetComponent<DoHitImpacts>();
                
                clipToPlay = hitPlayerSounds[UnityEngine.Random.Range(0, hitPlayerSounds.Length)];
                impactEffects.PlayHitImpactSound(clipToPlay, (firePoint.transform.position - hit.point).magnitude/30);

                if (hit.collider.TryGetComponent(out IDamagable damagable))
                {
                    PhotonDamageHandler.SendDamageRequest(_player.PlayerActorNumber, damagable.ActorID, damage);
                }
                
                Destroy(instantiatedHitImpact, 3);
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            animator.SetBool("IsAiming", true);
            walkingAnimator.SetBool("IsAiming", true);
            _fireSource.clip = aimSounds[UnityEngine.Random.Range(0, aimSounds.Length)];
            _fireSource.Play();
            _currentSwayAmount = aimedSwayAmount;
            _currentSwaySpeed = aimedSwaySpeed;
            controller.canDoStuff = false;
        }

        if (Input.GetMouseButtonUp(1))
        {
            animator.SetBool("IsAiming", false);
            walkingAnimator.SetBool("IsAiming", false);
            _fireSource.clip = aimSounds[UnityEngine.Random.Range(0, aimSounds.Length)];
            _fireSource.Play();

            _currentSwayAmount = swayAmount;
            _currentSwaySpeed = swaySpeed;
            controller.canDoStuff = true;
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            if(_isReloading || _currentAmmo > 0) 
                return;
            
            StartCoroutine("ReloadRoutine");
        }
    }

    [PunRPC]
    public void RPCEffects()
    {
        VisualEffect muzzleFlash = Instantiate(this.muzzleFlash, firePoint.position, firePoint.rotation)
            .GetComponent<VisualEffect>();
        
        muzzleFlash.Play();
        
        _fireSource.clip = fireSounds[UnityEngine.Random.Range(0, fireSounds.Length)];
        _fireSource.Play();
    }

    IEnumerator ReloadRoutine()
    {
        controller.canDoStuff = false;
        _isReloading = true;
        musketAnimator.SetTrigger("StartReload");
        _fireSource.clip = aimSounds[UnityEngine.Random.Range(0, aimSounds.Length)];
        _fireSource.Play();
        yield return new WaitForSeconds(reloadTime);
        musketAnimator.SetTrigger("EndReload");
        _fireSource.clip = aimSounds[UnityEngine.Random.Range(0, aimSounds.Length)];
        _fireSource.Play();
        _currentAmmo = maxAmmo;
        _isReloading = false;
        controller.canDoStuff = true;
    }
}
