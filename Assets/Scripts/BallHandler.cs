using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static BB_Utils;

public class BallHandler : MonoBehaviour // Dont laugh at the class name XD
{
    /*[System.Flags] public enum ScoringType {
        None        = 0,
        Headshot    = 1 << 0,
        LongRange   = 1 << 1,
        Style       = 1 << 2,
    }*/
    ScoringType scoringFlags;

    public float velocityDmgThreshold = 4.5f;
    public int damageAmount = 1;
    public float enemyBallDecayTime = 5f;

    public int combo;
    
    public Material whiteMat;
    public Material blueMat;
    public Material redMat;

    public AudioClip thumpRoll_S;
    public AudioClip thump_S;
    public AudioClip strike_S;
    AudioSource _as;

    public GameObject parent;
    public bool breakOnWall;

    ScoreHandler _sch;
    [SerializeField] Renderer _re;
    Rigidbody _rb;

    Vector3 positionThrown;

    // Start is called before the first frame update
    void Start()
    {
        _as = gameObject.GetComponent<AudioSource>();
        _rb = gameObject.GetComponent<Rigidbody>();
        _sch = GameObject.Find("/SceneHandler/ScoreHandler").GetComponent<ScoreHandler>();

        //Throw(Vector3.forward, 12f, true);
        _as.spatialBlend = 0.65f;
        _as.clip = thumpRoll_S; // only long sound that could be stopped prematurely, save it for Play()

        positionThrown = Vector3.zero;

        scoringFlags = ScoringType.None;
        combo = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y < -5) { // if fallen out of bounds of map
            transform.position = Vector3.zero;
            _rb.velocity = Vector3.zero;
        }

        if ((HorizontalDistance(positionThrown, transform.position) > _sch.longRangeThreshold) && (!_rb.isKinematic)) { // if travelled far and not held
            scoringFlags |= ScoringType.LongRange;
        }
        else {
            scoringFlags &= ~ScoringType.LongRange;
        }
        

        if(parent == null || parent.tag == "Enemy") {
            if(_rb.velocity.magnitude == 0) {
                Invoke("RemoveSelf", enemyBallDecayTime);
            }
            else {
                CancelInvoke("RemoveSelf");
            }
        }
    }

    private void OnCollisionEnter(Collision col) { // maybe OnCollisionStay?
        Vector3 horizontalVelocity = new Vector3(_rb.velocity.x, 0f, _rb.velocity.z);
        if(col.gameObject.layer == 8) { // PINS
            if(col.gameObject != parent) {
                if (_rb.velocity.magnitude > velocityDmgThreshold) {
                    
                    if(col.gameObject.tag == "Player") {
                        PlayerCondition _pc = col.gameObject.GetComponent<PlayerCondition>();
                        _pc.TakeDamage(damageAmount);
                        if (!_pc.isAlive) {
                            _as.PlayOneShot(strike_S, 1f);
                        }
                    }
                    else { // is Enemy
                        if (parent == null || parent.tag != "Enemy") { // if the ball is owned by an enemy, don't take damage
                            EnemyControl _ec = col.gameObject.GetComponent<EnemyControl>();

                            if (col.collider == _ec.head_C) // head collider / top capsule
                                scoringFlags |= ScoringType.Headshot;

                            if (_ec.isAlive)
                                if (parent != null)
                                    _ec.TakeDamage(damageAmount, scoringFlags, positionThrown);
                                else
                                    _ec.TakeDamage(damageAmount, scoringFlags, _ec.transform.position, true, false); // Don't add to combo

                            scoringFlags &= ~ScoringType.Headshot; // after damage taken into account, remove headshot flag, per-enemy basis
                        }
                    }
                }
            }
        }
        if(col.gameObject.layer == 9) { // GROUND
            //Debug.Log("GROUND HIT");
            if (Mathf.Abs(_rb.velocity.y) > 1f) {
                if (horizontalVelocity.magnitude < 5f) {
                    _as.PlayOneShot(thump_S, 1f);
                }
                else {
                    _as.Play();
                }
            }
        }
        if(col.gameObject.layer == 10) { // WALL
            _as.Stop();
            if (horizontalVelocity.magnitude > 1f)
                _as.PlayOneShot(thump_S, 1f);
            if (breakOnWall) {
                //_as.PlayOneShot(thump_S, 1f);
                Destroy(this.gameObject);
            }
        }
        //Debug.Log("Velocity: " + velocity.magnitude + " Horizontal: " + horizontalVelocity.magnitude + " Vertical: " + verticalVelocity);
        
    }

    private void OnTriggerEnter(Collider zone) { // for style zones
        if(zone.gameObject.transform.parent.gameObject.name == "StyleZones") {
            scoringFlags |= ScoringType.Style;
        }
    }

    public void Throw(Vector3 direction, float strength, bool raw) {
        ForceMode forceType;
        if (raw)
            forceType = ForceMode.VelocityChange;
        else
            forceType = ForceMode.Impulse;
        _rb.AddForce(direction * strength, forceType);
        positionThrown = new Vector3(parent.transform.position.x,0f,parent.transform.position.z);
    }

    public void ToggleFreeze() { // for when ball is held, therefore also reset combo and flags
        //_as.Stop();
        scoringFlags = ScoringType.None;
        _sch.EndCombo();
        _rb.isKinematic = !_rb.isKinematic;
        _rb.detectCollisions = !_rb.detectCollisions;
    }

    public void SetMat(Material mat) {
        _re.material = mat;
    }

    void RemoveSelf() {
        Destroy(gameObject);
    }
}
