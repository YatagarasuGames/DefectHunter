using UnityEngine;

public class ExclamationMark : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float rotationSpeed = 90f;
    [SerializeField] private float floatHeight = 0.1f;
    [SerializeField] private float floatSpeed = 3f;

    private Vector3 initialPosition;
    private float floatTimer;

    void Start()
    {
        initialPosition = transform.position;
    }

    void Update()
    {
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);

        floatTimer += Time.deltaTime * floatSpeed;
        float newY = initialPosition.y + Mathf.Sin(floatTimer) * floatHeight;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }
}
