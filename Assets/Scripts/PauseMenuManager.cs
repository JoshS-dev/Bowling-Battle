using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using static BB_Utils;

public class PauseMenuManager : MonoBehaviour
{
    SceneHandler _sh;
    MenuToggles _mt;
    TextMeshProUGUI resumeButtonText;
    
    // Start is called before the first frame update
    void Start(){
        _sh = GameObject.Find("/SceneHandler").GetComponent<SceneHandler>();
        _mt = GameObject.Find("/MenuToggler").GetComponent<MenuToggles>();
        resumeButtonText = transform.Find("Pause Menu/Resume_Button").gameObject.GetComponent<TextMeshProUGUI>();

        //Debug.Log(Application.dataPath);
    }

    // Update is called once per frame
    void Update(){
        if(_sh.currentState != GameState.Paused) {
            resumeButtonText.text = "Resume"; // Changes from "Start" to "Resume"
        }
    }

    public void ResumeButtonPress() {
        Time.timeScale = 1f;
        _sh.currentState = GameState.Undefined;
        _sh.visibleCursorToggle = ToggleCursor(_sh.visibleCursorToggle);
    }

    public void OptionsButtonPress() {
        Debug.Log("OPTION BUTTON PRESSED");
        _mt.ToggleOptions();
    }

    public void HelpButtonPress() {
        Debug.Log("HELP BUTTON PRESSED");
        _mt.ToggleHelp();
    }

    public void QuitButtonPress() {
        Debug.Log("QUIT BUTTON PRESSED");
        Application.Quit();
    }
}
