using UnityEngine;
using UnityEngine.Android;
using System;
using UnityEngine.UI;

public class BluetoothPlayerMovement : MonoBehaviour
{
    public float moveSpeed; // Speed of movement

    public Rigidbody rb;

    public string joystickData;

    // Predefined device name
    //private string deviceName; // = "HC-05";
    public static bool IsConnected;
    public string sampleData;
    Vector3 force;
    private float movementSensitivity = 5f;
    private int oldX;
    private int oldY;
    private GameObject playerGO;
    private AU_PlayerController playerController;
    private Animator playerAnimator;
    Vector3 movementVector;
    public string deviceName = null;

    void Start()
    {
    #if UNITY_2020_2_OR_NEWER
        #if UNITY_ANDROID
                if (!Permission.HasUserAuthorizedPermission(Permission.CoarseLocation)
                  || !Permission.HasUserAuthorizedPermission(Permission.FineLocation)
                  || !Permission.HasUserAuthorizedPermission("android.permission.BLUETOOTH_SCAN")
                  || !Permission.HasUserAuthorizedPermission("android.permission.BLUETOOTH_ADVERTISE")
                  || !Permission.HasUserAuthorizedPermission("android.permission.BLUETOOTH_CONNECT"))
                    Permission.RequestUserPermissions(new string[] {
                                Permission.CoarseLocation,
                                    Permission.FineLocation,
                                    "android.permission.BLUETOOTH_SCAN",
                                    "android.permission.BLUETOOTH_ADVERTISE",
                                     "android.permission.BLUETOOTH_CONNECT"
                            });
        #endif
    #endif

        rb = GetComponent<Rigidbody>();
        playerGO = rb.gameObject;
        playerController = playerGO.GetComponent<AU_PlayerController>();
        playerAnimator = playerGO.GetComponent<Animator>();

        IsConnected = false;
        

        //IsConnected = BluetoothService.StartBluetoothConnection(deviceName);

        oldX = 511;
        oldY = 511;


    }

    /*public void Connect(string deviceName)
    {
        if (deviceName != null && deviceName != "")
        {
            deviceName = deviceName.Trim();
            Debug.Log("deviceName ->" + deviceName.ToString());

            BluetoothService.CreateBluetoothObject();
            IsConnected = BluetoothService.StartBluetoothConnection(deviceName);
            Debug.Log("Connect IsConnected ->" + IsConnected.ToString());

            
        }
    }*/

    void FixedUpdate()
    {
        if (deviceName != null && deviceName != "")
        {
            if (!IsConnected) {
                deviceName = deviceName.Trim();
                Debug.Log("deviceName ->" + deviceName.ToString());

                BluetoothService.CreateBluetoothObject();
                IsConnected = BluetoothService.StartBluetoothConnection(deviceName);
                Debug.Log("FixedUpdate IsConnected ->" + IsConnected.ToString());
            }

            if (!playerController.isMoving /*&& ! VotingListControl.reportInProgress*/)
            {

                //joystickData = "";
                if (IsConnected)
                {
                    try
                    {
                        string datain = BluetoothService.ReadFromBluetooth();
                        if (datain.Length > 1)
                        {
                            joystickData = datain;
                            //print(joystickData);
                        }

                    }
                    catch (Exception e)
                    {
                        BluetoothService.Toast("Error in connection");
                    }
                }


                //sampleData = "0?1?1?0?0?1?1?1?1?88?88?88?";

                int[] tempParsedInputs = ParseJoystickData(joystickData);

                Debug.Log("joystickdata ->" + joystickData.ToString());

                if(tempParsedInputs.Length < 9)
                {
                    Debug.Log("Error on came input!");
                }

                int analogX = tempParsedInputs[7];
                int analogY = tempParsedInputs[8];
                bool killOrDoTaskButton = tempParsedInputs[2] == 1 ? false : true;
                bool reportButton = tempParsedInputs[1] == 1 ? false : true;

                if (killOrDoTaskButton)
                {
                    playerController.KillOrDoTask();
                }
                
                if (reportButton)
                {
                    playerController.ReportBody();
                }
                //Debug.Log("analogX ->" + analogX.ToString());
                //Debug.Log("analogY ->" + analogY.ToString());

                /*if (analogX == 512 && analogY == 512)
                {
                    analogX = oldX;
                    analogY = oldY;

                    //Debug.Log("if 512 analogX ->" + analogX.ToString());
                    //Debug.Log("if 512 analogY ->" + analogY.ToString());
                }
                else
                {
                    oldX = analogX;
                    oldY = analogY;
                }*/

                force = Vector3.zero;

                if (analogX < 200)
                {
                    force += Vector3.left;
                }
                if (analogX > 500)
                {
                    force += Vector3.right;
                }
                if (analogY < 200)
                {
                    force += Vector3.down;
                }
                if (analogY > 500)
                {
                    force += Vector3.up;
                }

                if (force.magnitude == 0)
                {
                    rb.velocity = Vector3.zero;
                }
                else
                {
                    movementVector = force * movementSensitivity;
                    rb.velocity = movementVector;
                    //Debug.Log("rb.velocity.magnitude ->" + rb.velocity.magnitude.ToString());
                }
            }


        }


    }

    void Update()
    {
        if (!playerController.isMoving)
        {
            // Update the animation
            if (force.magnitude > 0) 
            {
                //Debug.Log("force.magnitude ->" + force.magnitude.ToString());
                playerAnimator.SetFloat("Speed", movementVector.magnitude);

                // Update the scale based on the direction of movement
                if (movementVector.x != 0)
                {
                    Transform playerAvatar = playerGO.transform;
                    playerAvatar.localScale = new Vector3(Mathf.Sign(force.x), 1, 1);

                    Transform playerName = playerAvatar.GetChild(3);
                    playerName.localScale = new Vector3(Mathf.Sign(force.x), 1, 1);
                }
            }
        }
    }

    int[] ParseJoystickData(string data)
    {

        int[] integers = new int[9];

        //default params
        integers[0] = 1;
        integers[1] = 1;
        integers[2] = 1;
        integers[3] = 1;
        integers[4] = 1;
        integers[5] = 1;
        integers[6] = 1;
        integers[7] = 512;
        integers[8] = 512;

        if (data.Length > 3)
        {
            string[] parts = data.Split(new char[] { '?' }, System.StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length == 9)
            {
                for (int i = 0; i < 9; i++)
                {
                    if (int.TryParse(parts[i], out int value))
                    {
                        integers[i] = value;
                    }
                    else
                    {
                        Debug.LogWarning("Failed to parse: " + parts[i]);
                        // Handle parsing failure if needed
                    }
                }
            }

            // Use or manipulate the parsed floats here
            //for (int i = 0; i < 9; i++)
            /*for (int i = 1; i < 3; i++)
            {
                Debug.Log(i + ": " + integers[i]);
            }*/
        }
        return integers;
    }
}
