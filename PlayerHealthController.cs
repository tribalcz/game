using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealthController : MonoBehaviour
{
    //Nastavení proměnných pro zdraví + stavové proměnné
    public float maxHealth = 100f;
    public float currentHealth;
    public float regenRate;
    public bool isRegenerating = true;
    public bool isDead = false;
    public bool isBurning = false;
    //Reference na instanci hráče
    private Player player = null;
    
    //Inicializace třídy a nastavení hráče
    public void Init(Player pPlayer)
    {
        player = pPlayer;
    }
    
    //Nastavení aktuálního zdraví na maximum a nastavení regenerace
    private void Start()
    {
        currentHealth = maxHealth;
        regenRate = WorldController.Instance.healthRegenRate; //Získání regenerační rychlosti z nastavení instance herního světa.
    }
    
    public void UpdatePHC()
    {
        if (currentHealth <= 0)
        {
            Die();
        } else if (currentHealth < maxHealth && isRegenerating && !isDead)
        {
            Heal(regenRate);
        }
    }
    
    /**
     * Metoda pro zranění hráče
     */
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth); //Ošetření aby zdraví nešlo pod 0 a nad maximum
    }

    /**
     * Metoda pro léčení hráče
     */
    public void Heal(float healAmount)
    {
        currentHealth += healAmount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth); //Ošetření aby zdraví nešlo pod 0 a nad maximum
    }

    /**
     * Metoda pro oštření smrti hráče
     */
    private void Die()
    {
        Debug.Log(gameObject.name + " died.");
        //Nastavení proměnných pro smrt
        isDead = true;
        isRegenerating = false;
        player.AnimController.SetDeathState(); //Díky fasádě můžeme snadno zavolat metodu z jiného scriptu aniž bychom museli znát jeho instanci.
        WorldController.Instance.isGameOver = true;
        //Tohle je taková specialitka, pokud je povolený ghost mode, tak se hráč respawne jako duch, což mu umožní provést akci jako sebrat batoh aniž by na něho nějak působilo prostředí. V podstatě se jedná o formu spectate módu.
        if (WorldController.Instance.ghostModerEnabled)
        {
            WorldController.Instance.isGhostMode = true;
        }
    }
}
