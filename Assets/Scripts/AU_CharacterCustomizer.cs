using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;

public class AU_CharacterCustomizer : NetworkBehaviour
{
    [SerializeField] Color[] allColors;

    public void SetColor(int colorIndex)
    {
        AU_PlayerController.localPlayer.SetColor(allColors[colorIndex]);
    }

    public void NextScene(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }

    public void PreviousScene(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }
}