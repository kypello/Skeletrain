using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    Mystery mystery;
    public Material[] carriageColors;
    public MeshRenderer[] carriageRends;
    public SuspectAvatar suspectPrefab;

    void Start() {
        mystery = new Mystery();

        mystery.GenerateMystery();
        mystery.PrintOutCarriages();
        mystery.PrintOutTimeline();
        
        for (int i = 0; i < mystery.carriageCount; i++) {
            carriageRends[i].sharedMaterial = carriageColors[(int)mystery.carriages[i].color];

            for (int j = 0; j < mystery.carriages[i].passengers.Count; j++) {
                SuspectAvatar newSuspect = Instantiate(suspectPrefab, carriageRends[i].transform.GetChild(j).position, Quaternion.Euler(Vector3.up * -90f));
                newSuspect.suspect = mystery.carriages[i].passengers[j];
                newSuspect.SetUpAppearance();
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
}
