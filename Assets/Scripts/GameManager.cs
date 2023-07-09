using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Mystery mystery;
    public Material[] carriageColors;
    public MeshRenderer[] carriageRends;
    public Transform[] carriageTransforms;
    public SuspectAvatar suspectPrefab;
    public WorldItem worldItemPrefab;

    public Transform livingPerson;
    public ConductorIntro conductorIntro;
    public Transform player;

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

        Transform spawnPoint = carriageTransforms[mystery.necroCarriage.index].GetChild(Random.Range(0, carriageTransforms[mystery.necroCarriage.index].childCount));
        
        player.position = carriageTransforms[mystery.necroCarriage.index].position + Vector3.up * 2f;
        player.rotation = Quaternion.LookRotation(spawnPoint.position - carriageTransforms[mystery.necroCarriage.index].position);

        livingPerson.position = spawnPoint.position;
        livingPerson.rotation = Quaternion.LookRotation(carriageTransforms[mystery.necroCarriage.index].position - spawnPoint.position);

        
        
        StartCoroutine(conductorIntro.Intro());


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
        List<Transform> usedSpawnPoints = new List<Transform>();

        for (int i = 0; i < mystery.carriageCount; i++) {
            carriageRends[i*2].sharedMaterial = carriageColors[(int)mystery.carriages[i].color];
            carriageRends[i*2+1].sharedMaterial = carriageColors[(int)mystery.carriages[i].color];

            for (int j = 0; j < mystery.carriages[i].passengers.Count; j++) {
                Transform spawnPoint;
                do {
                    spawnPoint = carriageTransforms[i].GetChild(Random.Range(0, carriageTransforms[i].childCount));
                } while (usedSpawnPoints.Contains(spawnPoint));

                SuspectAvatar newSuspect = Instantiate(suspectPrefab, spawnPoint.position + new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f)), Quaternion.Euler(spawnPoint.localEulerAngles + Vector3.up * Random.Range(-15f, 15f)));
                usedSpawnPoints.Add(spawnPoint);
                newSuspect.suspect = mystery.carriages[i].passengers[j];
                newSuspect.gameManager = this;
                newSuspect.SetUpAppearance(true);
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
