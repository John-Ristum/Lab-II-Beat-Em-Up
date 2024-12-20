using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AttackingParty { Player, Enemy }

public class Hitbox : GameBehaviour
{
    AudioSource audioSource;
    PlayerAttack attack;
    Enemy enemy;

    IAttackProperties selfI;
    AttackProperties self;
    Collider collider;
    
    public AttackingParty user;
    //bool attackBlocked;

    private void Start()
    {
        audioSource = GetComponentInParent<AudioSource>();
        collider = GetComponent<Collider>();

        selfI = GetComponentInParent<IAttackProperties>();
        self = GetComponentInParent<AttackProperties>();

        switch (user)
        {
            case AttackingParty.Player:
                attack = GetComponentInParent<PlayerAttack>();
                break;
            case AttackingParty.Enemy:
                enemy = GetComponentInParent<Enemy>();
                break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        ApplyKnockback(other);
        //ApplyKnockbackUniversal(other);

        //switch (user)
        //{
        //    case AttackingParty.Player:
        //        ApplyKnockbackUniversal(other);
        //        break;
        //    case AttackingParty.Enemy:
        //        ApplyKnockbackUniversal(other);
        //        break;
        //}

        //collider.gameObject.SetActive(false);
    }

    void ApplyKnockback(Collider other)
    {
        if (self.attackLanded == true && self.multiHit == false)
            return;

        IDamageable damageable = other.GetComponent<IDamageable>();
        if (damageable != null)
        {
            Debug.Log("Interfaces Work");
            Rigidbody damageableRB = other.attachedRigidbody;

            if (other.CompareTag("Player"))
            {
                if (enemy.attackBlocked)
                {
                    _PLAYER.transform.LookAt(new Vector3(self.transform.position.x, _PLAYER.transform.position.y, self.transform.position.z));
                    attack = other.GetComponent<PlayerAttack>();
                    attack.blockTimer = 0.5f;

                    return;
                }
            }

            damageable.Damage(selfI.AttackDamage(), self.damageAnim);
            //enemy.Knockback(attack.knockbackXZ, attack.knockbackY);

            //Play SFX
            _AM.PlaySound(selfI.AttackLandSFX(), audioSource);

            if (other.CompareTag("Player"))
                _GM.CamUpdateLate();

            //Determine direction of knockback
            Vector3 knockbackDirection = damageableRB.transform.position - self.transform.position;
            //Resets velocity to prevent knockback compounding
            //enemyRB.isKinematic = false;
            damageableRB.velocity = new Vector3(0, 0, 0);
            //Apply knockback
            damageableRB.AddForce(knockbackDirection.x * selfI.KnockbackXZ(), knockbackDirection.y * selfI.KnockbackY(), knockbackDirection.z * selfI.KnockbackXZ());

            //_PLAYER.targetEnemy = other.gameObject;

            self.attackLanded = true;
        }
    }

    //void ApplyKnockbackUniversal(Collider other)
    //{
    //    if (self.attackLanded == true && self.multiHit == false)
    //        return;

    //    if (other.CompareTag("EnemyT"))
    //    {
    //        if (other.GetComponent<TestEnemy>() == null)
    //            return;

    //        TestEnemy enemy = other.GetComponent<TestEnemy>();
    //        Rigidbody enemyRB = other.attachedRigidbody;

    //        if (enemy.isGrounded == false && attack.knockbackY >= 0)
    //        {
    //            //Alters knockback force if airborne (forces differ in air compared to ground)
    //            if (attack.type == AttackType.Light)
    //                attack.knockbackXZ = attack.knockbackXZ / 4;
    //            else if (attack.type == AttackType.Heavy)
    //                attack.knockbackXZ = attack.knockbackXZ / 2;

    //            //Disables upwards knockback if enemy is airborne
    //            attack.knockbackY = 0;
    //            enemy.HitStun();
    //        }

    //        //Determine direction of knockback
    //        Vector3 knockbackDirection = enemyRB.transform.position - _PLAYER.transform.position;
    //        //Resets velocity to prevent knockback compounding
    //        enemyRB.velocity = new Vector3(0, 0, 0);
    //        //Apply knockback
    //        enemyRB.AddForce(knockbackDirection.x * attack.knockbackXZ, knockbackDirection.y * attack.knockbackY, knockbackDirection.z * attack.knockbackXZ, ForceMode.Force);
    //    }

    //    if (other.CompareTag("Enemy") || other.CompareTag("Player"))
    //    {

    //        AttackProperties opposing = other.GetComponent<AttackProperties>();
    //        Rigidbody opposingRB = other.attachedRigidbody;

    //        //if (enemy.anim.applyRootMotion == true)
    //        //    enemy.anim.applyRootMotion = false;

    //        //if (attack.isHeavy && enemy.onBoundary)
    //        //{
    //        //    int breakThroughLayer = LayerMask.NameToLayer("BreakThrough");
    //        //    enemy.gameObject.layer = breakThroughLayer;
    //        //}

    //        if (other.CompareTag("Player"))
    //        {
    //            if (enemy.attackBlocked)
    //            {
    //                _PLAYER.transform.LookAt(new Vector3(self.transform.position.x, _PLAYER.transform.position.y, self.transform.position.z));
    //                attack = other.GetComponent<PlayerAttack>();
    //                attack.blockTimer = 0.5f;

    //                return;
    //            }
    //        }

    //        opposing.Damage(self.damage, self.damageAnim);
    //        //enemy.Knockback(attack.knockbackXZ, attack.knockbackY);

    //        //Play SFX
    //        _AM.PlaySound(self.attackLandSFX, self.audioSource);

    //        if (other.CompareTag("Player"))
    //            _GM.CamUpdateLate();

    //        //Determine direction of knockback
    //        Vector3 knockbackDirection = opposingRB.transform.position - self.transform.position;
    //        //Resets velocity to prevent knockback compounding
    //        //enemyRB.isKinematic = false;
    //        opposingRB.velocity = new Vector3(0, 0, 0);
    //        //Apply knockback
    //        opposingRB.AddForce(knockbackDirection.x * self.knockbackXZ, knockbackDirection.y * self.knockbackY, knockbackDirection.z * self.knockbackXZ);

    //        //_PLAYER.targetEnemy = other.gameObject;

    //        self.attackLanded = true;
    //    }
    //}

    void ApplyKnockbackToEnemy(Collider other)
    {
        if (attack.attackLanded == true && attack.multiHit == false)
            return;

        if (other.CompareTag("EnemyT"))
        {
            if (other.GetComponent<TestEnemy>() == null)
                return;

            TestEnemy enemy = other.GetComponent<TestEnemy>();
            Rigidbody enemyRB = other.attachedRigidbody;

            if (enemy.isGrounded == false && attack.knockbackY >= 0)
            {
                //Alters knockback force if airborne (forces differ in air compared to ground)
                if (attack.type == AttackType.Light)
                    attack.knockbackXZ = attack.knockbackXZ / 4;
                else if (attack.type == AttackType.Heavy)
                    attack.knockbackXZ = attack.knockbackXZ / 2;

                //Disables upwards knockback if enemy is airborne
                attack.knockbackY = 0;
                enemy.HitStun();
            }

            //Determine direction of knockback
            Vector3 knockbackDirection = enemyRB.transform.position - _PLAYER.transform.position;
            //Resets velocity to prevent knockback compounding
            enemyRB.velocity = new Vector3(0, 0, 0);
            //Apply knockback
            enemyRB.AddForce(knockbackDirection.x * attack.knockbackXZ, knockbackDirection.y * attack.knockbackY, knockbackDirection.z * attack.knockbackXZ, ForceMode.Force);
        }

        if (other.CompareTag("Enemy"))
        {

            Enemy enemy = other.GetComponent<Enemy>();
            Rigidbody enemyRB = other.attachedRigidbody;

            if (enemy.anim.applyRootMotion == true)
                enemy.anim.applyRootMotion = false;

            if (attack.isHeavy && enemy.onBoundary)
            {
                int breakThroughLayer = LayerMask.NameToLayer("BreakThrough");
                enemy.gameObject.layer = breakThroughLayer;
            }

            enemy.TakeDamage(attack.damage, attack.damageAnim);
            //enemy.Knockback(attack.knockbackXZ, attack.knockbackY);

            //Play SFX
            _AM.PlaySound(attack.attackLandSFX, _PLAYER.audioSource);

            //Determine direction of knockback
            Vector3 knockbackDirection = enemyRB.transform.position - _PLAYER.transform.position;
            //Resets velocity to prevent knockback compounding
            //enemyRB.isKinematic = false;
            enemyRB.velocity = new Vector3(0, 0, 0);
            //Apply knockback
            enemyRB.AddForce(knockbackDirection.x * attack.knockbackXZ, knockbackDirection.y * attack.knockbackY, knockbackDirection.z * attack.knockbackXZ);

            //_PLAYER.targetEnemy = other.gameObject;

            attack.attackLanded = true;
        }
    }

    void ApplyKnockbackToPlayer(Collider other)
    {
        if (enemy.canAttack == false)
            return;

        //attackBlocked = false;

        if (other.CompareTag("BlockCollider"))
        {
            enemy.attackBlocked = true;
        }

        if (other.CompareTag("Player"))
        {
            Rigidbody enemyRB = GetComponentInParent<Rigidbody>();

            if (enemy.attackBlocked)
            {
                _PLAYER.transform.LookAt(new Vector3(enemyRB.transform.position.x, _PLAYER.transform.position.y, enemyRB.transform.position.z));
                attack = other.GetComponent<PlayerAttack>();
                attack.blockTimer = 0.5f;

                return;
            }


            _PLAYER.TakeDamage(enemy.damage, enemy.damageAnim);
            //enemy.Knockback(attack.knockbackXZ, attack.knockbackY);

            //Play SFX
            _AM.PlaySound(enemy.attackLandSFX, enemy.audioSource);

            _GM.CamUpdateLate();

            //Determine direction of knockback
            Vector3 knockbackDirection = _PLAYER.rb.transform.position - enemyRB.transform.position;
            //Resets velocity to prevent knockback compounding
            //enemyRB.isKinematic = false;
            _PLAYER.rb.velocity = new Vector3(0, 0, 0);
            //Apply knockback
            _PLAYER.rb.AddForce(knockbackDirection.x * enemy.knockbackXZ, knockbackDirection.y * enemy.knockbackY, knockbackDirection.z * enemy.knockbackXZ);
        }
    }
}
