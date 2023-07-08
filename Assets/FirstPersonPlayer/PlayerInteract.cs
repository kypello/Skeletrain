using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerInteract : MonoBehaviour
{
    public bool crosshairEnabled = false;
    public RectTransform crosshair;
    public Image crosshairImage;
    public float interactRange = 4f;
    public bool control = true;
    public LayerMask interactLayer;

    public TMP_Text interactText;
    Collider lookingAtCurrently = null;

    void Update() {
        if (crosshairEnabled) {
            crosshairImage.enabled = control;
            interactText.enabled = control;
        }
        
        RaycastHit hit;

        if (control && Physics.Raycast(transform.position, transform.forward, out hit, interactRange, interactLayer, QueryTriggerInteraction.Collide)) {
            if (crosshairEnabled) {
                crosshair.sizeDelta = Vector2.one * 12;
                if (hit.collider != lookingAtCurrently) {
                    interactText.text = hit.collider.GetComponent<Interactable>().InteractCommand;
                    lookingAtCurrently = hit.collider;
                }
                
            }

            if (Input.GetMouseButtonDown(0)) {
                StartCoroutine(hit.collider.GetComponent<Interactable>().Interact());
            }
        }
        else {
            if (crosshairEnabled) {
                crosshair.sizeDelta = Vector2.one * 4;
                interactText.text = "";
                lookingAtCurrently = null;
            }
        }
    }
}
