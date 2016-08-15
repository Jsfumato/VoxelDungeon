using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class LogoScript : MonoBehaviour
{
    public void GoToMainScene()
    {
        SceneManager.LoadScene("MainScene");
    }
}

