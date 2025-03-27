using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "PlayerNames", menuName = "Game Data/Player Names")]
public class PlayerNames : ScriptableObject
{
    public List<string> playerNames = new List<string>();
}