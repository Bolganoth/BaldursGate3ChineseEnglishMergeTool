using LZ4;
using System;
using System.IO;
using System.Runtime.InteropServices;
using LSLib.LS.Enums;

namespace LSLib.LS
{
    public static class BinUtils
    {
        public static T ReadStruct<T>(BinaryReader reader)
        {
            T outStruct;
            int count = Marshal.SizeOf(typeof(T));
            byte[] readBuffer = reader.ReadBytes(count);
            GCHandle handle = GCHandle.Alloc(readBuffer, GCHandleType.Pinned);
            outStruct = (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T));
            handle.Free();
            return outStruct;
        }

        public static void ReadStructs<T>(BinaryReader reader, T[] elements)
        {
            int elementSize = Marshal.SizeOf(typeof(T));
            int bytes = elementSize * elements.Length;
            byte[] readBuffer = reader.ReadBytes(bytes);
            GCHandle handle = GCHandle.Alloc(readBuffer, GCHandleType.Pinned);
            var addr = handle.AddrOfPinnedObject();
            for (var i = 0; i < elements.Length; i++)
            {
                var elementAddr = new IntPtr(addr.ToInt64() + elementSize * i);
                elements[i] = Marshal.PtrToStructure<T>(elementAddr);
            }
            handle.Free();
        }

        public static void WriteStruct<T>(BinaryWriter writer, ref T inStruct)
        {
            int count = Marshal.SizeOf(typeof(T));
            byte[] writeBuffer = new byte[count];
            GCHandle handle = GCHandle.Alloc(writeBuffer, GCHandleType.Pinned);
            Marshal.StructureToPtr(inStruct, handle.AddrOfPinnedObject(), true);
            handle.Free();
            writer.Write(writeBuffer);
        }

        public static void WriteStructs<T>(BinaryWriter writer, T[] elements)
        {
            int elementSize = Marshal.SizeOf(typeof(T));
            int bytes = elementSize * elements.Length;
            byte[] writeBuffer = new byte[bytes];
            GCHandle handle = GCHandle.Alloc(writeBuffer, GCHandleType.Pinned);
            var addr = handle.AddrOfPinnedObject();
            for (var i = 0; i < elements.Length; i++)
            {
                var elementAddr = new IntPtr(addr.ToInt64() + elementSize * i);
                Marshal.StructureToPtr(elements[i], elementAddr, true);
            }
            handle.Free();
            writer.Write(writeBuffer);
        }
        

        public static CompressionLevel CompressionFlagsToLevel(byte flags)
        {
            switch (flags & 0xf0)
            {
                case (int)CompressionFlags.FastCompress:
                    return CompressionLevel.FastCompression;

                case (int)CompressionFlags.DefaultCompress:
                    return CompressionLevel.DefaultCompression;

                case (int)CompressionFlags.MaxCompressionLevel:
                    return CompressionLevel.MaxCompression;

                default:
                    throw new ArgumentException("Invalid compression flags");
            }
        }

        public static byte MakeCompressionFlags(CompressionMethod method, CompressionLevel level)
        {
            if (method == CompressionMethod.None)
            {
                return 0;
            }

            byte flags = 0;
            if (method == CompressionMethod.Zlib)
                flags = 0x1;
            else if (method == CompressionMethod.LZ4)
                flags = 0x2;

            if (level == CompressionLevel.FastCompression)
                flags |= 0x10;
            else if (level == CompressionLevel.DefaultCompression)
                flags |= 0x20;
            else if (level == CompressionLevel.MaxCompression)
                flags |= 0x40;

            return flags;
        }

        public static byte[] Decompress(byte[] compressed, int decompressedSize, byte compressionFlags, bool chunked = false)
        {
            switch ((CompressionMethod)(compressionFlags & 0x0F))
            {
                case CompressionMethod.None:
                    return compressed;

                case CompressionMethod.LZ4:
                    if (chunked)
                    {
                        var decompressed = Native.LZ4FrameCompressor.Decompress(compressed);
                        return decompressed;
                    }
                    else
                    {
                        var decompressed = new byte[decompressedSize];
                        LZ4Codec.Decode(compressed, 0, compressed.Length, decompressed, 0, decompressedSize, true);
                        return decompressed;
                    }

                default:
                    {
                        var msg = String.Format("No decompressor found for this format: {0}", compressionFlags);
                        throw new InvalidDataException(msg);
                    }
            }
        }

        public static byte[] Compress(byte[] uncompressed, byte compressionFlags)
        {
            return Compress(uncompressed, (CompressionMethod)(compressionFlags & 0x0F), CompressionFlagsToLevel(compressionFlags));
        }

        public static byte[] Compress(byte[] uncompressed, CompressionMethod method, CompressionLevel compressionLevel, bool chunked = false)
        {
            switch (method)
            {
                case CompressionMethod.None:
                    return uncompressed;

                case CompressionMethod.LZ4:
                    return CompressLZ4(uncompressed, compressionLevel, chunked);

                default:
                    throw new ArgumentException("Invalid compression method specified");
            }
        }
        

        public static byte[] CompressLZ4(byte[] uncompressed, CompressionLevel compressionLevel, bool chunked = false)
        {
            if (chunked)
            {
                return Native.LZ4FrameCompressor.Compress(uncompressed);
            }
            else if (compressionLevel == CompressionLevel.FastCompression)
            {
                return LZ4Codec.Encode(uncompressed, 0, uncompressed.Length);
            }
            else
            {
                return LZ4Codec.EncodeHC(uncompressed, 0, uncompressed.Length);
            }
        }
    }
}
