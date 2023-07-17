using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonCharacter : MonoBehaviour
{
    public Transform head;

    public GameManager gameManager;

    public Transform skeleton;

    public bool followPlayer = true; 

    public float bodyRotateSpeed = 180f;
    public float bodyRotateMinDist = 10f;
    public float bodyRotateMinAngle = 15f;
    public float skullRotateSpeed = 180f;

    public float pitch = 1f;

    void Update() {
        if (followPlayer) {
            Vector3 playerPosition = new Vector3(gameManager.player.transform.position.x, 0f, gameManager.player.transform.position.z);
            if (Vector3.Distance(transform.position, gameManager.player.transform.position) < bodyRotateMinDist) {
                if (Vector3.Angle(skeleton.forward, playerPosition - transform.position) > bodyRotateMinAngle) {
                    skeleton.rotation = Quaternion.LookRotation(Vector3.RotateTowards(skeleton.forward, playerPosition - transform.position, bodyRotateSpeed * Mathf.Deg2Rad * Time.deltaTime, 0f));
                }
                head.rotation = Quaternion.LookRotation(Vector3.RotateTowards(head.forward, gameManager.player.playerLook.transform.position - head.position, skullRotateSpeed * Mathf.Deg2Rad * Time.deltaTime, 0f));
            }
        }
    }
}
