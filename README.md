# Sharpness

<p>Sharpness is a collection of AOSharp plugins that prioritize stability over new features.</p>

### Kill
<p>The kill plugin will automatically initiate combat and switch targets based on predefined handlers. Handlers are generally playfield-specific, however a default handler is also included in the event that a more specific handler is not available. Currently, custom handlers for the following areas are implemented:</p>

* Pandemonium

#### Quick Start
<p>Log in your characters and inject the plugin on each of them. If this is the first time you have used the plugin, the default configuration will used. It is fairly well-suited for general use, but if you want to customize things like attack/taunt range, or additional targets to ignore, see the configuration options below. Note that changes made to the configuration will affect the current session, but will be lost unless they are saved (again, see below).</p>

<p>Perhaps you wish to permanently adjust the taunt range setting for all of your characters from the default value of 30 down to 25. To do this you would issue the following commands:</p>

```
/kill tauntrange 25
/kill saveall
```

The first command will set the taunt range on the character that the command was ran from. The second command will propagate this character's settings to all other characters and save it as their new default.
</p>

<p>Review the remaining settings with the /kill config command. If everything looks good, move your characters to the area you wish to fight in, and enable the plugin using the /kill enable command. The plugin will begin looking for targets within range to fight, and if it finds them, will begin combat with them. The behavior of the plugin will vary based on which handler is being used, with more favorable results generally coming from area-specific handlers like Pandemonium.</p>

<p>When you are finished fighting, you can disable the plugin with the /kill disable command. Note: by default, the plugin is also disabled each time you zone.</p>

#### Configuration
<p>The plugin supports a number of configuration options which can be interacted with via chat commands. All commands are prefixed with a common keyword which defaults to /kill but can be changed (see below). Changes made to settings will be reverted back to the last saved value on inject and can be persisted with the save/saveall commands (see below).</p>

| Command | Parameters | Default | Description | Example |
|---------|------------|---------|-------------|---------|
| enable  | N/A        | N/A | Enables the plugin to begin initiating combat | /kill enable |
| disable | N/A        | N/A | Disables the plugin to stop initiating combat | /kill disable |
| attackrange | (float) range | 20 | Sets the maximum distance between your character and a target to attack it | /kill attackrange 25.32 |
| tauntrange | (float) range | 30 | Sets the maximum distance between your character and a target to use a taunt tool on it | /kill tauntrange 37.8 |
| disableonzone | true/false | true | When true, the plugin will be disabled each time you zone and require re-enabling | /kill disableonzone false |
| updateinterval | (double) interval | 0.1 | Sets how frequently to poll for targets in seconds. Increasing this value may improve performance on older PCs | /kill updateinterval 0.5 |
| stopattackingondisable | true/false | true | When true, characters will stop attacking their current target when the disable command is issued | /kill stopattackingondisable false |
| keyword | (string) keyword | kill | Sets the /keyword used for chat commands | /kill keyword bot (commands will now be /bot xyz instead of /kill xyz)|
| channel | (byte) 1-255 | 87 | Sets the IPC channel the plugin uses when communicating with other characters. Characters on the same channel will see each other's communication | /kill channel 123 |
| usedefaulthandler | true/false | true | When true, the plugin will use the default handler when a more specific one is not available. The default handler is very simplistic and could result in unintended actions, such as attacking guards in cities. Use with caution. | /kill usedefaulthandler false |
| ignore | (string) name | N/A | Adds an entry to the plugin's ignore list. If a target's name contains any of the words in the ignore list, it will not be attacked. (case-insensitive) | /kill ignore Heckler |
| unignore | (string) name | N/A | Removes an entry from the plugin's ignore list. The provided name must be a case-insensitive match with the entry in the ignore for it to be removed. | /kill unignore Heckler |
| chatloglevel | (LogLevel) level | Information | Sets the log verbosity for messages that are logged to the ingame chat window. Valid log levels are Verbose, Debug, Information, Warning, Error, Fatal, Off | /kill chatloglevel Debug |
| fileloglevel | (LogLevel) level | Off | Sets the log verbosity for messages that are logged to the log file. Valid log levels are Verbose, Debug, Information, Warning, Error, Fatal, Off | /kill fileloglevel Debug |
| debugloglevel | (LogLevel) level | Off | Sets the log verbosity for messages that are logged to an attached debugger. Valid log levels are Verbose, Debug, Information, Warning, Error, Fatal, Off | /kill debugloglevel Debug |
| config | N/A | N/A | Shows the current configuration of the plugin | /kill config |
| save | N/A | N/A | Saves the current configuration of the plugin for only the character that the command was run from. | /kill save |
| saveall | N/A | N/A | Sends the current configuration of the plugin that the command was run from to all characters on the same channel and saves it. This can be used to quickly set multiple characters to the same configuration. | /kill saveall |
| sendall | N/A | N/A | Sends the current configuration of the plugin that the command was run from to all characters on the same channel but does not save it. This can be used to quickly set multiple characters to the same configuration without changing their stored configuration. | /kill sendall |
