using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    [SerializeField]
    public Vector3 targetVec;
    public bool targetAquired;
    float rotateSpeed;
    [SerializeField]
    float speed;
    float acuracy;
    Vector3 direction;
     public int assignedTeam;
    float attackDamage;
    public bool hit;
    // Start is called before the first frame update
    private void Awake()
    {
        rotateSpeed = 1f;
        speed = 30;
        attackDamage = 5;
    }

    public void GiveArrowTarget(GameObject target, int parentTeam)
    {
        targetVec = new Vector3
            (target.transform.position.x + Random.Range(-0.1f + acuracy, 0.1f - acuracy),
            target.transform.position.y + Random.Range(-0.1f + acuracy, 0.1f - acuracy) + 0.2f,
            target.transform.position.z + Random.Range(-0.1f + acuracy, 0.1f - acuracy));
        transform.LookAt(targetVec);
        assignedTeam = parentTeam;
        targetAquired = true;

    }
    // Update is called once per frame
    void Update()
    {
        if (targetAquired)
        {
            direction = targetVec - transform.position;
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
        }
        if (gameObject.transform.position.y < -2.0f)
        {
            Destroy(this.gameObject);
        }
    }
    public void HitTerrain()
    {
        Destroy(this.gameObject, 3.0f);
    }
    public void HitBot(GameObject newParent)
    {
        hit = true;
        this.transform.parent = newParent.transform;
        if (newParent.GetComponent<Bot>().team != assignedTeam)
        {
            newParent.GetComponent<Bot>().Damage(attackDamage);
        }
        else
        {
            Destroy(this.gameObject, 2.0f);
        }
    }
}
