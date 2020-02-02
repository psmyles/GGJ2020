using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Projectile : Actor
{
    private DecalsManager m_DecalsManager;

    private Decal m_CurrentDecal = null;

    [SerializeField]
    private float m_DecalWidth = 1.0f;
    [SerializeField]
    private float m_DecalPointAddDist = 1.0f;

    [SerializeField]
    private float m_DecalAddDeAcceleration = 100.0f;
    [SerializeField]
    private float m_DecalUpOffset = 0.01f;

    protected bool m_Launch = false;
    protected bool m_Launched = false;
    protected Rigidbody m_Rigidbody;
    protected Vector3 m_LaunchPos;
    protected Vector3 m_LaunchTarget;
    protected float m_TimeToReach;
    protected ProjectileManager m_Manaer;
    protected bool m_ToBeDestroyed = false;
    protected bool m_ShowGroundDecalAndDestroy = false;

    protected Vector3 m_DecalAddDir;
    protected float m_CurrDecalAddSpeed;
    protected float m_CurrDecalAddDist;
    protected Vector3 m_LastDecalPos;
    protected bool m_AddingDecals = false;

    protected MeshRenderer m_MeshRenderer;

    public ProjectileManager Manager
    {
        get
        {
            return m_Manaer;
        }
        set
        {
            m_Manaer = value;
        }
    }

    public DecalsManager DecalManager
    {
        get
        {
            return m_DecalsManager;
        }
        set
        {
            m_DecalsManager = value;
        }
    }

    public Vector3 Velocity
    {
        get
        {
            return m_Rigidbody.velocity;
        }
    }

    protected virtual void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
        SetEnableActorTriggerBase(typeof(Attacker), false);
        m_MeshRenderer = GetComponent<MeshRenderer>();
    }

    // Use this for initialization
    protected override void Start()
    {
        
    }

    // Update is called once per frame
    protected override void Update()
    {
        if (m_Launch && !m_Launched)
        {
            m_Launched = true;
            Launch();
        }
        if (m_ToBeDestroyed)
        {
            m_ToBeDestroyed = false;
            Manager.DestroyProjectile(this);
        }
        if (!m_AddingDecals && m_ShowGroundDecalAndDestroy)
        {
            m_AddingDecals = true;
            m_CurrentDecal = m_DecalsManager.CreateDecal(DecalsManager.DecalsType.DT_Crack);
            m_CurrentDecal.Thickness = m_DecalWidth;


            RaycastHit rayHit;
            if (PhysicsUtils.RaycastToGround(transform.position, out rayHit))
            {
                m_LastDecalPos = rayHit.point;
            }
            else
            {
                m_LastDecalPos = transform.position;
            }

            m_CurrentDecal.AddPoint(m_LastDecalPos + Vector3.up * m_DecalUpOffset);
            
            Vector3 velocity = Velocity;
            velocity.y = 0.0f;
            m_CurrDecalAddSpeed = velocity.magnitude;
            m_DecalAddDir = velocity.normalized;
            m_CurrDecalAddDist = 0.0f;

            m_MeshRenderer.enabled = false;

            // 
            SetToDestroy();
            m_CurrentDecal.AddPoint(m_LastDecalPos + Vector3.up * m_DecalUpOffset + m_DecalAddDir * m_DecalWidth);
        }
        /*if (m_AddingDecals)
        {
            m_CurrDecalAddSpeed -= m_DecalAddDeAcceleration * Time.deltaTime;
            m_CurrDecalAddDist += m_CurrDecalAddSpeed * Time.deltaTime;

            if (m_CurrDecalAddDist >= m_DecalPointAddDist || m_CurrDecalAddSpeed <= 0.0f)
            {
                m_LastDecalPos += m_DecalAddDir * m_CurrDecalAddDist;
                m_CurrentDecal.AddPoint(m_LastDecalPos + Vector3.up * m_DecalUpOffset);
                m_CurrDecalAddDist = 0.0f;
            }
            
            if (m_CurrDecalAddSpeed <= 0)
            {
                SetToDestroy();
                m_AddingDecals = false;
            }
        }*/
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

    public override void CurrentAttackSuccedded(Actor defender)
    {
    }

    // Called when animation event happens
    public override void OnAnimEvent(AnimEventData data)
    {

    }

    public virtual void SetToLaunch(float timeToReach, Vector3 source, Vector3 target)
    {
        m_LaunchPos = source;
        m_LaunchTarget = target;
        m_TimeToReach = timeToReach;
        transform.position = source;
        m_Launch = true;
    }

    protected virtual void Launch()
    {
        SetEnableActorTriggerBase(typeof(Attacker), true);
        m_Launched = true;
    }

    public void SetToDestroy()
    {
        m_ToBeDestroyed = true;
    }

    public void ShowGroundDecalAndDestroy()
    {
        m_ShowGroundDecalAndDestroy = true;
        SetEnableActorTriggerBase(typeof(Attacker), false);
    }

    public void DestroyWithoutDecals()
    {
        SetToDestroy();
        SetEnableActorTriggerBase(typeof(Attacker), false);
    }
}
