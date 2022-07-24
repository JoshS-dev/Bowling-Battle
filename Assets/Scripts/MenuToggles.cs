using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

public class MenuToggles : MonoBehaviour
{
    SceneHandler _sh;
    LeaderboardManager _lbm;

    public GameObject pauseMenu;
    public GameObject helpMenu;
    public GameObject optionsMenu;
    public GameObject leaderboardMenu;
    [SerializeField]
    public GameObject[] leaderboardPlaces = new GameObject[10];

    public bool help_show;
    public bool options_show;
    public bool leaderboard_show;

    void Awake() {
        optionsMenu.SetActive(true);
    }

    // Start is called before the first frame update
    void Start(){
        _sh = GameObject.Find("/SceneHandler").GetComponent<SceneHandler>();
        _lbm = GameObject.Find("/SceneHandler").GetComponent<LeaderboardManager>();
        pauseMenu.SetActive(false);
        helpMenu.SetActive(false);  
        optionsMenu.SetActive(false);
        leaderboardMenu.SetActive(false);
        help_show = false;
        options_show = false;
        leaderboard_show = false;
    }

    // Update is called once per frame
    void Update(){
        if(_sh.currentState != BB_Utils.GameState.Paused) {
            if (help_show)          ToggleHelp();
            if (options_show)       ToggleOptions();
            if (leaderboard_show)   ToggleLeaderboard();
            pauseMenu.SetActive(false);
            optionsMenu.SetActive(false);
            leaderboardMenu.SetActive(false);
        }
        else/* if(_sh.currentState == BB_Utils.GameState.Paused)*/{
            pauseMenu.SetActive(true);
        }
    }

    public void ToggleHelp() {
        help_show = !help_show;
        if (help_show)  helpMenu.SetActive(true);
        else            helpMenu.SetActive(false);
    }

    public void ToggleOptions() {
        options_show = !options_show;
        if (options_show)   optionsMenu.SetActive(true);
        else                optionsMenu.SetActive(false);
    }

    public void ToggleLeaderboard() {
        leaderboard_show = !leaderboard_show;
        if (leaderboard_show) {
            leaderboardMenu.SetActive(true);
        }
        else leaderboardMenu.SetActive(false);

        for(int i = 0; i < 10; i++) {
            if(i < _lbm.lb.filledSpaces) {
                leaderboardPlaces[i].SetActive(true);
                //name, score*, wave
                (string, int, int) currPlace = _lbm.lb.topTen[i];
                leaderboardPlaces[i].transform.Find("Name").GetComponent<TextMeshProUGUI>().text = currPlace.Item1;
                leaderboardPlaces[i].transform.Find("Score").GetComponent<TextMeshProUGUI>().text = "" + currPlace.Item2;
                leaderboardPlaces[i].transform.Find("Wave").GetComponent<TextMeshProUGUI>().text = "" + currPlace.Item3;
            }
            else {
                leaderboardPlaces[i].SetActive(false);
            }
        }
    }
}
