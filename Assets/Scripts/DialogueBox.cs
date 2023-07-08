using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueBox : MonoBehaviour
{
    public GameObject textBoxObject;
    public TMP_Text textBox;

    public IEnumerator Display(string[] dialogues) {
        textBoxObject.SetActive(true);

        foreach (string dialogue in dialogues) {
            textBox.text = "";
            bool firstFrame = true;

            for (int i = 0; i <= dialogue.Length; i++) {
                if (!firstFrame && Input.GetMouseButtonDown(0)) {
                    i = dialogue.Length;
                }


                textBox.text = dialogue.Substring(0, i);
                yield return null;

                firstFrame = false;
            }

            while (!Input.GetMouseButtonDown(0)) {
                yield return null;
            }
        }

        textBoxObject.SetActive(false);
    }
}
