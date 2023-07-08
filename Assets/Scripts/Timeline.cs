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

    enum Pronoun{Subject = 0, Object = 1, Possessive = 2}
    string[,] pronouns = new string[,]{{"she", "her", "her"}, {"he", "him", "his"}};

    string[] selfMoveTemplates = new string[]{
        "I felt the need to stretch my legs, so I went to the [dest_color] carriage.", 
        "I decided to go for a wander and went to the [dest_color] carriage.",
        "I got tired of sitting, so I got up and went over to the [dest_color] carriage.",
        "My legs were starting to fall asleep, so I stood up and walked over to the [dest_color] carriage.",
        "I decided to check out the [dest_color] carriage, so I got up and left."
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



    public string[] GenerateTestimony(Suspect witness) {
        List<string> statements = new List<string>();

        foreach (Move move in moves) {
            if (move.movingSuspect == witness) {
                statements.Add(selfMoveTemplates[Random.Range(0, selfMoveTemplates.Length)].Replace("[dest_color]", Carriage.ColorAsString(move.destinationCarriage.color)));

                if (move.talksWithPassenger) {
                    statements.Add(selfMoveTalkTemplates[Random.Range(0, selfMoveTalkTemplates.Length)].Replace("[talk_suspect]", move.passengerTalkedTo.name));
                }
                else {
                    statements.Add(selfMoveNoTalkTemplates[Random.Range(0, selfMoveNoTalkTemplates.Length)]);
                }
            }
            else if (move.destinationCarriage == witness.carriage) {
                statements.Add(otherEnterTemplates[Random.Range(0, otherEnterTemplates.Length)].Replace("[move_suspect]", move.movingSuspect.name).Replace("[direction]", (move.movingSuspect.carriage.index > witness.carriage.index ? "back" : "front")));

                string returnStatement = "";

                if (move.talksWithPassenger) {
                    if (move.passengerTalkedTo == witness) {
                        returnStatement = otherEnterTalkSelfTemplates[Random.Range(0, otherEnterTalkSelfTemplates.Length)].Replace("[move_pronoun_sub]", pronouns[(int)move.movingSuspect.gender, (int)Pronoun.Subject]).Replace("[move_pronoun_obj]", pronouns[(int)move.movingSuspect.gender, (int)Pronoun.Object]);
                    }
                    else {
                        returnStatement = otherEnterTalkTemplates[Random.Range(0, otherEnterTalkTemplates.Length)].Replace("[move_pronoun_sub]", pronouns[(int)move.movingSuspect.gender, (int)Pronoun.Subject]).Replace("[move_pronoun_obj]", pronouns[(int)move.movingSuspect.gender, (int)Pronoun.Object]).Replace("[talk_suspect]", move.passengerTalkedTo.name);
                    }
                }
                else {
                    returnStatement = otherEnterNoTalkTemplates[Random.Range(0, otherEnterNoTalkTemplates.Length)].Replace("[move_pronoun_sub]", pronouns[(int)move.movingSuspect.gender, (int)Pronoun.Subject]).Replace("[move_pronoun_obj]", pronouns[(int)move.movingSuspect.gender, (int)Pronoun.Object]);
                }
                returnStatement = char.ToUpper(returnStatement[0]) + returnStatement.Substring(1);
                
                statements.Add(returnStatement);
            }
            else if (move.movingSuspect.carriage == witness.carriage) {
                statements.Add(otherLeaveTemplates[Random.Range(0, otherLeaveTemplates.Length)].Replace("[move_suspect]", move.movingSuspect.name).Replace("[direction]", (move.movingSuspect.carriage.index > move.destinationCarriage.index ? "front" : "back")));

                string returnStatement = otherLeaveReturnTemplates[Random.Range(0, otherLeaveReturnTemplates.Length)].Replace("[move_pronoun_sub]", pronouns[(int)move.movingSuspect.gender, (int)Pronoun.Subject]).Replace("[move_pronoun_pos]", pronouns[(int)move.movingSuspect.gender, (int)Pronoun.Possessive]);
                returnStatement = char.ToUpper(returnStatement[0]) + returnStatement.Substring(1);

                statements.Add(returnStatement);
            }
            else if ((move.movingSuspect.carriage.index > witness.carriage.index) != (move.destinationCarriage.index > witness.carriage.index)) {
                statements.Add(otherPassThroughTemplates[Random.Range(0, otherPassThroughTemplates.Length)].Replace("[move_suspect]", move.movingSuspect.name).Replace("[direction]", (move.movingSuspect.carriage.index > witness.carriage.index ? "front" : "back")));

                string returnStatement = otherPassThroughReturnTemplates[Random.Range(0, otherPassThroughReturnTemplates.Length)].Replace("[move_pronoun_sub]", pronouns[(int)move.movingSuspect.gender, (int)Pronoun.Subject]).Replace("[move_pronoun_obj]", pronouns[(int)move.movingSuspect.gender, (int)Pronoun.Object]);
                returnStatement = char.ToUpper(returnStatement[0]) + returnStatement.Substring(1);
                
                statements.Add(returnStatement);
            }
        }

        return statements.ToArray();
    }
}
