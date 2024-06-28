using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.Serialization;

/**
 * Kontroler pohybu hráče
 */
public class PlayerMovementController : MonoBehaviour
{
    //Nastavení proměnných pro pohyb + stavových proměnných
    public float moveSpeed = 5f;
    public float sprintSpeed = 10f;
    public float crouchSpeed = 2.5f;
    public float jumpForce;
    public float rotationSpeed = 200f;
    public float dashSpeed = 100f;
    public float dashCooldown = 12f;
    public bool isGrounded;
    public bool isJumping = false;
    public KeyCode dashKey = KeyCode.Q; //Umožní nastaavit klávesu pro dash z inspektoru
    public Transform groundCheck; //Transform pro kontrolu toho zda hráč stojí na zemi
    public float groundDistance = 0.4f;
    public LayerMask groundMask; //Umožní nastavit ground masku z inspktoru

    private Rigidbody rb;
    private Vector3 velocity;
    
    
    [SerializeField] private bool isCrouching;
    [SerializeField] private bool isSprinting;
    [SerializeField] private bool isDashing;
    [SerializeField] private float lastDashTime = 0f;
    [FormerlySerializedAs("isJumpAllowed")] [SerializeField] private bool canJump = false;
    [SerializeField] private bool canDash = true;
    
    //Reference na instanci hráče
    private Player player = null;
    
    //Inicializace třídy a nastavení hráče
    public void Init(Player pPlayer)
    {
        player = pPlayer;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        lastDashTime = -dashCooldown; //Nastavení výchozího času pro dash, což umožní hráči použít dash okamžitě po spuštění hry
        jumpForce = WorldController.Instance.jumpingForce;
    }

    // Update is called once per frame
    public void UpdatePMC()
    {
        if (Input.GetButtonDown("Jump") && isGrounded && canJump)
        {
            rb.velocity = new Vector3(0, jumpForce, 0);
            isJumping = true;
        } 
        
        
        if ((Input.GetKeyDown(dashKey) && isGrounded) && Time.time - lastDashTime >= dashCooldown && canDash)
        {
            isDashing = true;
            lastDashTime = Time.time;
        }
        
        playerAnim(); //Zavolání metody pro animace hráče. Tato metoda by měla být volána až po provedení všech operací s pohybem, ale neměla by být volána v FixedUpdate jelikož ten se nevykonává v rámci FPS
    }
    
    public void FixedUpdatePMC()
    {
        bool wasGrounded = isGrounded; //Uložení stavu zda byl hráč na zemi, tohle je taková specialitka pro ú4ely prác s animátorer
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask); //Kontrola zda je hráč na zemi, vytvoří sféru na pozici groundCheck s poloměrem groundDistance a pokud narazí na objekt s groundMask vrátí true 

        float x = Input.GetAxis("Horizontal"); //Získání hodnoty z klávesnice pro osu X
        float z = Input.GetAxis("Vertical"); //Získání hodnoty z klávesnice pro osu Z

        Vector3 move = (transform.right * x + transform.forward * z).normalized; //Vytvoření normalizovaného vektoru pro pohyb,  což zajistí rovnoměrnou rychlost pohybu ve všech směrech

        if (Input.GetKey(KeyCode.LeftShift)) //Pokud je stisknutý levý shift, zvýší se rychlost pohybu na běh
        {
            rb.velocity = new Vector3(move.x * sprintSpeed, rb.velocity.y, move.z * sprintSpeed);
            isSprinting = true;
        }
        else if (Input.GetKey(KeyCode.C)) //Pokud je stisknutý C, sníží se rychlost pohybu na na rychlost chůze v podřepu
        {
            rb.velocity = new Vector3(move.x * crouchSpeed, rb.velocity.y, move.z * crouchSpeed);
            isCrouching = true;
        }
        else if (isDashing) //Akce pro dash
        {
            Vector3 dashDirection = transform.forward;
            RaycastHit dashHit;

            // Pokud se hráč pokusí projít skrz objekt, zastaví se na něm
            if (Physics.Raycast(transform.position, dashDirection, out dashHit, dashSpeed * Time.fixedDeltaTime))
            {
                rb.velocity = new Vector3((dashHit.point - transform.position).x / Time.fixedDeltaTime, rb.velocity.y, (dashHit.point - transform.position).z / Time.fixedDeltaTime);
            }
            else //V opačném případě se dash provede normálně o dashSpeed => sice se to jmenuje speed, ale jedná se o vzdálenost
            {
                rb.velocity = new Vector3(dashDirection.x * dashSpeed, rb.velocity.y, dashDirection.z * dashSpeed);
            }

            isDashing = false; //Zrušení stavu dash
        }
        else if(!wasGrounded && isGrounded && isJumping) //Pokud hráč přistane po skoku, zruší se stav skoku
        {
            isJumping = false;
        }
        else 
        {
            rb.velocity = new Vector3(move.x * moveSpeed, rb.velocity.y, move.z * moveSpeed); //Pokud není stisknutý žádná z kláves, hráč se pohybuje rychlostí moveSpeed
            isSprinting = false;
            isCrouching = false;
        }

        // Vytvoří raycast z kamery na pozici myši, což umožní hráči otočit se směrem kam na pozici myši. 
        // !Toto lze použít poze při topdown pohledu, v případě FPS by bylo nutné použít jiný způsob ovládání kamery! 
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); //Vytvoření reycastu na pozici myši.
        RaycastHit hit; //Proměnná pro uložení informací o objektu na který narazil raycast
        
        if (!player.HealthController.isDead)
        {
            // Provede vysílání paprsku, a pokud narazí na objekt s vrstvou "Ground", natočí hráče tímto směrem.
            // TODO: Natáčení hráče by nemělo být závyslé jen na masce Ground. 
            if (Physics.Raycast(ray, out hit, 100f, groundMask)) //Pokud narazí na objekt s groundMask, uloží pozici objektu do hit.point
            {
                Vector3 targetPosition = hit.point; // Uložení pozice objektu do targetPosition
                targetPosition.y = transform.position.y; 
                
                // Otočení hráče do cílové pozice
                Vector3 direction = (targetPosition - transform.position).normalized;
                Quaternion lookRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.fixedDeltaTime * rotationSpeed);
            }
        }
        
    }

    //Metoda pro animace hráče
    private void playerAnim()
    {
        if (isGrounded)
        {
            if (rb.velocity.sqrMagnitude > 0.1f)
            {
                if (isSprinting)
                {
                    player.AnimController.SetMovementState(MovingState.Running); //Tady je opět vydět výhoda použití fasády - nemusíme znát instanci animátoru, stačí nám znát instanci hráče.
                    Debug.Log("Player running");
                } //TODO: Přidat animaci pro crouch, skok a pád
                else
                {
                    player.AnimController.SetMovementState(MovingState.Walking);
                    Debug.Log("Player walking");
                }
            }
            else
            {
                player.AnimController.SetMovementState(MovingState.Idle);
                Debug.Log("Player idle");
            }
        }
        else if (isJumping)
        {
            player.AnimController.SetJumpingState(isJumping);
            
        }
        else
        {
            player.AnimController.SetMovementState(MovingState.Idle);
            Debug.Log("Player idling");
        }
    }
}
