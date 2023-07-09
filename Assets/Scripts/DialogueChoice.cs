using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueChoice : MonoBehaviour
{
    public TMP_Text[] choiceTextBoxes;
    Animation[] choiceTextAnims;
    MouseOver[] mouseOvers;
    public RectTransform boxBackground;
    public GameObject choicesBoxObject;

    public Color defaultColor = Color.white;
    public Color highlightColor = Color.yellow;

    public int chosenChoice = 0;

    void Start() {
        choiceTextAnims = new Animation[7];
        mouseOvers = new MouseOver[7];
        for (int i = 0; i < choiceTextBoxes.Length; i++) {
            choiceTextAnims[i] = choiceTextBoxes[i].GetComponent<Animation>();
            mouseOvers[i] = choiceTextBoxes[i].GetComponent<MouseOver>();
        }

        //StartCoroutine(GetChoice(new string[]{"Choice 1", "And another choice", "And a third choice right here"}));
    }

    public IEnumerator GetChoice(string[] choiceTexts) {
        choicesBoxObject.SetActive(true);

        int startIndex = 7 - choiceTexts.Length;

        for (int i = 0; i < 7; i++) {
            choiceTextBoxes[i].enabled = false;
        }

        boxBackground.sizeDelta = new Vector2(2000f, 100f * choiceTexts.Length + 50f);

        for (int i = startIndex; i < 7; i++) {
            choiceTextBoxes[i].enabled = true;
            mouseOvers[i].mouseOver = false;
            choiceTextBoxes[i].text = choiceTexts[i - startIndex];
            choiceTextAnims[i].Play();
            yield return new WaitForSeconds(0.05f);
        }
        yield return new WaitForSeconds(0.45f);

        Cursor.lockState = CursorLockMode.None;

        while (true) {
            for (int i = startIndex; i < 7; i++) {
                if (mouseOvers[i].mouseOver) {
                    choiceTextBoxes[i].color = highlightColor;

                    if (Input.GetMouseButtonDown(0)) {
                        chosenChoice = i - startIndex;
                        choicesBoxObject.SetActive(false);
                        Cursor.lockState = CursorLockMode.Locked;
                        yield break;
                    }
                }
                else {
                    choiceTextBoxes[i].color = defaultColor;
                }
            }
            yield return null;
        }
    }
}
