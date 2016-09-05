using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Collections;

public class loginManager : MonoBehaviour
{
    // GameObject
    public GameObject NetworkManagerObj = null;
    private ClientAPI NetworkManager;

    public GameObject pData = null;
    private PlayerDataManager pDataManager = null;

    public InputField emailInput;
    public InputField pwInput;

    public Button LoginButton;

    public GameObject conIndicator;
    private Text conStateText;

    public GameObject PopUp;
    private GameObject InstPopup;
    public Canvas canvas;


    const int MAX_BYTE_SIZE = 1024;
    bool isPopuped = false;

    void Start()
    {
        pDataManager = pData.GetComponent<PlayerDataManager>();
        NetworkManager = NetworkManagerObj.GetComponent<ClientAPI>();

        pDataManager.SetConnectState(CONNECT_STATE.DISCONNECTED);
        pDataManager.SetPlayerState(SCENE_STATE.LOGIN_SCENE);

        InstPopup = Instantiate(PopUp) as GameObject;
        InstPopup.transform.SetParent(canvas.transform, false);
        InstPopup.SetActive(false);

        LoginButton.onClick.AddListener(UserLogin);
        conStateText = conIndicator.GetComponentInChildren<Text>();
    }

    void Update()
    {
        if(pDataManager.GetPlayerState() == SCENE_STATE.LOGIN_SCENE_FAILED
            && isPopuped == false)
        {
            //InstPopup = Instantiate(PopUp) as GameObject;
            //InstPopup.transform.SetParent(canvas.transform, false);
            InstPopup.SetActive(true);

            GameObject.Find("BtnNo").GetComponent<Button>().onClick.AddListener(() => { Destroy(InstPopup);});
            GameObject.Find("BtnYes").GetComponent<Button>().onClick.AddListener(() => {

                string email = emailInput.text;
                string pw = pwInput.text;

                byte[] packet = PacketInfo.MakeReqRegisterBody(email, pw);

                NetworkManager.SendData(packet);
                Destroy(InstPopup);
                //InstPopup.SetActive(false);
                isPopuped = false;
            });
            isPopuped = true;
        }

        if (pDataManager.GetConnectState() == CONNECT_STATE.DISCONNECTED)
            conStateText.text = "Disconnected";

        if (pDataManager.GetConnectState() == CONNECT_STATE.CONNECTING)
            conStateText.text = "Connecting...";

        if (pDataManager.GetConnectState() == CONNECT_STATE.CONNECTED)
            conStateText.text = "Connected";

        if (pDataManager.GetPlayerState() == SCENE_STATE.MY_SCENE)
            SceneManager.LoadScene("MyScene");
    }

    void UserLogin()
    {
        string email = emailInput.text;
        string pw = pwInput.text;

        var packetBytes = PacketInfo.MakeReqLoginPacket(email, pw);
        NetworkManager.SendData(packetBytes);
    }
}