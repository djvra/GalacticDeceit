using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class AU_PlayerController : MonoBehaviour
{
    [SerializeField] bool hasControl;
    public static AU_PlayerController localPlayer;
    

    //Components
    Rigidbody myRB;
    Animator myAnim;
    Transform myAvatar;
    //Player movement
    [SerializeField] InputAction WASD;
    Vector2 movementInput;
    [SerializeField] float movementSpeed;
    //Player Color
    static Color myColor;
    Transform myPart;
    SpriteRenderer myAvatarSprite;

    // Player Part
    SpriteRenderer myPartSprite;

    //Role
    [SerializeField] public bool isImposter;
    [SerializeField] InputAction KILL;
    float killInput;

    List<AU_PlayerController> targets;
    [SerializeField] Collider myCollider;

    bool isDead;

    [SerializeField] GameObject bodyPrefab;

    public float killCooldownTime = 15f;
    private float killCooldownTimer;
    private bool isKillOnCooldown;
    public int id;
    public UnityEvent<int> OnPlayerKilled = new UnityEvent<int>();
    public bool isMoving;

    private void Awake()
    {
        KILL.performed += KillTarget;
        
    }

    private void OnEnable()
    {
        WASD.Enable();
        KILL.Enable();
    }

    private void OnDisable()
    {
        WASD.Disable();
        KILL.Disable();
    }


    // Start is called before the first frame update
    void Start()
    {
        if (hasControl)
        {
            localPlayer = this;
        }
        targets = new List<AU_PlayerController>();
        myRB = GetComponent<Rigidbody>();
        myAnim = GetComponent<Animator>();
        myAvatar = transform.GetChild(0);
        myAvatarSprite = myAvatar.GetComponent<SpriteRenderer>();

        if (transform.childCount > 1)
        {
            myPart = transform.GetChild(1);
            // Check if there is a SpriteRenderer component on the second child
            myPartSprite = myPart.GetComponent<SpriteRenderer>();
        }

        if (!hasControl)
            return;

        /*if (myColor == Color.clear)
            myColor = Color.white;
        myAvatarSprite.color = myColor;*/
    }

    // Update is called once per frame
    void Update()
    {
        if (!hasControl)
            return;

        movementInput = WASD.ReadValue<Vector2>();
        myAnim.SetFloat("Speed", movementInput.magnitude);
        if (movementInput.x != 0)
        {
            myAvatar.localScale = new Vector2(Mathf.Sign(movementInput.x), 1);
        }

        if (isKillOnCooldown)
        {
            killCooldownTimer -= Time.deltaTime;
            if (killCooldownTimer <= 0)
            {
                isKillOnCooldown = false;
            }
        }

    }

    private void FixedUpdate()
    {
        myRB.velocity = movementInput * movementSpeed;
        isMoving = movementInput.magnitude != 0 ? true : false;
    }

    public void SetColor(Color newColor)
    {
        myColor = newColor;
        if (myAvatarSprite != null)
        {
            myAvatarSprite.color = myColor;
        }
    }

    public void SetRole(bool newRole)
    {
        isImposter = newRole;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            AU_PlayerController tempTarget = other.GetComponent<AU_PlayerController>();
            if (isImposter)
            {
                if (tempTarget.isImposter)
                    return;
                else
                {
                    targets.Add(tempTarget);
                    
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            AU_PlayerController tempTarget = other.GetComponent<AU_PlayerController>();
            if (targets.Contains(tempTarget))
            {
                    targets.Remove(tempTarget);
            }
        }
    }

    void KillTarget(InputAction.CallbackContext context) {
        if (isKillOnCooldown) {
            return;
        }

        if(context.phase == InputActionPhase.Performed && targets.Count > 0) {
            //Order the list by the distance to the killer
            targets.Sort((entry1, entry2)=> Vector3.Distance(entry1.transform.position, transform.position).CompareTo(Vector3.Distance(entry2.transform.position, transform.position)));
            //Loop through the list and kill the nearest person who is alive.
            for(int i = 0; i < targets.Count; i++) {
                AU_PlayerController target = targets[i];
                if(!target.isDead) {
                    transform.position = target.transform.position;
                    target.Die();
                    isKillOnCooldown = true;
                    killCooldownTimer = killCooldownTime;
                    OnPlayerKilled.Invoke(target.id);
                    break;
                }
            }
        }
    }

    public void KillTarget(bool killButton)
    {
        if (isKillOnCooldown) {
            return;
        }

        if(killButton && targets.Count > 0) {
            //Order the list by the distance to the killer
            targets.Sort((entry1, entry2)=> Vector3.Distance(entry1.transform.position, transform.position).CompareTo(Vector3.Distance(entry2.transform.position, transform.position)));
            //Loop through the list and kill the nearest person who is alive.
            for(int i = 0; i < targets.Count; i++) {
                AU_PlayerController target = targets[i];
                if(!target.isDead) {
                    transform.position = target.transform.position;
                    target.Die();
                    isKillOnCooldown = true;
                    killCooldownTimer = killCooldownTime;
                    OnPlayerKilled.Invoke(target.id);
                    break;
                }
            }
        }
    }

    public void Die()
    {
        if (isDead)
            return;

        isDead = true;

        myAnim.SetBool("IsDead", isDead);
        myCollider.enabled = false;

        AU_Body tempBody = Instantiate(bodyPrefab, transform.position, transform.rotation).GetComponent<AU_Body>();
        tempBody.SetColor(myAvatarSprite.color);

        if (myPartSprite != null) {
            myPartSprite.enabled = false;
        }

        //myPartSprite.color = Color.clear;
        
    }

    public int GetKillCooldownTimer() {
        if (isImposter) {
            return (int)killCooldownTimer;
        }

        return -1;
    }

    

}