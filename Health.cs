using DG.Tweening.Core.Easing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour, IDataPersistence
{
    public float maxHealth;
    public float currentHealth;

    [SerializeField] private HealthBar healthBar;
    [SerializeField] private GameObject player;
    [SerializeField] private ReviveScript reviveScript;
    [SerializeField] private PlayerClass playerClass;

    public bool playerIsDead = false;

    public void LoadData(GameData data)
    {
        CharacterClassSO classData = GameDataSO.CharacterClassDataMap[data.playerClass];
        maxHealth = classData.stamina * 10 * classData.staminaBenefitValue;
        currentHealth = data.currentHealth;
    }

    public void SaveData(GameData data)
    {
        if(currentHealth <= 0f)
        {
            data.currentHealth = 1f;
        }
        else
        {
            data.currentHealth = currentHealth;
        }
    }

    public void TakeDamage(float Damage)
    {
        if(currentHealth <= 0f)
        {
            return;
        }

        currentHealth -= Damage;
        healthBar.SetCurrentHealth(currentHealth);
        player.GetComponent<Animator>().SetTrigger("isHurt");
        SoundManager.instance.PlayEffectClip(SoundManager.instance.damaged, 0.5f);
       if (currentHealth <= 0f)
       {
            currentHealth = 0f;
            SoundManager.instance.PlayEffectClip(SoundManager.instance.playerKilled, 0.7f);
            PlayerHasDied();
        }

        GameObject floatingText = GameManager.instance.floatingTextPool.GetPooledObject();
        floatingText.GetComponent<FloatingTextScript>().ActivateText(-Damage, GameManager.instance.playerController.transform.position, false, true);
    }

    public void Healing(float healPoints, bool canHealWhenDead = false)
    {
        if(currentHealth == maxHealth)
        {
            return;
        }
        if(currentHealth <= 0 && !canHealWhenDead)
        {
            return;
        }
        else
        {
            currentHealth += healPoints;
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
            healthBar.SetCurrentHealth(currentHealth);
            if (healPoints > 0f)
            {
                GameObject floatingText = GameManager.instance.floatingTextPool.GetPooledObject();
                floatingText.GetComponent<FloatingTextScript>().ActivateText(healPoints, GameManager.instance.playerController.transform.position);
            }
        }
    }

    public void PercentHealing(float percentage, bool canHealWhenDead = false)
    {
        if (currentHealth == maxHealth)
        {
            return;
        }

        if (currentHealth <= 0 && !canHealWhenDead)
        {
            return;
        }
        else
        {
            currentHealth += maxHealth * percentage;
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
            healthBar.SetCurrentHealth(currentHealth);

            GameObject floatingText = GameManager.instance.floatingTextPool.GetPooledObject();
            floatingText.GetComponent<FloatingTextScript>().ActivateText(maxHealth * percentage, GameManager.instance.playerController.transform.position);
        }
    }

    public void SetHealth()
    {
        healthBar.SetMaxHealth(maxHealth);
        healthBar.SetCurrentHealth(currentHealth);
    }

    public void SetMaxHealth(bool checkCurrentHealth = true)
    {
        maxHealth = playerClass.stamina * 10 * playerClass.staminaBenefitValue;
        if(checkCurrentHealth)
        {
            if(currentHealth > maxHealth) { currentHealth = maxHealth;}
        }
        SetHealth();
    }

    public void GetVariables()
    {
        healthBar = FindObjectOfType<HealthBar>();
        player = GameManager.instance.playerController.gameObject;
        reviveScript = FindObjectOfType<ReviveScript>();
    }

    private void PlayerHasDied()
    {
        playerIsDead = true;
        if (GameManager.instance.inventory.inventoryOpen)
        {
            GameManager.instance.inventory.ToggleInventory();
        }
        GameManager.instance.playerController.DeactivateActiveItem();
        InputTypeManager.instance.inputActions.Player.Disable();
        InputTypeManager.instance.inputActions.UI.Enable();
        player.GetComponent<Animator>().SetTrigger("playerHasDied");
        StartCoroutine(reviveScript.RevivePanel());
    }
}
