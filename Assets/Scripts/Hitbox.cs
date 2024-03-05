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
        if (other.CompareTag("Enemy"))
        {
            //Debug.Log("Attack Landed");
            TestEnemy enemy = other.GetComponent<TestEnemy>();
            Rigidbody enemyRB = other.attachedRigidbody;

            //Disables upwards knockback if enemy is airborne
            if (enemy.isGrounded == false && attack.knockbackY >= 0)
            {
                if (attack.type == AttackType.Light)
                    attack.knockbackXZ = attack.knockbackXZ / 4;
                else if (attack.type == AttackType.Heavy)
                    attack.knockbackXZ = attack.knockbackXZ / 2;

                attack.knockbackY = 0;
                enemy.HitStun();
            }

            //Knockback
            
            Vector3 moveDirection = enemyRB.transform.position - _PLAYER.transform.position;

            enemyRB.velocity = new Vector3(0, 0, 0);

            enemyRB.AddForce(moveDirection.x * attack.knockbackXZ, moveDirection.y * attack.knockbackY, moveDirection.z * attack.knockbackXZ);

            //enemyRB.constraints = RigidbodyConstraints.FreezePosition;
            //enemyRB.constraints = RigidbodyConstraints.None;
            //enemyRB.constraints = RigidbodyConstraints.FreezeRotation;
        }
    }
}
