using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox : GameBehaviour
{
    PlayerAttack attack;

    private void Start()
    {
        attack = GetComponentInParent<PlayerAttack>();
    }

    private void OnTriggerEnter(Collider other)
    {
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

            enemy.TakeDamage(10);
            //enemy.Knockback(attack.knockbackXZ, attack.knockbackY);

            //Determine direction of knockback
            Vector3 knockbackDirection = enemyRB.transform.position - _PLAYER.transform.position;
            //Resets velocity to prevent knockback compounding
            //enemyRB.isKinematic = false;
            enemyRB.velocity = new Vector3(0, 0, 0);
            //Apply knockback
            enemyRB.AddForce(knockbackDirection.x * attack.knockbackXZ, knockbackDirection.y * attack.knockbackY, knockbackDirection.z * attack.knockbackXZ);
        }
    }
}
