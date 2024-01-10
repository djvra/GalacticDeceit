using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using TMPro;
using UnityEngine.Events;
using UnityEngine.UI;

public class VotingListControl : MonoBehaviour
{
    public GameObject buttonTemplate;
    public GameObject listContent;
    public Client myClient;
    public TextMeshProUGUI VotedPlayer;
    private string selectedPlayer;
    public TextMeshProUGUI remainingTime;
    public float votingTime = 12;
    public float time = 0; 
    public UnityEvent<int> OnPlayerVoted = new UnityEvent<int>();

    // Start is called before the first frame update
    void Start()
    {   
       /* if (time < 1f)
        {
             if (listContent.GetComponentInChildren<VotingListButton>() != null)
            {
                VotingListButton[] BLB = listContent.GetComponentsInChildren<VotingListButton>();
                foreach (VotingListButton i in BLB)
                {
                    if (i.gameObject != buttonTemplate)
                    {
                        Destroy(i.gameObject);
                    }
                }
            }
            buttonTemplate.SetActive(false);
            foreach (var playerData in myClient.otherPlayersData)
            {
                Debug.Log($"Key: {playerData.Key}, Value: {playerData.Value}");
                GameObject button = Instantiate(buttonTemplate) as GameObject;
                button.SetActive(true);
                button.GetComponent<VotingListButton>().SetText(playerData.Key.ToString());
                button.transform.SetParent(listContent.transform);
            }

            GameObject skipButton = Instantiate(buttonTemplate) as GameObject;
            skipButton.SetActive(true);
            skipButton.GetComponent<VotingListButton>().SetText("skip");
            skipButton.transform.SetParent(listContent.transform);

            time = 15;
            remainingTime.text = ((int)time).ToString();
            VotedPlayer.text = "";
        }
        StartCoroutine(StartTimer());*/
    }

    void Update(){
        if (myClient.votingForm.GetComponentInParent<Canvas>().isActiveAndEnabled)
        {
            if (time < 1f)
            {
                if (listContent.GetComponentInChildren<VotingListButton>() != null)
                {
                    VotingListButton[] BLB = listContent.GetComponentsInChildren<VotingListButton>();
                    foreach (VotingListButton i in BLB)
                    {
                        if (i.gameObject != buttonTemplate)
                        {
                            Destroy(i.gameObject);
                        }
                    }
                }
                buttonTemplate.SetActive(false);
                foreach (var playerData in myClient.otherPlayersData)
                {
                    Debug.Log($"Key: {playerData.Key}, Value: {playerData.Value}");
                    GameObject button = Instantiate(buttonTemplate) as GameObject;
                    button.SetActive(true);
                    button.GetComponent<VotingListButton>().SetText(playerData.Value.name);
                    button.transform.SetParent(listContent.transform);
                    button.GetComponent<Image>().color = Utils.colors[playerData.Value.color];
                }

                GameObject skipButton = Instantiate(buttonTemplate) as GameObject;
                skipButton.SetActive(true);
                skipButton.GetComponent<VotingListButton>().SetText("skip");
                skipButton.transform.SetParent(listContent.transform);

                time = 15;
                remainingTime.text = ((int)time).ToString();
                VotedPlayer.text = "";
                StartCoroutine(StartTimer());

            }  
        }
    }

    IEnumerator StartTimer()
    {
        while (time > 0f)
        {
            // Update the Text component with the remaining time
            remainingTime.text = ((int)time).ToString();
            // Wait for one second
            yield return new WaitForSeconds(1f);

            // Decrease the remaining time
            time = Mathf.Max(0f, time - 1f);

            //Debug.Log("while");
        }
        
        Debug.Log("end while");

        if (VotedPlayer.text == "" || VotedPlayer.text == "skip")
        {
            Debug.Log("skip");
            OnPlayerVoted.Invoke(-1);
        }
        else
        {
            Debug.Log("voted");
            // get id from name
            foreach (var playerData in myClient.otherPlayersData)
            {
                if (playerData.Value.name == VotedPlayer.text)
                {
                    OnPlayerVoted.Invoke(playerData.Key);
                }
            }
            
        }

        myClient.votingForm.SetActive(false);
        Canvas canvas = myClient.votingForm.GetComponentInParent<Canvas>();
        canvas.gameObject.SetActive(false);
        
    }

    // Update is called once per frame
    public void ButtonClicked(string textString)
    {
        selectedPlayer = textString;
        Debug.Log("selected player:" + selectedPlayer);
        VotedPlayer.text = textString;
        Debug.Log("voted player:" + VotedPlayer.text);

        
    }
}
