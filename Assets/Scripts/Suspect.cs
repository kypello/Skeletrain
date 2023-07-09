using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Suspect
{
    public enum Gender {Female, Male}
    public Gender gender;
    public string name;
    public Item item;
    public Carriage carriage;
    public bool isNecromancer;
    public enum SuspectColor {Red = 0, Blue = 1, Yellow = 2, Green = 3, Brown = 4, White = 5}
    public SuspectColor topColor;
    public SuspectColor bottomColor;
    public List<Item> knownItems;
    public Dictionary<Item, string> itemResponses;
    public List<Suspect> knownPassengers;
    public Dictionary<Suspect, string> nicknames;
    public int seed;

    public static string ColorAsString(SuspectColor color) {
        switch (color) {
            case SuspectColor.Red:
                return "dotted red";
            case SuspectColor.Blue:
                return "striped blue";
            case SuspectColor.Yellow:
                return "yellow checkered";
            case SuspectColor.Green:
                return "green grid-patterned";
            case SuspectColor.Brown:
                return "brown maze-patterned";
            case SuspectColor.White:
                return "plain white";
            default:
                return "invisible";
        }
    }
}
