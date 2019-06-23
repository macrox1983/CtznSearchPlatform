using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BreakingStorm.Common.Primitives;

namespace BreakingStorm.Common.Network.UDP
{
    public class CommandBaseUDP
    {
        public CommandBaseUDP(ushort type)
        {
            this.Type = type;
            this.BytesList = new BytesList();            
        }

        public ushort Type { get; private set; }
        public BytesList BytesList { get; private set; }


        protected virtual void createBytesList()
        {
            this.BytesList.AddUShort(this.Type);            
        }
    }
}
