using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class LoginUiController : MonoBehaviour
{

    [SerializeField]
    private TMP_InputField Username;
    [SerializeField]
    private TMP_InputField Password;
    [SerializeField]
    private TextMeshProUGUI ErrorText;
    [SerializeField]
    private Button LoginButton;
    [SerializeField]
    private Button SignButton;
    // Start is called before the first frame update
    [SerializeField]
    private GameObject LoginPage, 
        MainPage;
    private List<GameObject> _allPages;
    private void Start()
    {
        _allPages = new List<GameObject>() { LoginPage, MainPage };
        if (Managers.alreadyLogged)
        {
            ChangePage(MainPage);
            Managers.GameEvents.LoginEvent.Invoke();

        }
        else
        {
            ChangePage(LoginPage);
        }
    }   
    private void Awake()
    {
        Managers.GameEvents.LoginEvent.AddListener(()=> { ChangePage(MainPage); });
    }
    private void OnDestoy()
    {
        Managers.GameEvents.LoginEvent.RemoveListener(() => { ChangePage(MainPage); });

    }
    public async void OnLogin()
    {
        int response = await Managers.Metafab.AuthPlayer(Username.text, Password.text);

        //await Managers.Metafab.TransfertCurrensy("0xc33985f2f4F3DD9be94360b75f227Ab866CA39c5", 10);
        ErrorText.text  = response == 0 ? "Hi "+Username.text : "Credentials invalid, please retry." ;
        if (response == 1) return;
        Debug.Log("Submit");
        Managers.alreadyLogged = true;
        LoginButton.interactable = false;
        SignButton.interactable = false;
        Managers.GameEvents.LoginEvent.Invoke();
    }


    public async void OnSignIn()
    {
        int response = await Managers.Metafab.CreatePlayer(Username.text, Password.text);

        ErrorText.text = response == 0 ? "Hi " + Username.text : "Credentials invalid, please retry.";
        if (response == 1) return;
        Debug.Log("Submit");
        Managers.alreadyLogged = true;
        LoginButton.interactable = false;
        SignButton.interactable = false;
        await Managers.Metafab.MintforPlayer();
        Managers.GameEvents.LoginEvent.Invoke();
    }

    public void ChangePage(GameObject new_page)
    {
        foreach(GameObject page in _allPages)
        {
            page.SetActive(false);
        }
        Debug.Log("Change Page");
        new_page.SetActive(true);
    }
}
