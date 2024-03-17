using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AttackType { Light, Heavy}

public class PlayerAttack : GameBehaviour
{
    public AttackType type;

    public bool canCombo;

    public float knockbackXZ;
    public float knockbackY;
    public bool freezeY;

    void Update()
    {
        if (Input.GetButtonDown("AtkLight"))
            LightAttack();
        if (Input.GetButtonDown("AtkHeavy"))
            HeavyAttack();

        //Test player damage
        if (Input.GetKeyDown("0"))
            _PLAYER.RecieveDamage(10f);
        if (Input.GetKeyDown("9"))
            _PLAYER.RecoverHealth(10f);
    }

    void Attack()
    {
        _PLAYER.state = PlayerState.Attack;
        _PLAYER.anim.applyRootMotion = true;
        _PLAYER.anim.SetBool("cantMove", true);
        _PLAYER.inputDirection = new Vector3(0, 0, 0);

        ClearNearestEnemy();

        //checks for enemy in range
        if (_PLAYER.targetEnemy == null)
            FindNearestEnemy();

        //makes player face enemy if one is in range
        if (_PLAYER.targetEnemy != null)
            _PLAYER.transform.LookAt(new Vector3(_PLAYER.targetEnemy.transform.position.x, _PLAYER.transform.position.y, _PLAYER.targetEnemy.transform.position.z));    //transform.localPosition was causing problems when target was parented to other object
    }

    void LightAttack()
    {
        if (!canCombo)
            return;

        type = AttackType.Light;

        if (_PLAYER.state == PlayerState.Idle || _PLAYER.state == PlayerState.QuickStep)
        {
            _PLAYER.anim.CrossFadeInFixedTime("testATK1", 0.25f);
            //ClearNearestEnemy();
        }
        else
            _PLAYER.anim.SetTrigger("atkLight");

        Attack();
    }

    void HeavyAttack()
    {
        if (!canCombo)
            return;

        type = AttackType.Heavy;

        if (_PLAYER.state == PlayerState.Idle || _PLAYER.state == PlayerState.QuickStep)
        {
            _PLAYER.anim.CrossFadeInFixedTime("testFin0", 0.25f);
            //ClearNearestEnemy();
        }
        else
            _PLAYER.anim.SetTrigger("atkHeavy");

        Attack();
    }

    void FindNearestEnemy()
    {
        for (int i = 0; i < _PLAYER.enemiesInRange.Count; i++)
        {
            _PLAYER.distance = Vector3.Distance(this.transform.position, _PLAYER.enemiesInRange[i].transform.position);

            if (_PLAYER.distance < _PLAYER.nearestDistance)
            {
                _PLAYER.targetEnemy = _PLAYER.enemiesInRange[i];
                _PLAYER.nearestDistance = _PLAYER.distance;
            }
        }
    }

    void ClearNearestEnemy()
    {
        _PLAYER.targetEnemy = null;
        _PLAYER.nearestDistance = 1000f;
    }

    public void ResetAnimation()
    {
        _PLAYER.anim.CrossFadeInFixedTime("testIdle", 0.25f);
    }
}
