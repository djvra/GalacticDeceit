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

    public bool isDead;

    [SerializeField] GameObject bodyPrefab;
    public static List<Transform> allBodies;

    List<Transform> bodiesFound;
    [SerializeField] InputAction REPORT;
    [SerializeField] LayerMask ignoreForBody;

    public float killCooldownTime = 5f;
    private float killCooldownTimer;
    private bool isKillOnCooldown;
    public int id;
    public UnityEvent<int> OnPlayerKilled = new UnityEvent<int>();
    public UnityEvent<int> OnPlayerReported = new UnityEvent<int>();
    public UnityEvent<int> OnPlayerDoneTask = new UnityEvent<int>();
    public bool isMoving;
    public string deviceName = null;
    private CrewmateTask nearTask; // The task the player is currently near
    public bool isLocalPlayer = false;
    public List<int> taskIDs;
    private List<GameObject> allTasks;
    public Dictionary<int, GameObject> alltasksfromscene;
    [SerializeField] public GameObject taskPointerPrefab;
    private Dictionary<int, GameObject> taskPointers = new Dictionary<int, GameObject>();
    public bool reportingInProgress = false;
    private int reportId;
    [SerializeField] private TMPro.TextMeshProUGUI nameText;
    private AU_Body nearBody;

    private void Awake()
    {
        KILL.performed += KillTarget;
        REPORT.performed += ReportBody;
    }

    private void OnEnable()
    {
        WASD.Enable();
        KILL.Enable();
        REPORT.Enable();
    }

    private void OnDisable()
    {
        WASD.Disable();
        KILL.Disable();
        REPORT.Disable();
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

        allBodies = new List<Transform>();
        bodiesFound = new List<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!hasControl)
            return;
        
        if ( ! reportingInProgress)
        {
            movementInput = WASD.ReadValue<Vector2>();
            myAnim.SetFloat("Speed", movementInput.magnitude);
            if (movementInput.x != 0)
            {
                myAvatar.localScale = new Vector2(Mathf.Sign(movementInput.x), 1);
            }
        }

        if (isKillOnCooldown)
        {
            killCooldownTimer -= Time.deltaTime;
            if (killCooldownTimer <= 0)
            {
                isKillOnCooldown = false;
            }
        }

        if(allBodies.Count > 0)
        {
            BodySearch();
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

    public void SetName(string newName)
    {
        nameText.text = newName;
    }

    public void setDeviceName(string newDevice)
    {
        deviceName = newDevice;
        GameObject playerGO = this.gameObject;
        BluetoothPlayerMovement playerMovement = playerGO.GetComponent<BluetoothPlayerMovement>();
        playerMovement.deviceName = newDevice;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isLocalPlayer)
        {
            return; 
        }

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

        else if (other.CompareTag("Interactable")) // CrewmateTask
        {
            CrewmateTask task = other.GetComponent<CrewmateTask>();
            if (task != null)
            {
                // The player entered the area of a task
                nearTask = task;
                //Debug.Log("Entered task area");
            }
        }

        /*else if (other.CompareTag("Body"))
        {
            Transform body = other.transform;
            if (body != null)
            {
                // The player entered the area of a task
                nearBody = body.GetComponent<AU_Body>();
                //Debug.Log("Entered task area");
            }
        }*/
        
    }

    private void OnTriggerExit(Collider other)
    {
        if (!isLocalPlayer)
        {
            return; 
        }

        if (other.tag == "Player")
        {
            AU_PlayerController tempTarget = other.GetComponent<AU_PlayerController>();
            if (targets.Contains(tempTarget))
            {
                targets.Remove(tempTarget);
            }
        }

        else if (other.CompareTag("Interactable")) // CrewmateTask
        {
            CrewmateTask task = other.GetComponent<CrewmateTask>();
            if (task != null && nearTask == task)
            {
                // The player left the area of the current task
                nearTask = null;
                //Debug.Log("Left task area");
            }
        }

        /*else if (other.CompareTag("Body"))
        {
            Transform body = other.transform;
            if (body != null && nearBody == body)
            {
                // The player left the area of the current task
                nearBody = null;
                //Debug.Log("Left task area");
            }
        }*/
    }

    void KillTarget(InputAction.CallbackContext context) {
        KillOrDoTask(context);
    }

    void KillOrDoTask(InputAction.CallbackContext context) 
    { // if player is a imposter kill, if not do task


        if(context.phase == InputActionPhase.Performed) 
        {
            if (isImposter)
            {
                if (targets.Count > 0)
                {
                    if (isKillOnCooldown) 
                    {
                        return;
                    }

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

            else // do the tasks
            {
                if (nearTask != null)
                {   
                    if (taskIDs.Contains(nearTask.taskID))
                    {
                        bool success = nearTask.TryCompleteTask();
                        if (success) 
                        {
                            OnPlayerDoneTask.Invoke(id);
                            taskIDs.Remove(nearTask.taskID);
                            if (taskPointers.TryGetValue(nearTask.taskID, out GameObject toBeDestroyed))
                            {
                                taskPointers.Remove(nearTask.taskID);
                                Destroy(toBeDestroyed);
                            }
                        }
                    }
                }
            }
        }
        
    }

    public void KillOrDoTask() // called from BluetoothPlayerMovement
    { // if player is a imposter kill, if not do task

        if (isImposter && targets.Count > 0)
        {
            if (isKillOnCooldown) 
            {
                return;
            }

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

        else // do the tasks
        {
            if (nearTask != null)
            {   
                if (taskIDs.Contains(nearTask.taskID))
                {
                    bool success = nearTask.TryCompleteTask();
                    if (success) 
                    {
                        OnPlayerDoneTask.Invoke(id);
                        taskIDs.Remove(nearTask.taskID);
                        if (taskPointers.TryGetValue(nearTask.taskID, out GameObject toBeDestroyed))
                        {
                            taskPointers.Remove(nearTask.taskID);
                            Destroy(toBeDestroyed);
                        }
                    }
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
        //tempBody.bodyId = deadPlayerId;


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


    void BodySearch()
    {
        foreach(var body in allBodies)
        {
            if (body == null)
                continue;
                
            RaycastHit hit;
            Ray ray = new Ray(transform.position, body.position - transform.position);
            Debug.DrawRay(transform.position, body.position - transform.position, Color.cyan);
            if(Physics.Raycast(ray, out hit, 1000f, ~ignoreForBody))
            {
                
                if (hit.transform == body)
                {
                    //Debug.Log(hit.transform.name);
                    //Debug.Log(bodiesFound.Count);
                    if (bodiesFound.Contains(body))
                        return;
                    bodiesFound.Add(body.transform);
                }
                else
                {
                    
                    bodiesFound.Remove(body);
                }
            }
        }
    }

    private void ReportBody(InputAction.CallbackContext context)
    {
        if (bodiesFound == null)
            return;
        if (bodiesFound.Count == 0)
            return;
        if (isDead) // imposter da report edebilir
            return;    

        if(context.phase == InputActionPhase.Performed){
            //Order the list by the distance to the killer
            Transform tempBody = bodiesFound[bodiesFound.Count - 1];
            allBodies.Remove(tempBody);
            bodiesFound.Remove(tempBody);
            tempBody.GetComponent<AU_Body>().Report();
            Debug.Log("Reported player ");
            reportId++;
            OnPlayerReported.Invoke(reportId); 
        }    
    }

    public void ReportBody()
    {
        Debug.LogWarning("ReportBody() called");
        
        if (bodiesFound == null)
            return;
        if (bodiesFound.Count == 0)
            return;
        if (isDead) // imposter da report edebilir
            return;    

            //Order the list by the distance to the killer
            Transform tempBody = bodiesFound[bodiesFound.Count - 1];
            allBodies.Remove(tempBody);
            bodiesFound.Remove(tempBody);
            tempBody.GetComponent<AU_Body>().Report();
            Debug.Log("Reported player ");
            reportId++;
            OnPlayerReported.Invoke(reportId);
    }

    public void RemoveBodies()
    {
        Debug.Log("RemoveBodies() called");
        // print size of all bodies
        Debug.Log("allBodies size: " + allBodies.Count);
        // print size of bodiesFound
        Debug.Log("bodiesFound size: " + bodiesFound.Count);

        // Iterate over the bodiesFound list
        foreach (var body in bodiesFound)
        {
            Debug.Log("Removing body");
            // Remove the body from the allBodies list
            if (allBodies.Contains(body))
            {
                allBodies.Remove(body);
            }

            // Destroy the body GameObject
            if (body != null)
            {
                Debug.Log("Destroying body");
                Destroy(body.gameObject);
            }
        }

        // Clear the bodiesFound list
        bodiesFound.Clear();

        // Destroy all bodies in the scene
        foreach (var bodyInScene in FindObjectsOfType<AU_Body>())
        {
            Destroy(bodyInScene.gameObject);
        }
    }



    public void initializeTaskPointers()
    {
        // get all tasks
        if ( ! isImposter)
        {
            allTasks = new List<GameObject>();
            foreach (var task in FindObjectsOfType<CrewmateTask>())
            {
                if ( taskIDs.Contains(task.taskID) )
                {
                    allTasks.Add(task.gameObject);
                }
            }

            Debug.Log("Found " + allTasks.Count + " tasks");

            foreach (var task in allTasks)
            {
                GameObject pointer = Instantiate(taskPointerPrefab);
                Window_QuestPointer pointerScript = pointer.GetComponent<Window_QuestPointer>();
                Camera playerCamera = this.gameObject.GetComponentInChildren<Camera>(); 
                Transform taskTransform = task.transform;
                pointerScript.Initialize(playerCamera, taskTransform.position);
                taskPointers.Add(task.GetComponent<CrewmateTask>().taskID, pointer);
            }
        }
        
    }

    

}