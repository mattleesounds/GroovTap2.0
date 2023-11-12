using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class PlayButtonHandler : MonoBehaviour
{
    // This method will be called when the button is clicked
    public void LoadLevel1Scene()
    {
        SceneManager.LoadScene("Level1"); // Make sure the scene name matches exactly
    }
}
