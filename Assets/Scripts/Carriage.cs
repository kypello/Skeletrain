using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Carriage
{
    public static int ColorCount = 4;
    public enum CarColor {Red = 0, Yellow = 1, Green = 2, Blue = 3}
    public CarColor color;
    public List<Suspect> passengers;
    public Item[] items;
    public int index;
    public Carriage frontCarriage;
    public Carriage backCarriage;
    public bool isNecroCarriage;

    public static string ColorAsString(CarColor c) {
        switch (c) {
            case CarColor.Red:
                return "Red";
            case CarColor.Yellow:
                return "Yellow";
            case CarColor.Green:
                return "Green";
            case CarColor.Blue:
                return "Blue";
            default:
                return "Invisible";
        }
    }

    public string CarriageLetter() {
        switch (index) {
            case 0:
                return "A";
            case 1:
                return "B";
            case 2:
                return "C";
            case 3:
                return "D";
            default:
                return "?";
        }
    }

    public Carriage(int i) {
        index = i;
    }
    
    public string GetSummary() {
        string summery = ColorAsString(color) + " carriage, ";

        if (isNecroCarriage) {
            summery += "necro";
        }
        else {
            summery += " containging:";
            foreach (Suspect passenger in passengers) {
                summery += " " + passenger.name;
            }
        }

        return summery;
    }
}
