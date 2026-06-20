using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(
    fileName = "InputDisplayDatabase",
    menuName = "Input/Input Display Database"
)]
public class InputDisplayDatabase : ScriptableObject
{
    [SerializeField]
    public List<InputActionDisplayData> actions;

    private Dictionary<string, string> lookup;

    private void BuildLookup()
    {
        if (lookup != null)
            return;

        lookup = new();

        foreach (var action in actions)
        {
            if (action == null)
                continue;

            lookup[action.actionName] = action.displayName;
        }
    }

    public string GetDisplayName(string actionName)
    {
        if (lookup == null)
            BuildLookup();

        return lookup.TryGetValue(actionName, out string displayName) ? displayName : actionName;
    }
}