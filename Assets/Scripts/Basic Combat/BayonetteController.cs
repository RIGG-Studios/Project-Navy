using System.Collections;
using UnityEngine;

public class BayonetteController : MonoBehaviour
{
    public float attackDuration;
    public string targetTag;
    public float attackRate;

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
        yield return new WaitForSeconds(attackDuration);
        _canAttack = false;
        _isAttacking = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(!other.CompareTag(targetTag))
            return;
        
        if(!_canAttack || _dealtDamage)
            return;
        
        Debug.Log("Do damage here");
        _dealtDamage = true;
    }

    private void OnTriggerStay(Collider other)
    {
        if(!other.CompareTag(targetTag))
            return;
        
        if(!_canAttack || _dealtDamage)
            return;
        
        Debug.Log("Do damage here");
        _dealtDamage = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if(!other.CompareTag(targetTag))
            return;
        
        _dealtDamage = false;
    }
}
