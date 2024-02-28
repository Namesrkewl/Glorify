using TMPro;
using UnityEngine;

public class Client : MonoBehaviour {

    public static Client instance;
    public GameObject mainMenu;
    public TMP_InputField loginUsername;
    private int ID;

    private void Awake() {
        if (instance != null) {
            Destroy(gameObject);
        } else {
            Debug.Log("Set Client instance");
            instance = this;
            mainMenu.SetActive(true);
            LoadAPI();
        }
    }

    public void Login() {
        Debug.Log("Test");
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)) {
            if (loginUsername.text == "") {
                Debug.Log("Invalid Username!");
                return;
            } else {
                Debug.Log(loginUsername.text);
                API.instance.Login(loginUsername.text);
            }
        }
    }

    private void LoadAPI() {
        if (!UnityEngine.SceneManagement.SceneManager.GetSceneByName("API").isLoaded) {
            UnityEngine.SceneManagement.SceneManager.LoadScene("API", UnityEngine.SceneManagement.LoadSceneMode.Additive);
        }
    }

    public int GetID() {
        return ID;
    }

    public void SetID(int input) {
        ID = input;
    }
}
