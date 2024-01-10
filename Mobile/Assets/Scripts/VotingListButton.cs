using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class VotingListButton : MonoBehaviour
{
    [SerializeField]
    public TextMeshProUGUI myText;
    
    [SerializeField]
    public VotingListControl votingControl;

    private string myTextString;

    public void SetText(string textString){
        //Debug.Log(textString + "texting.");
        myTextString = textString;
        myText.text = textString;
    }

    public void OnClick(){
        votingControl.ButtonClicked(myTextString);
    }
}
