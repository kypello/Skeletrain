using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    Mystery mystery;
    public Material[] carriageColors;
    public MeshRenderer[] carriageRends;
    public Transform[] carriageTransforms;
    public SuspectAvatar suspectPrefab;
    public WorldItem worldItemPrefab;

    [System.Serializable]
    public struct ItemTemplate {
        public string itemName;
        public Material itemMaterial;
    }
    public ItemTemplate[] itemTemplates;

    void Start() {
        mystery = new Mystery();

        mystery.GenerateMystery();
        //mystery.PrintOutCarriages();
        //mystery.PrintOutTimeline();

        SpawnSuspects();

        SpawnItems();

        foreach (Suspect suspect in mystery.suspects) {
            string[] testimony = mystery.trueTimeline.GenerateTestimony(suspect);
            //Debug.Log(suspect.name + "'s testimony:");
            foreach (string statement in testimony) {
                //Debug.Log(statement);
            }
        }

        /*
        for (int i = 0; i < mystery.suspects.Length; i++) {
            SuspectAvatar newSuspect = Instantiate(suspectPrefab, carriageRends[0].transform.GetChild(i).position, Quaternion.Euler(Vector3.up * -90f));
            newSuspect.suspect = mystery.suspects[i];
            newSuspect.SetUpAppearance();
        }
        */
    }

    void SpawnSuspects() {
        for (int i = 0; i < mystery.carriageCount; i++) {
            carriageRends[i].sharedMaterial = carriageColors[(int)mystery.carriages[i].color];

            for (int j = 0; j < mystery.carriages[i].passengers.Count; j++) {
                SuspectAvatar newSuspect = Instantiate(suspectPrefab, carriageRends[i].transform.GetChild(j).position, Quaternion.Euler(Vector3.up * -90f));
                newSuspect.suspect = mystery.carriages[i].passengers[j];
                newSuspect.gameManager = this;
                newSuspect.SetUpAppearance();
            }
        }
    }

    void SpawnItems() {
        foreach (Item item in mystery.items) {
            Transform carriage = carriageTransforms[item.carriage.index];
            Vector3 spawnPoint;
            do {
                spawnPoint = new Vector3(Random.Range(carriage.position.x - 3.5f, carriage.position.x + 3.5f), 1f, Random.Range(carriage.position.z - 8.5f, carriage.position.z + 8.5f));
            } while (Physics.SphereCast(new Ray(spawnPoint + Vector3.up * 10f, Vector3.down), 1f, 20f, 1<<6, QueryTriggerInteraction.Collide));

            WorldItem newItem = Instantiate(worldItemPrefab, spawnPoint, Quaternion.Euler(Vector3.up * Random.Range(0f, 360f)));
            newItem.item = item;

            foreach (ItemTemplate itemTemplate in itemTemplates) {
                if (itemTemplate.itemName == item.name) {
                    newItem.renderer.sharedMaterial = itemTemplate.itemMaterial;
                }
            }
        }
    }

    public string[] GetTestimony(Suspect suspect) {
        if (suspect.isNecromancer) {
            return mystery.falseTimeline.GenerateTestimony(suspect);
        }
        return mystery.trueTimeline.GenerateTestimony(suspect);
    }
}
