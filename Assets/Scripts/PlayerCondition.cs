using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCondition : MonoBehaviour
{
    public int maxHealth = 1;
    public float iFrames = 1f;
    public float sprintTime = 3f;
    public float prematureSprintRegenScalar = 1.5f;
    public float cooldownSprintRegenScalar = 0.75f;
    
    public bool isAlive;
    public bool canSprint;
    public int health;
    public float curSprintTime;

    Rigidbody _rb;
    PlayerBallHandler _pbh;

    // Start is called before the first frame update
    void Start(){
        isAlive = true;
        canSprint = true;
        health = maxHealth;
        curSprintTime = sprintTime;

        _rb = GetComponent<Rigidbody>();
        _pbh = GetComponent<PlayerBallHandler>();

        _rb.isKinematic = true;
        //_rb.detectCollisions = true;
    }

    // Update is called once per frame
    void Update() {
        if (iFrames >= 0) {
            iFrames -= Time.deltaTime;
        }
        if (isAlive && health <= 0) {
            //_pbh.StartCoroutine(DropBall(false));
            _pbh.DispenseBall(false, _pbh.dropStrength);
            Kill();
        }
        /*
        if (!isAlive && Input.GetKey(KeyCode.Return)) {
            Revive(Vector3.zero);
        }
        */
        if (curSprintTime <= 0) {
            canSprint = false;
        }

        if (!canSprint) { // sprint is used up
            if (curSprintTime >= sprintTime) // curSprintTime has regenerated back to the max sprint
                canSprint = true;
            else
                curSprintTime += Time.deltaTime * cooldownSprintRegenScalar; // regenerate
        }
        else { // sprint is still on
            if (curSprintTime < sprintTime) { // if any is used up
                curSprintTime += Time.deltaTime * prematureSprintRegenScalar; // regenerate faster
            }
        }

        /*
        if (Input.GetMouseButtonDown(2)) {
            Kill();
        }
        */
    }

    public void TakeDamage(int dmg) {
        if (iFrames <= 0)
            health -= dmg;
    }

    public void Kill(bool limp = true) {
        if (isAlive) {
            isAlive = false;
            if(limp)
                ToggleRigidBody();
        }
    }

    public void Revive(Vector3 respawnPos, bool limp = true) {
        if (!isAlive){
            health = maxHealth;
            isAlive = true;
            if(limp)
                ToggleRigidBody();
            transform.rotation = Quaternion.identity;   
            transform.position = respawnPos;
        }
    }

    void ToggleRigidBody() {
        transform.position = new Vector3(transform.position.x, transform.position.y + 0.05f, transform.position.z);
        _rb.isKinematic = !_rb.isKinematic;
        _rb.WakeUp();
        //_rb.AddForce(Vector3.up * -2,ForceMode.Acceleration);
        //_rb.detectCollisions = !_rb.detectCollisions;
        _rb.AddForce(2*Vector3.one); // slight jitter so models dont get stuck as often
        _rb.AddTorque(new Vector3(transform.rotation.x + 30f, 0, 0));
    }
}
