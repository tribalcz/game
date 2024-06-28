using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class DamageDealer : MonoBehaviour
{
    //Nastavení proměnných pro poškození + stavové proměnné. 
    [SerializeField] private float damage; //poškození
    [SerializeField] private float postTriggerDamage; // Poškození po opuštění triggeru "poškození prostředím"
    [SerializeField] private bool destroyOnImpact = true; //Zda bude objekt zničený po srážce s hráčem 
    private Coroutine continuousDamageCoroutine; //Korutina pro následné poškození po opouštěnní triggeru, například poškození elementem
    
    [SerializeField] private PlayerHealthController healthController;
    [SerializeField] private MultiTag multiTag; //Instance třídy pro přidělení více tagů objektu

    private void Start()
    {
        //Nastavení jednotlivých druhů poškození z nastavení herního světa
        postTriggerDamage = WorldController.Instance.FireDamagePostTriggerDamage; //Nastavení poškození ohněm po opuštění triggeru, prozatím jen oheň. 
        damage = WorldController.Instance.StandardDamage; //Nastavení standardního poškození
    }

    /**
     * Ošetření poškození kolizí, tedy případu kdy collider objektu narazí do collideru hráče
     */
    private void OnCollisionEnter(Collision collision)
    {
        HandleDamage(collision.gameObject);
        Helper.ListComponents(collision.gameObject);
    }

    //Ošetření poškození triggerem, tedy případu kdy hráč vstoupí do nebezpečného prostředí. Například ohniště
    private void OnTriggerEnter(Collider collider)
    {
        Debug.Log("Triggered by: " + collider.gameObject.name);
        Helper.ListComponents(collider.gameObject);
        
        if (healthController != null) //Pokud hráč není mrtvý...
        {
            continuousDamageCoroutine = StartCoroutine(ContinuousDamage()); //Spustí se korutina pro následné poškození
            ToggleHealthRegen(); //Zastaví se regenerace zdraví
            Debug.Log("Dealt " + damage + " damage to " + collider.gameObject.name + ".");
            if (destroyOnImpact)
            {
                gameObject.SetActive(false);
                Debug.Log("Destroyed self.");
            }
            if (multiTag != null && multiTag.HasTag("Fire")) //Pokud má objekt tag "Fire"...
            {
                healthController.isBurning = true; //Nastavíme stavový efekt hoření
                damage = WorldController.Instance.FireDamage; //Poškodíme hráče
            }
        }
        else
        {
            Debug.Log("No HealthController found on " + collider.gameObject.name + ".");
        }
    }

    /**
     * Ošetření opuštění triggeru, tedy případu kdy hráč opustí nebezpečné prostředí
     */
    private void OnTriggerExit(Collider collider)
    {
        if (healthController != null && continuousDamageCoroutine != null) //Pokud hráč není mrtvý a korutina pro následné poškození běží...
        {
            StopCoroutine(continuousDamageCoroutine); //Zastaví se korutina pro následné poškození
        }
        if (healthController.isBurning) //Pokud hráč hoří...
        {
            StartCoroutine(PostTriggerDamage(WorldController.Instance.FirePostTriggerDamage)); //Spustí se korutina pro následné poškození po opuštění triggeru
            Debug.Log("Started post trigger damage.");
        } else
        {
            ToggleHealthRegen(); //Znovu se zapne regenerace zdraví
        }
    }

    //Metoda pro poškození hráče v případě že objekt je collider a ne trigger
    private void HandleDamage(GameObject gameObject)
    {
        if (healthController != null)
        {
            healthController.TakeDamage(damage);
            Debug.Log("Dealt " + damage + " damage to " + gameObject.name + ".");
        }
        else
        {
            Debug.Log("No HealthController found on " + gameObject.name + ".");
        }
        
        if (destroyOnImpact)
        {
            gameObject.SetActive(false);
            Debug.Log("Destroyed self.");
        }
    }
    
    //Přepíná regenerace zdraví
    private void ToggleHealthRegen()
    {
        if (healthController != null)
        {
            healthController.isRegenerating = !healthController.isRegenerating;
        }
    }

    /**
     * Poškozuje hráče každou sekundu
     */
    private IEnumerator ContinuousDamage()
    {
        while (true)
        {
            healthController.TakeDamage(damage);
            yield return new WaitForSeconds(1f);
        }
    }

    /**
     * Poškozuje hráče po určitou dobu, po opuštění triggeru reprezentující nebzpečné prostředí
     */
    private IEnumerator PostTriggerDamage(float postTriggerDamageDuration)
    {
        float endTime = Time.time + postTriggerDamageDuration;

        while (Time.time < endTime)
        {
            healthController.TakeDamage(postTriggerDamage);
            yield return new WaitForSeconds(1f);
        }
        
        healthController.isBurning = false;
        ToggleHealthRegen();
    }
}
