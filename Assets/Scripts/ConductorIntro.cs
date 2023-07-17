using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConductorIntro : SkeletonCharacter
{
    public Transform livingPerson;

    public AudioSource music;

    public GameObject endConductor;

    public MeshRenderer headRend;
    public Material skullClosed;
    public Material skullOpen;

    public IEnumerator Intro() {
        gameManager.player.fullControl = false;

        yield return gameManager.player.playerLook.LookAt(livingPerson.position);

        yield return gameManager.dialogueBox.Display(new string[]{"Alright, detective.", "This is the scene of the crime right here.", "Some poor skeleton has been brought back to life."}, skullClosed, skullOpen, headRend, pitch);

        yield return gameManager.player.playerLook.LookAt(head.position);

        yield return gameManager.dialogueBox.Display(new string[]{"Which means there's a sneaky necromancer somewhere on board this train.", "All passengers have returned to their assigned seats.", "Take your time interrogating them, and come and find me at the front of the train when you know who our necromancer is!", "Also, make sure the mouse sensitivity feels right for you.", "If it doesn't, you can press the up and down arrow keys to adjust it.", "That's everything! Good luck detective!"}, skullClosed, skullOpen, headRend, pitch);

        yield return gameManager.player.playerLook.LookAt(livingPerson.position);

        gameManager.player.fullControl = true;

        music.Play();

        endConductor.SetActive(true);

        gameObject.SetActive(false);
    }
}
