using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Move {
    public Suspect movingSuspect;
    public Carriage destinationCarriage;
    public bool talksWithPassenger;
    public bool isNecroMove;
    public bool necroCommited;
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
    public Mystery mystery;

    public Timeline() {
        moves = new List<Move>();
    }

    public enum Pronoun{Subject = 0, Object = 1, Possessive = 2}
    public static string[,] pronouns = new string[,]{{"she", "her", "her"}, {"he", "him", "his"}};

    string[] selfMoveTemplates = new string[]{
        "I felt the need to stretch my legs, so I went to Carriage [dest_letter].", 
        "I decided to go for a wander and went to Carriage [dest_letter].",
        "I got tired of sitting, so I got up and went over to Carriage [dest_letter].",
        "my legs were starting to fall asleep, so I stood up and walked over to Carriage [dest_letter].",
        "I decided to check out Carriage [dest_letter], so I got up and left."
    };

    string[] selfMoveTalkTemplates = new string[]{
        "I exchanged a few words with [talk_suspect] before returning to my seat.",
        "I saw [talk_suspect] there, and we chatted briefly before I came back to my carriage.",
        "[talk_suspect] and I engaged in polite conversation before I went back the way I came.",
        "[talk_suspect] and I conversed briefly before I went back to my seat.",
        "I had a moments conversation with [talk_suspect] before returning."
    };

    string[] selfMoveNoTalkTemplates = new string[]{
        "I went back to my seat a few moments later.",
        "I lingered for a while before returning to my seat.",
        "I then turned around and came back to my seat.",
        "After a minute or two, I came back.",
        "I then returned to my seat.",
        "I was only there for a moment before coming back."
    };

    string[] selfMoveVictimDeadTemplates = new string[]{
        "Passing through Carriage [necro_letter], I noticed the victim was still very much undead, before contuing to Carriage [dest_letter].",
        "I went through Carriage [necro_letter] and saw the victim looking as skeletal as ever, then continued to Carriage [dest_letter].",
        "The victim was still undead at this point, as I noticed when I was passing through Carriage [necro_letter] while on my way to Carriage [dest_letter]."
    };

    string[] selfMoveVictimAliveTemplates = new string[]{
        "To my horror, I saw the victim alive and well in Carriage [necro_letter]. Nevertheless I continued onwards to Carriage [dest_letter].",
        "It was at this moment while passing through Carriage [necro_letter] that I saw the victim full of life. Ignoring the frightening sight, I continued to Carriage [dest_letter].",
        "The victim had already been resurrected at this point, as I noted while passing through Carriage [necro_letter] before continuing to Carriage [dest_letter]."
    };

    string[] otherLeaveTemplates = new string[]{
        "I saw [move_suspect] get up and move towards the [direction] of the train.",
        "I noticed [move_suspect] leave towards the [direction] of the train.",
        "[move_suspect] got up and left towards the [direction] of the train.",
    };

    string[] otherLeaveReturnTemplates = new string[]{
        "[move_pronoun_sub] came back a few moments later.",
        "After a while, [move_pronoun_sub] returned to [move_pronoun_pos] seat.",
        "[move_pronoun_sub] returned to [move_pronoun_pos] seat a couple of minutes later.",
    };

    string[] otherEnterTemplates = new string[] {
        "[move_suspect] entered from the [direction] of the carriage.",
        "I saw [move_suspect] enter the carriage from the [direction].",
        "[move_suspect] walked into the carriage from the [direction]."
    };

    string[] otherEnterTalkTemplates = new string[] {
        "I noticed [move_pronoun_obj] talking with [talk_suspect] before leaving.",
        "[move_pronoun_sub] spoke with [talk_suspect] briefly before going back the way [move_pronoun_sub] came.",
        "After exchanging a few words with [talk_suspect], [move_pronoun_sub] left.",
        "I saw [move_pronoun_obj] have a conversation with [talk_suspect] before [move_pronoun_sub] left the carriage.",
        "[move_pronoun_sub] had a few words with [talk_suspect] before leaving the same way [move_pronoun_sub] came in."
    };

    string[] otherEnterTalkSelfTemplates = new string[] {
        "[move_pronoun_sub] said a few words to me before leaving.",
        "[move_pronoun_sub] and I had a polite conversation before [move_pronoun_sub] left the way [move_pronoun_sub] came.",
        "I chatted with [move_pronoun_obj] briefly before [move_pronoun_sub] left."
    };

    string[] otherEnterNoTalkTemplates = new string[] {
        "After a moment, [move_pronoun_sub] left the same way [move_pronoun_sub] came in.",
        "[move_pronoun_sub] was only here for a moment before leaving.",
        "A few minutes later, I saw [move_pronoun_obj] leave.",
        "[move_pronoun_sub] lingered briefly before leaving."
    };

    string[] otherPassThroughTemplates = new string[] {
        "[move_suspect] passed through the carriage towards the [direction] of the train.",
        "I saw [move_suspect] moving through the carriage towards the [direction].",
        "I noticed [move_suspect] pass through, going towards the [direction] of the train.",
        "[move_suspect] entered the carriage and kept moving towards the [direction] of the train."
    };

    string[] otherPassThroughReturnTemplates = new string[] {
        "I saw [move_pronoun_obj] come back through a few moments later.",
        "[move_pronoun_sub] came back the other way after a couple of minutes.",
        "[move_pronoun_sub] then came back through in the opposite direction.",
        "After a moment, [move_pronoun_sub] came back through."
    };

    string[] startingWords = new string[]{"First, ", "Firstly, ", "First thing that happened, ", "To begin with, "};
    string[] nextWords = new string[]{"Next, ", "Then, ", "Next ", "And then ", "Then ", "Next thing that happened, ", "After that, ", "Next thing I remember, "};


    public string[] GenerateTestimony(Suspect witness) {
        List<string> statements = new List<string>();

        Random.seed = witness.seed;

        bool firstStatement = true;
        string startingWord = "";

        foreach (Move move in moves) {
            if (firstStatement) {
                startingWord = startingWords[Random.Range(0, startingWords.Length)];
            }
            else {
                startingWord = nextWords[Random.Range(0, nextWords.Length)];
            }

            if (move.movingSuspect == witness) {
                statements.Add(startingWord + selfMoveTemplates[Random.Range(0, selfMoveTemplates.Length)].Replace("[dest_letter]", move.destinationCarriage.CarriageLetter()));

                if ((witness.carriage.index < mystery.necroCarriage.index) != (move.destinationCarriage.index < mystery.necroCarriage.index)) {
                    if (move.necroCommited) {
                        statements.Add(selfMoveVictimAliveTemplates[Random.Range(0, selfMoveVictimAliveTemplates.Length)].Replace("[necro_letter]", mystery.necroCarriage.CarriageLetter()).Replace("[dest_letter]", move.destinationCarriage.CarriageLetter()));
                    }
                    else {
                        statements.Add(selfMoveVictimDeadTemplates[Random.Range(0, selfMoveVictimDeadTemplates.Length)].Replace("[necro_letter]", mystery.necroCarriage.CarriageLetter()).Replace("[dest_letter]", move.destinationCarriage.CarriageLetter()));
                    }
                }

                if (move.talksWithPassenger) {
                    string statement = selfMoveTalkTemplates[Random.Range(0, selfMoveTalkTemplates.Length)].Replace("[talk_suspect]", witness.nicknames[move.passengerTalkedTo]);
                    statement = char.ToUpper(statement[0]) + statement.Substring(1);
                    statements.Add(statement);
                }
                else {
                    statements.Add(selfMoveNoTalkTemplates[Random.Range(0, selfMoveNoTalkTemplates.Length)]);
                }

                firstStatement = false;
            }
            else if (move.destinationCarriage == witness.carriage) {
                string statement = startingWord + otherEnterTemplates[Random.Range(0, otherEnterTemplates.Length)].Replace("[move_suspect]", witness.nicknames[move.movingSuspect]).Replace("[direction]", (move.movingSuspect.carriage.index > witness.carriage.index ? "back" : "front"));
                statements.Add(statement);

                string returnStatement = "";

                if (move.talksWithPassenger) {
                    if (move.passengerTalkedTo == witness) {
                        returnStatement = otherEnterTalkSelfTemplates[Random.Range(0, otherEnterTalkSelfTemplates.Length)].Replace("[move_pronoun_sub]", pronouns[(int)move.movingSuspect.gender, (int)Pronoun.Subject]).Replace("[move_pronoun_obj]", pronouns[(int)move.movingSuspect.gender, (int)Pronoun.Object]);
                    }
                    else {
                        returnStatement = otherEnterTalkTemplates[Random.Range(0, otherEnterTalkTemplates.Length)].Replace("[move_pronoun_sub]", pronouns[(int)move.movingSuspect.gender, (int)Pronoun.Subject]).Replace("[move_pronoun_obj]", pronouns[(int)move.movingSuspect.gender, (int)Pronoun.Object]).Replace("[talk_suspect]", witness.nicknames[move.passengerTalkedTo]);
                    }
                }
                else {
                    returnStatement = otherEnterNoTalkTemplates[Random.Range(0, otherEnterNoTalkTemplates.Length)].Replace("[move_pronoun_sub]", pronouns[(int)move.movingSuspect.gender, (int)Pronoun.Subject]).Replace("[move_pronoun_obj]", pronouns[(int)move.movingSuspect.gender, (int)Pronoun.Object]);
                }
                returnStatement = char.ToUpper(returnStatement[0]) + returnStatement.Substring(1);
                
                statements.Add(returnStatement);

                firstStatement = false;
            }
            else if (move.movingSuspect.carriage == witness.carriage) {
                string statement = startingWord + otherLeaveTemplates[Random.Range(0, otherLeaveTemplates.Length)].Replace("[move_suspect]", witness.nicknames[move.movingSuspect]).Replace("[direction]", (move.movingSuspect.carriage.index > move.destinationCarriage.index ? "front" : "back"));
                statements.Add(statement);

                string returnStatement = otherLeaveReturnTemplates[Random.Range(0, otherLeaveReturnTemplates.Length)].Replace("[move_pronoun_sub]", pronouns[(int)move.movingSuspect.gender, (int)Pronoun.Subject]).Replace("[move_pronoun_pos]", pronouns[(int)move.movingSuspect.gender, (int)Pronoun.Possessive]);
                returnStatement = char.ToUpper(returnStatement[0]) + returnStatement.Substring(1);

                statements.Add(returnStatement);

                firstStatement = false;
            }
            else if ((move.movingSuspect.carriage.index > witness.carriage.index) != (move.destinationCarriage.index > witness.carriage.index)) {
                string statement = startingWord + otherPassThroughTemplates[Random.Range(0, otherPassThroughTemplates.Length)].Replace("[move_suspect]", witness.nicknames[move.movingSuspect]).Replace("[direction]", (move.movingSuspect.carriage.index > witness.carriage.index ? "front" : "back"));
                statements.Add(statement);

                string returnStatement = otherPassThroughReturnTemplates[Random.Range(0, otherPassThroughReturnTemplates.Length)].Replace("[move_pronoun_sub]", pronouns[(int)move.movingSuspect.gender, (int)Pronoun.Subject]).Replace("[move_pronoun_obj]", pronouns[(int)move.movingSuspect.gender, (int)Pronoun.Object]);
                returnStatement = char.ToUpper(returnStatement[0]) + returnStatement.Substring(1);
                
                statements.Add(returnStatement);

                firstStatement = false;
            }
        }

        return statements.ToArray();
    }
}
