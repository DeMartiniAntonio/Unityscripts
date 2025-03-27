using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayGame : MonoBehaviour
{
    [Header("Play")]
    public string _ImStriker;

    public void OpenGame()
    {
        SceneManager.LoadScene(_ImStriker);
    }

    
}
