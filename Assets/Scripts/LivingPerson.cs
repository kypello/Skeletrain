using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LivingPerson : MonoBehaviour, Interactable
{
    public Transform head;

    public string InteractCommand {
        get {
            return "Look at living person";
        }
    }

    public IEnumerator Interact() {
        GameManager.instance.player.fullControl = false;

        yield return GameManager.instance.player.playerLook.LookAt(head.position);

        yield return GameManager.instance.dialogueBox.Display(new string[]{"Everyone knows the living don't talk, silly!"});

        GameManager.instance.player.fullControl = true;
    }   
}
