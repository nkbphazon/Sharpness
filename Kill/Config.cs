using AOSharp.Core.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using AOSharp.Core.IPC;
using Kill.IPC;
using SmokeLounge.AOtomation.Messaging.Serialization.MappingAttributes;
using SmokeLounge.AOtomation.Messaging.Serialization;

namespace Kill
{
    internal class Config : INotifyPropertyChanged
    {
        public static Config FromString(string value)
        {
            return JsonConvert.DeserializeObject<Config>(value);
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

        [JsonIgnore]
        public static Config Default => new Config
        {
            AttackRange = 20,
            TauntRange = 30,
            DisableOnZone = true,
            IPCChannelNumber = 87,
            UpdateInterval = .1,
            StopAttackingOnDisable = true,
            CommandKeyword = "kill",
            UseDefaultHandler = true,
            ChatLogLevel = LogLevel.Information,
            FileLogLevel = LogLevel.Off,
            DebugLogLevel = LogLevel.Off,
            IgnoreList = {}
        };

        public event PropertyChangedEventHandler PropertyChanged;
        private float attackRange;
        private float tauntRange;
        private bool disableOnZone;
        private byte ipcChannelNumber;
        private double updateInterval;
        private bool stopAttackingOnDisable;
        private string commandKeyword;
        private bool useDefaultHandler;
        private LogLevel chatLogLevel;
        private LogLevel fileLogLevel;
        private LogLevel debugLogLevel;
        private string[] ignoreList;

        [AoMember(0)]
        [DefaultValue(20)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public float AttackRange
        {
            get { return attackRange; }
            set { attackRange = value; NotifyPropertyChanged(); }
        }

        [AoMember(1)]
        [DefaultValue(30)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public float TauntRange
        {
            get { return tauntRange; }
            set { tauntRange = value; NotifyPropertyChanged(); }
        }

        [AoMember(2)]
        [DefaultValue(true)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public bool DisableOnZone
        {
            get { return disableOnZone; }
            set { disableOnZone = value; NotifyPropertyChanged(); }
        }

        [AoMember(3)]
        [DefaultValue(87)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public byte IPCChannelNumber
        {
            get { return ipcChannelNumber; }
            set { ipcChannelNumber = value; NotifyPropertyChanged(); }
        }

        [AoMember(4)]
        [DefaultValue(.1)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public double UpdateInterval
        {
            get { return updateInterval; }
            set { updateInterval = value; NotifyPropertyChanged(); }
        }

        [AoMember(5)]
        [DefaultValue(true)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public bool StopAttackingOnDisable
        {
            get { return stopAttackingOnDisable; }
            set { stopAttackingOnDisable = value; NotifyPropertyChanged(); }
        }

        [AoMember(6, SerializeSize = ArraySizeType.Int16)]
        [DefaultValue("kill")]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public string CommandKeyword
        {
            get { return commandKeyword; }
            set { commandKeyword = value; NotifyPropertyChanged(); }
        }

        [AoMember(7)]
        [DefaultValue(true)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public bool UseDefaultHandler
        {
            get { return useDefaultHandler; }
            set { useDefaultHandler = value; NotifyPropertyChanged(); }
        }

        [AoMember(8)]
        [DefaultValue(LogLevel.Information)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public LogLevel ChatLogLevel
        {
            get { return chatLogLevel; }
            set { chatLogLevel = value; NotifyPropertyChanged(); }
        }

        [AoMember(9)]
        [DefaultValue(LogLevel.Off)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public LogLevel FileLogLevel
        {
            get { return fileLogLevel; }
            set { fileLogLevel = value; NotifyPropertyChanged(); }
        }

        [AoMember(10)]
        [DefaultValue(LogLevel.Off)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public LogLevel DebugLogLevel
        {
            get { return debugLogLevel; }
            set { debugLogLevel = value; NotifyPropertyChanged(); }
        }

        [AoMember(11, SerializeSize = ArraySizeType.Int16)]
        [DefaultValue(new string[] {})]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public string[] IgnoreList
        {
            get { return ignoreList; }
            set { ignoreList = value; NotifyPropertyChanged(); }
        }
        
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
