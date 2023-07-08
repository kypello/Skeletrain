using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Move {
    public Suspect movingSuspect;
    public Carriage destinationCarriage;
    public bool talksWithPassenger;
    public bool isNecroMove;
    public Suspect passengerTalkedTo;

    public string GetSummary() {
        string summery = movingSuspect.name + " went to the " + Carriage.ColorAsString(destinationCarriage.color) + " carriage";
        if (talksWithPassenger) {
            summery += " and spoke with " + passengerTalkedTo.name;
        }
        if (isNecroMove) {
            summery += " and commits the resurrection";
        }
        return summery;
    }
}

public class Timeline
{
    public List<Move> moves;

    public Timeline() {
        moves = new List<Move>();
    }
}
