using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketProjectile : Projectile
{
    protected override void Awake()
    {
        base.Awake();
    }

    // Use this for initialization
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    public override bool CanAttack(Actor attacker)
    {
        return base.CanAttack(attacker);
    }

    // Called when any types weapon collides with this, returns true if it is successful attack
    // returns false if attack is blocked
    public override bool Attacked(Actor attacker, GameObject bodyPartGotHit, float damage)
    {
        return base.Attacked(attacker, bodyPartGotHit, damage);
    }

    // Called when attack is blocked
    public override void CurrentAttackBlocked(Actor defender)
    {
        base.CurrentAttackBlocked(defender);
    }

    // Called when animation event happens
    public override void OnAnimEvent(AnimEventData data)
    {
        base.OnAnimEvent(data);
    }

    public override void SetToLaunch(float timeToReach, Vector3 source, Vector3 target)
    {
        base.SetToLaunch(timeToReach, source, target);
    }

    protected override void Launch()
    {
        base.Launch();
        transform.position = m_LaunchPos;
        Vector3 distVec = m_LaunchTarget - m_LaunchPos;
        transform.forward = distVec.normalized;
        m_Rigidbody.velocity = distVec / m_TimeToReach;
    }

    /*
       public void GetSpawnParam(float timeToReach, out Vector3 location, out Vector3 velocity)
    {
        location = transform.position;
        velocity = transform.forward;

        Vector3 targetPoint = m_ProjectileTargetArea.GetRandomPoint();
        Vector3 spawnPoint = GetRandomPoint();

        Vector3 distVec = targetPoint - spawnPoint;
        Vector3 horzDistVec = distVec;
        horzDistVec.y = 0.0f;
        float horzDist = horzDistVec.magnitude;
        float vertDist = distVec.y;

        float initialVertSpeed = (vertDist - 0.5f * timeToReach * timeToReach * Physics.gravity.y) / timeToReach;
        float initialHorzSpeed = horzDist / timeToReach;

        Vector3 horzDIr = horzDistVec.normalized;

        location = spawnPoint;

        velocity = horzDIr * initialHorzSpeed + Vector3.up * initialVertSpeed;
    }*/
     
}
