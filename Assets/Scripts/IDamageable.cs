using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    public void ReceiveDamage(int damage, bool isCritical, Entity attacker);
    public void Attack();
    void OnHpChange();
    void Death(Entity attacker);

    //void GetStun(float duration);
    //void GetKnockUp(float duration, float distance);
}
