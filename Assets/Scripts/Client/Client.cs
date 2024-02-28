using TMPro;
using UnityEngine;

public class Client : MonoBehaviour {

    public static Client instance;
    public GameObject mainMenu;
    public TMP_InputField username;
    public TMP_InputField password;
    private Key key;

    private void Awake() {
        if (instance != null) {
            Destroy(gameObject);
        } else {
            instance = this;
            mainMenu.SetActive(true);
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

    public Key GetKey() {
        return key;
    }

    public void SetKey(Key _key) {
        key = _key;
    }
}
