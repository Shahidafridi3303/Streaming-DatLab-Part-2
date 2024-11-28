/*
This RPG data streaming assignment was created by Fernando Restituto with
pixel RPG characters created by Sean Browning.
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

#region Assignment Instructions

/*  Hello!  Welcome to your first lab :)

Wax on, wax off.

    The development of saving and loading systems shares much in common with that of networked gameplay development.  
    Both involve developing around data which is packaged and passed into (or gotten from) a stream.  
    Thus, prior to attacking the problems of development for networked games, you will strengthen your abilities to develop solutions using the easier to work with HD saving/loading frameworks.

    Try to understand not just the framework tools, but also, 
    seek to familiarize yourself with how we are able to break data down, pass it into a stream and then rebuild it from another stream.


Lab Part 1

    Begin by exploring the UI elements that you are presented with upon hitting play.
    You can roll a new party, view party stats and hit a save and load button, both of which do nothing.
    You are challenged to create the functions that will save and load the party data which is being displayed on screen for you.

    Below, a SavePartyButtonPressed and a LoadPartyButtonPressed function are provided for you.
    Both are being called by the internal systems when the respective button is hit.
    You must code the save/load functionality.
    Access to Party Character data is provided via demo usage in the save and load functions.

    The PartyCharacter class members are defined as follows.  */

public partial class PartyCharacter
{
    public int classID;

    public int health;
    public int mana;

    public int strength;
    public int agility;
    public int wisdom;

    public LinkedList<int> equipment;

}


/*
    Access to the on screen party data can be achieved via …..

    Once you have loaded party data from the HD, you can have it loaded on screen via …...

    These are the stream reader/writer that I want you to use.
    https://docs.microsoft.com/en-us/dotnet/api/system.io.streamwriter
    https://docs.microsoft.com/en-us/dotnet/api/system.io.streamreader

    Alright, that’s all you need to get started on the first part of this assignment, here are your functions, good luck and journey well!
*/


#endregion

#region Assignment Part 1

static public class AssignmentPart1
{
    private static string saveFilePath = "SaveDataParty.txt";

    /// <summary>
    /// Called when the save button is pressed. Saves the current party data to a file.
    /// </summary>
    static public void SavePartyButtonPressed()
    {
        using (StreamWriter writer = new StreamWriter(saveFilePath))
        {
            foreach (PartyCharacter character in GameContent.partyCharacters)
            {
                // Save character stats as a comma-separated string
                writer.WriteLine($"{character.classID},{character.health},{character.mana},{character.strength},{character.agility},{character.wisdom}");

                // Write equipment data as a space-separated list
                foreach (int item in character.equipment)
                {
                    writer.Write($"{item} ");
                }
                writer.WriteLine(); // End the equipment line
            }
        }

        Debug.Log("Party data saved successfully.");
    }

    /// <summary>
    /// Called when the load button is pressed. Loads party data from a file and updates the UI.
    /// </summary>
    static public void LoadPartyButtonPressed()
    {
        GameContent.partyCharacters.Clear(); // Reset party data

        if (File.Exists(saveFilePath))
        {
            Debug.Log("Save file located.");

            using (StreamReader reader = new StreamReader(saveFilePath))
            {
                string line;

                while ((line = reader.ReadLine()) != null)
                {
                    // Split the line into attributes
                    string[] characterData = line.Split(',');

                    if (characterData.Length == 6 &&
                        int.TryParse(characterData[0], out int classID) &&
                        int.TryParse(characterData[1], out int health) &&
                        int.TryParse(characterData[2], out int mana) &&
                        int.TryParse(characterData[3], out int strength) &&
                        int.TryParse(characterData[4], out int agility) &&
                        int.TryParse(characterData[5], out int wisdom))
                    {
                        // Create and populate a new character instance
                        PartyCharacter character = new PartyCharacter(classID, health, mana, strength, agility, wisdom);

                        // Read equipment data
                        string equipmentLine = reader.ReadLine();
                        if (!string.IsNullOrEmpty(equipmentLine))
                        {
                            foreach (string item in equipmentLine.Split(' '))
                            {
                                if (int.TryParse(item, out int equipmentID))
                                {
                                    character.equipment.AddLast(equipmentID);
                                }
                            }
                        }

                        GameContent.partyCharacters.AddLast(character);
                    }
                    else
                    {
                        Debug.LogError($"Failed to parse character data: {line}");
                    }
                }
            }
        }
        else
        {
            Debug.LogError("No save file found.");
        }

        GameContent.RefreshUI(); // Refresh UI with new data
    }
}
#endregion

#region Assignment Part 2

//  Before Proceeding!
//  To inform the internal systems that you are proceeding onto the second part of this assignment,
//  change the below value of AssignmentConfiguration.PartOfAssignmentInDevelopment from 1 to 2.
//  This will enable the needed UI/function calls for your to proceed with your assignment.
static public class AssignmentConfiguration
{
    public const int PartOfAssignmentThatIsInDevelopment = 1;
}

/*

In this part of the assignment you are challenged to expand on the functionality that you have already created.  
    You are being challenged to save, load and manage multiple parties.
    You are being challenged to identify each party via a string name (a member of the Party class).

To aid you in this challenge, the UI has been altered.  

    The load button has been replaced with a drop down list.  
    When this load party drop down list is changed, LoadPartyDropDownChanged(string selectedName) will be called.  
    When this drop down is created, it will be populated with the return value of GetListOfPartyNames().

    GameStart() is called when the program starts.

    For quality of life, a new SavePartyButtonPressed() has been provided to you below.

    An new/delete button has been added, you will also find below NewPartyButtonPressed() and DeletePartyButtonPressed()

Again, you are being challenged to develop the ability to save and load multiple parties.
    This challenge is different from the previous.
    In the above challenge, what you had to develop was much more directly named.
    With this challenge however, there is a much more predicate process required.
    Let me ask you,
        What do you need to program to produce the saving, loading and management of multiple parties?
        What are the variables that you will need to declare?
        What are the things that you will need to do?  
    So much of development is just breaking problems down into smaller parts.
    Take the time to name each part of what you will create and then, do it.

Good luck, journey well.

*/

static public class AssignmentPart2
{
    private static string saveDirectoryPath = "SavedParties/";
    static List<string> listOfPartyNames;

    static public void GameStart()
    {
        listOfPartyNames = new List<string>();

        if (!Directory.Exists(saveDirectoryPath))
        {
            Directory.CreateDirectory(saveDirectoryPath);
        }

        foreach (string filePath in Directory.GetFiles(saveDirectoryPath, "*.txt"))
        {
            listOfPartyNames.Add(Path.GetFileNameWithoutExtension(filePath));
        }

        GameContent.RefreshUI();
        Debug.Log("Loaded existing party names.");
    }

    static public List<string> GetListOfPartyNames()
    {
        return listOfPartyNames;
    }

    static public void LoadPartyDropDownChanged(string selectedName)
{
    string filePath = saveDirectoryPath + selectedName + ".txt";

    if (File.Exists(filePath))
    {
        GameContent.partyCharacters.Clear();

        using (StreamReader reader = new StreamReader(filePath))
        {
            string line;

            while ((line = reader.ReadLine()) != null)
            {
                string[] characterData = line.Split(',');

                if (characterData.Length == 6 &&
                    int.TryParse(characterData[0], out int classID) &&
                    int.TryParse(characterData[1], out int health) &&
                    int.TryParse(characterData[2], out int mana) &&
                    int.TryParse(characterData[3], out int strength) &&
                    int.TryParse(characterData[4], out int agility) &&
                    int.TryParse(characterData[5], out int wisdom))
                {
                    PartyCharacter pc = new PartyCharacter(classID, health, mana, strength, agility, wisdom);

                    string equipmentLine = reader.ReadLine();
                    if (!string.IsNullOrWhiteSpace(equipmentLine))
                    {
                        foreach (string equip in equipmentLine.Split(' '))
                        {
                            if (int.TryParse(equip, out int equipmentID))
                            {
                                pc.equipment.AddLast(equipmentID);
                            }
                        }
                    }

                    GameContent.partyCharacters.AddLast(pc);
                }
            }
        }

        Debug.Log($"Party '{selectedName}' loaded successfully.");
        GameContent.RefreshUI();
    }
    else
    {
        Debug.LogError($"Save file not found for party '{selectedName}'.");
    }
}


    static public void SavePartyButtonPressed()
    {
        string partyName = GameContent.GetPartyNameFromInput();

        if (string.IsNullOrWhiteSpace(partyName))
        {
            Debug.Log("Party name cannot be empty!");
            return;
        }

        string filePath = saveDirectoryPath + partyName + ".txt";

        using (StreamWriter writer = new StreamWriter(filePath))
        {
            foreach (PartyCharacter pc in GameContent.partyCharacters)
            {
                writer.WriteLine($"{pc.classID},{pc.health},{pc.mana},{pc.strength},{pc.agility},{pc.wisdom}");
                foreach (int equip in pc.equipment)
                {
                    writer.Write($"{equip} ");
                }
                writer.WriteLine();
            }
        }

        if (!listOfPartyNames.Contains(partyName))
        {
            listOfPartyNames.Add(partyName);
        }

        Debug.Log($"Party '{partyName}' created.");
        GameContent.RefreshUI();
    }

    static public void DeletePartyButtonPressed()
    {
        GameContent.RefreshUI();
    }

}

#endregion


