using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float cameraSpeed;
    [SerializeField] private float distance;
    [SerializeField] private Vector3 offset;

    public float current_distance;

    float x = 0;
    float y = 0;

    RaycastHit hit;
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        current_distance = distance;
    }

    void Update()
    {
        x += Input.GetAxis("Mouse X") * cameraSpeed * Time.deltaTime;
        y += Input.GetAxis("Mouse Y") * cameraSpeed * Time.deltaTime;

        y = Mathf.Clamp(y, -10, 60);
        Debug.Log(y);
        Quaternion rot = Quaternion.Euler(y, -x, 0);
        transform.position = rot * new Vector3(0, 0, -current_distance) + target.position + offset;
        transform.LookAt(target.position + offset);
    }

    void FixedUpdate()
    {
        if (Physics.SphereCast(target.position, 1.5f, transform.position, out hit, distance))
        {
            if (hit.transform.tag != "Player" && hit.distance > 0.5f && hit.distance < distance)
            {
                //current_distance = Mathf.Lerp(current_distance, hit.distance, cameraSpeed * Time.fixedDeltaTime);
                current_distance = hit.distance;
            }
        }
        else
        {
            current_distance = Mathf.Lerp(current_distance, distance, Time.fixedDeltaTime);
        }
    }

    void OnDrawGizmos()
    {
        if (target != null)
        {
            Gizmos.color = Color.red;
            // Dibujar una esfera en la posición de la cámara
            Gizmos.DrawWireSphere(transform.position, 1.5f);

            Gizmos.color = Color.green;
            // Dibujar una línea desde el jugador hacia la posición de la cámara
            Gizmos.DrawLine(target.position, transform.position);

            if (hit.collider != null)
            {
                Gizmos.color = Color.blue;
                // Dibujar una esfera en el punto de colisión
                Gizmos.DrawWireSphere(hit.point, 1.5f);
            }
        }
    }
}