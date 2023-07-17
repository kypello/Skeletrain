using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuspectAvatar : SkeletonCharacter, Interactable
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

    [System.Serializable]
    public struct SkullMaterialPair {
        public Material closedMouthMat;
        public Material openMouthMat;
    }

    public SkullMaterialPair[] skullMaterialPairs;
    SkullMaterialPair skullMaterials;
    public MeshRenderer skull;

    public GameObject[] hats;
    public Transform hatSpawnPoint;

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

        Random.seed = suspect.seed;

        skullMaterials = skullMaterialPairs[Random.Range(0, skullMaterialPairs.Length)];
        skull.sharedMaterial = skullMaterials.closedMouthMat;

        if (Random.Range(0, 3) != 0) {
            Instantiate(hats[Random.Range(0, hats.Length)], hatSpawnPoint);
        }

        if (suspect.gender == Suspect.Gender.Female) {
            pitch = Random.Range(0.8f, 2.5f);
        }
        else {
            pitch = Random.Range(0.5f, 1.4f);
        }
    }

    public IEnumerator Interact() {
        gameManager.player.fullControl = false;

        yield return gameManager.player.playerLook.LookAt(head.position);

        List<string> dialogueChoices = new List<string>();
        dialogueChoices.Add("- Tell me everything you remember.");

        foreach (Item item in Item.itemsFound) {
            dialogueChoices.Add("- What do you know about this " + item.name + "?");
        }

        yield return gameManager.dialogueChoice.GetChoice(dialogueChoices.ToArray());

        if (gameManager.dialogueChoice.chosenChoice == 0) {
            yield return gameManager.dialogueBox.Display(gameManager.GetTestimony(suspect), skullMaterials.closedMouthMat, skullMaterials.openMouthMat, skull, pitch);
        }
        else {
            yield return gameManager.dialogueBox.Display(new string[]{suspect.itemResponses[Item.itemsFound[gameManager.dialogueChoice.chosenChoice-1]]}, skullMaterials.closedMouthMat, skullMaterials.openMouthMat, skull, pitch);
        }

        

        gameManager.player.fullControl = true;
    }

    public string InteractCommand {
        get {
            return "Talk to <b>" + suspect.name;
        }
    }
}
