﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace F500Tool
{
    public class Const
    {
        public const UInt32 FileHeaderMagik = 0x33219FBD;

        public const UInt32 SectionHeaderMagik = 0xA324EB90;

        public const UInt32 RomHeaderMagik = 0x66FC328A;

        public const UInt32 RomFileHeaderMagik = 0x2387AB76;

        public const UInt16 BitmapMagik = 0x0811;

        public const int RomFilenameLength = 116;

        public static byte[] FileHeaderConst;
    }


    unsafe public struct FileHeader
    {
        public Int32 BstStart;
        public Int32 BstEnd;

        public Int32 BldStart;
        public Int32 BldEnd;

        public UInt64 Empty0;

        public Int32 PriStart;
        public Int32 PriEnd;

        public UInt64 Empty1;

        public Int32 RomStart;
        public Int32 RomEnd;

        public Int32 DspStart;
        public Int32 DspEnd;

        public UInt32 Magik;

        public fixed byte Const[0x19F - 0x03C + 1];

        public fixed byte Empty2[0x7FF - 0x1A0 + 1];

        public Int32 MaxRomLength
        {
            get { return DspStart - RomStart; }
        }
    }

    unsafe public struct SectionHeader
    {
        public UInt32 CRC32;
        public UInt32 Version;
        public UInt32 Date;
        public Int32 ImgLen;
        public UInt32 Mem;
        public UInt32 Flags;
        public UInt32 Magik;

        public fixed byte Empty0[0x8FF - 0x81C + 1];
    }

    unsafe public struct RomHeader
    {
        public Int32 FilesCount;
        public Int32 Magik;

        public fixed byte Empty0[0x10FF - 0x0908 + 1];
    }

    unsafe public struct RomFileHeader
    {
        public fixed byte FileNameBuf[Const.RomFilenameLength];
        public Int32 FileOffset;
        public Int32 FileLength;
        public UInt32 Magik;

        public string FileName
        {
            get
            {
                var bytes = new byte[Const.RomFilenameLength];

                fixed (byte* namePtr = FileNameBuf)
                {
                    for (var i = 0; i < Const.RomFilenameLength; i++)
                    {
                        if (namePtr[i] == 0) break;
                        bytes[i] = namePtr[i];
                    }
                }

                return Encoding.ASCII.GetString(bytes).Trim('\0');
            }
        }
    }

    unsafe public struct BitmapsBinFileHeader
    {
        public fixed byte Unknown0[10];

        public Int16 BitmapCount;

        public fixed byte Unknown1[20];
    }

    public struct BitmapHeader
    {
        public UInt32 Start;
        public UInt32 Length;

        public UInt32 Zero0;
        public UInt32 Zero1;
    }

    public struct BitmapData
    {
        public UInt16 Magik;

        public Int16 Width;
        public Int16 Height;

        public byte[] Data;

        public int GetFixedSize
        {
            get { return sizeof(UInt16) + sizeof(Int16) + sizeof(Int16); }
        }

        public void FillFromByteArray(byte[] arr)
        {            
            Magik = (UInt16)((arr[1] * 0x100) | arr[0]);

            if (Magik != Const.BitmapMagik)
                throw new Exception("Wrong Magik. Array isn't valid bitmap array");

            Width = (Int16)((arr[3] * 0x100) | arr[2]);
            Height = (Int16)((arr[5] * 0x100) | arr[4]);

            Data = new byte[arr.Length - GetFixedSize];

            Array.Copy(arr,GetFixedSize,Data,0,Data.Length);
        }
    }
}
