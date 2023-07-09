using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuspectAvatar : MonoBehaviour, Interactable
{
    public Suspect suspect;

    [System.Serializable]
    public struct Outfit {
        public Material dress;
        public Material suit;
    }
    public Outfit[] outfits;

    public MeshRenderer torso;
    public MeshRenderer leftLeg;
    public MeshRenderer rightLeg;
    public MeshRenderer dress;

    public Material[] skullMaterials;
    public MeshRenderer skull;

    public GameObject[] hats;
    public Transform hatSpawnPoint;

    DialogueChoice dialogueChoice;
    DialogueBox dialogueBox;
    Player player;
    PlayerLook playerLook;
    PlayerInteract playerInteract;

    public GameManager gameManager;

    void Start() {
        dialogueChoice = FindObjectOfType<DialogueChoice>();
        dialogueBox = FindObjectOfType<DialogueBox>();
        player = FindObjectOfType<Player>();
        playerLook = FindObjectOfType<PlayerLook>();
        playerInteract = FindObjectOfType<PlayerInteract>();
    }

    public void SetUpAppearance() {
        if (suspect.gender == Suspect.Gender.Female) {
            dress.enabled = true;
            leftLeg.enabled = false;
            rightLeg.enabled = false;
            torso.sharedMaterial = outfits[(int)suspect.topColor].dress;
            dress.sharedMaterial = outfits[(int)suspect.bottomColor].dress;
        }
        else {
            dress.enabled = false;
            leftLeg.enabled = true;
            rightLeg.enabled = true;
            torso.sharedMaterial = outfits[(int)suspect.topColor].suit;
            rightLeg.sharedMaterial = outfits[(int)suspect.bottomColor].suit;
            leftLeg.sharedMaterial = outfits[(int)suspect.bottomColor].suit;
        }

        skull.sharedMaterial = skullMaterials[Random.Range(0, skullMaterials.Length)];

        if (Random.Range(0, 3) != 0) {
            Instantiate(hats[Random.Range(0, hats.Length)], hatSpawnPoint);
        }
    }

    public IEnumerator Interact() {
        player.control = false;
        playerLook.control = false;
        playerInteract.control = false;

        List<string> dialogueChoices = new List<string>();
        dialogueChoices.Add("- Tell me everything you remember.");

        foreach (Item item in Item.itemsFound) {
            dialogueChoices.Add("- What do you know about this " + item.name + "?");
        }

        yield return dialogueChoice.GetChoice(dialogueChoices.ToArray());

        if (dialogueChoice.chosenChoice == 0) {
            yield return dialogueBox.Display(gameManager.GetTestimony(suspect));
        }
        else {
            yield return dialogueBox.Display(new string[]{suspect.itemResponses[Item.itemsFound[dialogueChoice.chosenChoice-1]]});
        }

        

        player.control = true;
        playerLook.control = true;
        playerInteract.control = true;
    }   

    public string InteractCommand {
        get {
            return "Talk to <b>" + suspect.name;
        }
    }
}
