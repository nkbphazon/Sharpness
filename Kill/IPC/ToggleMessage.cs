using AOSharp.Core.IPC;
using SmokeLounge.AOtomation.Messaging.Serialization.MappingAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kill.IPC
{
    [AoContract((int)OpCode.Toggle)]
    internal class ToggleMessage : IPCMessage
    {
        public override short Opcode => (short)OpCode.Toggle;

        public ToggleMessage() { }

        public ToggleMessage(bool isEnabled)
        {
            IsEnabled = isEnabled;
        }

        [AoMember(0)]
        public bool IsEnabled { get; set; }
    }
}
