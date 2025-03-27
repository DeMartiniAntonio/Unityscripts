using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "BadgeAndColorsData", menuName = "Game Data/Badge And Colors")]
public class BadgeAndColorsData : ScriptableObject
{
    public List<BadgeAndColors> badgesAndColors = new List<BadgeAndColors>();
}