using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static BB_Utils;

public class ScoreHandler : MonoBehaviour {
    public int score;
    public int scoreFromEnemies;
    public int scoreFromWaves;
    public int scoreFromDeaths;
    public BonusTallies scoreFromBonuses = new BonusTallies();
    public class BonusTallies {
        public int scoreFromCombos;
        public int scoreFromHeadshots;
        public int scoreFromLongRange;
        public int scoreFromStyle;

        public void ResetValues() {
            scoreFromCombos = 0;
            scoreFromHeadshots = 0;
            scoreFromLongRange = 0;
            scoreFromStyle = 0;
        }
        public int TotalScoreFromBonuses() {
            return scoreFromCombos + scoreFromHeadshots + scoreFromLongRange + scoreFromStyle;
        }
    }

    [SerializeField] public int currentCombo;
    [SerializeField] int recordedCombo;

    public int scorePerEnemy = 2;
    public int scorePerWave = 5;
    public int scorePerPlayerDeath = -5;

    public float longRangeThreshold = 23f;

    public ComboScoringFunction comboFunction = ComboScoringFunction.SummationEquation;
    public int comboScalar = 2;

    public int headshotBonus = 1;
    public int longrangeBonus = 2;
    public int styleBonus = 2;

    CanvasManager _cm;

    // Start is called before the first frame update
    void Start() {
        ResetScore();
        _cm = GameObject.Find("/User Interface").GetComponent<CanvasManager>();
    }
    /*
    // Update is called once per frame
    void Update() {

    }
    */

    public void ResetScore() {
        score = 0;
        scoreFromEnemies = 0;
        scoreFromWaves = 0;
        scoreFromDeaths = 0;
        scoreFromBonuses.ResetValues();
        EndCombo();
    }

    public void EndCombo() {
        currentCombo = -1;
        recordedCombo = -1;
    }

    public void EnemyKilled(ScoringType flags, bool addCombo = true) {
        int scoreAddition = scorePerEnemy;
        scoreFromEnemies += scorePerEnemy;

        if (addCombo) {
            currentCombo++;
            int comboBonus = ComboScoreBonus(comboFunction);
            scoreAddition += comboBonus;
            scoreFromBonuses.scoreFromCombos += comboBonus;
        }

        if (HasFlag(flags, ScoringType.Headshot)) {
            scoreAddition += headshotBonus;
            scoreFromBonuses.scoreFromHeadshots += headshotBonus;
        }

        if (HasFlag(flags, ScoringType.LongRange)) {
            scoreAddition += longrangeBonus;
            scoreFromBonuses.scoreFromLongRange += longrangeBonus;
        }

        if (HasFlag(flags, ScoringType.Style)) {
            scoreAddition += styleBonus;
            scoreFromBonuses.scoreFromStyle += styleBonus;
        }

        _cm.InterpretFlags(flags);

        score += scoreAddition;
    }

    public void PlayerDeath() {
        score += scorePerPlayerDeath;
        scoreFromDeaths += scorePerPlayerDeath;
    }

    public void WaveComplete() {
        score += scorePerWave;
        scoreFromWaves += scorePerWave;
    }

    //  Kill 1      Kill 2      Kill 3      Kill 4      Kill 5      COMBO NUM
    //  SUMMATION
    //  +0          +1          +1+2=3      +1+2+3=6    +1+2+3+4=10 IN TOTAL (ASSUMING INC SCORE PER COMBO = 1)
    //  +0          +1          +2          +3          +4          SCORE PER ENEMY ( '' )
    //  
    //  +0          +2          +2+4=6      +2+4+6=12   +2+4+6+8=20 IN TOTAL (ASSUMING INC SCORE PER COMBO = 2)
    //  +0          +2          +4          +6          +8          SCORE PER ENEMY ( '' )
    public int ComboScoreBonus(ComboScoringFunction fn) {
        if (recordedCombo != currentCombo && currentCombo > 0) {
            if (fn == ComboScoringFunction.SummationEquation) {

                int returnVal = SummationEquation(currentCombo, comboScalar) - SummationEquation(recordedCombo, comboScalar); // I can write a paper on why this works
                recordedCombo = currentCombo;
                return returnVal;
            }
            else if (fn == ComboScoringFunction.LinearEquation) {
                return 0; // DO LATER
            }
            else
                return 0;
        }
        else {
            recordedCombo = currentCombo;
            return 0;
        }
    }
}
