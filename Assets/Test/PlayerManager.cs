using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.SceneManagement;

public class PlayerManager : MonoBehaviour {
    
    // Player
    NavMeshAgent navMeshAgent;
    Vector3 lastCheckpointPos;
    Quaternion lastCheckpointRot;
    float playerSpeed;
    public Transform goal;
    public GameObject playAgainText;

    // Camera
    Camera cam;
    public float camSpeed = 1.0f;
    float radius = 0.0f;

    bool gameFinished = false;

    // Start is called before the first frame update
    void Start() {
        cam = gameObject.GetComponentInChildren<Camera>();
        cam.gameObject.transform.LookAt(transform);
        radius = Mathf.Abs(cam.transform.localPosition.z);
        lastCheckpointPos = transform.position;
        lastCheckpointRot = transform.rotation;

        navMeshAgent = GetComponent<NavMeshAgent>();
        playerSpeed = navMeshAgent.speed;

        navMeshAgent.SetDestination(goal.position);
        navMeshAgent.speed = 0;
    }

    // Update is called once per frame
    void Update() {
        if (Input.touchCount > 0) {
            Touch touch = Input.GetTouch(0);

            if (!gameFinished)
            switch (touch.phase) {
                case TouchPhase.Began:
                    navMeshAgent.speed = playerSpeed;
                    break;
                case TouchPhase.Ended:
                    navMeshAgent.speed = 0;
                    break;
            }
            else 
                if (touch.phase == TouchPhase.Began)
                    SceneManager.LoadScene("MainScene");
        }
    }

    IEnumerator MoveCamera(float from, float target) {
        bool reached = false;
        float angle = from;
        float angleDelta = 0.0f;
        while (!reached) {
            if (target < from) { // To Normal
                if (angle <= target) reached = true;
            }
            else { // To Lateral
                if (angle >= target) reached = true;
            }
            angleDelta += Time.deltaTime * camSpeed;
            angle = Mathf.Lerp(from, target, angleDelta);
            cam.gameObject.transform.localPosition = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad) * radius, cam.gameObject.transform.localPosition.y, Mathf.Sin(angle * Mathf.Deg2Rad) * radius);
            cam.gameObject.transform.LookAt(transform);
            yield return null;
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag == "CheckPoint") {
            lastCheckpointPos = gameObject.transform.position;
            lastCheckpointRot = gameObject.transform.rotation;
        }
        if (other.gameObject.tag == "Obstacle") {
            gameObject.transform.position = lastCheckpointPos;
            gameObject.transform.rotation = lastCheckpointRot;
        }
        if (other.gameObject.tag == "Lateral") {
            StartCoroutine(MoveCamera(-90.0f, 0.0f));
        }
        if (other.gameObject.tag == "Goal") {
            StartCoroutine(MoveCamera(-90.0f, 90.0f));
            gameFinished = true;
            playAgainText.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.gameObject.tag == "Lateral") {
            StartCoroutine(MoveCamera(0.0f, -90.0f));
        }
    }
}
