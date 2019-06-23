using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BreakingStorm.Common.Primitives;
using BreakingStorm.Common.Cryptography;

namespace BreakingStorm.Common.Network.UDP
{
    public class ShellCommand
    {
        public const int maxByteSize = 508-12;

        public ShellCommand(CommandBaseUDP command, ushort initPartsPackageId)
        {
            if (command.BytesList.Count>maxByteSize)
            {
                ushort parts = (ushort)(command.BytesList.Count / maxByteSize);
                this.PartsPackageId = initPartsPackageId;
                for (ushort part =0; part < parts; part++)
                {
                    BytesList data = new BytesList(); data.Add(command.BytesList.GetBytes(maxByteSize*part, maxByteSize));
                    this.Packages.Add(new UDPPackage(this.PartsPackageId, parts, part, data));
                }
            } else
            {
                this.Packages.Add(new UDPPackage(command.BytesList));
            }            
        }

        public IList<UDPPackage> Packages { get; private set; } = new List<UDPPackage>();
        public ushort PartsPackageId { get; private set; } = 0; //идентификатор пакета         

        
    }
}
