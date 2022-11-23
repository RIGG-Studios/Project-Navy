using System;
using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
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
    public CannonController cannon;
    public bool fire;
    
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

    void Awake()
    {
        _body = GetComponent<Rigidbody>();
        _moveSpeed = walkSpeed;
        canDoAnything = true;
    }

    private void Update()
    {
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

        if (Input.GetKeyDown(KeyCode.L))
        {
            cannon.Occupy(this);
        }

        if (Input.GetMouseButtonDown(0))
        {
            fire = true;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            fire = false;
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
        if(!canDoAnything)
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
        Vector3 playerVelocity = moveDirection.x * transform.forward + moveDirection.y * transform.right;
        playerVelocity = playerVelocity.normalized;
        _body.AddForce(playerVelocity * _moveSpeed);
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
}
