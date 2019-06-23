using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BreakingStorm.Common.Helpers;

namespace BreakingStorm.Common.Primitives
{
    public class BytesList : IEnumerable
    {
        #region Static Fields

        public static readonly byte[] Empty = new byte[0];

        #endregion

        #region Fields

        private readonly List<byte> list = new List<byte>();

        #endregion

        #region Public Methods and Operators

        public int Count { get { return this.list.Count; } }

        public static implicit operator byte[] (BytesList bytesList)
        {
            return bytesList.GetBytes();
        }

        public void CopyFromBytesList(BytesList bytesList)
        {
            this.list.Clear();
            this.AddBytesList(bytesList);
        }

        public void Add(byte singleByte)
        {
            this.list.AddRange(HelperBitsConvertor.GetBytes((int)singleByte));
        }


        public void AddByte(byte singleByte)
        {
            byte[] b = new byte[1];
            b[0] = (byte)singleByte;
            this.list.AddRange(b);
        }

        public void AddBool(bool Bool)
        {
            byte[] b = new byte[1];
            if (Bool) b[0] = 1; else b[0] = 0;
            this.list.AddRange(b);
        }

        public void AddShort(short value)
        {
            this.list.AddRange(HelperBitsConvertor.GetBytes((short)value));
        }

        public void AddUShort(ushort value)
        {
            this.list.AddRange(HelperBitsConvertor.GetBytes((ushort)value));
        }

        public void Add(double value)
        {
            this.list.AddRange(HelperBitsConvertor.GetBytes(value));
        }

        public void Add(float value)
        {
            this.list.AddRange(HelperBitsConvertor.GetBytes(value));
        }

        public void AddFloat(float value)
        {
            this.list.AddRange(HelperBitsConvertor.GetBytes(value));
        }

        public void Add(sbyte singleSignedByte)
        {
            this.list.AddRange(HelperBitsConvertor.GetBytes(singleSignedByte));
        }

        public void Add(byte[] bytes)
        {
            this.list.AddRange(bytes);
        }

        public void AddBytesList(BytesList bytesList)
        {
            this.list.AddRange(bytesList.toArray());
        }


        public void AddUInt(uint value)
        {
            this.list.AddRange(HelperBitsConvertor.GetBytes((uint)value));
        }

        public void AddLong(long value)
        {
            this.list.AddRange(HelperBitsConvertor.GetBytes((long)value));
        }

        public void Add(uint value)
        {
            this.list.AddRange(HelperBitsConvertor.GetBytes((uint)value));
        }

        public void Add(ushort value)
        {
            this.list.AddRange(HelperBitsConvertor.GetBytes((int)value));
        }

        public void Add(bool value)
        {
            this.list.Add(value ? (byte)1 : (byte)0);
        }

        public void AddInt(int value)
        {
            this.list.AddRange(HelperBitsConvertor.GetBytes((int)value));
        }

        public void Add(int value)
        {
            this.list.AddRange(HelperBitsConvertor.GetBytes(value));
        }

        public void Add(short value)
        {
            this.list.AddRange(HelperBitsConvertor.GetBytes((int)value));
        }

        public void Add(long value)
        {
            this.list.AddRange(HelperBitsConvertor.GetBytes(value));
        }

        public void Add(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                this.list.AddRange(HelperBitsConvertor.GetBytes((ushort)0));
                return;
            }

            byte[] bytes = HelperBitsConvertor.ToBytes(str);
            byte[] byt2es = HelperBitsConvertor.GetBytes((ushort)bytes.Length);
            this.list.AddRange(HelperBitsConvertor.GetBytes((ushort)bytes.Length));
            this.list.AddRange(bytes);
        }

        public void AddString1ByteHead(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                this.list.Add((byte)0);
                return;
            }
            byte[] bytes = HelperBitsConvertor.ToBytes(str);
            this.list.AddRange(HelperBitsConvertor.GetBytes((byte)bytes.Length));
            this.list.AddRange(bytes);
        }

        public void AddString2ByteHead(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                this.list.AddRange(HelperBitsConvertor.GetBytes((ushort)0));
                return;
            }
            byte[] bytes = HelperBitsConvertor.ToBytes(str);
            this.list.AddRange(HelperBitsConvertor.GetBytes((ushort)bytes.Length));
            this.list.AddRange(bytes);
        }

        public void Add(DateTime date)
        {
            this.Add(date.GetUtcMilliseconds());
        }

        public byte[] GetBytes()
        {
            return this.list.ToArray();
        }

        public byte[] GetBytes(int index, int count)
        {
            return this.list.GetRange(index, count).ToArray();
        }

        public IEnumerator GetEnumerator()
        {
            return this.list.GetEnumerator();
        }

        public void Insert(int pos, int value)
        {
            this.list.InsertRange(pos, HelperBitsConvertor.GetBytes(value));
        }

        public void Insert(int pos, ushort value)
        {
            this.list.InsertRange(pos, HelperBitsConvertor.GetBytes((int)value));
        }

        public void Insert(int pos, short value)
        {
            this.list.InsertRange(pos, HelperBitsConvertor.GetBytes((int)value));
        }

        public void Insert(int pos, uint value)
        {
            this.list.InsertRange(pos, HelperBitsConvertor.GetBytes((int)value));
        }

        public void Insert(int pos, byte value)
        {
            this.list.InsertRange(pos, HelperBitsConvertor.GetBytes((int)value));
        }

        public void Insert(int pos, sbyte value)
        {
            this.list.InsertRange(pos, HelperBitsConvertor.GetBytes((int)value));
        }

        public void Insert(int pos, double value)
        {
            this.list.InsertRange(pos, HelperBitsConvertor.GetBytes(value));
        }

        public void Insert(int pos, float value)
        {
            this.list.InsertRange(pos, HelperBitsConvertor.GetBytes(value));
        }

        public void Insert(int pos, long value)
        {
            this.list.InsertRange(pos, HelperBitsConvertor.GetBytes(value));
        }

        public byte[] toArray()
        {
            return this.list.ToArray();
        }
    }
}
#endregion
