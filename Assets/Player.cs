using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class HeroPlayer : MonoBehaviour
{
    public Animator animController;
    public int maxHealth = 100;
    public int heroHealth = 100;
    public TextMeshProUGUI healthText;
    public Image healthBarImage;

    
    public Button attack1Button;
    public Button attack2Button;
    public Color pressedTintColor = new Color(0.75f, 0.75f, 0.75f, 1f);
    public float pressEffectDuration = 0.12f;

    
    public TextMeshProUGUI endGameText;
    public GameObject losePanel;

    
    public GameObject attack1Vfx;
    public GameObject attack2Vfx;
    public Transform vfxSpawnPoint;

    public Enemy targetEnemy;

    public static bool isGameOver = false;
    private bool isDead = false;

    void Start()
    {
        isGameOver = false;
        isDead = false;
        heroHealth = Mathf.Clamp(heroHealth, 0, maxHealth);
        UpdateUI();

        if (losePanel != null) losePanel.SetActive(false);
        if (endGameText != null) endGameText.gameObject.SetActive(false);

        SetButtonsInteractable(true);

        
        if (attack1Button != null)
        {
            attack1Button.onClick.RemoveAllListeners();
            attack1Button.onClick.AddListener(GetAttack1);
        }

        if (attack2Button != null)
        {
            attack2Button.onClick.RemoveAllListeners();
            attack2Button.onClick.AddListener(GetAttack2);
        }
    }

    void Update()
    {
        if (isGameOver || isDead)
        {
            SetButtonsInteractable(false);
            return;
        }

        if (Keyboard.current != null)
        {
            if (Keyboard.current.qKey.wasPressedThisFrame)
            {
                TriggerButtonPressVisual(attack1Button);
                GetAttack1();
            }

            if (Keyboard.current.rKey.wasPressedThisFrame)
            {
                TriggerButtonPressVisual(attack2Button);
                GetAttack2();
            }
        }
    }

    private void TriggerButtonPressVisual(Button targetButton)
    {
        if (targetButton == null || !targetButton.interactable) return;
        StartCoroutine(ButtonPressRoutine(targetButton));
    }

    private IEnumerator ButtonPressRoutine(Button btn)
    {
        Image img = btn.targetGraphic as Image;
        Color originalColor = img != null ? img.color : Color.white;
        Vector3 originalScale = btn.transform.localScale;
        Vector3 pressedScale = originalScale * 0.9f;

        if (img != null) img.color = pressedTintColor;
        btn.transform.localScale = pressedScale;

        yield return new WaitForSeconds(pressEffectDuration);

        if (img != null) img.color = originalColor;
        btn.transform.localScale = originalScale;
    }

    private void SetButtonsInteractable(bool state)
    {
        if (attack1Button != null) attack1Button.interactable = state;
        if (attack2Button != null) attack2Button.interactable = state;
    }

    public void GetAttack1()
    {
        if (isGameOver || isDead) return;

        if (animController != null)
        {
            animController.SetTrigger("Attack");
        }

        SpawnVfx(attack1Vfx);

        if (targetEnemy != null)
        {
            targetEnemy.GetHurt(10);
        }
    }

    public void GetAttack2()
    {
        if (isGameOver || isDead) return;

        if (animController != null)
        {
            animController.SetTrigger("Attack2");
        }

        SpawnVfx(attack2Vfx);

        if (targetEnemy != null)
        {
            targetEnemy.GetHurt(20);
        }
    }

    private void SpawnVfx(GameObject vfxPrefab)
    {
        if (vfxPrefab == null) return;

        Vector3 spawnPos;
        if (vfxSpawnPoint != null)
        {
            spawnPos = vfxSpawnPoint.position;
        }
        else if (targetEnemy != null)
        {
            spawnPos = targetEnemy.transform.position;
        }
        else
        {
            spawnPos = transform.position;
        }

        spawnPos.z = -1f;

        GameObject targetObj;
        if (!vfxPrefab.scene.IsValid())
        {
            targetObj = Instantiate(vfxPrefab, spawnPos, Quaternion.identity);
            Destroy(targetObj, 2f);
        }
        else
        {
            targetObj = vfxPrefab;
            targetObj.transform.position = spawnPos;
            targetObj.SetActive(false);
        }

        targetObj.SetActive(true);

        foreach (ParticleSystem ps in targetObj.GetComponentsInChildren<ParticleSystem>(true))
        {
            ps.Clear();
            ps.Play();
        }
    }

    public void GetHurt(int damage)
    {
        if (isGameOver || isDead) return;

        heroHealth = Mathf.Max(0, heroHealth - damage);
        UpdateUI();

        if (heroHealth <= 0)
        {
            GameOverLose();
        }
        else if (animController != null)
        {
            animController.SetTrigger("Hurt");
        }
    }

    void GameOverLose()
    {
        isDead = true;
        isGameOver = true;
        SetButtonsInteractable(false);

        if (animController != null)
        {
            animController.SetBool("Death", true);
        }

        if (losePanel != null)
        {
            losePanel.SetActive(true);
        }

        if (endGameText != null)
        {
            endGameText.gameObject.SetActive(true);
            endGameText.text = "Game Over You Lose";
        }
    }

    void UpdateUI()
    {
        if (healthText != null)
        {
            healthText.text = "Health: " + heroHealth.ToString();
        }

        if (healthBarImage != null && maxHealth > 0)
        {
            healthBarImage.fillAmount = (float)heroHealth / maxHealth;
        }
    }
}