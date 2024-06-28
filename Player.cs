using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Fasáda pro hráče
 */
public class Player : MonoBehaviour
{
    //Reference na jednotlivé scripty
    [SerializeField] private PlayerMovementController movementController = null;
    [SerializeField] private PlayerAnimationController animationController = null;
    [SerializeField] private PlayerHealthController healthController = null;
    
    //Gettery pro získání jednotlivých scriptů, což umožnňuje získat přístup k jejich metodám, metody ke kterým chceme přistupovat musí být nastavené jako public
    public PlayerAnimationController AnimController => animationController;
    public PlayerHealthController HealthController => healthController;
    public PlayerMovementController MovementController => movementController;

    //Inicializace jednotlivých scriptů
    private void Start()
    {
        movementController.Init(this);
        animationController.Init(this);
        healthController.Init(this);
    }

    /**
     * Metoda Update se provolává až do jádra enginu. Proto je lepší ji zavolat jen jednou v rámci fasády.
     * Zajistíme si tak že se nebude volat jinde a zbytečne tak vyžírat výpočetní prostředky
     * Metodu update v jednotlivých scriptech tak přejmenujeme, dle koncevece Update + název scriptu
     * UpdatePMC = Update metody PlayerMovementController
     * UpdatePHC = Update metody PlayerHealthController atd.
     * Stejnou konvenci zavedeme i u LateUpdate a FixedUpdate
    */
    private void Update()
    {
        movementController.UpdatePMC();
        healthController.UpdatePHC();
    }

    private void LateUpdate()
    {
        
    }
    
    private void FixedUpdate()
    {
        movementController.FixedUpdatePMC();
    }
}
