using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerLook : MonoBehaviour
{
    public Transform player;
    public float sensitivity = 500f;
    float xRotation = 0f;

    public bool control = true;
    Vector3 lockOnPoint;

    bool lookingAtPoint = false;
    bool mouseMovedSinceUnlocking;

    public TMP_Text sensitivityText;

    float textTimer = -1f;

    void Update()
    {
        if (textTimer > 0f) {
            textTimer -= Time.deltaTime;
            sensitivityText.enabled = true;
            sensitivityText.text = "Sensitivity: " + Mathf.FloorToInt(sensitivity);
        }
        else {
            sensitivityText.enabled = false;
        }
        if (control) {
            Cursor.lockState = CursorLockMode.Locked;

            if (mouseMovedSinceUnlocking) {
                xRotation -= Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;
                xRotation = Mathf.Clamp(xRotation, -90f, 90f);
                transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

                player.Rotate(Vector3.up * Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime);
            }
            else {
                if (Input.GetAxis("Mouse X") != 0f || Input.GetAxis("Mouse Y") != 0f) {
                    mouseMovedSinceUnlocking = true;
                }
            }

            if (Input.GetKeyDown(KeyCode.UpArrow)) {
                sensitivity = Mathf.Min(sensitivity + 25f, 2000f);
                textTimer = 2f;
            }
            if (Input.GetKeyDown(KeyCode.DownArrow)) {
                sensitivity = Mathf.Max(sensitivity - 25f, 25f);
                textTimer = 2f;
            }
        }
        else {
            mouseMovedSinceUnlocking = false;
        }
    }

    public IEnumerator LookAt(Vector3 point) {
        while (lookingAtPoint) {
            yield return null;
        }

        lookingAtPoint = true;

        Quaternion targetRotation = Quaternion.LookRotation(point - transform.position);

        float dotProduct;

        do {
            if (control) {
                break;
            }

            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 360f * Time.deltaTime);

            

            /*
            Vector3 delta = Vector3.RotateTowards(transform.forward, targetDir, (dotProduct * -dotProduct + 1f) * 2f * Mathf.PI * Time.deltaTime, 0f);

            transform.localRotation = Quaternion.LookRotation(delta);
            transform.localRotation = Quaternion.Euler(Vector3.right * transform.localEulerAngles.x);

            player.localRotation = Quaternion.LookRotation(delta);
            player.localRotation = Quaternion.Euler(Vector3.up * player.localEulerAngles.y);

            xRotation = Mathf.Repeat(transform.localEulerAngles.x + 90f, 180f) - 90f;
            */

            yield return null;
        } while (Quaternion.Angle(transform.rotation, targetRotation) > 1f);

        lookingAtPoint = false;
    }
}
