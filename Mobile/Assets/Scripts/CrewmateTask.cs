using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrewmateTask : MonoBehaviour
{
    // Start is called before the first frame update
    private bool isNearObject = false;
    [SerializeField] public int taskID;

    /*private void Update()
    {
        if (isNearObject && Input.GetKeyDown(KeyCode.Space)) // Replace KeyCode.Space with your specific key
        {
            Debug.Log("Task completed");
        }
    }*/

    public bool TryCompleteTask()
    {
        if (isNearObject)
        {
            Debug.Log("Task completed with ID: " + taskID);
            return true;
        } else {
            Debug.Log("Task unsuccessfull ID: " + taskID);
            return false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player")) 
        {
            isNearObject = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player")) 
        {
            isNearObject = false;
        }
    }
}
