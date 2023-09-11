# Oxygen Not Included Twitch Integration

## Developer Documentation:

Looking to add events to the Twitch Integration mod? See the [API Documentation](xref:api_index).

You will need to reference the [Twitch Integration API](https://github.com/asquared31415/ONITwitch/releases/latest) dll and use [ILRepack](https://github.com/gluck/il-repack) or a similar tool to include the library in your mod. The API library allows you to interact with the Twitch Integration mod without requiring a reference to the Twitch Integration mod, which would crash if the user did not have the mod active.

Complete examples can be found on [GitHub](https://github.com/asquared31415/ONITwitch/blob/main/TwitchTestExtension/TestTwitchExtension.cs#L21).
