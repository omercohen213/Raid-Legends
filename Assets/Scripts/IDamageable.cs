using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    void ReceiveDamage(int damageAmount);
    void Death();
    //bool IsDamageToKill(float damage);
    //void GetStun(float duration);
    //void GetKnockUp(float duration, float distance);
}
