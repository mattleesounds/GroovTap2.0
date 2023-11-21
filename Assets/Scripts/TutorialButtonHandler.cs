using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class TutorialButtonHandler : MonoBehaviour
{
    // This method will be called when the button is clicked
    public void TutorialScene()
    {
        SceneManager.LoadScene("Tutorial"); // Make sure the scene name matches exactly
    }
}