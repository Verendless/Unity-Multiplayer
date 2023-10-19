using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewTeamColorLookup", menuName = "Team Color Lookup")]
public class TeamColorLookup : ScriptableObject
{
    [SerializeField] private List<Color> teamColors = new List<Color>();

    public Color GetTeamColor(int teamIndex)
    {
        if (teamIndex < 0 || teamIndex >= teamColors.Count)
        {
            return Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
        }
        return teamColors[teamIndex];
    }
}
