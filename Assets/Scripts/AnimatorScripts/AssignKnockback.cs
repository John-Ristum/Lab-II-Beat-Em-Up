using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssignKnockback : StateMachineBehaviour
{
    public AttackingParty user;

    PlayerAttack attack;
    Enemy enemy;
    AttackProperties attackProperties;
    IAttackProperties attackPropertiesInterface;

    public float knockbackXZ = 100f;
    public float knockbackY;
    public int damage = 10;
    public float atkDashDist = 1f;
    public bool isHeavy;
    public int damageAnim = 1;
    public bool multiHit;
    public bool freezeY;

    public AudioClip attackLandSFX;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        attackProperties = animator.GetComponent<AttackProperties>();

        attackProperties.knockbackXZ = knockbackXZ;
        attackProperties.knockbackY = knockbackY;
        attackProperties.damage = damage;
        attackProperties.atkDashDist = atkDashDist;
        attackProperties.isHeavy = isHeavy;
        attackProperties.damageAnim = damageAnim;
        attackProperties.multiHit = multiHit;
        attackProperties.freezeY = freezeY;
        attackProperties.attackLandSFX = attackLandSFX;

        switch (user)
        {
            case AttackingParty.Player:
                attack = animator.GetComponent<PlayerAttack>();

                attack.knockbackXZ = knockbackXZ;
                attack.knockbackY = knockbackY;
                attack.damage = damage;
                attack.atkDashDist = atkDashDist;
                attack.isHeavy = isHeavy;
                attack.damageAnim = damageAnim;
                attack.multiHit = multiHit;
                attack.freezeY = freezeY;
                attack.attackLandSFX = attackLandSFX;
                break;
            case AttackingParty.Enemy:
                enemy = animator.GetComponent<Enemy>();

                enemy.knockbackXZ = knockbackXZ;
                enemy.knockbackY = knockbackY;
                //enemy.freezeY = freezeY;
                enemy.damage = damage;
                enemy.isHeavy = isHeavy;
                enemy.damageAnim = damageAnim;
                enemy.attackBlocked = false;
                enemy.attackLandSFX = attackLandSFX;
                break;
        }
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
