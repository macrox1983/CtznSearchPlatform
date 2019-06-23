using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BreakingStorm.Common.Helpers
{
    public static class HelperBitsConvertor
    {
        public static Encoding Encoding = Encoding.UTF8;

        public static byte[] ToBytes(string text)
        {
            return Encoding.GetBytes(text);
        }

        public static byte[] ToBytes(byte[] bytes, string text)
        {
            byte[] textBytes = Encoding.GetBytes(text);
            byte[] result = new byte[bytes.Length + text.Length];
            Array.Copy(bytes, 0, result, 0, bytes.Length);
            Array.Copy(textBytes, 0, result, bytes.Length, textBytes.Length);
            return result;
        }

        public static byte[] ToBytes(byte[] bytes, byte addedByte)
        {
            byte[] result = new byte[bytes.Length + 1];
            Array.Copy(bytes, 0, result, 0, bytes.Length);
            result[bytes.Length + 1] = addedByte;
            return result;
        }

        public static string ToText(byte[] bytes)
        {
            return Encoding.GetString(bytes);
        }

        public static string ToText(byte[] bytes, int index)
        {
            ushort count = BitConverter.ToUInt16(bytes, index);
            return Encoding.GetString(bytes, index + 2, count);
        }

        public static byte[] GetBytes(bool value)
        {
            byte[] result = BitConverter.GetBytes((bool)value);
            if (BitConverter.IsLittleEndian) return result; else return result.Reverse().ToArray();
        }
        public static byte[] GetBytes(char value)
        {
            byte[] result = BitConverter.GetBytes((char)value);
            if (BitConverter.IsLittleEndian) return result; else return result.Reverse().ToArray();
        }
        public static byte[] GetBytes(short value)
        {
            byte[] result = BitConverter.GetBytes((short)value);
            if (BitConverter.IsLittleEndian) return result; else return result.Reverse().ToArray();
        }
        public static byte[] GetBytes(int value)
        {
            byte[] result = BitConverter.GetBytes((int)value);
            if (BitConverter.IsLittleEndian) return result; else return result.Reverse().ToArray();
        }
        public static byte[] GetBytes(long value)
        {
            byte[] result = BitConverter.GetBytes((long)value);
            if (BitConverter.IsLittleEndian) return result; else return result.Reverse().ToArray();
        }

        public static byte[] GetBytes(ushort value)
        {
            byte[] result = BitConverter.GetBytes((ushort)value);
            if (BitConverter.IsLittleEndian) return result; else return result.Reverse().ToArray();
        }

        public static byte[] GetBytes(uint value)
        {
            byte[] result = BitConverter.GetBytes((uint)value);
            if (BitConverter.IsLittleEndian) return result; else return result.Reverse().ToArray();
        }

        public static byte[] GetBytes(ulong value)
        {
            byte[] result = BitConverter.GetBytes((ulong)value);
            if (BitConverter.IsLittleEndian) return result; else return result.Reverse().ToArray();
        }


        public static byte[] GetBytes(float value)
        {
            byte[] result = BitConverter.GetBytes((float)value);
            if (BitConverter.IsLittleEndian) return result; else return result.Reverse().ToArray();
        }

        public static byte[] GetBytes(double value)
        {
            byte[] result = BitConverter.GetBytes((double)value);
            if (BitConverter.IsLittleEndian) return result; else return result.Reverse().ToArray();
        }
    }
}
