using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldController : MonoBehaviour
{
    public static WorldController Instance { get; private set; }
    
    public float FireDamage = 3f; //Poškození způsobené ohněm
    public float FireDamagePostTriggerDamage = 2f; //Poškození způsobené ohněm po opuštění triggeru
    public float FirePostTriggerDamage = 10f; //Jak dlouho bude hráč po opuštění triggeru poškozován ohněm
    public float StandardDamage = 8f; //Standardní poškození pro neelementální útoky
    public float damageMultiplier = 1f; //Násobitel poškození 
    public float fireDamageMultiplier = 1f; //Násobitel poškození ohněm
    public float gasDamageMultiplier = 1f; //Násobitel poškození plynem
    public float autosaveInterval = 300f; //Interval automatického ukládání v sekundách
    public float healthRegenRate = .001f; //Rychlost regenerace zdraví
    public float jumpingForce = 8f; //Síla skoku
    public bool isPaused = false; //Zda je hra pozastavena
    public bool isGameOver = false; //Zda je hra ukončena
    public bool isGhostMode = false; //Zda je hráč v ghost módu
    public bool ghostModerEnabled = false; //Zda je ghost mód povolen
    public bool autosaveEnabled = true; //Zda je povoleno automatické ukládání
    public bool isGodModeEnabled = false; //Zda je god mód povolen

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
        
        DontDestroyOnLoad(gameObject);
    }
}
