
/*
This RPG data streaming assignment was created by Fernando Restituto with 
pixel RPG characters created by Sean Browning.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;


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
    private static string saveFilePath = "SaveDateParty.txt";

    static public void SavePartyButtonPressed()
    {
        try
        {
            using (StreamWriter writer = new StreamWriter(saveFilePath))
            {
                foreach (var partyCharacter in GameContent.partyCharacters)
                {
                    // Save character attributes as a comma-separated line
                    writer.WriteLine($"{partyCharacter.classID},{partyCharacter.health},{partyCharacter.mana},{partyCharacter.strength},{partyCharacter.agility},{partyCharacter.wisdom}");

                    // Save equipment IDs as space-separated values
                    writer.WriteLine(string.Join(" ", partyCharacter.equipment));
                }
            }

            Debug.Log("Party data saved successfully!");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error saving party data: {ex.Message}");
        }
    }

    static public void LoadPartyButtonPressed()
    {
        GameContent.partyCharacters.Clear(); // Clear existing party data

        if (!File.Exists(saveFilePath))
        {
            Debug.LogError("Save file not found!");
            return;
        }

        try
        {
            using (StreamReader reader = new StreamReader(saveFilePath))
            {
                string line;

                while ((line = reader.ReadLine()) != null)
                {
                    var characterData = line.Split(',');
                    if (characterData.Length != 6)
                    {
                        Debug.LogError("Incorrect number of data elements in the line!");
                        continue;
                    }

                    // Parse the character attributes
                    if (int.TryParse(characterData[0], out int classID) &&
                        int.TryParse(characterData[1], out int health) &&
                        int.TryParse(characterData[2], out int mana) &&
                        int.TryParse(characterData[3], out int strength) &&
                        int.TryParse(characterData[4], out int agility) &&
                        int.TryParse(characterData[5], out int wisdom))
                    {
                        var partyCharacter = new PartyCharacter(classID, health, mana, strength, agility, wisdom);

                        // Read the next line for equipment data
                        string equipmentLine = reader.ReadLine();
                        if (!string.IsNullOrWhiteSpace(equipmentLine))
                        {
                            var equipmentIDs = equipmentLine.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                            foreach (var equip in equipmentIDs)
                            {
                                if (int.TryParse(equip, out int equipmentID))
                                {
                                    partyCharacter.equipment.AddLast(equipmentID);
                                }
                            }
                        }

                        // Add the party character to the list
                        GameContent.partyCharacters.AddLast(partyCharacter);
                    }
                    else
                    {
                        Debug.LogError($"Failed to parse character attributes: {line}");
                    }
                }
            }

            Debug.Log("Party data loaded successfully!");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error loading party data: {ex.Message}");
        }

        GameContent.RefreshUI(); // Update the UI with the loaded data
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
    public const int PartOfAssignmentThatIsInDevelopment = 2;
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
    private static readonly string saveDirectoryPath = "SavedParties/";  // Save directory path
    private static List<string> listOfPartyNames = new List<string>();  // List to hold party names

    // Initialize the game by ensuring the save directory exists and loading saved party names
    static public void GameStart()
    {
        EnsureSaveDirectoryExists();
        LoadPartyNames();
        GameContent.RefreshUI();  // Refresh UI after loading data
    }

    // Ensure the save directory exists; create it if not
    static private void EnsureSaveDirectoryExists()
    {
        if (!Directory.Exists(saveDirectoryPath))
        {
            Directory.CreateDirectory(saveDirectoryPath);
        }
    }

    // Load party names from the save directory
    static private void LoadPartyNames()
    {
        listOfPartyNames.Clear();
        foreach (string filePath in Directory.GetFiles(saveDirectoryPath, "*.txt"))
        {
            listOfPartyNames.Add(Path.GetFileNameWithoutExtension(filePath));  // Add party name without extension
        }
    }

    // Get the list of party names
    static public List<string> GetListOfPartyNames() => listOfPartyNames;

    // Load a selected party based on the dropdown choice
    static public void LoadPartyDropDownChanged(string selectedName)
    {
        string filePath = GetFilePath(selectedName);

        if (File.Exists(filePath))
        {
            LoadPartyData(filePath);
            GameContent.RefreshUI();  // Refresh UI after loading party
        }
        else
        {
            Debug.LogError($"Save file not found for the party: {selectedName}");
        }
    }

    // Helper method to construct file path
    static private string GetFilePath(string partyName) => Path.Combine(saveDirectoryPath, partyName + ".txt");

    // Load party data from the save file
    static private void LoadPartyData(string filePath)
    {
        GameContent.partyCharacters.Clear();  // Clear existing party data

        using (StreamReader reader = new StreamReader(filePath))
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                string[] characterData = line.Split(',');

                // Parse character data
                if (characterData.Length == 6 && TryParseCharacterData(characterData, out PartyCharacter pc))
                {
                    // Read equipment data from next line
                    string equipmentLine = reader.ReadLine();
                    if (!string.IsNullOrWhiteSpace(equipmentLine))
                    {
                        ParseEquipmentData(pc, equipmentLine);
                    }

                    GameContent.partyCharacters.AddLast(pc);  // Add character to the party
                }
            }
        }

        Debug.Log($"Party loaded successfully: {Path.GetFileNameWithoutExtension(filePath)}");
    }

    // Try parsing the character data
    static private bool TryParseCharacterData(string[] data, out PartyCharacter pc)
    {
        pc = null;
        if (int.TryParse(data[0], out int classID) &&
            int.TryParse(data[1], out int health) &&
            int.TryParse(data[2], out int mana) &&
            int.TryParse(data[3], out int strength) &&
            int.TryParse(data[4], out int agility) &&
            int.TryParse(data[5], out int wisdom))
        {
            pc = new PartyCharacter(classID, health, mana, strength, agility, wisdom);
            return true;
        }
        return false;
    }

    // Parse and add equipment data
    static private void ParseEquipmentData(PartyCharacter pc, string equipmentLine)
    {
        string[] equipmentData = equipmentLine.Split(' ');

        foreach (string equip in equipmentData)
        {
            if (int.TryParse(equip, out int equipmentID))
            {
                pc.equipment.AddLast(equipmentID);
            }
        }
    }

    // Save the current party to a file
    static public void SavePartyButtonPressed()
    {
        string partyName = GameContent.GetPartyNameFromInput();

        if (string.IsNullOrWhiteSpace(partyName))
        {
            Debug.LogError("Party name cannot be empty!");
            return;
        }

        string filePath = GetFilePath(partyName);

        SavePartyData(filePath);
        AddPartyToList(partyName);

        Debug.Log($"Party saved successfully: {partyName}");
        GameContent.RefreshUI();
    }

    // Save party data to file
    static private void SavePartyData(string filePath)
    {
        using (StreamWriter writer = new StreamWriter(filePath))
        {
            foreach (PartyCharacter pc in GameContent.partyCharacters)
            {
                writer.WriteLine($"{pc.classID},{pc.health},{pc.mana},{pc.strength},{pc.agility},{pc.wisdom}");
                writer.WriteLine(string.Join(" ", pc.equipment));  // Write equipment in one line
            }
        }
    }

    // Add party name to the list if it's not already present
    static private void AddPartyToList(string partyName)
    {
        if (!listOfPartyNames.Contains(partyName))
        {
            listOfPartyNames.Add(partyName);
        }
    }

    // Delete the selected party
    static public void DeletePartyButtonPressed()
    {
        string partyName = GameContent.GetPartyNameFromInput();

        if (string.IsNullOrWhiteSpace(partyName))
        {
            Debug.LogError("Party name cannot be empty!");
            return;
        }

        string filePath = GetFilePath(partyName);

        if (File.Exists(filePath))
        {
            File.Delete(filePath);
            listOfPartyNames.Remove(partyName);
            Debug.Log($"Party successfully deleted: {partyName}");
        }
        else
        {
            Debug.LogError($"Save file not found for party: {partyName}");
        }

        GameContent.RefreshUI();
    }
}

#endregion


