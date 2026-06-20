using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(EnemyIndexAttribute))]
public class EnemyIndexDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var registry = EnemyRegistryDatabase.Registry;

        if (registry == null || registry.enemies.Count == 0)
        {
            EditorGUI.PropertyField(position, property, label);
            return;
        }

        string[] enemyNames = registry.enemies
            .ConvertAll(enemy => enemy.enemy.name)
            .ToArray();

        property.intValue = EditorGUI.Popup(
            position,
            label.text,
            property.intValue,
            enemyNames
        );
    }
}