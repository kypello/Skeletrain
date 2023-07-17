using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueBox : MonoBehaviour
{
    public GameObject textBoxObject;
    public TMP_Text textBox;
    public float lettersPerSecond = 60f;
    public AudioSource audioSource;

    public IEnumerator Display(string[] dialogues) {
        yield return Display(dialogues, null, null, null, -1f);
    }

    public IEnumerator Display(string[] dialogues, Material closedMouthMat, Material openMouthMat, Renderer rend, float pitch) {
        textBoxObject.SetActive(true);

        float letterDelta = 1f / lettersPerSecond;

        float pitchChange = pitch / 6f;

        foreach (string dialogue in dialogues) {
            textBox.text = "";
            bool firstFrame = true;

            float t = 0f;
            int character = 0;

            while (character < dialogue.Length) {
                float totalDelta = 0f;
                bool skipped = false;
                do {
                    yield return null;
                    totalDelta += Time.deltaTime;

                    if (Input.GetMouseButtonDown(0)) {
                        character = dialogue.Length;
                        skipped = true;
                        break;
                    }
                } while (totalDelta < letterDelta);

                if (!skipped) {
                    t += totalDelta * lettersPerSecond;
                    character = Mathf.Min(Mathf.FloorToInt(t), dialogue.Length);
                }

                if (rend != null) {
                    if ((character / 3) % 2 == 0 || character == dialogue.Length) {
                        rend.sharedMaterial = closedMouthMat;
                    }
                    else {
                        rend.sharedMaterial = openMouthMat;
                        audioSource.pitch = pitch + Random.Range(-pitchChange, pitchChange);
                        audioSource.Play();
                    }
                }

                textBox.text = dialogue.Substring(0, character);
            }

            do {
                yield return null;
            } while (!Input.GetMouseButtonDown(0));
        }

        textBoxObject.SetActive(false);
    }
}
