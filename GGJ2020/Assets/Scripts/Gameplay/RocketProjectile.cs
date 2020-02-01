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
}
