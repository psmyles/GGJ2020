using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameScoreManager : MonoBehaviour
{
    private int m_PatchedCount = 0;
    private float m_PlayerHealth = 1.0f;
    private int m_NumLiveCracks = 0;

    [SerializeField]
    private Player m_Player;

    [SerializeField]
    private int m_MaxNoOfLiveCracks = 100;

    public int PatchedCount
    {
        get { return m_PatchedCount; }
    }

    public float PlayerHealth
    {
        get { return m_PlayerHealth; }
        set { m_PlayerHealth = value; }
    }

    public void IncrementPatchCount()
    {
        m_PatchedCount++;
    }

    public void IncrementLiveCracks()
    {
        m_NumLiveCracks++;
    }

    public void DecrementLiveCracks()
    {
        m_NumLiveCracks--;
    }

    public int NumLiveCracks
    {
        get { return m_NumLiveCracks; }
    }

    public int MaxNumLiveCracks
    {
        get { return m_MaxNoOfLiveCracks; }
    }

    private void Awake()
    {
        gGameScoreManager = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float damageBarWidthPercent = (float)NumLiveCracks / MaxNumLiveCracks;
        if (damageBarWidthPercent >= 1.0f)
        {
            m_Player.MakePlayerDie();
        }
    }

    // Singleton variables and functions
    private static GameScoreManager gGameScoreManager;

    public static GameScoreManager Get()
    {
        return gGameScoreManager;
    }
}
