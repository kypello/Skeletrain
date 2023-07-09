using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldItem : MonoBehaviour, Interactable
{
    public Item item;
    public Renderer renderer;
    bool found = false;

    public Animation flash;
    public AudioSource flashSound;

    public string InteractCommand {
        get {
            return "Take picture of " + item.name;
        }
    }
    
    void Start() {
        flash = GameObject.Find("Fade").GetComponent<Animation>();
    }

    public IEnumerator Interact() {
        flash["FadeIn"].time = 0f;
        flash.Play();
        flashSound.Play();
        if (!found) {
            Item.itemsFound.Add(item);
            found = true;
        }
        yield break;
    }
}
