using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct FootstepSoundSet {
    public LayerMask layerMask;
    public AudioSource landSound;
    public GameObject footstepSoundContainer;
    AudioSource[] footstepSounds;

    public void SetUpFootstepSounds() {
        footstepSounds = footstepSoundContainer.GetComponents<AudioSource>();
    }

    public void PlayFootstepSound() {
        footstepSounds[Random.Range(0, footstepSounds.Length)].Play();
    }
}

public class Player : MonoBehaviour
{
    public float speed = 8f;
    public float gravity = -5f;
    public float jumpStrength = 1.6f;
    float yVelocity;

    public AudioSource jumpSound;
    public FootstepSoundSet[] footstepSoundSets;
    int footstepSoundSetIndex = 0;
    float footstepTimer = 0f;

    float vx;
    float vy;
    float vz;
    public float moveForce = 10f;

    bool wasGroundedLastFrame;

    public bool control = true;

    public PlayerLook playerLook;
    public PlayerInteract playerInteract;

    public bool fullControl {
        set {
            control = value;
            playerLook.control = value;
            playerInteract.control = value;
        }
    }

    CharacterController controller;

    Animation headBobAnim;

    void Awake() {
        controller = GetComponent<CharacterController>();
        headBobAnim = GetComponentInChildren<Animation>();

        
    }

    void Start() {
        for (int i = 0; i < footstepSoundSets.Length; i++) {
            footstepSoundSets[i].SetUpFootstepSounds();
        }
    }

    void Update()
    {
        bool input = false;

        if (control && Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D)) {
            input = true;
            vx = Mathf.Max(vx - Time.deltaTime * moveForce, -1f);
        }
        else if (control && Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.A)) {
            input = true;
            vx = Mathf.Min(vx + Time.deltaTime * moveForce, 1f);
        }
        else {
            if (vx > 0f) {
                vx = Mathf.Max(vx - Time.deltaTime * moveForce, 0f);
            }
            else if (vx < 0f) {
                vx = Mathf.Min(vx + Time.deltaTime * moveForce, 0f);
            }
        }

        if (control && Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.W)) {
            input = true;
            vz = Mathf.Max(vz - Time.deltaTime * moveForce, -1f);
        }
        else if (control && Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.S)) {
            input = true;
            vz = Mathf.Min(vz + Time.deltaTime * moveForce, 1f);
        }
        else {
            if (vz > 0f) {
                vz = Mathf.Max(vz - Time.deltaTime * moveForce, 0f);
            }
            else if (vz < 0f) {
                vz = Mathf.Min(vz + Time.deltaTime * moveForce, 0f);
            }
        }

        if (control && controller.isGrounded && Input.GetKeyDown(KeyCode.Space)) {
            jumpSound.Play();
            vy = jumpStrength;
        }
        else if (!controller.isGrounded && wasGroundedLastFrame && vy <= 0f) {
            vy = 0f;
        }
        else if (controller.isGrounded && !wasGroundedLastFrame) {
            footstepSoundSets[footstepSoundSetIndex].landSound.Play();
        }
        wasGroundedLastFrame = controller.isGrounded;


        Vector3 move = transform.right * vx + transform.forward * vz;

        if (Mathf.Sqrt((move.x * move.x) + (move.z * move.z)) > 1f) {
            move.Normalize();
        }

        if (controller.isGrounded && vy <= 0f) {
            vy = -10f;
        }
        else {
            vy += gravity * Time.deltaTime;
        }

        move += Vector3.up * vy;

        controller.Move(move * speed * Time.deltaTime);

        if (input && controller.isGrounded) {
            footstepTimer -= Time.deltaTime;
            if (footstepTimer <= 0f) {
                footstepTimer = 0.35f;
                footstepSoundSets[footstepSoundSetIndex].PlayFootstepSound();
            }
        }
        else {
            footstepTimer = 0.35f;
        }

        if (input && !headBobAnim.isPlaying) {
            headBobAnim.Play();
        }
    }

    void OnControllerColliderHit(ControllerColliderHit hit) {
        if (hit.moveDirection.y < -0.4f) {
            for (int i = 0; i < footstepSoundSets.Length; i++) {
                if ((footstepSoundSets[i].layerMask.value & 1<<hit.collider.gameObject.layer) > 0) {
                    footstepSoundSetIndex = i;
                    break;
                }
            }
        }
    }
}
