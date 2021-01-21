using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAttackable
{
    float AttackCoolDown { get; set; }
    float AttackDamage { get; set; }
    void Attack();
}
