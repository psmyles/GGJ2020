using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ground : Actor
{
    [SerializeField]
    private GameObject m_ParticlePrefab;
    // Use this for initialization
    protected void Awake()
    {

    }
    // Use this for initialization
    protected override void Start()
    {
        
    }

    // Update is called once per frame
    protected override void Update()
    {

    }

    public override bool CanAttack(Actor attacker)
    {
        if (attacker.GetType().IsSubclassOf(typeof(Projectile)))
        {
            return true;
        }

        return false;
    }

    // Called when any types weapon collides with this, returns true if it is successful attack
    // returns false if attack is blocked
    public override bool Attacked(Actor attacker, GameObject bodyPartGotHit, float damage)
    {
        if (attacker.GetType().IsSubclassOf(typeof(Projectile)))
        {
            Projectile proj = (Projectile)attacker;
            if (proj != null)
            {
                GameObject go = GameObject.Instantiate(m_ParticlePrefab);
                RaycastHit rayHit;
                if (PhysicsUtils.RaycastToGround(transform.position, out rayHit))
                {
                    go.transform.position = rayHit.point;
                }
                else
                {
                    go.transform.position = proj.transform.position;
                }
                proj.ShowGroundDecalAndDestroy();
            }
        }
        return true;
    }

    // Called when attack is blocked
    public override void CurrentAttackBlocked(Actor defender)
    {
    }

    public override void CurrentAttackSuccedded(Actor defender)
    {
    }

    // Called when animation event happens
    public override void OnAnimEvent(AnimEventData data)
    {

    }
}
