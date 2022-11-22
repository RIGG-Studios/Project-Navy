using System.Collections;
using UnityEngine;

public class BayonetteController : MonoBehaviour
{
    public float attackDuration;
    public string targetTag;
    public float attackRate;
    public Animator animator;
    public AudioClip[] aimSounds;
    public AudioClip[] stabSounds;
    public AudioClip[] hitPlayerSounds;
    public float timeUntilStab;
    public AudioSource source;
    public MusketController controller;

    private bool _canAttack;
    private bool _isAttacking;
    private bool _dealtDamage;
    private float _timeSinceLastAttack;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            if(_isAttacking)
                return;
            
            if (Time.time <= _timeSinceLastAttack + attackRate && _timeSinceLastAttack != 0)
                return;

            _timeSinceLastAttack = Time.time;
            StartCoroutine("AttackRoutine");
        }
    }

    IEnumerator AttackRoutine()
    {
        _isAttacking = true;
        _canAttack = true;
        controller.canDoAnything = false;
        animator.SetTrigger("Stab");
        source.clip = aimSounds[UnityEngine.Random.Range(0, aimSounds.Length)];
        source.Play();
        yield return new WaitForSeconds(timeUntilStab);
        source.clip = stabSounds[UnityEngine.Random.Range(0, stabSounds.Length)];
        source.Play();
        yield return new WaitForSeconds(attackDuration);
        _canAttack = false;
        _isAttacking = false;
        controller.canDoAnything = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(targetTag))
        {
            return;
        }

        if(!_canAttack || _dealtDamage)
            return;
        
        source.clip = hitPlayerSounds[UnityEngine.Random.Range(0, hitPlayerSounds.Length)];
        source.Play();
        Debug.Log("Do damage here");
        animator.SetTrigger("Hit");
        _dealtDamage = true;
    }

    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag(targetTag))
        {
            return;
        }

        if(!_canAttack || _dealtDamage)
            return;
        
        source.clip = hitPlayerSounds[UnityEngine.Random.Range(0, hitPlayerSounds.Length)];
        source.Play();
        Debug.Log("Do damage here");
        animator.SetTrigger("Hit");
        _dealtDamage = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if(!other.CompareTag(targetTag))
            return;
        
        _dealtDamage = false;
    }
}
