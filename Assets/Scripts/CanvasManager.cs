using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using static BB_Utils;

public class CanvasManager : MonoBehaviour
{
    public GameObject _player;
    PlayerCondition _pc;
    SceneHandler _sh;
    ScoreHandler _sch;

    // Font from 1001 fonts

    TextMeshProUGUI scoreText;
    TextMeshProUGUI statusText;
    TextMeshProUGUI waveText;
    TextMeshProUGUI timerText;
    TextMeshProUGUI helpText;
    TextMeshProUGUI sprintText;
    TextMeshProUGUI notificationText;
    TextMeshProUGUI flavorText;
    TextMeshProUGUI scoringText;

    Image scoringBacking;
    
    int notificationLimit = 10; // Text box can hold 10 rows!
    public float notificationFade = 5f;
    public float newFlavorThreshold = 1.5f;
    
    [System.Serializable]
    public class NotificationData {
        public ScoringType flag;
        public int count;
        public double timeUpdated;
        public string flavor;
    }

    private readonly List<NotificationData> notifications = new List<NotificationData>();

    double currentTime;

    bool helpShown;

    Dictionary<ScoringType, string[]> flavors = new Dictionary<ScoringType, string[]>();
    public int rareFlavorMultiplier = 2;
    // Start is called before the first frame update
    void Start() {
        _pc = _player.GetComponent<PlayerCondition>();
        _sh = GameObject.Find("/SceneHandler").GetComponent<SceneHandler>();
        _sch = GameObject.Find("/SceneHandler/ScoreHandler").GetComponent<ScoreHandler>();
        //"User_Interface/" ""
        scoreText = GetTextByName("","Text_Score");
        statusText = GetTextByName("", "Text_Status");
        waveText = GetTextByName("", "Text_Wave");
        timerText = GetTextByName("", "Text_Timer");
        helpText = GetTextByName("", "Text_Help");
        sprintText = GetTextByName("", "Text_Sprint");
        notificationText = GetTextByName("", "Text_Notification");
        flavorText = GetTextByName("", "Text_Flavor");
        scoringText = GetTextByName("", "Text_Scoring");

        scoringBacking = GetImageByName("", "Backing_Scoring");

        helpShown = false;
        currentTime = 0.0;
        scoringBacking.enabled = false;

        flavors[ScoringType.Headshot] = new string[] {  "BOOM! HEADSHOT!", "Right between the... eyes?", "They'll feel that in the morning.",
                                                        "Seeing stars!","Knockout!","Mike Tyson'ed!",
                                                        "Absolutely rocked!","TKO","Splattered!",
                                                        // Rarer
                                                        "John Wilkes Booth'ed!"};

        flavors[ScoringType.LongRange] = new string[] { "Rolling Thunder!", "Sniped!","Trailblazer!",
                                                        "Around the block!","Frequent-flyer miles!","Stamp the passport!",
                                                        "Gonna need binoculars!","Geometric!","Looking down a scope?",
                                                        // Rarer
                                                        "What a high magnitude vector!"};

        flavors[ScoringType.Style] = new string[] {     "Very stylish!", "Radical!","Now thats just showing off.",
                                                        "Totally tubular, dude!","Bold move, and it paid off!","Dopamine!",
                                                        "5 stars!","Two thumbs up!","10/10",
                                                        // Rarer
                                                        "Play it loud and stereo, dude!"};
        
    }

    TextMeshProUGUI GetTextByName(string parentRoot, string name) {
        return transform.Find(parentRoot + name).gameObject.GetComponent<TextMeshProUGUI>();
    }
    Image GetImageByName(string parentRoot, string name) {
        return transform.Find(parentRoot + name).gameObject.GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        // ------------------ SCORE ------------------
        ScoreTextUpdater();

        // ------------------ STATUS ------------------
        StatusTextUpdater();

        // ------------------ WAVE ------------------
        WaveTextUpdater();

        // ------------------ TIME ------------------
        TimerTextUpdater();

        // ------------------ HELP ------------------
        HelpTextUpdater();

        // ------------------ SPRINT ------------------
        SprintTextUpdater();

        // ------------------ NOTIFICATIONS ------------------
        NotificationAndFlavorTextUpdater();

        // ------------------ SCORING ------------------
        ScoringTextUpdater();

        currentTime += Time.deltaTime;
    }

    void ScoreTextUpdater() {
        int currentCombo = 0;
        if (_sch.comboFunction == ComboScoringFunction.SummationEquation)
            currentCombo = SummationEquation(_sch.currentCombo, _sch.comboScalar);
        else if (_sch.comboFunction == ComboScoringFunction.LinearEquation)
            currentCombo = LinearEquation(_sch.currentCombo, _sch.comboScalar);
        scoreText.text = "Score:  " + _sch.score + "  Combo:  " + (_sch.currentCombo + 1) + "x  " +
                         "(" + currentCombo + ")";
    }

    void StatusTextUpdater() {
        if (_sh.currentState == GameState.Dead) {
            statusText.text = "You have died!\nRespawn with [Enter]!";
        }
        else if (_sh.currentState == GameState.Complete) {
            statusText.text = "Time is up!\nRestart with [Enter]!";
        }
        else if (_sh.currentState == GameState.Paused) {
            statusText.text = "You are paused, neighborino!";
        }
        else
            statusText.text = string.Empty;
    }

    void WaveTextUpdater() {
        waveText.text = "Wave:  " + _sh.waveTally;
    }

    void TimerTextUpdater() {
        timerText.text = "Time:  " + (Mathf.RoundToInt(_sh.timeRemaining * 10f) / 10f).ToString("#0.0");
    }

    void HelpTextUpdater() {
        helpText.text = "Press [Esc] to pause the game.\n";
        //if (Input.GetKeyDown(KeyCode.H)) // Press H to show help
        //helpShown = !helpShown;
        helpText.text += ControlInfo();
    }

    void SprintTextUpdater() {
        sprintText.text = "Sprint:  " + Mathf.Min(100f * _pc.curSprintTime / _pc.sprintTime, 100f).ToString("0.") + "%";
        if (_pc.canSprint)
            sprintText.text += "\nSprint: AVAILABLE";
        else
            sprintText.text += "\nSprint: ON COOLDOWN";
    }

    void NotificationAndFlavorTextUpdater() {
        notificationText.text = string.Empty;
        flavorText.text = string.Empty;
        if (_pc.isAlive) {
            while (notifications.Count > notificationLimit) { // futureproofing, if there are ever too many notifications for the textbox, clear out oldest ones
                NotificationData oldestEntry = notifications[0];
                foreach (NotificationData entry in notifications) {
                    if (currentTime - entry.timeUpdated > currentTime - oldestEntry.timeUpdated)
                        oldestEntry = entry;
                }
                notifications.Remove(oldestEntry);
            }
            for (int i = 0; i < notifications.Count; i++) {
                NotificationData entry = notifications[i];
                if (currentTime - entry.timeUpdated > notificationFade) {
                    notifications.Remove(entry);
                }
                else {
                    string flagString;
                    if (entry.flag == ScoringType.LongRange)
                        flagString = "Long Range";
                    else
                        flagString = entry.flag.ToString();

                    notificationText.text += flagString + /*" " + entry.count +*/ "\n";
                    flavorText.text += entry.flavor + "\n";
                }
            }
        }
        else // if player is dead
            notifications.Clear();
    }

    void ScoringTextUpdater() {
        if (_sh.currentState == GameState.Complete) {
            scoringBacking.enabled = true;
            scoringText.text = "Score: " + _sch.score +
                               "\nScore from killing enemies: " + _sch.scoreFromEnemies +
                               "\nScore from waves: " + _sch.scoreFromWaves +
                               "\nScore from deaths: " + _sch.scoreFromDeaths +
                               "\n\nScore from combos: " + _sch.scoreFromBonuses.scoreFromCombos +
                               "\nScore from headshots: " + _sch.scoreFromBonuses.scoreFromHeadshots +
                               "\nScore from long range: " + _sch.scoreFromBonuses.scoreFromLongRange +
                               "\nScore from style: " + _sch.scoreFromBonuses.scoreFromStyle +
                               "\nTotal score from bonuses: " + _sch.scoreFromBonuses.TotalScoreFromBonuses();
        }
        else {
            scoringText.text = string.Empty;
            scoringBacking.enabled = false;
        }
    }



    public void InterpretFlags(ScoringType flags) {
        foreach(ScoringType flag in System.Enum.GetValues(typeof(ScoringType))) { 
            if(flag != ScoringType.None && flags.HasFlag(flag)) {
                int posIdx = NotificationsFlagPresent(flag);
                if (posIdx != -1) // Edit existing
                    UpdateNotification(notifications[posIdx]);
                else
                    CreateNotification(flag);
            }
        }
    }

    void CreateNotification(ScoringType flag) {
        NotificationData newEntry = new NotificationData {
            flag = flag,
            count = 1,
            flavor = FlavorPicker(flag),
            timeUpdated = currentTime,
        };
        notifications.Add(newEntry);
    }

    void UpdateNotification(NotificationData entry) {
        entry.count++;
        if (currentTime - entry.timeUpdated > newFlavorThreshold) {
            string newFlavor = FlavorPicker(entry.flag);
            while (newFlavor == entry.flavor) 
                newFlavor = FlavorPicker(entry.flag);
            entry.flavor = newFlavor;
        }
        entry.timeUpdated = currentTime;
    }

    string FlavorPicker(ScoringType flag, int numRare = 1) { // weights the last numRare elements in the array to be rarer
        string[] target = flavors[flag];
        int pivot = target.Length - numRare - 1;

        int ranVal = Random.Range(0, rareFlavorMultiplier * (target.Length - numRare) + numRare );
        if (ranVal >= rareFlavorMultiplier * (target.Length - numRare)) { // RARE
            return target[pivot + (ranVal - (rareFlavorMultiplier * pivot + 1))];
        }
        else {
            return target[Mathf.FloorToInt(1f * ranVal / rareFlavorMultiplier)];
        }
    }

    int NotificationsFlagPresent(ScoringType flag) {
        for(int i=0; i <= notifications.Count-1; i++) {
            if (notifications[i].flag == flag)
                return i;
        }
        return -1;
    }

    public void ToggleShowControls() {
        helpShown = !helpShown;
    }

    string ControlInfo() {
        if (!helpShown) {
            return string.Empty;
        }
        return HELP_TEXT; // BB_Utils
    }


}
