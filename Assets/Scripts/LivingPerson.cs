using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LivingPerson : MonoBehaviour, Interactable
{
    DialogueBox dialogueBox;
    Player player;
    PlayerLook playerLook;
    PlayerInteract playerInteract;

    public Transform head;

    void Start() {
        dialogueBox = FindObjectOfType<DialogueBox>();
        player = FindObjectOfType<Player>();
        playerLook = FindObjectOfType<PlayerLook>();
        playerInteract = FindObjectOfType<PlayerInteract>();
    }

    public string InteractCommand {
        get {
            return "Look at living person";
        }
    }

    public IEnumerator Interact() {
        player.control = false;
        playerLook.control = false;
        playerInteract.control = false;

        yield return playerLook.LookAt(head.position);

        yield return dialogueBox.Display(new string[]{"Everyone knows the living don't talk, silly!"});

        player.control = true;
        playerLook.control = true;
        playerInteract.control = true;
    }   
}
