using AOSharp.Core.IPC;
using SmokeLounge.AOtomation.Messaging.Serialization;
using SmokeLounge.AOtomation.Messaging.Serialization.MappingAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kill.IPC
{
    [AoContract((int)OpCode.ConfigUpdate)]
    internal class ConfigUpdateMessage : IPCMessage
    {
        public ConfigUpdateMessage() { }

        public ConfigUpdateMessage(Config config, bool persistChanges)
        {
            Config = config;
            PersistChanges = persistChanges;
        }

        [AoMember(0)]
        public Config Config { get; set; }

        [AoMember(1)]
        public bool PersistChanges { get; set; }
    }
}
