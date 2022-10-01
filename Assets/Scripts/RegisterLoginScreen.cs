using Firebase;
using Firebase.Auth;
using Firebase.Extensions;
using Firebase.Firestore;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RegisterLoginScreen : MonoBehaviour
{


    public DependencyStatus dependencyStatus;
    public FirebaseAuth firebaseAuth;
    public static FirebaseUser firebaseUser;
    public static FirebaseFirestore databaseReference;


    //ADDITIONAL USER DATA
    public static int currentLvl;
    public static string username;
    public static float time;
    public string dataPath;
    public bool userEnabled;


    private int defaultMin = 10;
    private int defaultSec = 30;
    //TO COMPENSATE FOR Time.realtimeSinceStartup
    public static float timeSpentDuringLogin = 0;
    public GameObject LoadingPanel;
    public GameObject quitPanel;
    public GameObject verifyEmailPanel;
    public GameObject userEnablePanel;


    [Header("Login")]
    public GameObject LoginPanel;
    public TMP_InputField loginEmail;
    public TMP_InputField loginPassword;
    public TMP_Text warningLoginText;

    [Header("Register")]
    public GameObject RegisterPanel;
    public TMP_InputField registerUsername;
    public TMP_InputField registerEmail;
    public TMP_InputField registerPassword;
    public TMP_Text warningRegisterText;
    public TMP_Text verifEmailText;

    [Header("Admin")]
    public TMP_InputField adminPassword;
    public GameObject adminLoginPanel;
    public GameObject AdminPanel;
    public GameObject usersPanel;
    public GameObject userElementPrefab;
    public TMP_Text adminLoginText;



    //open register window
    public void OpenRegisterPanel()
    {
        RegisterPanel.SetActive(true);
        LoginPanel.SetActive(false);
        loginEmail.text = "";
        loginPassword.text = "";
        warningLoginText.text = "";
    }

    //open login window
    public void OpenLoginPanel()
    {
        RegisterPanel.SetActive(false);
        LoginPanel.SetActive(true);
        //reset the registration panel
        registerUsername.text = "";
        registerEmail.text = "";
        registerPassword.text = "";
        warningRegisterText.text = "";
    }

    public void ExitGame()
    {
        Application.Quit();
    }
    public void BackPanel()
    {
        quitPanel.SetActive(true);
    }
    public void ContinueGame()
    {
        quitPanel.SetActive(false);
    }
    public void CloseRegisteredSuccessfullyPanel()
    {
        verifyEmailPanel.SetActive(false);
        //reset the registration panel
        registerUsername.text = "";
        registerEmail.text = "";
        registerPassword.text = "";
        warningRegisterText.text = "";

        RegisterPanel.SetActive(false);
        LoginPanel.SetActive(true);

    }



    // Start is called before the first frame update
    void Start()
    {
        RegisterPanel.SetActive(false);
        AdminPanel.SetActive(false);
        LoginPanel.SetActive(true);
        userEnablePanel.SetActive(false);
        warningLoginText.text = "";
        warningRegisterText.text = "";
        verifEmailText.text = "";
        LoadingPanel.SetActive(false);
        verifyEmailPanel.SetActive(false);
        quitPanel.SetActive(false);
        adminLoginPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void Awake()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
                InitializeFirebase();
            else
            {
                Debug.LogError("Could not find all dependencies:" + dependencyStatus);
            }
        }
        );
    }
    public void InitializeFirebase()
    {

        firebaseAuth = FirebaseAuth.DefaultInstance;
        Debug.Log("setted up firebase auth");
        databaseReference = FirebaseFirestore.DefaultInstance;
        Debug.Log("setted up database auth");
    }

    void OpenLoadingPanel()
    {
        LoadingPanel.SetActive(true);

    }
    public void OpenUserEnablePanel()
    {
        userEnablePanel.SetActive(true);
    }
    public void CloseUserEnablePanel()
    {
        userEnablePanel.SetActive(false);
        loginEmail.text = "";
        loginPassword.text = "";
        warningLoginText.text = "";
    }

    #region LOGIN
    public void Login()
    {
        StartCoroutine(LoginCouroutine(loginEmail.text, loginPassword.text));
    }
    IEnumerator LoginCouroutine(string _loginEmail, string _loginPassword)
    {
        var loginStatus = firebaseAuth.SignInWithEmailAndPasswordAsync(_loginEmail, _loginPassword);
        yield return new WaitUntil(predicate: () => loginStatus.IsCompleted);

        if (loginStatus.Exception != null)
        {
            //If there are errors handle them
            Debug.LogWarning(message: $"Failed to register task with {loginStatus.Exception}");
            FirebaseException firebaseEx = loginStatus.Exception.GetBaseException() as FirebaseException;
            AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

            string message = "Login Failed!";
            switch (errorCode)
            {
                case AuthError.MissingEmail:
                    message = "Missing Email";
                    break;
                case AuthError.MissingPassword:
                    message = "Missing Password";
                    break;
                case AuthError.WrongPassword:
                    message = "Wrong Password";
                    break;
                case AuthError.InvalidEmail:
                    message = "Invalid Email";
                    break;
                case AuthError.UserNotFound:
                    message = "Account does not exist";
                    break;
            }
            warningLoginText.text = message;
            Debug.Log(message);
        }
        else
        {
            firebaseUser = loginStatus.Result;
            GetUserDataOnLogin(firebaseUser);
            yield return new WaitForSeconds(2f);
            if(!firebaseUser.IsEmailVerified)
            {
                warningLoginText.text = "Email Not verified";
            }
            else if (userEnabled==false)
            {
                OpenUserEnablePanel();
            }

            else if (firebaseUser.IsEmailVerified && userEnabled)
            {
                

                //User is now logged in
                //Now get the result
                Debug.LogFormat("User signed in successfully: {0} ({1})", firebaseUser.DisplayName, firebaseUser.Email);
                warningLoginText.text = "User signed in successfully";


                //dbManager.StartCoroutine(LoadCurrentLevelData());
                OpenLoadingPanel();
                yield return new WaitForSeconds(4f);//to wait for data to be fetched
                Debug.Log("Logged In");
                //pass the min and sec variables as obtained form the game timer set by admin


                // timeSpentDuringLogin = Time.timeSinceLevelLoad;//to reduce the time in login register scene from playtime
                
                SceneManager.LoadScene("MainMenu");//load main menu if login is successful
            }



        }
    }
    #endregion






    #region REGISTER
    public void Register()
    {
        StartCoroutine(RegisterCouroutine(registerUsername.text, registerEmail.text, registerPassword.text));
    }
    IEnumerator RegisterCouroutine(string _registerUsername, string _registerEmail, string _registerPassword)
    {
        if (_registerUsername == "")
        {
            //If the username field is blank show a warning
            warningRegisterText.text = "Missing Username";
        }
        else
        {
            //Call the Firebase auth signin function passing the email and password
            var RegisterTask = firebaseAuth.CreateUserWithEmailAndPasswordAsync(_registerEmail, _registerPassword);
            //Wait until the task completes
            yield return new WaitUntil(predicate: () => RegisterTask.IsCompleted);

            if (RegisterTask.Exception != null)
            {
                //If there are errors handle them
                Debug.LogWarning(message: $"Failed to register task with {RegisterTask.Exception}");
                FirebaseException firebaseEx = RegisterTask.Exception.GetBaseException() as FirebaseException;
                AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

                string message = "Register Failed!";
                switch (errorCode)
                {
                    case AuthError.MissingEmail:
                        message = "Missing Email";
                        break;
                    case AuthError.MissingPassword:
                        message = "Missing Password";
                        break;
                    case AuthError.WeakPassword:
                        message = "Weak Password";
                        break;
                    case AuthError.EmailAlreadyInUse:
                        message = "Email Already In Use";
                        break;
                }
                warningRegisterText.text = message;
            }
            else
            {
                //User has now been created
                //Now get the result
                firebaseUser = RegisterTask.Result;

                if (firebaseUser != null)
                {
                    //Create a user profile and set the username
                    UserProfile profile = new UserProfile { DisplayName = _registerUsername };

                    //Call the Firebase auth update user profile function passing the profile with the username
                    var ProfileTask = firebaseUser.UpdateUserProfileAsync(profile);
                    //Wait until the task completes
                    yield return new WaitUntil(predicate: () => ProfileTask.IsCompleted);

                    if (ProfileTask.Exception != null)
                    {
                        //If there are errors handle them
                        Debug.LogWarning(message: $"Failed to register task with {ProfileTask.Exception}");
                        firebaseUser.DeleteAsync();
                        FirebaseException firebaseEx = ProfileTask.Exception.GetBaseException() as FirebaseException;
                        AuthError errorCode = (AuthError)firebaseEx.ErrorCode;
                        warningRegisterText.text = $"Failed to register task with {ProfileTask.Exception}";
                    }
                    else
                    {

                        StartCoroutine(UpdateUsernameAuth(_registerUsername));
                        yield return new WaitForSeconds(3f);//to wait for dat to be fetched...
                        SetNewUserData(_registerUsername, 1, firebaseUser);//new user starts from lvl 1

                        yield return new WaitForSeconds(3f);//to wait for data to be fetched...
                        StartCoroutine(SendVerificationEmail());
                    }
                }
            }
        }

    }
    #endregion


    private IEnumerator UpdateUsernameAuth(string _username)
    {
        //Create a user profile and set the username
        UserProfile profile = new UserProfile { DisplayName = _username };

        //Call the Firebase auth update user profile function passing the profile with the username
        var ProfileTask = firebaseUser.UpdateUserProfileAsync(profile);
        //Wait until the task completes
        yield return new WaitUntil(predicate: () => ProfileTask.IsCompleted);

        if (ProfileTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {ProfileTask.Exception}");
            warningRegisterText.text = "UpdateusernameAuth failed";
        }
        else
        {
            //Auth username is now updated
            Debug.Log("Profile created");
            warningRegisterText.text = "Created Profile";

        }
    }
    public void SetNewUserData(string _userName, int _currentLevel, FirebaseUser _userId)
    {
        dataPath = $"users/{_userId.UserId}";
        var characterData = new CharacterData
        {
            userName = _userName,
            currentLevel = _currentLevel,
            min = defaultMin,
            sec = defaultSec,
            enabled = false
        };
        databaseReference.Document(dataPath).SetAsync(characterData);
        warningRegisterText.text = "Generated User Data";
        dataPath = "";//reset data path
        userEnabled=false;
    }

    public void GetUserDataOnLogin(FirebaseUser _userId)
    {
        dataPath = $"users/{_userId.UserId}";
        databaseReference.Document(dataPath).GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            Assert.IsNull(task.Exception);

            var characterData = task.Result.ConvertTo<CharacterData>();

            RegisterLoginScreen.username = characterData.userName;
            RegisterLoginScreen.currentLvl = characterData.currentLevel;
            SetPlayTime.min = characterData.min;
            SetPlayTime.sec = characterData.sec;
            userEnabled = characterData.enabled;

        }
        );

    }
    public void AwaitVerification(bool _emailSent, string _output)
    {
        verifyEmailPanel.SetActive(true);
        if (_emailSent)
            verifEmailText.text = $"Sent Email Verification\n Please verify: {firebaseUser.Email}";
        else
            verifEmailText.text = $"Email Not Sent :{_output}";

    }
    private IEnumerator SendVerificationEmail()
    {
        if (firebaseUser != null)
        {
            var emailTask = firebaseUser.SendEmailVerificationAsync();
            yield return new WaitUntil(predicate: () => emailTask.IsCompleted);

            if (emailTask.Exception != null)
            {
                FirebaseException firebaseException = (FirebaseException)emailTask.Exception.GetBaseException();
                AuthError error = (AuthError)firebaseException.ErrorCode;

                string output = "Unknown Error: Try Again!";
                switch (error)
                {
                    case AuthError.Cancelled:
                        output = "Verification Cancelled";
                        break;
                    case AuthError.InvalidRecipientEmail:
                        output = "Invalid Email";
                        break;
                    case AuthError.TooManyRequests:
                        output = "Too Many Requests";
                        break;
                }
                warningRegisterText.text = output;
                AwaitVerification(false, output);
            }
            else
            {
                //Auth username is now updated
                Debug.Log("Email Verification Sent Successfully");
                warningRegisterText.text = "Email Verification Sent Successfully";
                AwaitVerification(true, null);


            }
        }


    }



    //ADMIN LOGIN

    public void OpenAdminLoginScreen()
    {
        RegisterPanel.SetActive(false);
        LoginPanel.SetActive(false);
        LoadingPanel.SetActive(false);
        verifyEmailPanel.SetActive(false);
        quitPanel.SetActive(false);

        adminLoginPanel.SetActive(true);
        adminLoginText.text = "";

    }
    public void BackToLoginScreen()
    {
        adminLoginPanel.SetActive(false);
        AdminPanel.SetActive(false);
        LoginPanel.SetActive(true);

    }
    public void AdminLogin()
    {
        if (adminPassword.text == "ROOTPASSWORD")
        {
            adminLoginPanel.SetActive(false);
            AdminPanel.SetActive(true);
            AdminListUpdate();
            adminLoginText.text = "Admin Logged In";
        }
        else
        {
            adminLoginText.text = "Incorrect Admin Password";

        }

    }

    public void AdminListUpdate()
    {
        for (int i = 0; i < usersPanel.transform.childCount; i++)
        {
            Destroy(usersPanel.transform.GetChild(i).gameObject);
        }

        Query allUsersQuery = databaseReference.Collection("users");
        allUsersQuery.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            int i = 1;
            QuerySnapshot allUsersQuerySnapshot = task.Result;
            foreach (DocumentSnapshot documentSnapshot in allUsersQuerySnapshot.Documents)
            {
                string _userName="", _currentLevel="", _enabled = "",_min = ""  ,_sec = ""  ;

                Debug.Log(String.Format("Document data for {0} document:", documentSnapshot.Id));
                Dictionary<string, object> users = documentSnapshot.ToDictionary();
                foreach (KeyValuePair<string, object> pair in users)
                {
                    if(pair.Key=="userName")
                    {
                        _userName=pair.Value.ToString();    
                    }
                    else if (pair.Key == "currentLevel")
                    {
                        _currentLevel=pair.Value.ToString();    
                    }
                    else if (pair.Key == "enabled")
                    {
                        _enabled=pair.Value.ToString();
                    }
                    else if (pair.Key == "min")
                    {
                        _min=pair.Value.ToString();
                    }
                    else if (pair.Key == "sec")
                    {
                        _sec=pair.Value.ToString();
                    }

                }


                GameObject _listElement = Instantiate(userElementPrefab, usersPanel.transform);
                _listElement.name = documentSnapshot.Id;

                //set the serial number of list item
                _listElement.GetComponentInChildren<Transform>().Find("Sno").GetComponent<TextMeshProUGUI>().text = (i).ToString() + ".";

                //set the userName in list element
                _listElement.GetComponentInChildren<Transform>().Find("Username").GetComponent<TextMeshProUGUI>().text = "Username: " + _userName;


                //set the current level
                _listElement.GetComponentInChildren<Transform>().Find("Level").GetComponent<TextMeshProUGUI>().text = "CurrentLevel: " + _currentLevel;

                
                //set the min of user
               _listElement.GetComponentInChildren<Transform>().Find("MinField").GetComponent<TMP_InputField>().text = _min.ToString();


                //set the sec of user
                _listElement.GetComponentInChildren<Transform>().Find("SecField").GetComponent<TMP_InputField>().text = _sec.ToString();



                //enabled toggle
                var _Enabled = _listElement.GetComponentInChildren<Transform>().Find("EnableUser");

                    if (_enabled == "True")
                        _Enabled.GetComponent<Toggle>().isOn = true;
                    else if (_enabled == "False")
                        _Enabled.GetComponent<Toggle>().isOn = false;

                i++;
            }
        });

    }


    public void UserActivationChanged(string _id,bool _val)
    {
        dataPath = $"users/{_id}";
        var characterData = new CharacterData
        {
            enabled = _val,
        };
        databaseReference.Document(dataPath).SetAsync(characterData, SetOptions.MergeFields("enabled"));
        dataPath = "";//reset data path
    }
    public void UserMinChanged(string _id, int _min)
    {
        dataPath = $"users/{_id}";
        var characterData = new CharacterData
        {
            min = _min,
        };
        databaseReference.Document(dataPath).SetAsync(characterData, SetOptions.MergeFields("min"));
        dataPath = "";//reset data path
    }
    public void UserSecChanged(string _id, int _sec)
    {
        dataPath = $"users/{_id}";
        var characterData = new CharacterData
        {
            sec = _sec,
        };
        databaseReference.Document(dataPath).SetAsync(characterData, SetOptions.MergeFields("sec"));
        dataPath = "";//reset data path
    }
}
