using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerType { Normal, Placeholder}
public enum AttackType { Light, Heavy}

public class PlayerAttack : GameBehaviour, IAttackProperties
{
    public PlayerType playerType;
    public AttackType type;

    public bool canCombo;

    public AudioClip attackLandSFX;

    [Header("Knockback & Damage")]
    public float knockbackXZ;
    public float knockbackY;
    public int damage;
    public bool attackLanded;
    public bool isHeavy;
    public int damageAnim;
    public bool multiHit;
    public bool freezeY;

    [Header("Attack Dash")]
    public float atkDashDist = 1f;
    public float atkDashTime = 0.25f;
    float atkStartTime;
    public int atkDashNum;
    public int atkDashLimit = 2;
    Vector3 atkDashDir;

    [Header("Blocking")]
    public GameObject blockCollider;
    public float blockTimer = 0f;

    void Update()
    {
        if (_UI.isPaused)
            return;

        if (_PLAYER.state == PlayerState.Dead || _PLAYER.inCutscene == true)
            return;

        if (_PLAYER.state != PlayerState.Block && _PLAYER.state != PlayerState.Damage)
        {
            if (Input.GetButtonDown("AtkLight"))
                LightAttack();
            if (Input.GetButtonDown("AtkHeavy"))
                HeavyAttack();
            if (Input.GetAxisRaw("Block") > 0)
                Block();
        }


        if (_PLAYER.state == PlayerState.Block)
        {
            if (blockTimer > 0)
            {
                blockTimer -= Time.deltaTime;
            }
            Debug.Log(blockTimer);

            if (Input.GetAxisRaw("Block") <= 0 && blockTimer <= 0)
                ExitBlock();
        }
    }

    public void Attack()
    {
        //_PLAYER.anim.ResetTrigger("atkLight");
        //_PLAYER.anim.ResetTrigger("atkHeavy");

        //Debug.Log("Attack");
        _PLAYER.state = PlayerState.Attack;
        _PLAYER.canCancel = false;
        _PLAYER.attackProperties.attackLanded = false;
        if (playerType == PlayerType.Placeholder)
            _PLAYER.anim.applyRootMotion = true;
        _PLAYER.anim.SetBool("cantMove", true);
        _PLAYER.inputDirection = new Vector3(0, 0, 0);

        ClearNearestEnemy();

        SnapPlayer();

        if (_PLAYER.targetEnemy != null)
            _PLAYER.lockRotation = true;
        else
            _PLAYER.lockRotation = false;

        //if (playerType == PlayerType.Normal)
        //{
        //    StartCoroutine(AttackDash());
        //    atkDashNum++;
        //}
    }

    public void ChangeDirection()
    {
        _PLAYER.lockRotation = true;
    }

    void LightAttack()
    {
        type = AttackType.Light;

        if (_PLAYER.state == PlayerState.Idle || _PLAYER.state == PlayerState.QuickStep)
        {
            _PLAYER.anim.ResetTrigger("atkLight");
            _PLAYER.anim.ResetTrigger("atkHeavy");

            _PLAYER.anim.CrossFadeInFixedTime("ATK1", 0.25f);
            //Attack();
            ClearNearestEnemy();
            SnapPlayer();
        }
        else if (canCombo)
        {
            _PLAYER.anim.SetTrigger("atkLight");
            //Attack();
            ClearNearestEnemy();
            SnapPlayer();
        }
    }

    void HeavyAttack()
    {
        type = AttackType.Heavy;

        if (_PLAYER.state == PlayerState.Idle || _PLAYER.state == PlayerState.QuickStep)
        {
            if (_PLAYER.heavyTimer > 0)
                return;

            _PLAYER.anim.CrossFadeInFixedTime("Fin0", 0.25f);
            //Attack();
            ClearNearestEnemy();
            ClearNearestEnemy();
            SnapPlayer();
            _PLAYER.heavyTimer = 1f;
        }
        else if (canCombo)
        {
            _PLAYER.anim.SetTrigger("atkHeavy");
            //Attack();
            ClearNearestEnemy();
            SnapPlayer();
        }
    }

    void SnapPlayer()
    {
        //checks for enemy in range
        if (_PLAYER.targetEnemy == null)
            FindNearestEnemy();

        //makes player face enemy if one is in range
        if (_PLAYER.targetEnemy != null)
            _PLAYER.transform.LookAt(new Vector3(_PLAYER.targetEnemy.transform.position.x, _PLAYER.transform.position.y, _PLAYER.targetEnemy.transform.position.z));    //transform.localPosition was causing problems when target was parented to other object
    }

    public void AttackDash()
    {
        StopCoroutine(AttackDashCo());
        StartCoroutine(AttackDashCo());
    }

    IEnumerator AttackDashCo()
    {
        //Gets direction player is facing
        atkDashDir = _PLAYER.transform.forward;

        atkStartTime = Time.time;

        int atkDashNum2 = atkDashNum;

        //Sets local variables to value of global equivilents (done to prevent new values being used for previous attacks)
        float localAtkDashDist = atkDashDist;

        //Quickstep movement
        while (Time.time < atkStartTime + atkDashTime)
        {
            _PLAYER.controller.Move(atkDashDir * localAtkDashDist * Time.deltaTime);

            //if (atkDashNum2 != atkDashNum)
            //    break;

            yield return null;
        }
    }

    void Block()
    {
        if (_PLAYER.state != PlayerState.Idle && _PLAYER.state != PlayerState.Block)
            return;

        _PLAYER.state = PlayerState.Block;
        _PLAYER.anim.SetBool("blocking", true);
        blockCollider.SetActive(true);
    }

    public void ExitBlock()
    {
        if (_PLAYER.state == PlayerState.Block || _PLAYER.state == PlayerState.Damage)
        {
            _PLAYER.state = PlayerState.Idle;
            _PLAYER.anim.SetBool("blocking", false);
            blockCollider.SetActive(false);
        }
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
        _PLAYER.anim.CrossFadeInFixedTime("Idle", 0.25f);
    }

    #region IAttackProperties Functions
    public AudioClip AttackLandSFX()
    {
        return attackLandSFX;
    }

    public float KnockbackXZ()
    {
        return knockbackXZ;
    }

    public float KnockbackY()
    {
        return knockbackY;
    }

    public int AttackDamage()
    {
        return damage;
    }
    #endregion
}
