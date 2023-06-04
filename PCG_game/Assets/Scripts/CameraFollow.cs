using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] Transform target;
    private float offset;
    /*
    [SerializeField] float smoothSpeed = 5f;

    private void FixedUpdate()
    {
        if (target != null)
        {
            Vector3 desiredPosition = new Vector3(target.position.x, target.position.y, transform.position.z);
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
            transform.position = smoothedPosition;
        }
    }
    */
    private void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
        offset = transform.position.z - target.position.z;
    }
    private void Update()
    {
        if(target.position.x >= 9 && target.position.y >= 5.2f && target.position.x <= 491)
            transform.position = new Vector3(target.position.x, target.transform.position.y, target.transform.position.z + offset);
    }
}
