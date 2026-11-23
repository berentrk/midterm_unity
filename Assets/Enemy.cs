using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class Enemy : MonoBehaviour
{
    public Animator animController;
    public int maxHealth = 100;
    public int enemyHealth = 100;
    public TextMeshProUGUI healthText;
    public Image healthBarImage;

    
    public TextMeshProUGUI endGameText;
    public GameObject winPanel;

    public HeroPlayer targetPlayer;

    private bool isDead = false;

    void Start()
    {
        isDead = false;
        enemyHealth = Mathf.Clamp(enemyHealth, 0, maxHealth);
        UpdateUI();

        if (winPanel != null) winPanel.SetActive(false);
        if (endGameText != null) endGameText.gameObject.SetActive(false);
    }

    void Update()
    {
        
        if (HeroPlayer.isGameOver || isDead) return;

        if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            GetAttack1();
        }
    }

    public void GetHurt(int damage)
    {
        if (HeroPlayer.isGameOver || isDead) return;

       
        enemyHealth = Mathf.Max(0, enemyHealth - damage);
        UpdateUI();

        if (enemyHealth <= 0)
        {
            GameOverWin();
        }
        else if (animController != null)
        {
            animController.SetTrigger("Hurt");
        }
    }

    void GameOverWin()
    {
        isDead = true;
        HeroPlayer.isGameOver = true;

        if (animController != null)
        {
            animController.SetBool("Death", true);
        }

        if (winPanel != null)
        {
            winPanel.SetActive(true);
        }

        if (endGameText != null)
        {
            endGameText.gameObject.SetActive(true);
            endGameText.text = "Game Over You Win";
        }
    }

    public void GetAttack1()
    {
        if (HeroPlayer.isGameOver || isDead) return;

        if (animController != null)
        {
            animController.SetTrigger("Attack");
        }

        if (targetPlayer != null)
        {
            targetPlayer.GetHurt(10);
        }
    }

    void UpdateUI()
    {
        if (healthText != null)
        {
            healthText.text = "Enemy Health: " + enemyHealth.ToString();
        }

        if (healthBarImage != null && maxHealth > 0)
        {
            healthBarImage.fillAmount = (float)enemyHealth / maxHealth;
        }
    }
}