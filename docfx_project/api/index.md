---
uid: api_index
---

### Main API Entry Points

#### Check that the Twitch Integration Mod is enabled!!!

[TwitchModInfo.TwitchIsPresent](xref:ONITwitchLib.TwitchModInfo.TwitchIsPresent)
Most other methods will crash if called when the Twitch Integration mod is not present. Check the status of this bool to be sure that the Twitch Integration mod is present and enabled.

Most methods should be called **after** the Twitch Integration mod has run its initialization. This typically means running all of your event setup in a patch:
```cs
[HarmonyPatch(typeof(Db), "Initialize")]
[HarmonyAfter(TwitchModInfo.StaticID)]
public static class Db_Initialize_Patch {
```
The `HarmonyAfter` attribute takes the StaticID of the mod that you want to run after, and in this case is used to run after the Twitch Integration mod.

Complete examples can be found on [GitHub](https://github.com/asquared31415/ONITwitch/blob/main/TwitchTestExtension/TestTwitchExtension.cs#L21).

#### Creating an Event

Create an @ONITwitchLib.EventGroup:
 - [EventGroup.GetOrCreateGroup(string)](xref:ONITwitchLib.EventGroup.GetOrCreateGroup(System.String))
 - [EventGroup.DefaultSingleEventGroup(string, int, string)](xref:ONITwitchLib.EventGroup.DefaultSingleEventGroup(System.String,System.Int32,System.String))
   - This creates both an event and a group, returning both to be used.

Add an event to an EventGroup:
 - [EventGroup.AddEvent(string,int,string)](xref:ONITwitchLib.EventGroup.AddEvent(System.String,System.Int32,System.String))

Modify an @ONITwitchLib.EventInfo:
 - Set the display name of the event (if not already set): [FriendlyName](xref:ONITwitchLib.EventInfo.FriendlyName)
 - Specify how dangerous the event is: [Danger](xref:ONITwitchLib.EventInfo.Danger)
 - Add code to the event: [AddListener](xref:ONITwitchLib.EventInfo.AddListener(System.Action{System.Object}))
 - Add a condition for the event to appear in a vote: [AddCondition](xref:ONITwitchLib.EventInfo.AddCondition(System.Func{System.Object,System.Boolean}))
   - NOTE: The condition may become false between it showing up in a vote and the code being run. Design your event's code to be resistant to any possible states. Do nothing and show a toast rather than crash.
   - Examples of invalidated state:
     - At least one dupe was alive before, but now none are, so an event needs to find a default target or do nothing.
     - A rocket was in flight when the condition ran, but landed or self destructed.
     - A building existed when the condition ran, but has since been deconstructed, melted, or otherwise damaged.

Add an EventGroup to the deck of possible events:
 - [TwitchDeckManager.Instance](xref:ONITwitchLib.Core.TwitchDeckManager.Instance)
 - [TwitchDeckManager.Instance.AddGroup](xref:ONITwitchLib.Core.TwitchDeckManager.AddGroup(ONITwitchLib.EventGroup))
