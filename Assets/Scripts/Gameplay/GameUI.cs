using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameUI : MonoBehaviour
{
    [SerializeField]
    private GameInputMessageGenerator m_MessageGenerator;

    [SerializeField]
    private Player m_Player;

    [SerializeField]
    private Text m_Text;

    [SerializeField]
    private float m_HealthBarMaxWidth = 194;

    [SerializeField]
    private float m_DamageBarMaxWidth = 194;

    [SerializeField]
    private Image m_HealthBar;
    [SerializeField]
    private Image m_DamagehBar;

    [SerializeField]
    private Canvas m_HealthCanvas;

    [SerializeField]
    private Canvas m_ScoreCanvas;

    [SerializeField]
    private Canvas m_DamageCanvas;

    [SerializeField]
    private Canvas m_GameOverCanvas;

    private bool m_GameOver = false;
    private bool m_GameOverScreenShown = false;
    private float m_GameOverTimer = 2.0f;

    // Start is called before the first frame update
    void Start()
    {
        m_GameOverCanvas.gameObject.SetActive(false);
        m_GameOver = false;
        m_GameOverScreenShown = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!m_GameOver)
        {
            m_Text.text = "" + (GameScoreManager.Get() != null ? GameScoreManager.Get().PatchedCount : 0);
            float healthBarWIdth = GameScoreManager.Get().PlayerHealth * m_HealthBarMaxWidth;
            m_HealthBar.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, healthBarWIdth);

            float damageBarWidthPercent = (float)GameScoreManager.Get().NumLiveCracks / GameScoreManager.Get().MaxNumLiveCracks;
            if (damageBarWidthPercent >= 1.0f)
            {
                damageBarWidthPercent = 1.0f;
                m_GameOver = true;
                m_MessageGenerator.gameObject.SetActive(false);
            }
            if (GameScoreManager.Get().PlayerHealth <= 0.0f)
            {
                m_GameOver = true;
                m_MessageGenerator.gameObject.SetActive(false);
            }

            m_DamagehBar.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, damageBarWidthPercent * m_DamageBarMaxWidth);
        }
        else
        {
            if (m_Player.DeathStateCompleted && !m_GameOverScreenShown)
            {
                m_GameOverScreenShown = true;
                m_HealthCanvas.gameObject.SetActive(false);
                m_ScoreCanvas.gameObject.SetActive(false);
                m_DamageCanvas.gameObject.SetActive(false);
                m_GameOverCanvas.gameObject.SetActive(true);
                m_GameOverTimer = 2.0f;
            }
            if (m_GameOverScreenShown)
            {
                m_GameOverTimer -= Time.deltaTime;
                if (m_GameOverTimer <= 0.0f)
                {
                    if (Input.anyKey)
                    {
                        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                    }
                }
            }
        }
    }
}
