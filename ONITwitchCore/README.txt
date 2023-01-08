Twitch Integration Setup

1. Credentials
  You must provide a username and oauth token in the SECRET_credentials.json file to have the game chat on your behalf.
  You may use your account or a new account, but it is suggested to use your account as the broadcaster so that
  it can bypass spam limits.
  The oauth token must be of the form "oauth:ABCDEFGHIJKLMNOPQRSTUVWXYZ1234", but it is case insensitive
  
  IMPORTANT LINK:
  https://asquared31415.github.io/twitchintegration/index.html

2. Config
  The config.json file contains some simple configuration to do with voting.
   - Channel is the channel to READ and CHAT to to check for votes and announce votes.
   - Vote delay is the amount of time in seconds that elapses between the end of one vote and the start of the next.
     (Default = 10)
   - Vote time is the amount of time in seconds that users will have to get in votes before the vote is locked in.
     (Default = 25)
  NOTE: This is measured starting as soon as the vote is locked in, not from the end of the vote's effect!  For example
  if you have a 10 second vote delay and the previous vote has an effect that lasts 20 seconds, the effect will overlap
  the start of the next vote by 10 seconds.
   - MinDanger is a value from 0 to 6 (inclusive) that represents the *least* danger that you want to be able to show up
     in a vote.  (Default = 0)
   - MaxDanger is a value from 0 to 6 (inclusive) that represents the *most* danger that you want to be able to show up
     in a vote.  (Default = 4)
     - The danger values from 0 to 6 in order are:
        None (No harm at all)
     	Minimal (Unlikely indirect harm, typically tiles being places they shouldn't be)
     	Small (Possible indirect harm if mishandled)
     	Medium (Likely indirect harm)
     	High (Likely indirect or direct harm)
     	Extreme (Very dangerous and likely death)
     	Deadly (Direct death of dupes or important things)
  
3. Commands
  The commands.json file stores the data about all commands that are possible to be chosen and their weights.
   - Name is the name of the command.
   - Weight is a decimal number that indicates how relatively likely a command is to be chosen.  A command with a weight
     ten times larger than another command is ten times more likely to be chosen.  Weights may be any positive real number.
   - Data documentation coming soon  