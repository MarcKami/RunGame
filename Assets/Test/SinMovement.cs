using UnityEngine;

public class SinMovement : MonoBehaviour {

    Vector3 startPosition;
    [SerializeField, Range(0.0f,5.0f)]
    float speed = 1.0f;
    [SerializeField, Range(1.0f, 5.0f)]
    float multiplier = 1.0f;

    void Start() {
        startPosition = transform.position;
    }

    void Update() {
        transform.position = startPosition + new Vector3(0.0f, Mathf.Abs(Mathf.Sin(Time.time * speed)) * multiplier, 0.0f);
    }
}
