using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecalsManager : MonoBehaviour
{
    public enum DecalsType
    {
        DT_Tape = 0,
        DT_Crack,
        DT_Count
    }

    [SerializeField]
    private Decal m_TapeDecalePrefab;

    [SerializeField]
    private Decal m_CrackDecalsPrefab;

    private List<Decal> m_AllDecals = new List<Decal>();


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Decal CreateDecal(DecalsType type)
    {
        if ((int)type >= 0 && type < DecalsType.DT_Count)
        {
            switch (type)
            {
                case DecalsType.DT_Tape:
                    {
                        GameObject go = GameObject.Instantiate(m_TapeDecalePrefab.gameObject);
                        m_AllDecals.Add(go.GetComponent<Decal>());
                        return go.GetComponent<Decal>();
                    }

                case DecalsType.DT_Crack:
                    {
                        GameObject go = GameObject.Instantiate(m_CrackDecalsPrefab.gameObject);
                        m_AllDecals.Add(go.GetComponent<Decal>());
                        return go.GetComponent<Decal>();
                    }
            }
        }

        return null;
    }

    public void DestroyDecal(Decal dec)
    {
        if (dec == null)
            return;
                
        if (m_AllDecals.Contains(dec))
        {
            m_AllDecals.Remove(dec);
        }

        GameObject.Destroy(dec.gameObject);
    }
}
