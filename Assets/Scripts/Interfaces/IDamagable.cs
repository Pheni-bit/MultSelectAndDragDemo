using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamagable
{
     float Health { get; set; }
     float Armour { get; set; }
    void Damage(float damage);

}
