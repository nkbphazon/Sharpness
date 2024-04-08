using AOSharp.Core;
using AOSharp.Core.IPC;
using AOSharp.Core.Logging;
using AOSharp.Core.UI;
using Kill.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using System.IO;
using Kill.IPC;
using System.ComponentModel;
using AOSharp.Common.GameData;
using AOSharp.Core.Inventory;

namespace Kill
{
    public class Kill : AOPluginEntry
    {
        private List<IAttackHandler> handlers;
        private DefaultAttackHandler defaultAttackHandler;
        private bool isEnabled = false;
        private double lastUpdateTime = 0;
        private IPCChannel ipcChannel;
        private Config config;

        private bool IsEnabled {
            get { return isEnabled; }
            set { isEnabled = value; Logger.Information($"IsEnabled={isEnabled}"); }
        }

        public override void Run()
        {
            ChatLogLevel = LogLevel.Information;
            FileLogLevel = LogLevel.Debug;
            DebugLogLevel = LogLevel.Off;

            Logger.Information("Plugin loading..");

            LoadConfig();
            config.PropertyChanged += OnConfigChanged;

            handlers = new List<IAttackHandler>();
            defaultAttackHandler = new DefaultAttackHandler();
            handlers.Add(new PandemoniumAttackHandler());

            ipcChannel = new IPCChannel(config.IPCChannelNumber);
            ipcChannel.RegisterCallback((int)OpCode.Toggle, OnToggleMessage);
            ipcChannel.RegisterCallback((int)OpCode.ConfigUpdate, OnConfigUpdateMessage);

            Game.OnUpdate += OnUpdate;
            Game.TeleportStarted += TeleportStarted;
            Chat.RegisterCommand(config.CommandKeyword, OnChatCommand);

            Logger.Information($"Plugin loaded :: {handlers.Count} handlers found. Use /{config.CommandKeyword} for help.");
        }

        private void OnChatCommand(string command, string[] args, ChatWindow window)
        {
            string helpText = $@"Available commands:
    enable : Enable the plugin
    disable : Disable the plugin
    attackrange &lt;float&gt; : Set how close a target must be to attack it
    tauntrange &lt;float&gt; : Set how close a target must be to taunt it
    disableonzone &lt;true|false&gt; : Disable the plugin each time you zone
    updateinterval &lt;double&gt; : How frequently to check for new targets
    stopattackingondisable &lt;bool&gt; : Stop attacking when the plugin is disabled
    keyword &lt;string&gt; : Set the keyword to access commands (currently {config.CommandKeyword})
    channel &lt;byte&gt; : Set the IPC channel number
    usedefaulthandler &lt;true|false&gt; : Enable or disable the default handler
    ignore &lt;string&gt; : Do not attack targets with a name containing this value (case insensitive)
    unignore &lt;string&gt; : Remove an ignore entry (case insensitive)
    chatloglevel &lt;LogLevel&gt; ({string.Join(", ", Enum.GetNames(typeof(LogLevel)))}) : Log verbosity when writing to chat
    fileloglevel &lt;LogLevel&gt; ({string.Join(", ", Enum.GetNames(typeof(LogLevel)))}) : Log verbosity when writing to the log file
    debugloglevel &lt;LogLevel&gt; ({string.Join(", ", Enum.GetNames(typeof(LogLevel)))}) : Log verbosity when writing to debugger
    config : View current configuration
    save : Save this character's configuration to disk
    saveall : Send this character's configuration to all listening characters and saves them to disk
    sendall : Send this character's configuration to all listening characters but does not save them to disk
            ";

            if (args.Length == 0)
            {
                Logger.Information(helpText);
                return;
            }

            switch (args[0].ToLower())
            {
                case "enable":
                    IsEnabled = true;
                    ipcChannel.Broadcast(new ToggleMessage(true));
                    break;
                case "disable":
                    IsEnabled = false;
                    ipcChannel.Broadcast(new ToggleMessage(false));
                    break;
                case "attackrange":
                    if (args.Length == 2 && float.TryParse(args[1], out float attackRange))
                        config.AttackRange = attackRange;
                    break;
                case "tauntrange":
                    if (args.Length == 2 && float.TryParse(args[1], out float tauntRange))
                        config.TauntRange = tauntRange;
                    break;
                case "disableonzone":
                    if (args.Length == 2 && bool.TryParse(args[1], out bool disableOnZone))
                        config.DisableOnZone = disableOnZone;
                    break;
                case "updateinterval":
                    if (args.Length == 2 && double.TryParse(args[1], out double updateInterval))
                        config.UpdateInterval = updateInterval;
                    break;
                case "stopattackingondisable":
                    if (args.Length == 2 && bool.TryParse(args[1], out bool stopAttackingOnDisable))
                        config.StopAttackingOnDisable = stopAttackingOnDisable;
                    break;
                case "chatloglevel":
                    if (args.Length == 2 && Enum.TryParse(args[1], out LogLevel chatLogLevel))
                        config.ChatLogLevel = chatLogLevel;
                    break;
                case "fileloglevel":
                    if (args.Length == 2 && Enum.TryParse(args[1], out LogLevel fileLogLevel))
                        config.FileLogLevel = fileLogLevel;
                    break;
                case "debugloglevel":
                    if (args.Length == 2 && Enum.TryParse(args[1], out LogLevel debugLogLevel))
                        config.DebugLogLevel = debugLogLevel;
                    break;
                case "keyword":
                    if (args.Length == 2 && !string.IsNullOrEmpty(args[1]))
                        config.CommandKeyword = args[1];
                    break;
                case "channel":
                    if (args.Length == 2 && byte.TryParse(args[1], out byte channel))
                        config.IPCChannelNumber = channel;
                    break;
                case "usedefaulthandler":
                    if (args.Length == 2 && bool.TryParse(args[1], out bool useDefaultHandler))
                        config.UseDefaultHandler = useDefaultHandler;
                    break;
                case "ignore":
                    if (args.Length == 2 && !string.IsNullOrEmpty(args[1]) && !config.IgnoreList.Contains(args[1]))
                        config.IgnoreList = config.IgnoreList.Append(args[1]).ToArray();
                    break;
                case "unignore":
                    if (args.Length == 2 && !string.IsNullOrEmpty(args[1]) && config.IgnoreList.Contains(args[1]))
                        config.IgnoreList = config.IgnoreList.Where(i => i != args[1]).ToArray();
                    break;
                case "config":
                    Logger.Information($"Current configuration: {config}");
                    break;
                case "save":
                    SaveConfig();
                    break;
                case "saveall":
                    ipcChannel.Broadcast(new ConfigUpdateMessage(config, true));
                    break;
                case "sendall":
                    ipcChannel.Broadcast(new ConfigUpdateMessage(config, false));
                    break;
            }

        }

        private void OnConfigChanged(object sender, PropertyChangedEventArgs e)
        {
            var propertyInfo = sender.GetType().GetProperty(e.PropertyName);
            var type = propertyInfo.PropertyType;
            var value = propertyInfo.GetValue(sender, null);

            switch (type.Name)
            {
                case "String[]":
                    var arrayValues = value as string[];
                    if (arrayValues.Length > 0)
                        Logger.Information($"{e.PropertyName} set to: \n{string.Join("\n", arrayValues)}");
                    else
                        Logger.Information($"{e.PropertyName} is now empty");
                    break;
                default:
                    Logger.Information($"{e.PropertyName} set to {value}");
                    break;
            }
        }

        private void SaveConfig()
        {
            File.WriteAllText(PlayerSettingsFile.FullName, config.ToString());
        }

        private void LoadConfig()
        {
            if (!File.Exists(PlayerSettingsFile.FullName))
            {
                config = Config.Default;
                SaveConfig();
            }

            config = Config.FromString(File.ReadAllText(PlayerSettingsFile.FullName));
        }

        private void TeleportStarted(object sender, EventArgs e)
        {
            if (config.DisableOnZone)
                IsEnabled = false;
        }

        private void OnUpdate(object sender, float e)
        {
            if (!IsEnabled || Time.NormalTime < lastUpdateTime + config.UpdateInterval)
                return;

            lastUpdateTime = Time.NormalTime;

            // if we already have an attack queued, return
            if (DynelManager.LocalPlayer.IsAttackPending)
                return;

            // pick a target
            var target = GetPriorityTarget();

            // if there are no targets available, stop attacking and return
            if (target == null)
            {
                if (DynelManager.LocalPlayer.IsAttacking)
                    DynelManager.LocalPlayer.StopAttack();
                return;
            }

            // if there is a target available but we are already fighting it, return
            if (target.InstanceEquals(DynelManager.LocalPlayer.FightingTarget))
                return;

            // if there is a target available that is outside of attack range but inside of taunt range, try to taunt it
            if (config.TauntRange > config.AttackRange && target.DistanceFrom(DynelManager.LocalPlayer) > config.AttackRange)
            {
                // there is no point taunting rooted targets as they cannot approach us
                if (target.CanMove())
                    Taunt(target);

                return;
            }

            // if there is a target available but we are not attacking it, start attacking it
            DynelManager.LocalPlayer.Attack(target);
        }

        private void OnToggleMessage(int sender, IPCMessage message)
        {
            ToggleMessage toggleMessage = message as ToggleMessage;
            if (toggleMessage != null)
                IsEnabled = toggleMessage.IsEnabled;
        }

        private void OnConfigUpdateMessage(int sender, IPCMessage message)
        {
            ConfigUpdateMessage settingsUpdateMessage = message as ConfigUpdateMessage;
            if (settingsUpdateMessage != null)
            {
                Logger.Information($"New config received :: {settingsUpdateMessage.Config} :: PersistChanges={settingsUpdateMessage.PersistChanges}");
                config.PropertyChanged -= OnConfigChanged;
                config =settingsUpdateMessage.Config;
                config.PropertyChanged += OnConfigChanged;
                
                if (settingsUpdateMessage.PersistChanges)
                    SaveConfig();
            }
        }

        private SimpleChar GetPriorityTarget()
        {
            var handler = handlers.FirstOrDefault(h => h.IsActive(Playfield.ModelIdentity.Instance, DynelManager.LocalPlayer.Position));

            if (handler == null)
            {
                if (!config.UseDefaultHandler)
                    return null;

                handler = defaultAttackHandler;
            }

            // get targets to be prioritized by the handler
            var validTargets = DynelManager.Characters
                .Where(target =>
                    (!target.IsPet || handler.TargetsIncludePets()) &&
                    (!target.IsPlayer || handler.TargetsIncludePlayers()) &&
                    !IsManuallyIgnored(target) &&
                    target.IsAlive &&
                    (target.DistanceFrom(DynelManager.LocalPlayer) <= config.AttackRange || target.DistanceFrom(DynelManager.LocalPlayer) <= config.TauntRange) &&
                    target.IsInLineOfSight);

            // don't call the handler if there aren't targets to prioritize
            if (validTargets.Count() == 0)
                return null;

            // the handler will reorder the list of targets
            validTargets = handler.PrioritizeTargets(validTargets);

            // return the first target from the list as it represents the highest priority
            return handler.PrioritizeTargets(validTargets).FirstOrDefault();
        }

        private void Taunt(SimpleChar target)
        {
            if (LocalCooldown.IsOnCooldown(Stat.Psychology))
                return;

            if (Item.HasPendingUse)
                return;

            int[] tauntTools = new int[]
            {
                83919,  // Aggression Multiplier
                83920,  // Aggression Enhancer
                151692, // Modified Aggression Enhancer (low)
                151693, // Modified Aggression Enhancer (High)
                152028, // Aggression Multiplier (Jealousy Augmented)
                152029, // Aggression Enhancer (Jealousy Augmented)
                253186, // Codex of the Insulting Emerto (Low)
                253187, // Codex of the Insulting Emerto (High)
            };

            var tauntTool = Inventory.Items.FirstOrDefault(item => tauntTools.Contains(item.Id));

            if (tauntTool == null)
                return;

            tauntTool.Use(target, true);
        }

        private bool IsManuallyIgnored(SimpleChar character)
        {
            if (config.IgnoreList.Length == 0)
                return false;

            foreach (string name in config.IgnoreList)
            {
                if (character.Name.ToLower().Contains(name.ToLower()))
                    return true;
            }

            return false;
        }
    }
}
