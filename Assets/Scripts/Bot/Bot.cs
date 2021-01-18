using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

public class Bot : MonoBehaviour, IDamagable, IAttackable
{
    LayerMask layerMask;
    public bool isDead;
    BotProxy botProxy;
    public GameObject enemyTarget;
    BotNav botNav;
    BotClickerData botClickerData;
    [SerializeField]
    public int team;
    private readonly float attackDamage = 1;
    private readonly float health = 10;
    
    public bool chargingEnemy, enemysInRange, canAttack, directOrder, returnToOrders;
    public float Health { get; set; }
    public float Armour { get; set; }
    public float AttackCoolDown { get; set; }
    public float AttackDamage { get; set; }
    public int AttackType { get; set; }
    

    private void Awake()
    {
        botNav = GetComponent<BotNav>();
        botClickerData = GetComponent<BotClickerData>();
        botProxy = transform.GetChild(0).GetComponent<BotProxy>();
        canAttack = true;
        AttackCoolDown = 2;
        AttackDamage = attackDamage;
        Health = health;
        layerMask = gameObject.layer;
        GameObject cohort = NewCohort(gameObject.transform.position, gameObject.transform.rotation.eulerAngles);
        transform.parent = cohort.transform;
    }
    private void Update()
    {
        if (isDead)
        {
            return;
        }
        ManageOrders();
    }

    public void ManageOrders()
    {

        if (directOrder)
        {
            returnToOrders = true;
            directOrder = !botNav.CompletedOrder();
        }
        else if (enemysInRange)
        {
            returnToOrders = true;
            if (botProxy.EnemyProxies.Count != 0)
            {
                enemyTarget = botProxy.EnemyProxies[0];
                if (enemyTarget != null) 
                {
                    if (Vector3.Distance(botClickerData.orderedPosition, enemyTarget.transform.position) <= botProxy.colliderRadius + 1)
                    {
                        Attack();
                    }
                    else if (returnToOrders)
                    {
                        botNav.SetNavDestination(botClickerData.orderedPosition);
                    }
                }
                
            }
            
        }
        else if (returnToOrders)
        {
            botNav.SetNavDestination(botClickerData.orderedPosition);
            returnToOrders = false;
        }
    }

    public void Attack()
    {
        botNav.SetStoppingDistance(1.4f);
        
        if (enemyTarget)
        {
            Ray attackRay = new Ray(transform.position, botNav.navMeshAgent.transform.forward);
            botNav.SetNavDestination(enemyTarget.transform.position);
            botNav.SetNavRotation(enemyTarget);
            if (Physics.Raycast(attackRay, out RaycastHit hit, 1.5f, ~layerMask))
            {
                if (hit.transform.gameObject == enemyTarget)
                {
                    if (canAttack)
                    {
                        canAttack = false;
                        StartCoroutine(AttackCoolDownRoutine(enemyTarget));
                    }
                }
            }
        }
    }

    public void Damage(float damage)
    {
        //Debug.Log("were damaging");
        Health -= damage;
        directOrder = false;
        if (Health <= 0)
        {
            botClickerData.RemoveWhenDead();
            isDead = true;
            StopAllCoroutines();
            this.gameObject.GetComponent<Collider>().enabled = false;
            if (transform.parent.CompareTag("Cohort"))
            {
                if (transform.parent.transform.childCount <= 1)
                {
                    Destroy(transform.parent.gameObject, 0.5f);
                }
            }
            Destroy(this.gameObject, 0.5f);
        }
    }
    IEnumerator AttackCoolDownRoutine(GameObject hitObj)
    {
        transform.Translate(Vector3.forward * 0.2f);
        yield return new WaitForSeconds(0.1f);
        if(hitObj != null)
        {
            hitObj.GetComponent<IDamagable>()
                  .Damage(AttackDamage);
        }
        transform.Translate(Vector3.forward * -0.2f);
        yield return new WaitForSeconds(1.0f);
        canAttack = true;
    }

    GameObject NewCohort(Vector3 pos, Vector3 rot)
    {
        GameObject cohort = new GameObject();
        cohort.transform.position = pos;
        cohort.transform.rotation = Quaternion.Euler(rot);
        cohort.name = "Cohort";
        cohort.tag = "Cohort";
        cohort.transform.parent = gameObject.transform.parent;
        cohort.AddComponent<Cohort>();
        return cohort;
    }




    }
