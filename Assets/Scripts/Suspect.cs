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
}
