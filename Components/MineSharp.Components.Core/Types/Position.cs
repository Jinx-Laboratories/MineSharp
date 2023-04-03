﻿namespace MineSharp.Components.Core.Types
{
    public struct Position
    {
        public Position(ulong value)
        {
            this.X = (int)(value >> 38);
            this.Y = (int)(value & 0xFFF);
            this.Z = (int)(value >> 12 & 0x3FFFFFF);

            if (this.X >= Math.Pow(2, 25)) { this.X -= (int)Math.Pow(2, 26); }
            if (this.Y >= Math.Pow(2, 11)) { this.Y -= (int)Math.Pow(2, 12); }
            if (this.Z >= Math.Pow(2, 25)) { this.Z -= (int)Math.Pow(2, 26); }
        }

        public Position(int x, int y, int z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        public int X {
            get;
        }
        public int Y {
            get;
        }
        public int Z {
            get;
        }

        public ulong ToULong() => ((ulong)this.X & 0x3FFFFFF) << 38 | ((ulong)this.Z & 0x3FFFFFF) << 12 | (ulong)this.Y & 0xFFF;

        public override string ToString() => $"({this.X} / {this.Y} / {this.Z})";

        //public static implicit operator Vector3(Position x) => new Vector3(x.X, x.Y, x.Z);
        //public static explicit operator Position(Vector3 x) => new Position((int)x.X, (int)x.Y, (int) x.Z);
    }
}
