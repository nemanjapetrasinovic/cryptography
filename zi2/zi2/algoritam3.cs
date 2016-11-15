using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace zi2
{
    class algoritam3
    {
        public uint[] EncodeTEA(uint[] v, uint[] k)
        {
            // ulazna reč se podeli na svoja dva dela (levi v0 i desni v1)
            // vrednost pomoćne promenljive sum se postavi na 0
            uint v0 = v[0], v1 = v[1], sum = 0, i;           /* set up */

            // magična delta konstanta
            uint delta = 0x9e3779b9;     /* a key schedule constant */

            // ključ se podeli na svoje četiri nezavisne reči

            uint k0 = k[0], k1 = k[1], k2 = k[2], k3 = k[3];   // cache key
            for (i = 0; i < 32; i++)
            {    // algoritam ima 32 runde
                 // algoritam kodiranja se sastoji samo od sledeća tri reda
                sum += delta;
                v0 += ((v1 << 4) + k0) ^ (v1 + sum) ^ ((v1 >> 5) + k1);
                v1 += ((v0 << 4) + k2) ^ (v0 + sum) ^ ((v0 >> 5) + k3);
            }
            v[0] = v0; v[1] = v1;

            return v;
        }

        public uint[] DecodeTEA(uint[] v, uint[] k)
        {
            // inicijalizacija, ovde se može napisati i sum = delta << 5
            uint v0 = v[0], v1 = v[1], sum = 0xC6EF3720, i;  /* set up */
            uint delta = 0x9e3779b9; /* a key schedule constant */
            uint k0 = k[0], k1 = k[1], k2 = k[2], k3 = k[3];
            for (i = 0; i < 32; i++)
            {
                // proces dekodiranja u okviru jedne runde se sastoji
                // samo od sledeća tri reda
                v1 -= ((v0 << 4) + k2) ^ (v0 + sum) ^ ((v0 >> 5) + k3);
                v0 -= ((v1 << 4) + k0) ^ (v1 + sum) ^ ((v1 >> 5) + k1);
                sum -= delta;
            }
            v[0] = v0; v[1] = v1;

            return v;
        }

        public uint[] EncodeXTEA(uint[] v, uint[] key)
        {
            int rounds = 64;
            uint v0 = v[0], v1 = v[1], sum = 0, delta = 0x9E3779B9;
            for (int i = 0; i < rounds; i++)
            {
                v0 += (((v1 << 4) ^ (v1 >> 5)) + v1) ^ (sum + key[sum & 3]);
                sum += delta;
                v1 += (((v0 << 4) ^ (v0 >> 5)) + v0) ^ (sum + key[(sum >> 11) & 3]);
            }
            v[0] = v0; v[1] = v1;

            return v;
        }

        public uint[] DecodeXTEA(uint[] v, uint[] key)
        {
            int rounds = 64;
            uint v0 = v[0], v1 = v[1], delta = 0x9E3779B9, sum = (uint)(delta * rounds);
            for (int i = 0; i < rounds; i++)
            {
                v1 -= (((v0 << 4) ^ (v0 >> 5)) + v0) ^ (sum + key[(sum >> 11) & 3]);
                sum -= delta;
                v0 -= (((v1 << 4) ^ (v1 >> 5)) + v1) ^ (sum + key[sum & 3]);
            }
            v[0] = v0; v[1] = v1;

            return v;
        }
    }
}
