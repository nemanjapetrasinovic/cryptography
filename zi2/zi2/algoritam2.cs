using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace zi2
{
    class algoritam2
    {
        public const UInt32 R1Mask = 0x07FFFF;
        public const UInt32 R2Mask = 0x3FFFFF;
        public const UInt32 R3Mask = 0x7FFFFF;
        public const UInt32 R4Mask = 0x01FFFF;

        public const UInt32 R4Tap1 = 0x000400;
        public const UInt32 R4Tap2 = 0x000008;
        public const UInt32 R4Tap3 = 0x000080;

        public const UInt32 R1Taps = 0x072000;
        public const UInt32 R2Taps = 0x300000;
        public const UInt32 R3Taps = 0x700080;
        public const UInt32 R4Taps = 0x010800;

        public static ulong R1;
        public static ulong R2;
        public static ulong R3;
        public static ulong R4;

        public static byte key;

        public static ushort parity(ulong x)
        {
            x ^= x >> 16;
            x ^= x >> 8;
            x ^= x >> 4;
            x ^= x >> 2;
            x ^= x >> 1;
            return (ushort)(x & 1);
        }

        public static ulong clockone(ulong reg, ulong mask, ulong taps, ulong loaded_bit)
        {
            ulong t = reg & taps;
            reg = (reg << 1) & mask;
            reg |= parity(t);
            reg |= loaded_bit;
            return reg;
        }

        public static ushort majority(ulong w1, ulong w2, ulong w3)
        {
            int sum = 0;
            //if (w1 != 0)
            //    sum++;
            //if (w2 != 0)
            //    sum++;
            //if (w3 != 0)
            //    sum++;

            sum = System.Convert.ToInt32(w1);
            sum += System.Convert.ToInt32(w2);
            sum += System.Convert.ToInt32(w3);


            if (sum >= 2)
                return 1;
            else
                return 0;
        }

        public static void clock(int allP, int loaded)
        {
            ushort maj = majority(R4 & R4Tap1, R4 & R4Tap2, R4 & R4Tap3);
            ushort pom = 0;

            if ((R4 & R4Tap1) != 0)
                pom = 1;
            else
                pom = 0;

            if (allP > 0 || (pom == maj))
                R1 = clockone(R1, R1Mask, R1Taps, (ulong)loaded << 15);

            if ((R4 & R4Tap2) != 0)
                pom = 1;
            else
                pom = 0;

            if (allP > 0 || (pom == maj))
                R2 = clockone(R2, R2Mask, R2Taps, (ulong)loaded << 16);

            if ((R4 & R4Tap3) != 0)
                pom = 1;
            else
                pom = 0;

            if (allP > 0 || (pom == maj))
                R3 = clockone(R3, R3Mask, R3Taps, (ulong)loaded << 18);

            R4 = clockone(R4, R4Mask, R4Taps, (ulong)loaded << 10);

        }

        public static ushort getbit()
        {
            ushort topbits = (ushort)(((R1 >> 18) ^ (R2 >> 21) ^ (R3 >> 22) & 0x01));

            ushort delaybit = 0;

            delaybit = (ushort)(topbits
                        ^ majority(R1 & 0x80000, (~R1) & 0x4000, R1 & 0x1000)
                        ^ majority((~R2) & 0x10000, R2 & 0x2000, R2 & 0x200)
                        ^ majority(R3 & 0x40000, R3 & 0x10000, (~R3) & 0x2000)
                );

            return delaybit;
        }

        public static void keysetup(byte[] key, ulong frame)
        {
            int i;
            ushort keybit, framebit;
            R1 = R2 = R3 = R4 = 0;

            for (i = 0; i < 64; i++)
            {
                clock(1, 0);
                keybit = (ushort)((key[i / 8] >> (i & 7)) & 1);
                R1 ^= keybit;
                R2 ^= keybit;
                R3 ^= keybit;
                R4 ^= keybit;
            }

            int pom = 0;
            if (i == 21)
                pom = 1;
            else
                pom = 0;

            for (i = 0; i < 22; i++)
            {
                clock(1, pom);
                framebit = (ushort)((frame >> i) & 1);
                R1 ^= framebit;
                R2 ^= framebit;
                R3 ^= framebit;
                R4 ^= framebit;
            }

            for (i = 0; i < 100; i++)
                clock(0, 0);
            getbit();
        }


        public static byte[] run(byte[] AtoBkeystream, byte[] BtoAkeystream)
        {
            int i;
            for (i = 0; i < 113 / 8; i++)
                AtoBkeystream[i] = BtoAkeystream[i] = 0;
            for (i = 0; i < 114; i++)
            {
                clock(0, 0);
                AtoBkeystream[i / 8] |= (byte)(getbit() << (7 - (i & 7)));
            }

            for (i = 0; i < 114; i++)
            {
                clock(0, 0);
                BtoAkeystream[i / 8] |= (byte)(getbit() << (7 - (i & 7)));
            }
            return AtoBkeystream;
        }

    }
}
