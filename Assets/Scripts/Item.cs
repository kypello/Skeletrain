using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item
{
    public string name;
    public Suspect owner;
    public Carriage carriage;

    public static List<Item> itemsFound;
}
