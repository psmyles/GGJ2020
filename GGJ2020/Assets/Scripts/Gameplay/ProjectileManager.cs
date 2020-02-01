using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileManager : MonoBehaviour
{
    [SerializeField]
    private Projectile[] m_AllProjectilePrefabs;

    [SerializeField]
    private DecalsManager m_DecalsManager;

    [SerializeField]
    private ProjectileSpawnArea[] m_AllSpawnArea;

    [SerializeField]
    private float m_ProjectileSpawnMinTime = 0.6f;
    [SerializeField]
    private float m_ProjectileSpawnMaxTime = 2.0f;

    [SerializeField]
    private float m_ProjectileMinTimeToReachTarget = 1.0f;
    [SerializeField]
    private float m_ProjectileMaxTimeToReachTarget = 2.0f;

    private Dictionary<System.Type, List<Projectile>> m_ProjectilePrefabsBasedOnType = new Dictionary<System.Type, List<Projectile>>();
    private List<System.Type> m_ProjectileTypes = new List<System.Type>();
    private List<Projectile> m_AllProjectiles = new List<Projectile>();

    private float m_CurrTime = 0;

    void Awake()
    {
        for (int i = 0; i < m_AllProjectilePrefabs.Length; i++)
        {
            Projectile proj = m_AllProjectilePrefabs[i];
            if (!m_ProjectilePrefabsBasedOnType.ContainsKey(proj.GetType()))
            {
                m_ProjectilePrefabsBasedOnType.Add(proj.GetType(), new List<Projectile>());
                m_ProjectileTypes.Add(proj.GetType());
            }

            m_ProjectilePrefabsBasedOnType[proj.GetType()].Add(proj);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    //bool created = false;
    // Update is called once per frame
    void Update()
    {
        if (m_CurrTime <= 0.0f)
        {
            //created = true;
            int randInd = Random.Range(0, m_ProjectilePrefabsBasedOnType.Count);
            System.Type projType = m_ProjectileTypes[randInd];

            Projectile projectile = SpawnRandomProjectileOfType(projType);
            projectile.Manager = this;

            Vector3 outLocation;
            Vector3 outTarget;
            GetSpawnParam(out outLocation, out outTarget);

            projectile.SetToLaunch(Random.Range(m_ProjectileMinTimeToReachTarget, m_ProjectileMaxTimeToReachTarget), outLocation, outTarget);

            m_CurrTime = Random.Range(m_ProjectileSpawnMinTime, m_ProjectileSpawnMaxTime);
        }
        m_CurrTime -= Time.deltaTime;
    }

    public void GetSpawnParam(out Vector3 location, out Vector3 target)
    {
        location = transform.position;
        target = transform.position;

        int randAreaInd = Random.Range(0, m_AllSpawnArea.Length);
        if (randAreaInd < m_AllSpawnArea.Length)
        {
            ProjectileSpawnArea area = m_AllSpawnArea[randAreaInd];

            location = area.GetRandomPoint();
            target = area.GetRandomTarget();
        }
    }

    public void DestroyProjectile(Projectile proj)
    {
        m_AllProjectiles.Remove(proj);
        if (proj != null)
        {
            GameObject.Destroy(proj.gameObject);
        }
    }

    private Projectile SpawnRandomProjectileOfType(System.Type type)
    {
        if (m_ProjectilePrefabsBasedOnType.ContainsKey(type))
        {
            int projCount = m_ProjectilePrefabsBasedOnType[type].Count;
            int randInd = Random.Range(0, projCount);
            Projectile prefab = m_ProjectilePrefabsBasedOnType[type][randInd];
            if (prefab != null)
            {
                GameObject proj = GameObject.Instantiate(prefab.gameObject);
                Projectile projectile = proj.GetComponent<Projectile>();
                projectile.DecalManager = m_DecalsManager;
                return proj.GetComponent<Projectile>();
            }
        }

        return null;

    }
    private Projectile SpawnRandomProjectileOfType<T>() where T : Projectile
    {
        if (m_ProjectilePrefabsBasedOnType.ContainsKey(typeof(T)))
        {
            int projCount = m_ProjectilePrefabsBasedOnType[typeof(T)].Count;
            int randInd = Random.Range(0, projCount);
            Projectile prefab = m_ProjectilePrefabsBasedOnType[typeof(T)][randInd];
            if (prefab != null)
            {
                GameObject proj = GameObject.Instantiate(prefab.gameObject);
                return proj.GetComponent<Projectile>();
            }
        }

        return null;
    }
}
