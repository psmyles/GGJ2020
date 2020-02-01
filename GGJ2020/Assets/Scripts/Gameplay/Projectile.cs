using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Projectile : Actor
{
    private Vector3 m_LastVelocity;
    private Rigidbody m_RigidBody;

    protected virtual void Awake()
    {
        m_RigidBody = GetComponent<Rigidbody>();
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
        return false;
    }

    // Called when any types weapon collides with this, returns true if it is successful attack
    // returns false if attack is blocked
    public override bool Attacked(Actor attacker, GameObject bodyPartGotHit, float damage)
    {
        return true;
    }

    // Called when attack is blocked
    public override void CurrentAttackBlocked(Actor defender)
    {
    }

    // Called when animation event happens
    public override void OnAnimEvent(AnimEventData data)
    {

    }

    public void Launch(Vector3 velocity)
    {
        m_LastVelocity = velocity;
        m_RigidBody.velocity = velocity;
    }
}
