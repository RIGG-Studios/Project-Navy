using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Assertions.Must;

public class PlayerController : MonoBehaviour
{
    public float shipCorrection;
    public Transform groundCheck;
    public float groundRadius;
    public LayerMask groundLayer;
    public MusketController musketController;
    public BayonetteController bayonetteController;
    public AudioClip[] walkingSounds;
    public AudioClip[] aimingSounds;
    public AudioClip[] landedSounds;
    public AudioSource source;
    public Animator animator;
    public float walkSpeed;
    public float runSpeed;
    public float jumpHeight;
    public float footstepSoundLength;
    public float footstepSoundSpeed;
    public float runSoundSpeed;
    public bool canDoAnything;
    public Vector2 moveDirection;
    public bool fire;
    public float cannonInteractRange;
    public GameObject cannonBallPrefab;
    public string cannonBallTag;
    
    private bool _isGrounded;
    private Rigidbody _body;
    private bool wPressed;
    private bool sPressed;
    private bool aPressed;
    private bool dPressed;
    private bool _isSprinting;
    private float _moveSpeed;
    private bool _jumpedLastFrame;
    private bool _isMoving;
    private bool _playingFootstepSound;
    private CannonController _occupiedCannon;
    private bool _hasCannonBall;
    private GameObject _instantiatedCannonBall;
    private Player _player;

    public bool canRecieveInput;

    private Ship _ship;
    
    void Awake()
    {
        _body = GetComponent<Rigidbody>();
        _player = GetComponent<Player>();
        _moveSpeed = walkSpeed;
        canDoAnything = true;
        canRecieveInput = true;
    }

    private void Update()
    {
        if (!canRecieveInput)
        {
            wPressed = false;
            sPressed = false;
            aPressed = false;
            dPressed = false;
            moveDirection = Vector2.zero;
            return;
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            wPressed = true;
        }
        else if (Input.GetKeyUp(KeyCode.W))
        {
            wPressed = false;
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            sPressed = true;
        }
        else if (Input.GetKeyUp(KeyCode.S))
        {
            sPressed = false;
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            aPressed = true;
        }
        else if (Input.GetKeyUp(KeyCode.A))
        {
            aPressed = false;
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            dPressed = true;
        }
        else if (Input.GetKeyUp(KeyCode.D))
        {
            dPressed = false;
        }

        if (Input.GetMouseButtonDown(0))
        {
            fire = true;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            fire = false;
        }

        RaycastHit hit;
        if (Physics.Raycast(musketController.camera.position, musketController.camera.forward, out hit))
        {
            if (!_occupiedCannon)
            {
                _player.ToggleInteractHelper(true);
            }

            if (Input.GetKeyDown(KeyCode.F))
            {
                if ((hit.transform.position - transform.position).magnitude > cannonInteractRange) return;

                CannonController controller = null;

                controller = hit.collider.GetComponent<CannonController>();
                
                _player.ToggleCannonUI(true);

                if (!controller && !hit.transform.CompareTag(cannonBallTag)) return;

                if (hit.transform.CompareTag(cannonBallTag))
                {
                    if (_hasCannonBall) return;

                    source.clip = aimingSounds[UnityEngine.Random.Range(0, aimingSounds.Length)];
                    source.Play();

                    _hasCannonBall = true;
                    musketController.canDoAnything = false;
                    bayonetteController.canDoStuff = false;
                    musketController.musketAnimator.transform.gameObject.SetActive(false);
                    _instantiatedCannonBall = Instantiate(cannonBallPrefab);
                    _instantiatedCannonBall.transform.position = musketController.musketAnimator.transform.position;
                    _instantiatedCannonBall.transform.parent = musketController.musketAnimator.transform.parent;

                    return;
                }

                if (_hasCannonBall)
                {
                    source.clip = aimingSounds[UnityEngine.Random.Range(0, aimingSounds.Length)];
                    source.Play();

                    controller.Reload();
                    _hasCannonBall = false;
                    musketController.canDoAnything = true;
                    bayonetteController.canDoStuff = true;
                    Destroy(_instantiatedCannonBall);
                    musketController.musketAnimator.transform.gameObject.SetActive(true);

                    return;
                }

                source.clip = aimingSounds[UnityEngine.Random.Range(0, aimingSounds.Length)];
                source.Play();
                controller.Occupy(this);
                _occupiedCannon = controller;
            }
        }
        else
        {
            _player.ToggleInteractHelper(false);
        }
        
        CheckForShip();
        
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if(!_occupiedCannon) return;
            
            _occupiedCannon.UnOccupy(this);
            _player.ToggleCannonUI(false);
            _occupiedCannon = null;
        }

        moveDirection.x = wPressed ? 1 : 0;
        moveDirection.x = sPressed ? -1 : moveDirection.x;
        
        moveDirection.y = dPressed ? 1 : 0;
        moveDirection.y = aPressed ? -1 : moveDirection.y;
        
        if(!canDoAnything)
            return;
        
        if (!_playingFootstepSound && _isMoving)
        {
            StartCoroutine("PlayFootstepSounds");
        }

        if (_isGrounded && _jumpedLastFrame)
        {
            animator.SetTrigger("Landed");
            source.clip = landedSounds[UnityEngine.Random.Range(0, landedSounds.Length)];
            source.Play();
            _jumpedLastFrame = false;
        }
        
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            if (musketController.isAiming)
                return;
            
            source.clip = aimingSounds[UnityEngine.Random.Range(0, aimingSounds.Length)];
            source.Play();
            
            _isSprinting = true;
            bayonetteController.canDoStuff = false;
            musketController.canDoAnything = false;
            animator.SetBool("Running", true);
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            if (musketController.isAiming)
                return;
            
            source.clip = aimingSounds[UnityEngine.Random.Range(0, aimingSounds.Length)];
            source.Play();
            
            _isSprinting = false;
            bayonetteController.canDoStuff = true;
            musketController.canDoAnything = true;
            animator.SetBool("Running", false);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            source.clip = aimingSounds[UnityEngine.Random.Range(0, aimingSounds.Length)];
            source.Play();
            
            StartCoroutine("Jump");
        }

        if (moveDirection.x == 0 && moveDirection.y == 0)
        {
            animator.SetBool("Walking", false);
            _isMoving = false;
        }
        else
        {
            animator.SetBool("Walking", true);
            _isMoving = true;
        }

        _moveSpeed = _isSprinting ? runSpeed : walkSpeed;
    }

    private void FixedUpdate()
    {
        if (!canDoAnything)
            return;
        
        _isGrounded = Physics.CheckSphere(groundCheck.position, groundRadius, groundLayer);

        Move();
    }
    
    private IEnumerator Jump()
    {
        if(_isGrounded)
        {
            source.clip = aimingSounds[UnityEngine.Random.Range(0, aimingSounds.Length)];
            source.Play();
            
            animator.SetTrigger("Jumped");
            _body.AddForce(Vector3.up * jumpHeight);
            yield return new WaitForSeconds(0.3f);
            _jumpedLastFrame = true;
        }
    }
    
    void Move()
    {
        _body.AddForceAtPosition(Physics.gravity, transform.position, ForceMode.Acceleration);
        
        Vector3 playerVelocity = transform.position +
                                 (moveDirection.y * transform.right + moveDirection.x * transform.forward) *
                                 (Time.fixedDeltaTime * _moveSpeed);

        _body.MovePosition(playerVelocity);

        if (_ship != null)
        {
            Vector3 shipVelocity = _ship.Rigidbody.velocity;
            Vector3 forwardVel = _body.velocity - shipVelocity;

            Vector3 shipAngularVelocity = _ship.Rigidbody.angularVelocity;
            Vector3 angularVel = _body.angularVelocity - shipAngularVelocity;

            _body.AddForce(forwardVel + angularVel, ForceMode.Force);
        }
    }

    IEnumerator PlayFootstepSounds()
    {
        _playingFootstepSound = true;
        if (_isSprinting)
        {
            yield return new WaitForSeconds(footstepSoundLength * 1/_moveSpeed * 1/runSoundSpeed);
        }
        else
        {
            yield return new WaitForSeconds(footstepSoundLength * 1/_moveSpeed * 1/footstepSoundSpeed);
        }
        
        if (_isMoving && _isGrounded)
        {
            source.clip = walkingSounds[UnityEngine.Random.Range(0, walkingSounds.Length)];
            source.Play();
        }
        _playingFootstepSound = false;
    }

    private void CheckForShip()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, -transform.up, out hit, 10))
        {
            if (hit.collider.TryGetComponent(out ShipVitalPoint ship))
            {
                if (ship.ShipHealth != null)
                {
                    _ship = ship.ShipHealth.Ship;
                }
            }
        }
    }
}
