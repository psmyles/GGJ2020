using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Decal))]
public class Patch : Actor
{
    [SerializeField]
    private BoxCollider m_AttackerCollider;

    [SerializeField]
    private float m_PercentOfPatchSizeToBoxSizeWidth = 0.25f;

    [SerializeField]
    private float m_PercentOfPatchSizeToBoxSizeLength = 0.75f;

    [SerializeField]
    private float m_ColliderHeight = 0.25f;

    private Decal m_Decal;
    private int m_NoOfPointsAdded = 0;

    // Use this for initialization
    protected void Awake()
    {
        m_Decal = GetComponent<Decal>();
        m_NoOfPointsAdded = 0;
    }
    // Use this for initialization
    protected override void Start()
    {
        
    }

    // Update is called once per frame
    protected override void Update()
    {
        RefreshBoxSize();
    }

    protected void RefreshBoxSize()
    {
        int currNoOfPointsAdded = m_Decal.PointsCount;
        if (currNoOfPointsAdded != m_NoOfPointsAdded && currNoOfPointsAdded >= 2)
        {
            Vector3 StartPt = m_Decal.GetPoint(0);
            Vector3 EndPt = m_Decal.GetPoint(currNoOfPointsAdded - 1);

            float boxlen = (EndPt - StartPt).magnitude * m_PercentOfPatchSizeToBoxSizeLength;
            float boxWidth = m_Decal.Thickness * m_PercentOfPatchSizeToBoxSizeWidth;

            m_AttackerCollider.size = new Vector3(boxWidth, m_ColliderHeight, boxlen);
            m_AttackerCollider.transform.position = (StartPt + EndPt) * 0.5f;

            m_AttackerCollider.transform.forward = (EndPt - StartPt).normalized;
        }
    }

    public override bool CanAttack(Actor attacker)
    {
        return false;
    }

    // Called when any types weapon collides with this, returns true if it is successful attack
    // returns false if attack is blocked
    public override bool Attacked(Actor attacker, GameObject bodyPartGotHit, float damage)
    {
        return false;
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
