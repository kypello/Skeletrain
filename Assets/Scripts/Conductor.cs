using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Conductor : MonoBehaviour, Interactable
{
    public GameObject wall;
    public AudioSource boing;
    public AudioSource music;
    public AudioSource dramatic;
    public Animation fade;
    public TMP_Text resultText;
    public Animation textAnim;

    public Transform head;

    public GameManager gameManager;

    public SuspectAvatar ejectedSuspect;

    public IEnumerator Interact() {
        gameManager.player.fullControl = false;

        DialogueBox dialogueBox = gameManager.dialogueBox;
        DialogueChoice dialogueChoice = gameManager.dialogueChoice;

        yield return gameManager.player.playerLook.LookAt(head.position);

        string[] names = new string[7];
        for (int i = 0; i < 6; i++) {
            names[i] = "- " + gameManager.GetSuspect(i).name;
        }
        names[6] = "- I'm not sure.";

        yield return dialogueBox.Display(new string[]{"Do you know who the necromancer is?"});

        yield return dialogueChoice.GetChoice(names);

        if (dialogueChoice.chosenChoice == 6) {
            yield return dialogueBox.Display(new string[]{"Let me know when you've figured it out."});

            gameManager.player.fullControl = true;
        }
        else {
            Suspect chosenSuspect = gameManager.GetSuspect(dialogueChoice.chosenChoice);

            foreach (SuspectAvatar suspect in FindObjectsOfType<SuspectAvatar>()) {
                if (suspect.suspect == chosenSuspect) {
                    suspect.gameObject.SetActive(false);
                    break;
                }
            }

            ejectedSuspect.gameObject.SetActive(true);
            ejectedSuspect.suspect = chosenSuspect;
            ejectedSuspect.SetUpAppearance();
            wall.SetActive(false);

            

            string kickOff = "We'd better kick [gender] off the train then.";
            kickOff = kickOff.Replace("[gender]", Timeline.pronouns[(int)chosenSuspect.gender, (int)Timeline.Pronoun.Object]);

            music.Stop();

            yield return dialogueBox.Display(new string[]{kickOff});

            dramatic.Play();

            yield return gameManager.player.playerLook.LookAt(ejectedSuspect.transform.position + Vector3.up * 3f + Vector3.right * 2f);

            yield return new WaitForSeconds(1.5f);
            boing.Play();
            Rigidbody rb = ejectedSuspect.GetComponent<Rigidbody>();
            rb.isKinematic = false;
            rb.AddForce(Vector3.right * 1000f + Vector3.up * 500f);
            rb.AddTorque(Vector3.forward * -500f);

            yield return new WaitForSeconds(0.5f);
            rb.AddForce(Vector3.forward * -1000f);

            yield return new WaitForSeconds(4f);

            fade.Play("FadeOut");
            yield return new WaitForSeconds(0.5f);
            if (chosenSuspect.isNecromancer) {
                resultText.text = "(" + Timeline.pronouns[(int)chosenSuspect.gender, (int)Timeline.Pronoun.Subject] + " was the necromancer.)";
            }
            else {
                resultText.text = "(" + Timeline.pronouns[(int)chosenSuspect.gender, (int)Timeline.Pronoun.Subject] + " was not the necromancer.)";
            }
            textAnim.Play();
            
        }

        
    }

    public string InteractCommand {
        get {
            return "Talk to Conductor";
        }
    }
}
