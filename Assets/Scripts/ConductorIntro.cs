using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConductorIntro : MonoBehaviour
{
    public GameManager gameManager;

    public Transform conductorHead;
    public Transform livingPerson;

    public AudioSource music;

    DialogueChoice dialogueChoice;
    DialogueBox dialogueBox;
    Player player;
    PlayerLook playerLook;
    PlayerInteract playerInteract;

    public GameObject endConductor;

    void Start() {
        dialogueChoice = FindObjectOfType<DialogueChoice>();
        dialogueBox = FindObjectOfType<DialogueBox>();
        player = FindObjectOfType<Player>();
        playerLook = FindObjectOfType<PlayerLook>();
        playerInteract = FindObjectOfType<PlayerInteract>();
    }

    public IEnumerator Intro() {
        player.control = false;
        playerLook.control = false;
        playerInteract.control = false;

        yield return playerLook.LookAt(livingPerson.position);

        yield return dialogueBox.Display(new string[]{"Alright, detective.", "This is the scene of the crime right here.", "Some poor skeleton has been brought back to life."});

        yield return playerLook.LookAt(conductorHead.position);

        yield return dialogueBox.Display(new string[]{"Which means there's a sneaky necromancer somewhere on board this train.", "All passengers have returned to their assigned seats.", "Take your time interrogating them, and come and find me at the front of the train when you know who our necromancer is!", "Also, make sure the mouse sensitivity feels right for you.", "If it doesn't, you can press the up and down arrow keys to adjust it.", "That's everything! Good luck detective!"});

        yield return playerLook.LookAt(livingPerson.position);

        player.control = true;
        playerLook.control = true;
        playerInteract.control = true;

        music.Play();

        endConductor.SetActive(true);

        gameObject.SetActive(false);
    }
}
