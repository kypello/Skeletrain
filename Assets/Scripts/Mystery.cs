using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mystery
{
    public Timeline trueTimeline;
    public Suspect[] suspects;
    public Carriage[] carriages;
    Carriage necroCarriage;
    Suspect necromancer;
    public int suspectCount = 6;
    public int carriageCount = 4;

    string[] testNames = new string[]{"Alice", "Bob", "Charlie", "David", "Elise", "Felicity"};

    public void GenerateMystery() {
        GenerateCharacters();
        GenerateCarriages();
        AssignCarriages();
        GenerateTimeline();
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

        List<Suspect.SuspectColor> takenTopColors = new List<Suspect.SuspectColor>();
        List<Suspect.SuspectColor> takenBottomColors = new List<Suspect.SuspectColor>();

        for (int i = 0; i < suspectCount; i++) {
            suspects[i] = new Suspect();
            suspects[i].gender = (Random.Range(0, 2) == 0 ? Suspect.Gender.Female : Suspect.Gender.Male);
            suspects[i].name = testNames[i];

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
        trueTimeline.moves.Insert(Random.Range(0, trueTimeline.moves.Count), necroMove);
    }
}
