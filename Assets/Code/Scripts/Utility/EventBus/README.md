# Unity C# Event Bus

## Description
This EventBus system provides a way to create decoupled architectures in Unity projects. It allows communication between different parts of an application without requiring direct references.

## Code Structure
This EventBus system contains several C# classes residing in the Scripts\EventBus directory:

1. `EventBus.cs` - Main EventBus class that provides static functions for registering, deregistering, and triggering custom events.

2. `EventBinding.cs` - IEventBinding interface and class definition for EventBinding, which is used to bind functions to events.

3. `Events.cs` - IEvent interface and sample code, which shows how to define custom events.

4. `PredefinedAssemblyUtil.cs` - Utility class for locating assemblies and finding types within them. See [Unity Documentation](https://docs.unity3d.com/Manual/ScriptCompileOrderFolders.html).

5. `EventBusUtil.cs` - Static initialization methods and additional utilities used for EventBus.


## Example Usage

The usage generally works like:

```csharp 
public struct PlayerEvent : IEvent {
    public int health;
    public int mana;
}

EventBinding<PlayerEvent> playerEventBinding;

void OnEnable() {    
    playerEventBinding = new EventBinding<PlayerEvent>(HandlePlayerEvent);
    EventBus<PlayerEvent>.Register(playerEventBinding);

    // Can Add or Remove Actions to/from the EventBinding
}

void OnDisable() {
    EventBus<PlayerEvent>.Deregister(playerEventBinding);
}

void Start() {
    EventBus<PlayerEvent>.Raise(new PlayerEvent {
        health = healthComponent.GetHealth(),
        mana = manaComponent.GetMana()
    });    
}

void HandlePlayerEvent(PlayerEvent playerEvent) {
    Debug.Log($"Player event received! Health: {playerEvent.health}, Mana: {playerEvent.mana}");
}
```