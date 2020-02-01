using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileManager : MonoBehaviour
{
    [SerializeField]
    private Projectile[] m_AllProjectilePrefabs;

    private Dictionary<System.Type, List<Projectile>> m_ProjectilePrefabsBasedOnType = new Dictionary<System.Type, List<Projectile>>();

    void Awake()
    {
        for (int i = 0; i < m_AllProjectilePrefabs.Length; i++)
        {
            Projectile proj = m_AllProjectilePrefabs[i];
            if (!m_ProjectilePrefabsBasedOnType.ContainsKey(proj.GetType()))
            {
                m_ProjectilePrefabsBasedOnType.Add(proj.GetType(), new List<Projectile>());
            }

            m_ProjectilePrefabsBasedOnType[proj.GetType()].Add(proj);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Projectile SpawnRandomProjectileOfType<T>() where T : Projectile
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
