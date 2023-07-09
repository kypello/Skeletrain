using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mystery
{
    public Timeline trueTimeline;
    public Timeline falseTimeline;
    public Suspect[] suspects;
    public Carriage[] carriages;
    public Carriage necroCarriage;
    Suspect necromancer;
    public int suspectCount = 6;
    public int carriageCount = 4;
    public Item[] items;
    public string[] itemNames = new string[]{"banana", "bone", "boot", "flag", "hammer", "mug", "octopus plushie", "frying pan", "pair of sunglasses", "wrench"};

    string[] testNames = new string[]{"Alice", "Bob", "Charlie", "David", "Elise", "Felicity"};

    public void GenerateMystery() {
        GenerateCharacters();
        GenerateCarriages();
        AssignCarriages();
        GenerateNicknames();
        GenerateTimeline();
        GenerateItems();
        GenerateItemKnowledge();
        DistributeItems();
        GenerateFalseTimeline();
    }

    public void PrintOutCarriages() {
        foreach (Carriage carriage in carriages) {
            Debug.Log(carriage.GetSummary());
        }
    }

    public void PrintOutTimeline() {
        foreach (Move move in trueTimeline.moves) {
            Debug.Log(move.GetSummary());
        }
    }
    
    void GenerateCharacters() {
        suspects = new Suspect[suspectCount];
        NameGenerator nameGenerator = new NameGenerator();

        List<Suspect.SuspectColor> takenTopColors = new List<Suspect.SuspectColor>();
        List<Suspect.SuspectColor> takenBottomColors = new List<Suspect.SuspectColor>();

        for (int i = 0; i < suspectCount; i++) {
            suspects[i] = new Suspect();
            suspects[i].gender = (Random.Range(0, 2) == 0 ? Suspect.Gender.Female : Suspect.Gender.Male);
            suspects[i].name = nameGenerator.GenerateName(suspects[i].gender);
            suspects[i].seed = Random.Range(-10000000, 10000000);

            do {
                suspects[i].topColor = (Suspect.SuspectColor)Random.Range(0, 6);
            } while (takenTopColors.Contains(suspects[i].topColor));
            takenTopColors.Add(suspects[i].topColor);

            do {
                suspects[i].bottomColor = (Suspect.SuspectColor)Random.Range(0, 6);
            } while (takenBottomColors.Contains(suspects[i].bottomColor));
            takenBottomColors.Add(suspects[i].bottomColor);
        }

        necromancer = suspects[Random.Range(0, suspectCount)];
        necromancer.isNecromancer = true;

        Debug.Log("Necromancer: " + necromancer.name);
    }

    void GenerateNicknames() {
        foreach (Suspect suspect in suspects) {
            suspect.knownPassengers = new List<Suspect>();
        }

        int relationshipCount = Random.Range(6, 10);

        for (int i = 0; i < relationshipCount; i++) {
            Suspect suspectA;
            Suspect suspectB;

            do {
                suspectA = suspects[Random.Range(0, suspectCount)];
                suspectB = suspects[Random.Range(0, suspectCount)];
            } while (suspectA == suspectB || suspectA.knownPassengers.Contains(suspectB));

            suspectA.knownPassengers.Add(suspectB);
            suspectB.knownPassengers.Add(suspectA);
        }

        foreach (Suspect suspect in suspects) {
            suspect.nicknames = new Dictionary<Suspect, string>();

            foreach (Suspect other in suspects) {
                if (suspect == other) {
                    continue;
                }

                if (suspect.knownPassengers.Contains(other)) {
                    switch (Random.Range(0, 3)) {
                        case 0:
                            suspect.nicknames[other] = other.name.Split(' ')[0];
                            break;
                        case 1:
                            suspect.nicknames[other] = other.name;
                            break;
                        default:
                            if (other.gender == Suspect.Gender.Female) {
                                suspect.nicknames[other] = (Random.Range(0, 2) == 0 ? "Miss " : "Mrs ") + other.name.Split(' ')[1];
                            }
                            else {
                                suspect.nicknames[other] = "Mr " + other.name.Split(' ')[1];
                            }
                            break;
                    }
                }
                else {
                    string nickname = "";
                    nickname += (other.carriage == suspect.carriage ? "that " : "a ");

                    if (other.gender == Suspect.Gender.Female) {
                        nickname += (Random.Range(0, 2) == 0 ? "woman " : "lady ");
                    }
                    else {
                        nickname += (Random.Range(0, 2) == 0 ? "man " : "gentleman ");
                    }

                    nickname += (other.carriage == suspect.carriage ? "with the " : "in a ");

                    if (other.gender == Suspect.Gender.Female) {
                        if (other.topColor == other.bottomColor) {
                            nickname += Suspect.ColorAsString(other.topColor) + " dress";
                        }
                        else {
                            if (Random.Range(0, 2) == 0) {
                                nickname += Suspect.ColorAsString(other.topColor) + " top";
                            }
                            else {
                                nickname += Suspect.ColorAsString(other.bottomColor) + " dress";
                            }
                        }
                    }
                    else {
                        if (other.topColor == other.bottomColor) {
                            nickname += Suspect.ColorAsString(other.topColor) + " suit";
                        }
                        else {
                            if (Random.Range(0, 2) == 0) {
                                nickname += Suspect.ColorAsString(other.topColor) + " coat";
                            }
                            else {
                                nickname += "pair of " + Suspect.ColorAsString(other.bottomColor) + " trousers";
                            }
                        }
                    }

                    suspect.nicknames[other] = nickname;
                }
            }
        }
    }

    void GenerateCarriages() {
        carriages = new Carriage[carriageCount];
        List<Carriage.CarColor> takenColors = new List<Carriage.CarColor>();

        for (int i = 0; i < carriageCount; i++) {
            carriages[i] = new Carriage(i);

            if (i != 0) {
                carriages[i-1].backCarriage = carriages[i];
                carriages[i].frontCarriage = carriages[i-1];
            }

            do {
                carriages[i].color = (Carriage.CarColor)Random.Range(0, Carriage.ColorCount);
            } while (takenColors.Contains(carriages[i].color));
            takenColors.Add(carriages[i].color);
        }

        necroCarriage = carriages[Random.Range(1, carriageCount-1)];
        necroCarriage.isNecroCarriage = true;
    }

    void AssignCarriages() {
        do {
            foreach (Carriage carriage in carriages) {
                carriage.passengers = new List<Suspect>();
            }

            foreach (Suspect suspect in suspects) {
                do {
                    suspect.carriage = carriages[Random.Range(0, carriageCount)];
                } while (suspect.carriage.isNecroCarriage);
                suspect.carriage.passengers.Add(suspect);
            }
        } while (CheckForEmptyCarriage());
    }

    bool CheckForEmptyCarriage() {
        foreach (Carriage carriage in carriages) {
            if (!carriage.isNecroCarriage && carriage.passengers.Count == 0) {
                return true;
            }
        }
        return false;
    }

    void GenerateTimeline() {
        trueTimeline = new Timeline();
        trueTimeline.mystery = this;

        List<Suspect> movedSuspects = new List<Suspect>();

        //suspects that moved, but didn't pass through necro carriage
        int nonWitnessMovers = Random.Range(0, 3);
        for (int i = 0; i < nonWitnessMovers; i++) {
            Move move = new Move();

            //find two different carriages that are both on the same side of the necro carriage
            Carriage carriageA;
            Carriage carriageB;
            do {
                carriageA = carriages[Random.Range(0, carriageCount)];
                carriageB = carriages[Random.Range(0, carriageCount)];
            } while (carriageA == carriageB || carriageA.isNecroCarriage || carriageB.isNecroCarriage || ((carriageA.index < necroCarriage.index) != (carriageB.index < necroCarriage.index)));

            //make sure start carriage has at least one passenger that hasn't moved yet, and switch them around if it does
            bool carriageAContainsAtLeastOneUnmovedInnocentPassenger = false;
            foreach (Suspect passenger in carriageA.passengers) {
                if (!passenger.isNecromancer && !movedSuspects.Contains(passenger)) {
                    carriageAContainsAtLeastOneUnmovedInnocentPassenger = true;
                    break;
                }
            }
            if (!carriageAContainsAtLeastOneUnmovedInnocentPassenger) {
                bool carriageBContainsAtLeastOneUnmovedInnocentPassenger = false;
                foreach (Suspect passenger in carriageB.passengers) {
                    if (!passenger.isNecromancer && !movedSuspects.Contains(passenger)) {
                        carriageBContainsAtLeastOneUnmovedInnocentPassenger = true;
                        break;
                    }
                }
                
                if (!carriageBContainsAtLeastOneUnmovedInnocentPassenger) {
                    continue;
                }
                else {
                    Carriage carriageC = carriageA;
                    carriageA = carriageB;
                    carriageB = carriageC;
                }
            }

            move.destinationCarriage = carriageB;
            do {
                move.movingSuspect = carriageA.passengers[Random.Range(0, carriageA.passengers.Count)];
            } while (move.movingSuspect.isNecromancer || movedSuspects.Contains(move.movingSuspect));

            movedSuspects.Add(move.movingSuspect);

            move.talksWithPassenger = move.destinationCarriage.passengers.Count > 1 && Random.Range(0, 2) == 0;
            if (move.talksWithPassenger) {
                move.passengerTalkedTo = move.destinationCarriage.passengers[Random.Range(0, move.destinationCarriage.passengers.Count)];
            }

            trueTimeline.moves.Insert(Random.Range(0, trueTimeline.moves.Count), move);
        }

        //suspects that moved through necro carriage
        int innocentNecroWitnessedMovers = Random.Range(2, 4);
        for (int i = 0; i < innocentNecroWitnessedMovers; i++) {
            Move move = new Move();
            do {
                move.movingSuspect = suspects[Random.Range(0, suspectCount)];
            } while (move.movingSuspect.isNecromancer || movedSuspects.Contains(move.movingSuspect));
            
            movedSuspects.Add(move.movingSuspect);

            if (move.movingSuspect.carriage.index > necroCarriage.index) {
                move.destinationCarriage = carriages[Random.Range(0, necroCarriage.index)];
            }
            else {
                move.destinationCarriage = carriages[Random.Range(necroCarriage.index + 1, carriageCount)];
            }

            move.talksWithPassenger = move.destinationCarriage.passengers.Count > 1 && Random.Range(0, 2) == 0;
            if (move.talksWithPassenger) {
                move.passengerTalkedTo = move.destinationCarriage.passengers[Random.Range(0, move.destinationCarriage.passengers.Count)];
            }

            trueTimeline.moves.Insert(Random.Range(0, trueTimeline.moves.Count), move);
        }

        Move necroMove = new Move();
        necroMove.movingSuspect = necromancer;
        necroMove.destinationCarriage = necroCarriage;
        necroMove.isNecroMove = true;

        int necroMoveIndex = Random.Range(0, trueTimeline.moves.Count);
        trueTimeline.moves.Insert(necroMoveIndex, necroMove);

        for (int i = necroMoveIndex+1; i < trueTimeline.moves.Count; i++) {
            Move move = trueTimeline.moves[i];
            move.necroCommited = true;
            trueTimeline.moves[i] = move;
        }
    }

    void GenerateItems() {
        Item.itemsFound = new List<Item>();
        items = new Item[suspectCount];

        List<string> takenNames = new List<string>();

        for (int i = 0; i < suspectCount; i++) {
            items[i] = new Item();
            items[i].owner = suspects[i];
            suspects[i].item = items[i];

            do {
                items[i].name = itemNames[Random.Range(0, itemNames.Length)];
            } while (takenNames.Contains(items[i].name));

            takenNames.Add(items[i].name);
        }
    }

    string[] ownItemResponses = new string[]{
        "Yes, that belongs to me!",
        "That's my [item].",
        "Yeah, that's mine.",
        "That [item] belongs to me.",
        "Oh, that's my [item]. I must have dropped it.",
        "Ah, I was wondering where my [item] went!",
        "It's mine.",
        "It's my [item].",
        "That's mine.",
        "That [item] is mine."
    };

    string[] knownItemResponses = new string[]{
        "I think I saw [owner] carrying it.",
        "Yes, that belongs to [owner].",
        "That looks like [owner]'s [item].",
        "[owner] has a [item] just like that.",
        "I'm pretty sure that belongs to [owner].",
        "Oh yeah, that belongs to [owner]!",
        "[owner] was holding it earlier."
    };

    string[] unknownItemResponses = new string[]{
        "Nope, never seen it before, sorry.",
        "I'm afraid I don't recognize it.",
        "No, sorry.",
        "I don't know anything about it.",
        "Nah, I don't know whose that is.",
        "Not mine, sorry.",
        "Doesn't ring a bell.",
        "Doesn't look familiar to me.",
        "Sorry, not mine.",
        "I haven't seen it before I'm afraid.",
        "Sorry, I can't help you there."
    };

    void GenerateItemKnowledge() {
        foreach (Suspect suspect in suspects) {
            suspect.knownItems = new List<Item>();
        }

        foreach (Item item in items) {
            int itemKnowers = Random.Range(2, 5);

            for (int i = 0; i < itemKnowers; i++) {
                Suspect itemKnower;
                do {
                    itemKnower = suspects[Random.Range(0, suspectCount)];
                } while (itemKnower == item.owner || itemKnower.knownItems.Contains(item));
                itemKnower.knownItems.Add(item);
            }
        }

        foreach (Suspect suspect in suspects) {
            suspect.itemResponses = new Dictionary<Item, string>();
            foreach (Item item in items) {
                if (suspect.item == item) {
                    if (suspect.isNecromancer && suspect.carriage.passengers.Count == 1) {
                        suspect.itemResponses[item] = unknownItemResponses[Random.Range(0, unknownItemResponses.Length)];
                    }
                    else {
                        suspect.itemResponses[item] = ownItemResponses[Random.Range(0, ownItemResponses.Length)].Replace("[item]", item.name);
                    }
                }
                else if (suspect.knownItems.Contains(item)) {
                    suspect.itemResponses[item] = knownItemResponses[Random.Range(0, knownItemResponses.Length)].Replace("[owner]", suspect.nicknames[item.owner]).Replace("[item]", item.name);
                    suspect.itemResponses[item] = char.ToUpper(suspect.itemResponses[item][0]) + suspect.itemResponses[item].Substring(1);
                }
                else {
                    suspect.itemResponses[item] = unknownItemResponses[Random.Range(0, unknownItemResponses.Length)];
                }
            }
        }
    }

    void DistributeItems() {
        foreach (Item item in items) {
            if (item.owner.isNecromancer) {
                item.carriage = necroCarriage;
                continue;
            }

            List<Carriage> possibleCarriages = new List<Carriage>();

            foreach (Move move in trueTimeline.moves) {
                if (move.movingSuspect == item.owner) {
                    int startIndex = item.owner.carriage.index;
                    int delta;
                    if (startIndex > move.destinationCarriage.index) {
                        delta = -1;
                    }
                    else {
                        delta = 1;
                    }

                    for (int i = startIndex; i != move.destinationCarriage.index; i += delta) {
                        possibleCarriages.Add(carriages[i]);
                    }
                    possibleCarriages.Add(move.destinationCarriage);
                    break;
                }
            }

            if (possibleCarriages.Count == 0) {
                item.carriage = item.owner.carriage;
            }
            else {
                item.carriage = possibleCarriages[Random.Range(0, possibleCarriages.Count)];
            }
        }
    }

    void GenerateFalseTimeline() {
        falseTimeline = new Timeline();
        falseTimeline.mystery = this;

        int necroMoveIndex = 0;

        for (int i = 0; i < trueTimeline.moves.Count; i++) {
            falseTimeline.moves.Add(trueTimeline.moves[i]);
            if (trueTimeline.moves[i].isNecroMove) {
                necroMoveIndex = i;
            }
        }

        if (necromancer.carriage.passengers.Count == 1) {
            falseTimeline.moves.Remove(falseTimeline.moves[necroMoveIndex]);
        }
        else {
            Move falseNecroMove = new Move();
            falseNecroMove.movingSuspect = necromancer;
            falseNecroMove.necroCommited = Random.Range(0, 2) == 0;

            if (necromancer.carriage.index > necroCarriage.index) {
                falseNecroMove.destinationCarriage = carriages[Random.Range(0, necroCarriage.index)];
            }
            else {
                falseNecroMove.destinationCarriage = carriages[Random.Range(necroCarriage.index+1, carriageCount)];
            }

            if (Random.Range(0, 2) == 0) {
                falseNecroMove.talksWithPassenger = true;
                falseNecroMove.passengerTalkedTo = falseNecroMove.destinationCarriage.passengers[Random.Range(0, falseNecroMove.destinationCarriage.passengers.Count)];
            }

            falseTimeline.moves[necroMoveIndex] = falseNecroMove;
        }
    }
}
