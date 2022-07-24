using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static BB_Utils;

public class EnemyControl : MonoBehaviour
{
    public ScoringType inheritedFlags;
    public Vector3 inheritedThrownPos;
    
    public int maxHealth = 1;
    public float bodyDecayTime = 4.5f;
    public float iFrames = 0.2f;
    public int collisionDamageAmount = 1;

    public bool isAlive;
    public int health;
    public bool comboCounted;

    public AudioClip singleHit_S;
    public AudioClip strike_S;
    AudioSource _as;

    public Collider body_C;
    public Collider head_C;

    Rigidbody _rb;
    SceneHandler _sh;
    ScoreHandler _sch;

    public GameObject beam;
    GameObject currentBeam;

    bool freezed;

    // Start is called before the first frame update
    void Start()
    {
        isAlive = true;
        comboCounted = false;
        health = maxHealth;

        _rb = GetComponent<Rigidbody>();
        _sh = GameObject.Find("/SceneHandler").GetComponent<SceneHandler>();
        _as = GetComponent<AudioSource>();
        _sch = GameObject.Find("/SceneHandler/ScoreHandler").GetComponent<ScoreHandler>();

        _as.spatialBlend = 0.65f;

        inheritedFlags = ScoringType.None;
        inheritedThrownPos = transform.position;

        //_rb.isKinematic = true;
        freezed = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(iFrames >= 0) {
            iFrames -= Time.deltaTime;
        }
        
        if (isAlive) {
            if (currentBeam == null)
                currentBeam = Instantiate(beam, Vector3.zero, Quaternion.identity);
            currentBeam.transform.position = new Vector3(transform.position.x,
                                                         transform.position.y + currentBeam.transform.Find("Cylinder").localScale.y,
                                                         transform.position.z);
        }
        
    }

    private void OnCollisionEnter(Collision col) { // will only collide like this when already dead
        if (col.gameObject.layer == 8) { // PINS
            if(col.gameObject.tag != "Player") {
                if (_rb != null && _rb.velocity.magnitude > 5f) {
                    EnemyControl _ec = col.gameObject.GetComponent<EnemyControl>();

                    if (_ec.isAlive) {
                        _ec.TakeDamage(collisionDamageAmount, inheritedFlags & ~ScoringType.Headshot, inheritedThrownPos); // Dont pass along being headshot
                    }
                    //if (!_ec.isAlive && !_ec.comboCounted) { // kill
                        //_ec.comboCounted = true; // shouldnt collide with BallHandler code, as this also sets true and checks if false
                        //_sh.currentCombo += 1;
                        //_sh.UpdateCombo(); // may not be necessary... but just in case
                    //}
                }
            }
        }
    }

    public void TakeDamage(int dmg, ScoringType flags, Vector3 thrownPos, bool gainPoints = true, bool addCombo = true) {
        if(iFrames <= 0)
            health -= dmg;
        if (health <= 0) {
            inheritedFlags = flags;
            inheritedThrownPos = thrownPos;
            if (gainPoints)
                Kill(flags, thrownPos);
            else if (!addCombo)
                Kill(flags, thrownPos, true, false);
            else
                Kill(flags, Vector3.zero, false);
        }
    }

    public void Kill(ScoringType flags, Vector3 thrownPos, bool gainPoints = true, bool addCombo = true, bool overrideDefault=false,float newDecayTime = 0f) {
        if (isAlive) {
            isAlive = false;
            if (gainPoints) {
                if (Vector3.Distance(transform.position, thrownPos) >= _sch.longRangeThreshold)
                    flags |= ScoringType.LongRange;
                if (!addCombo)
                    _sch.EnemyKilled(flags, false);
                else
                    _sch.EnemyKilled(flags);
            }

            ToggleRigidBody();
            Destroy(currentBeam);
            _rb.WakeUp();

            if (_sh.waveEnemies.Count == 1) { // if this is the last living enemy
                _as.PlayOneShot(strike_S, 1f);
            }
            else {
                _as.PlayOneShot(singleHit_S, 1f);
            }
            float decayTime;
            if (overrideDefault)
                decayTime = newDecayTime;
            else
                decayTime = bodyDecayTime;
            Invoke("RemoveSelf", decayTime); // Remove body after time passed
        }
    }

    void ToggleRigidBody() {
        freezed = !freezed;
        if (freezed) {
            _rb.constraints = RigidbodyConstraints.FreezeAll;
        }
        else {
            _rb.constraints = RigidbodyConstraints.None;
            _rb.AddForce(new Vector3(Random.value,0f,Random.value).normalized*2); // slight jitter for glancing blows
        }
        
        //_rb.isKinematic = !_rb.isKinematic;
        //_rb.AddForce(Vector3.one); // slight jitter so models dont get stuck as often
    }

    void RemoveSelf() {
        Destroy(gameObject);
    }
}
