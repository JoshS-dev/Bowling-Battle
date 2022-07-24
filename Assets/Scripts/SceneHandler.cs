using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

using static BB_Utils;

public class SceneHandler : MonoBehaviour
{
    int seed = 420;
    bool useSeed = false;
    /*
    float floorYLevel = -4.5f;
    float raisedfloorYLevel = -2.5f;
    */
    [Range(1,99)]
    public int minEnemies = 3;
    [Range(1,99)]
    public int maxEnemies = 9;
    
    public GameObject _player;
    public GameObject _balls;
    public GameObject _enemies;
    public GameObject _zones;
    public GameObject _leaderboardPopupPrefab;

    PlayerCondition _pc;
    PlayerBallHandler _pbh;
    ScoreHandler _sch;
    LeaderboardManager _lbm;

    public readonly List<EnemyControl> waveEnemies = new List<EnemyControl>();

    public int waveTally;

    public float duration = 60f;
    public float timeRemaining;

    //public bool gameRunning;
    public GameState currentState;

    public bool visibleCursorToggle;

    // Start is called before the first frame update
    void Start() {
        if (useSeed) {
            Random.InitState(seed);
        }

        _pc = _player.GetComponent<PlayerCondition>();
        _pbh = _player.GetComponent<PlayerBallHandler>();
        _sch = transform.Find("ScoreHandler").gameObject.GetComponent<ScoreHandler>();
        _lbm = GetComponent<LeaderboardManager>();

        waveTally = 0;
        timeRemaining = duration;
        //gameRunning = true;
        currentState = GameState.Paused;
        if(currentState == GameState.Paused)
            Time.timeScale = 0f;
        else
            Time.timeScale = 1f;

        visibleCursorToggle = currentState == GameState.Paused;
        visibleCursorToggle = ToggleCursor(visibleCursorToggle);

        Vector3 offset = new Vector3(0f, 0f, 3f);
        GameObject p_ball = Instantiate(_balls, _player.transform.position + offset, Quaternion.Euler(0f, 200f, 0f));
        _pbh.ball = p_ball;
        BallHandler _bh = p_ball.GetComponent<BallHandler>();

        _bh.parent = _player;
        _bh.SetMat(_bh.blueMat);

        SpawnWave(false);
    }

    // Update is called once per frame
    void Update() {
        //Debug.Log(currentState);
        if(currentState == GameState.Undefined) {
            if (Time.timeScale == 0)
                currentState = GameState.Paused;
            else if (!_pc.isAlive) {
                if (timeRemaining < 0)
                    currentState = GameState.Complete;
                else
                    currentState = GameState.Dead;
            }
            else
                currentState = GameState.Running;
        }


        if(currentState == GameState.Running || currentState == GameState.Dead) { 
            foreach (EnemyControl e in waveEnemies.ToList()) {
                if (!e.isAlive) {
                    waveEnemies.Remove(e);
                }
            }

            if(timeRemaining >= 0) {
                timeRemaining -= Time.deltaTime;
            }
            else { // timer is up! -----------------------------------------------------------
                _pc.Kill();

                currentState = GameState.Complete;
                foreach (EnemyControl e in waveEnemies.ToList()) {
                    e.Kill(ScoringType.None, e.transform.position, false, false, true, 0f);
                    waveEnemies.Remove(e);
                }

                if(_lbm.FitInLeaderboard(_sch.score) != -1) {
                    visibleCursorToggle = ToggleCursor(visibleCursorToggle);
                    Instantiate(_leaderboardPopupPrefab).GetComponent<LeaderboardPopupManager>().Initialize(_sch.score, waveTally);
                }
            }

            if(currentState != GameState.Complete) {
                if (waveEnemies.Count <= 0) {
                    SpawnWave();
                }
            }
        }


        //if (!gameRunning && Input.GetKeyDown(KeyCode.Return)) {
        if (Input.GetKeyDown(KeyCode.Return)) {
            if((currentState == GameState.Complete || currentState == GameState.Dead)&&(transform.Find("/Leaderboard Popup(Clone)") == null)) {
                _pc.Revive(Vector3.zero);
                if(currentState == GameState.Complete) {
                    _sch.ResetScore();
                    waveTally = 0;

                    Rigidbody _rb = _pbh.ball.GetComponent<Rigidbody>();
                    _rb.constraints = RigidbodyConstraints.FreezeRotation;
                    _pbh.ball.transform.position = new Vector3(0f, 0f, 3f);
                    _pbh.ball.transform.rotation = Quaternion.Euler(0f, 200f, 0f);
                    _rb.velocity = Vector3.zero;
                    _rb.constraints = RigidbodyConstraints.None;

                    Camera.main.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);

                    timeRemaining = duration;
                    SpawnWave(false);
                }
                currentState = GameState.Running;
            }
        }

        if(!_pc.isAlive && currentState == GameState.Running) { // premature death
            _sch.PlayerDeath();
            currentState = GameState.Dead;
        }

        if (Input.GetKeyDown(KeyCode.Escape)) {
            if(Time.timeScale == 1f) { // to pause
                currentState = GameState.Paused;
                Time.timeScale = 0f;
            }
            else { // to unpause
                currentState = GameState.Undefined;
                Time.timeScale = 1f;
            }
            visibleCursorToggle = ToggleCursor(visibleCursorToggle);
        }
    }

    public void SH_ToggleCursor() {
        visibleCursorToggle = ToggleCursor(visibleCursorToggle);
    }

    void SpawnWave(bool givePoints = true) {
        if (givePoints)
            _sch.WaveComplete();
        waveTally++;
        for (int _ = 0; _ < Random.Range(minEnemies, maxEnemies + 1); _++) {
            int childIndex = Random.Range(0, _zones.transform.childCount);
            Transform tgtZone = _zones.transform.GetChild(childIndex);
            /*
            float spawnY;
            if (childIndex == 0)
                spawnY = raisedfloorYLevel;
            else
                spawnY = floorYLevel;
            */
            float minX = tgtZone.position.x - tgtZone.localScale.x / 2f;
            float maxX = tgtZone.position.x + tgtZone.localScale.x / 2f;
            float minZ = tgtZone.position.z - tgtZone.localScale.z / 2f;
            float maxZ = tgtZone.position.z + tgtZone.localScale.z / 2f;
            //Debug.Log("IDX: " + tgtZone + "MINX" + minX + "MAXX" + maxX + "MINZ" + minZ + "MAXZ" + maxZ);
            waveEnemies.Add(SpawnEnemy(new Vector3(Random.Range(minX, maxX),
                                                   //spawnY,
                                                   tgtZone.position.y,
                                                   Random.Range(minZ, maxZ)),
                                                   Quaternion.identity)) ;
        }
    }

    EnemyControl SpawnEnemy(Vector3 pos, Quaternion rot) {
        return Instantiate(_enemies, pos, rot).GetComponent<EnemyControl>();
    }
}
