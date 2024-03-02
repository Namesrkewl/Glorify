using TMPro;
using UnityEngine;

public class Client : MonoBehaviour {

    public static Client instance;
    public GameObject mainMenu;
    public GameObject chatManager;
    public TMP_InputField username;
    public TMP_InputField password;

    private void Awake() {
        if (instance != null) {
            Destroy(gameObject);
        } else {
            instance = this;
            mainMenu.SetActive(true);
            chatManager.SetActive(false);
            LoadAPI();
        }
    }

    public void Login() {
        if (username.text == "") {
            Debug.Log("Invalid Username!");
            return;
        } else if (password.text == "") {
            Debug.Log("Invalid Password!");
            return;
        } else {
            Debug.Log(username.text);
            API.instance.Login(username.text, password.text);
        }
    }

    private void LoadAPI() {
        if (!UnityEngine.SceneManagement.SceneManager.GetSceneByName("API").isLoaded) {
            UnityEngine.SceneManagement.SceneManager.LoadScene("API", UnityEngine.SceneManagement.LoadSceneMode.Additive);
        }
    }
}
