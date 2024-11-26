using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAttackProperties
{
    AudioClip AttackLandSFX();
    float KnockbackXZ();
    float KnockbackY();
    int AttackDamage();
    //float AttackDashDistance();
    //bool IsHeavy();
    //int DamageAnim();
    //bool FreezeY();
    //bool MultiHit();
    //bool AttackLanded();
}
