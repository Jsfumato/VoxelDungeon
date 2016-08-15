using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class MainScript : MonoBehaviour
{
    public Button LoginBtn = null;

	void Start ()
    {
        LoginBtn.onClick.AddListener(UserLogin);
    }
	
	void UserLogin()
    {
        SceneManager.LoadScene("MyScene");
    }
}
