Twitch Integration Setup

Credentials
    You must provide a username and oauth token in the SECRET_credentials.json file to have the game chat on your behalf.
    You may use your account or a new account, but it is suggested to use your account as the broadcaster so that
    it can bypass spam restrictions or filters.
    The oauth token must be of the form "oauth:ABCDEFGHIJKLMNOPQRSTUVWXYZ1234", but it is case insensitive
    
    Get the login token here:
    https://asquared31415.github.io/twitchintegration/index.html

Config
    Channel Name: The name of the Twitch channel to interact with.  This should be your username. (example: asquared31415)
    Time Between Votes: The amount of real time, in seconds, that should pass between the end of one vote and the start of the next. (example: 600)
    Voting Time: The amount of real time, in seconds, that a vote should be active for. (example: 60)
    Options per Vote: The number of options that should be available to vote for.
    Use Twitch Username Colors: Whether to copy a chatter's username color to their duplicant's name, if they are spawned.
                                Note: If a user has not set their username color manually, it will be white, even though Twitch chat picks a random color.
    Show Notifications: Whether to show any notifications from the Twitch Integration mod or any events.
    Show Vote Choice Notifications: Whether to show notifications when a vote starts. This can be disabled if you want to not know what options chat has.
    Min Danger: Events with a danger less than this setting will not appear.
    Max Danger: Events with a danger greater than this setting will not appear.
    Edit Event Config: Opens a page in a browser to edit the current event config. (WARNING: SPOILERS)
