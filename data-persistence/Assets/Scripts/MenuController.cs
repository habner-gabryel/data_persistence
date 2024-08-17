using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public InputField inputField;
    public string nameToSave;
    
    void Start ()
    {
        inputField.onEndEdit.AddListener(SubmitName);  
    }

    public void SubmitName(string name){
        nameToSave = name;
    }

    public void StartNew() {
        SceneManager.LoadScene(1);
        MainManager.Instance.SavePlayerName(nameToSave);
    }
    

    // Update is called once per frame
    void Update()
    {
        
    }
}
