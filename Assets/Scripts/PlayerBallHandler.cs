using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBallHandler : MonoBehaviour
{
    public float dropStrength = 3f;
    public float throwStrenth = 36.0f;
    public float holdDistance = 2.0f;

    public float postThrowCooldown = 0.5f;

    public float ballFollowCamUpScalar = 2f;
    public float verticalThrowScalar = 1.35f;
    
    public bool hasBall;
    
    bool beamExists;
    bool isAlive;
    bool acceptCollisions;

    public GameObject ball;
    //private Renderer ballRenderer;
    public GameObject beam;
    GameObject currentBeam;
    
    PlayerCondition _pc;
    
    // Start is called before the first frame update
    void Start() {
        hasBall = false;
        acceptCollisions = true;
        beamExists = false;
        
        _pc = GetComponent<PlayerCondition>();
        //ballRenderer = ball.transform.Find("Ball").GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    void Update() {
        isAlive = _pc.isAlive;

        if (hasBall) {
            beamExists = false;
            Destroy(currentBeam);
            CancelInvoke("ManageBeam");
            Transform cam = transform.Find("Main Camera");
            ball.transform.position = transform.position
                                    + new Vector3(cam.forward.x, cam.forward.y/ ballFollowCamUpScalar, cam.forward.z) * holdDistance;
            ball.transform.rotation = Quaternion.Euler(0f, cam.eulerAngles.y+180f, 0f);
            /*
            Color ballMatColor = ballRenderer.sharedMaterial.color;
            ballMatColor.a = 0.5f;
            Debug.Log(ballRenderer.sharedMaterial.color);
            ballRenderer.sharedMaterial.color = ballMatColor;
            Debug.Log(ballRenderer.sharedMaterial.color); */
        }
        else {
            Invoke("ManageBeam", 2.5f);
        }
        if(GameObject.Find("/SceneHandler").GetComponent<SceneHandler>().currentState == BB_Utils.GameState.Running)
            if (Input.GetMouseButtonDown(0) && isAlive && hasBall)
                DispenseBall(true,throwStrenth);
            if (Input.GetMouseButtonDown(1) && isAlive && hasBall) 
                DispenseBall(false,dropStrength);

        if(hasBall && !isAlive)
            DispenseBall(false,dropStrength);
    }

    void OnCollisionStay(Collision col) {
        if (isAlive) {
            if (acceptCollisions) {
                if (col.gameObject == ball) {
                    //Debug.Log("Player&Ball Collide");
                    GrabBall(ball);
                }
            }
            else {
                if (col.gameObject == ball) {
                    //Debug.Log("AntiCollide");
                    //Physics.IgnoreCollision(GetComponent<Collider>(), col.collider);
                }
            }
        }
    }

    void GrabBall(GameObject heldBall) {
        if (!hasBall) {
            hasBall = true;
            heldBall.GetComponent<BallHandler>().ToggleFreeze();
            heldBall.GetComponent<AudioSource>().Stop();
            //gameObject.transform.Find("Blue Beam/Cylinder").gameObject.GetComponent<MeshRenderer>().enabled = true;
        }
    }

    public void DispenseBall(bool doThrow, float strength) {
        StartCoroutine(DropBall(doThrow,strength));
    }

    private IEnumerator DropBall(bool doThrow, float strength) { 
        if (hasBall) {
            hasBall = false;
            BallHandler _bh = ball.GetComponent<BallHandler>();
            Rigidbody _brb = ball.GetComponent<Rigidbody>();
            //gameObject.transform.Find("Blue Beam/Cylinder").gameObject.GetComponent<MeshRenderer>().enabled = false;
            Transform cam = transform.Find("Main Camera");

            _bh.ToggleFreeze();
            acceptCollisions = false;
            Vector3 towardsCam = new Vector3(cam.forward.x, cam.forward.y / verticalThrowScalar, cam.forward.z);
            if (doThrow) {
                _bh.Throw(towardsCam, strength, false);
                yield return new WaitForSeconds(postThrowCooldown);
            }
            else {
                ball.transform.position = new Vector3(ball.transform.position.x, ball.transform.position.y+1.5f, ball.transform.position.z);
                towardsCam.y = -1.5f;
                _bh.Throw(towardsCam, strength, true);
                yield return new WaitForSeconds(postThrowCooldown / 2f);
            }
            acceptCollisions = true;
        }
    }

    void ManageBeam() {
        if (!beamExists && !hasBall) {
            beamExists = true;
            currentBeam = Instantiate(beam, Vector3.zero, Quaternion.identity);
            
        }
        if (!hasBall) {
            currentBeam.transform.position = new Vector3(ball.transform.position.x,
                                                         ball.transform.position.y + currentBeam.transform.Find("Cylinder").localScale.y,
                                                         ball.transform.position.z);
        }
    }
}
