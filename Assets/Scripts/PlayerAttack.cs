using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : GameBehaviour
{
    public bool canCombo;

    void Update()
    {
        if (Input.GetButtonDown("AtkLight"))
            LightAttack();
        if (Input.GetButtonDown("AtkHeavy"))
            HeavyAttack();
    }

    void Attack()
    {
        _PLAYER.state = PlayerState.Attack;
        _PLAYER.anim.applyRootMotion = true;
        _PLAYER.anim.SetBool("cantMove", true);
        _PLAYER.moveDirection = new Vector3(0, 0, 0);
        
        //checks for enemy in range
        if (_PLAYER.targetEnemy == null)
            FindNearestEnemy();

        //makes player face enemy if one is in range
        if (_PLAYER.targetEnemy != null)
            _PLAYER.transform.LookAt(new Vector3(_PLAYER.targetEnemy.transform.localPosition.x, _PLAYER.transform.position.y, _PLAYER.targetEnemy.transform.localPosition.z));
    }

    void LightAttack()
    {
        if (!canCombo)
            return;

        if (_PLAYER.state == PlayerState.Idle)
        {
            _PLAYER.anim.CrossFadeInFixedTime("testATK1", 0.25f);
            ClearNearestEnemy();
        }
        else
            _PLAYER.anim.SetTrigger("atkLight");

        Attack();
    }

    void HeavyAttack()
    {
        if (!canCombo)
            return;

        if (_PLAYER.state == PlayerState.Idle)
        {
            _PLAYER.anim.CrossFadeInFixedTime("testFin0", 0.25f);
            ClearNearestEnemy();
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