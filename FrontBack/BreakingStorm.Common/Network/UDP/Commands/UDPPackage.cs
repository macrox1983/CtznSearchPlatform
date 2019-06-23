using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BreakingStorm.Common.Primitives;
using BreakingStorm.Common.Cryptography;

namespace BreakingStorm.Common.Network.UDP
{
    public class UDPPackage
    {
        public UDPPackage()
        {
            this.Broken = true;
        }

        public UDPPackage(BytesList body, UInt32 sign=0)
        {
            this.Body = body;
            this.Sign = sign;

            BytesList preCRCData = new BytesList();
            preCRCData.AddUInt(this.Sign);
            preCRCData.AddUShort(this.Parts);//==0
            preCRCData.Add(body);

            this.CRC = CRC32.Calculate(preCRCData);
            this.FullData.AddUInt(this.CRC);
            this.FullData.Add(preCRCData);            
        }

        public UDPPackage(ushort partsPackageId, ushort parts, ushort part, BytesList body, UInt32 sign = 0)
        {
            this.PartsPackageId = partsPackageId;
            this.Parts = parts;
            this.Part = part;
            this.Body = body;
            this.Sign = sign;

            BytesList preCRCData = new BytesList();
            preCRCData.AddUInt(this.Sign);
            preCRCData.AddUShort(this.Parts);//!=0
            preCRCData.AddUShort(this.Part);
            preCRCData.AddUShort(this.PartsPackageId);
            preCRCData.Add(body);

            this.CRC = CRC32.Calculate(preCRCData);
            this.FullData.AddUInt(this.CRC);
            this.FullData.Add(preCRCData);            
        }

        public bool Broken { get; private set; } = false;
        public UInt32 CRC { get; private set; }
        public UInt32 Sign { get; private set; } //подпись
        public ushort Parts { get; private set; } = 0;
        public ushort Part { get; private set; } = 0;
        public ushort PartsPackageId { get; private set; } = 0; //идентификатор пакета 
        public BytesList Body { get; private set; }        
        public BytesList FullData { get; private set; }
        

        public static UDPPackage Parse(byte[] bytes)
        {
            Stream stream = new MemoryStream(bytes);
            BinaryReader reader = new BinaryReader(stream);
            uint crc = reader.ReadUInt32();
            uint checkCRC = CRC32.Calculate(reader.ReadBytes(bytes.Length - 4));
            if (crc != checkCRC) return new UDPPackage(); //пакет битый
            stream.Position = 4;
            uint sign = reader.ReadUInt32();
            ushort parts = reader.ReadUInt16();
            BytesList body = new BytesList();
            if (parts == 0)
            {
                body.Add(reader.ReadBytes(bytes.Length - 4 - 4 - 2));
                return new UDPPackage(body, sign);
            } else
            {
                ushort part = reader.ReadUInt16();
                ushort partsPackageId = reader.ReadUInt16();
                body.Add(reader.ReadBytes(bytes.Length - 4 - 4 - 2 - 2 - 2));
                return new UDPPackage(partsPackageId, parts, part, body, sign);
            }            
        }
        
    }
}
