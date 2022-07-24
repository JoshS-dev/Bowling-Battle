using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

using static BB_Utils;

public class LeaderboardPopupManager : MonoBehaviour
{
    public string inputName;
    public int scoreCount;
    public int waveCount;
    public int wouldbePosition;

    [SerializeField]
    TextMeshProUGUI scorePosition, scoreText, waveText, inputNameField, nameLabel;

    private LeaderboardManager _lb;
    private SceneHandler _sh;

    // Start is called before the first frame update
    void Awake(){
        _lb = GameObject.Find("/SceneHandler").GetComponent<LeaderboardManager>();
        _sh = GameObject.Find("/SceneHandler").GetComponent<SceneHandler>();
    }

    public void Initialize(int score, int wave) {
        scoreCount = score;
        waveCount = wave;
        scoreText.text += " " + scoreCount;
        waveText.text += " " + waveCount;
        int placement = _lb.FitInLeaderboard(scoreCount) + 1;
        scorePosition.text += "" + placement;
        switch (placement) {
            case 1:
                scorePosition.text += "st";
                break;
            case 2:
                scorePosition.text += "nd";
                break;
            case 3:
                scorePosition.text += "rd";
                break;
            default:
                scorePosition.text += "th";
                break;
        }
        scorePosition.text += " place!";
    }

    private void Update() {
        
    }

    public void UpdateName() {
        inputName = inputNameField.text;
    }

    public void CancelPopup() {
        Destroy(gameObject); // Delete Self (whole popup)
        _sh.visibleCursorToggle = ToggleCursor(_sh.visibleCursorToggle);
    }

    public void ConfirmPopup() {
        //UpdateName();
        inputName.Replace(' ', ' ');
        if (string.IsNullOrWhiteSpace(inputName) || inputName == "") { // idk this just doesnt work, always passes with a fully space name
            nameLabel.color = Color.red;
        }
        else {
            _lb.UpdateLeaderboard(inputName, scoreCount, waveCount);
            CancelPopup();
        }
    }
}
