using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhistleChain : MonoBehaviour, Interactable
{
    public Collider collider;
    public Animation anim;
    public AudioSource[] whistleNoises;
    
    public string InteractCommand {
        get {
            return "Pull chain";
        }
    }

    public IEnumerator Interact() {
        anim.Play();
        whistleNoises[Random.Range(0, whistleNoises.Length)].Play();
        collider.enabled = false;
        yield return new WaitForSeconds(0.5f);
        collider.enabled = true;
    }
}
