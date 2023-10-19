using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerColorDisplay : MonoBehaviour
{
    [SerializeField] private TankPlayer player;
    [SerializeField] private TeamColorLookup colorLookup;
    [SerializeField] private List<SpriteRenderer> spriteRenderers;

    // Start is called before the first frame update
    void Start()
    {
        HandlePlayerColorChanged(-1, player.TeamIndex.Value);
        player.TeamIndex.OnValueChanged += HandlePlayerColorChanged;
    }

    private void HandlePlayerColorChanged(int previousValue, int newValue)
    {
        Color teamColor = colorLookup.GetTeamColor(newValue);

        foreach (var spriteRenderer in spriteRenderers)
        {
            spriteRenderer.color = teamColor;
        }
    }

    private void OnDestroy()
    {
        player.TeamIndex.OnValueChanged -= HandlePlayerColorChanged;
    }
}