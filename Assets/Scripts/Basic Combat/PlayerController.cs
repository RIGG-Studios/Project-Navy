using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public LayerMask ignoreLayer;
    public Text cannonsText;
    public GameObject pickedUpUI;
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
    private CameraShake _cameraShake;
    private bool wPressed;
    private bool sPressed;
    private bool aPressed;
    private bool dPressed;
    private bool _isSprinting;
    private float _moveSpeed;
    private bool _jumpedLastFrame;
    private bool _isMoving;
    private bool _playingFootstepSound;

    public CannonController occupiedCannon
    {
        get;
        private set;
    }
    
    private bool _hasCannonBall;
    private GameObject _instantiatedCannonBall;
    private Player _player;
    private ShipControl _shipControl;

    public bool canRecieveInput;
    private Ship _ship;

    public int cannonBalls { get; private set; }

    private IInteractable _currentInteractable;
    
    void Awake()
    {
        _body = GetComponent<Rigidbody>();
        _player = GetComponent<Player>();
        _cameraShake = GetComponentInChildren<CameraShake>();
        _shipControl = GetComponent<ShipControl>();
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
        if (Physics.Raycast(musketController.camera.position, musketController.camera.forward, out hit,
                cannonInteractRange, ignoreLayer))
        {
            hit.collider.TryGetComponent(out IInteractable interactable);

            if (interactable != null)
            {
                _player.ToggleInteractHelper(!occupiedCannon);

                interactable.LookAt();

                if (Input.GetKeyDown(KeyCode.F))
                {
                    interactable.Interact(this);

                    if (interactable.InteractTypes == InteractTypes.Cannon && !occupiedCannon)
                    {
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
                        occupiedCannon = controller;
                    }
                }
            }
        }
        else
        {
            _player.ToggleInteractHelper(false);
        }

        CheckForShip();

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            UnOccupyCannon();
        }

        moveDirection.x = wPressed ? 1 : 0;
        moveDirection.x = sPressed ? -1 : moveDirection.x;

        moveDirection.y = dPressed ? 1 : 0;
        moveDirection.y = aPressed ? -1 : moveDirection.y;

        if (!canDoAnything)
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

    public void GiveCannonBall()
    {
        cannonBalls++;
        cannonBalls = Mathf.Clamp(cannonBalls, 0, 5);
        cannonsText.text = cannonBalls.ToString();
    }

    public void HidePickedUpUI()
    {
        pickedUpUI.SetActive(false);
    }

    public void TakeCannonBall()
    {
        cannonBalls--;
        cannonBalls = Mathf.Clamp(cannonBalls, 0, 5);
        cannonsText.text = cannonBalls.ToString();
    }

    public void Reset()
    {
        wPressed = false;
        sPressed = false;
        aPressed = false;
        dPressed = false;
        moveDirection = Vector2.zero;
        
        UnOccupyCannon();
    }


    private void FixedUpdate()
    {
        if (!canDoAnything)
            return;
        
        _isGrounded = Physics.CheckSphere(groundCheck.position, groundRadius, groundLayer);

        Move();
    }


    public void UnOccupyCannon()
    {
        if(!occupiedCannon) return;
            
        occupiedCannon.UnOccupy(this);
        _player.ToggleCannonUI(false);
        occupiedCannon = null;
    }

    public void ToggleShipControl(bool state)
    {
        if (state)
        {
            _shipControl.Enable();
        }
        else
        {
            _shipControl.Disable();
        }
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
