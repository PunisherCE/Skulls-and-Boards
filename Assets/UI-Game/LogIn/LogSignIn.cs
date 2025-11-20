using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UIElements;

public class LogSignIn : MonoBehaviour
{

    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject connectionManager;

    VisualElement mainScreen;
    Button logInButton;
    Button mainButton;
    TextField usernameInput;
    TextField passwordInput;
    TextField confirmPassword;
    TextField emailInput;

    PopupWindow popup;

    bool isLogIn = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable()
    {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;

        mainScreen = root.Q<VisualElement>("mainScreen");
        logInButton = root.Q<Button>("logIn");
        mainButton = root.Q<Button>("LogSignButton");

        emailInput = root.Q<TextField>("LogInMail");
        usernameInput = root.Q<TextField>("LogInName");
        passwordInput = root.Q<TextField>("LogInPassword");
        confirmPassword = root.Q<TextField>("ConfirmPassword");

        popup = new PopupWindow();
        popup.cancelled += () => root.Remove(popup);

        logInButton.clicked += LogButtonClicked;
        mainButton.clicked += MainButtonClicked;
    }

    private async void MainButtonClicked()
    {
        string jsonPayload;
        if (usernameInput.text.Length < 3 || usernameInput.text.Length > 16)
        {
            ShowPopUp("Username must be between 3 and 16 characters long.");
            return;
        };
        if (passwordInput.text.Length < 5 || passwordInput.text.Length > 30)
        {
            ShowPopUp("Password must be between 5 and 30 characters long.");
            return;
        };
        if (passwordInput.text != confirmPassword.text && !isLogIn)
        { 
            ShowPopUp("Passwords do not match.");
            return;
        };
        if (!isLogIn)
        {
            if (emailInput.text.Length < 5 || emailInput.text.Length > 50 || !emailInput.text.Contains("@"))
            {
                ShowPopUp("Please enter a valid email address.");
                return;
            };
        }

        if (isLogIn)
        {
            var payload = new LoginPayload
            {
                user_name = usernameInput.text,
                password = passwordInput.text
            };
            jsonPayload = JsonUtility.ToJson(payload);
        }
        else
        {
            var payload = new RegisterPayload
            {
                user_name = usernameInput.text,
                password = passwordInput.text,
                email = emailInput.text
            };
            jsonPayload = JsonUtility.ToJson(payload);
        }

        byte[] bytes = System.Text.Encoding.UTF8.GetBytes(jsonPayload);
        UnityWebRequest request;
        if (isLogIn)
        {
            request = new UnityWebRequest("https://26e5e4c3060c.ngrok-free.app/login", "POST");
        }
        else
        {
            request = new UnityWebRequest("https://26e5e4c3060c.ngrok-free.app/register", "POST");
        }
        request.uploadHandler = new UploadHandlerRaw(bytes);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        await request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string json = request.downloadHandler.text;
            Debug.Log("Response: " + json);

            LoginResponse response = JsonUtility.FromJson<LoginResponse>(json);

            Debug.Log("Response: " + request.downloadHandler.text);
            // Handle successful response
            connectionManager.SetActive(true);
            mainMenu.SetActive(true);
            this.gameObject.SetActive(false);
        }
        else
        {
            ShowPopUp("User or password not correct");
            Debug.LogError("Error: " + request.error);
            // Handle error response
        }
    }

    private void LogButtonClicked()
    {
        isLogIn = !isLogIn;
        if (!isLogIn)
        {
            logInButton.text = "Log In";
            logInButton.style.backgroundColor = new StyleColor(new Color32(0x00, 0x05, 0x8E, 0x78));
            mainScreen.style.backgroundColor = new StyleColor(new Color32(0x00, 0x05, 0x8E, 0x78));
            var blueColor = new StyleColor(new Color32(0x00, 0x05, 0x8E, 0xFF)); // Fully opaque

            mainScreen.style.borderTopColor = blueColor;
            mainScreen.style.borderBottomColor = blueColor;
            mainScreen.style.borderLeftColor = blueColor;
            mainScreen.style.borderRightColor = blueColor;

            emailInput.style.display = DisplayStyle.Flex;
            confirmPassword.style.display = DisplayStyle.Flex;
            mainButton.text = "Sign In";
            emailInput.SetEnabled(true);
            confirmPassword.SetEnabled(true);
        } else 
        {
            logInButton.text = "Sign In";
            logInButton.style.backgroundColor = new StyleColor(new Color32(0x8E, 0x00, 0x00, 0x78));
            mainScreen.style.backgroundColor = new StyleColor(new Color32(0x8E, 0x00, 0x00, 0x78));
            var redColor = new StyleColor(new Color32(0x8E, 0x00, 0x00, 0xFF)); // Fully opaque

            mainScreen.style.borderTopColor = redColor;
            mainScreen.style.borderBottomColor = redColor;
            mainScreen.style.borderLeftColor = redColor;
            mainScreen.style.borderRightColor = redColor;

            emailInput.style.display = DisplayStyle.None;
            confirmPassword.style.display = DisplayStyle.None;
            mainButton.text = "Log In";
            emailInput.SetEnabled(false);
            confirmPassword.SetEnabled(false);
        }
    }

    private void ShowPopUp(string message)
    {
        //popup.SetTitle(title);
        //popup.SetMessage(message);
        popup.Prompt = message;
        GetComponent<UIDocument>().rootVisualElement.Add(popup);
    }
}

[System.Serializable]
public class LoginPayload
{
    public string user_name;
    public string password;
}

[System.Serializable]
public class RegisterPayload : LoginPayload
{
    public string email;
}

[System.Serializable]
public class LoginResponse
{
    public string id;
    public string user_name;
}

