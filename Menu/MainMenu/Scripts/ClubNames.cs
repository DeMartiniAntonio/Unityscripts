using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "ClubNames", menuName = "Game Data/Club Names")]
public class ClubNames : ScriptableObject
{
    public List<string> names = new List<string>();
}