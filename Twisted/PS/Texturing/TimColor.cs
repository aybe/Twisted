using System;
using System.Drawing;
using System.IO;
using Twisted.Extensions;

namespace Twisted.PS.Texturing
{
    public readonly struct TimColor : IEquatable<TimColor>
    {
        public TimColor(BinaryReader reader)
            : this(reader.ReadInt16(Endianness.LE))
        {
        }

        public TimColor(short value)
        {
            R = (byte)((value >> 00) & 0b11111);
            G = (byte)((value >> 05) & 0b11111);
            B = (byte)((value >> 10) & 0b11111);
            A = (byte)((value >> 11) & 0b1);
        }

        public byte R { get; }

        public byte G { get; }

        public byte B { get; }

        public byte A { get; }

        public Color ToColor(bool translucency)
        {
            // STP/R,G,B   Translucent processing on   Translucent processing off
            // 0/0,0,0     Transparent                 Transparent
            // 0/X,X,X     Not transparent             Not transparent
            // 1/X,X,X     Semi-transparent            Not transparent
            // 1/0,0,0     Non-transparent black       Non-transparent black

            byte a;

            if (A == 0)
            {
                if (R == 0 && G == 0 && B == 0)
                {
                    if (translucency)
                    {
                        a = byte.MinValue;
                    }
                    else
                    {
                        a = byte.MinValue;
                    }
                }
                else
                {
                    if (translucency)
                    {
                        a = byte.MaxValue;
                    }
                    else
                    {
                        a = byte.MaxValue;
                    }
                }
            }
            else
            {
                if (R != 0 || G != 0 || B != 0)
                {
                    if (translucency)
                    {
                        a = byte.MaxValue / 2;
                    }
                    else
                    {
                        a = byte.MaxValue;
                    }
                }
                else
                {
                    if (translucency)
                    {
                        a = byte.MaxValue;
                    }
                    else
                    {
                        a = byte.MaxValue;
                    }
                }
            }

            return Color.FromArgb(a, (byte)(R * 255 / 31), (byte)(G * 255 / 31), (byte)(B * 255 / 31));
        }

        public override string ToString()
        {
            return $"{nameof(R)}: {R}, {nameof(G)}: {G}, {nameof(B)}: {B}, {nameof(A)}: {A}";
        }

        public bool Equals(TimColor other)
        {
            return R == other.R && G == other.G && B == other.B && A == other.A;
        }

        public override bool Equals(object? obj)
        {
            return obj is TimColor other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(R, G, B, A);
        }

        public static bool operator ==(TimColor left, TimColor right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(TimColor left, TimColor right)
        {
            return !left.Equals(right);
        }
    }
}