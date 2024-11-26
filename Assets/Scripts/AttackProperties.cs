using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackProperties : GameBehaviour
{
    public AttackingParty user;
    public AudioSource audioSource;
    public AudioClip attackLandSFX;

    public float knockbackXZ = 100f;
    public float knockbackY;
    public int damage = 10;
    public float atkDashDist = 1f;
    public bool isHeavy;
    public int damageAnim = 1;
    public bool freezeY;
    public bool multiHit;
    public bool attackLanded;


    Enemy enemy;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        if (user == AttackingParty.Enemy)
            enemy = GetComponent<Enemy>();
    }

    
    void Update()
    {
        
    }

    //public void Damage(int _damage, int _anim = 1)
    //{
    //    switch (user)
    //    {
    //        case AttackingParty.Player:
    //            _PLAYER.TakeDamage(_damage, _anim);
    //            break;
    //        case AttackingParty.Enemy:
    //            enemy.TakeDamage(_damage, _anim);
    //            break;
    //    }
    //}
}
