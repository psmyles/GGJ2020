using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Decal))]
public class Crack : Actor
{
    [SerializeField]
    private BoxCollider m_DefenderCollider;

    private Decal m_Decal;
    private int m_NoOfPointsAdded;

    // Use this for initialization
    protected void Awake()
    {
        m_Decal = GetComponent<Decal>();
        m_NoOfPointsAdded = 0;
        GameScoreManager.Get().IncrementLiveCracks();
    }
    // Use this for initialization
    protected override void Start()
    {
    }

    // Update is called once per frame
    protected override void Update()
    {
        RefreshBoxLocation();
    }

    protected void RefreshBoxLocation()
    {
        int currNoOfPointsAdded = m_Decal.PointsCount;
        if (currNoOfPointsAdded != m_NoOfPointsAdded && currNoOfPointsAdded >= 2)
        {
            Vector3 StartPt = m_Decal.GetPoint(0);
            Vector3 EndPt = m_Decal.GetPoint(currNoOfPointsAdded - 1);

            m_DefenderCollider.center = (StartPt + EndPt) * 0.5f;
        }
    }

    public override bool CanAttack(Actor attacker)
    {
        if (attacker.GetType().IsSubclassOf(typeof(Patch)) || attacker.GetType() == typeof(Patch))
        {
            return true;
        }

        return false;
    }

    // Called when any types weapon collides with this, returns true if it is successful attack
    // returns false if attack is blocked
    public override bool Attacked(Actor attacker, GameObject bodyPartGotHit, float damage)
    {
        if (attacker.GetType().IsSubclassOf(typeof(Patch)) || attacker.GetType() == typeof(Patch))
        {
            Patch patch = (Patch)attacker;
            if (patch != null)
            {
                if (GameScoreManager.Get() != null)
                    GameScoreManager.Get().IncrementPatchCount();
                //patch.SetToDestroy();
                SetToDestroy();
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

    public void SetToDestroy()
    {
        m_Decal.SetToDestroy();
        GameScoreManager.Get().DecrementLiveCracks();
        //SetEnableAllActorTriggerBase(false);
        m_DefenderCollider.enabled = false;
    }
}
