using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class PlayButtonHandler : MonoBehaviour
{
    public void LoadLevel1Scene()
    {
        SceneManager.LoadScene("Level1"); // Make sure the scene name matches exactly
    }
}
