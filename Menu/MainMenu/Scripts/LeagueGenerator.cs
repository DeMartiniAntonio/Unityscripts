using System.Collections.Generic;
using UnityEngine;

public class LeagueGenerator : MonoBehaviour
{
    public ClubNames clubNamesData;               // Reference to Club Names ScriptableObject
    public BadgeAndColorsData badgeColorsData;    // Reference to Badge and Colors ScriptableObject
    public PlayerNames playerNamesData;           // Reference to Player Names ScriptableObject

    private List<GeneratedClub> generatedClubs = new List<GeneratedClub>(); // List of all generated clubs
    public int leagues = 7;          // Number of leagues
    public int teamsPerLeague = 20;  // Teams per league

    void Start()
    {
        GenerateLeagues();
    }
    public void StartLeagueGeneration()
    {
        Debug.Log("Play button clicked, generating leagues..."); 
        GenerateLeagues(); // Call your existing GenerateLeagues method
    }

    void GenerateLeagues()
    {
        // Copy data to avoid modifying the original lists
        List<string> availableClubNames = new List<string>(clubNamesData.names);
        List<BadgeAndColors> availableBadges = new List<BadgeAndColors>(badgeColorsData.badgesAndColors);
        List<string> availablePlayerNames = new List<string>(playerNamesData.playerNames);

        int clubIndex = 1; // Start indexing clubs from 1

        for (int league = 0; league < leagues; league++)
        {
            Debug.Log($"Generating League {league + 1}:");

            for (int team = 0; team < teamsPerLeague; team++)
            {
                // Randomize and pick unique data
                string clubName = GetRandomElement(availableClubNames);
                BadgeAndColors badgeColors = GetRandomElement(availableBadges);
                List<string> players = GetRandomPlayers(availablePlayerNames, 3);

                // Create the club
                GeneratedClub newClub = new GeneratedClub
                {
                    index = clubIndex,
                    clubName = clubName,
                    badge = badgeColors.badge,
                    color1 = badgeColors.color1,
                    color2 = badgeColors.color2,
                    players = players
                };

                // Store the club
                generatedClubs.Add(newClub);

                // Log the club's details
                Debug.Log($"Club {clubIndex}: {clubName}, Colors: {badgeColors.color1} & {badgeColors.color2}, Players: {string.Join(", ", players)}");

                clubIndex++; // Increment the index
            }
        }
    }

    T GetRandomElement<T>(List<T> list)
    {
        int randomIndex = Random.Range(0, list.Count);
        T element = list[randomIndex];
        list.RemoveAt(randomIndex); // Ensure uniqueness
        return element;
    }

    List<string> GetRandomPlayers(List<string> availablePlayerNames, int count)
    {
        List<string> players = new List<string>();
        for (int i = 0; i < count; i++)
        {
            string player = GetRandomElement(availablePlayerNames);
            players.Add(player);
        }
        return players;
    }
}