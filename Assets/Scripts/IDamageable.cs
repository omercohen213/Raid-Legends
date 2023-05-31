using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    public void ReceiveDamage(int damage, bool isCritical);
    public void Attack(Entity entity);
    void OnHpChange();
    void Death();
    //bool IsDamageToKill(float damage);
    //void GetStun(float duration);
    //void GetKnockUp(float duration, float distance);
}
