using System;
using JetBrains.Annotations;
using UnityEngine;

namespace Twisted.Formats.Graphics2D
{
    /// <summary>
    ///     PlayStation 16-bit semi-transparent color.
    /// </summary>
    [PublicAPI]
    public readonly struct TransparentColor : IEquatable<TransparentColor>
    {
        private readonly short Value;

        public TransparentColor(short value)
        {
            Value = value;
        }

        public byte R => (byte)(Value & 0b11111);

        public byte G => (byte)((Value >> 5) & 0b11111);

        public byte B => (byte)((Value >> 10) & 0b11111);

        public bool A => ((Value >> 15) & 0b1) != 0;

        /// <summary>
        ///     Converts this instance to a color, specifying semi-transparency processing.
        /// </summary>
        /// <param name="mode">
        ///     Specifies the semi-transparency mode desired.
        /// </param>
        /// <returns>
        ///     The converted color.
        /// </returns>
        public Color32 ToColor(TransparentColorMode mode = TransparentColorMode.None)
        {
            // NOTE: discrepancy about black transparency in these tables

            // LibOver47.pdf
            // Table 8-11: Transparent/Semi-Transparent Pixels
            // --------------------------------------------------------------------------
            // STP, R, G, B     Semi-transparent primitive      Non-transparent primitive       
            // --------------------------------------------------------------------------
            // 1, 0, 0, 0       Semi-transparent                Non-transparent                 
            // 1, X, X, X       Semi-transparent                Non-transparent                 
            // 0, 0, 0, 0       Transparent                     Transparent                     
            // 0, X, X, X       Non-transparent                 Non-transparent


            // File Format 47.pdf
            // Table 3-1: STP Bit Function in Combination with R, G, B Data
            // ---------------------------------------------------------------------------
            // STP, R, G, B     Translucent processing on       Translucent processing off
            // ---------------------------------------------------------------------------
            // 1, 0, 0, 0       Non-transparent                 Non-transparent           
            // 1, X, X, X       Semi-transparent                Non-transparent           
            // 0, 0, 0, 0       Transparent                     Transparent               
            // 0, X, X, X       Non-transparent                 Non-transparent           

            var transparentColor = mode.HasFlagFast(TransparentColorMode.Color);
            var transparentBlack = mode.HasFlagFast(TransparentColorMode.Black);

            byte a;

            var black = R == 0 && G == 0 && B == 0;

            if (A)
            {
                if (black)
                {
                    if (transparentColor)
                    {
                        a = (byte)(transparentBlack ? byte.MaxValue / 2 : byte.MaxValue);
                    }
                    else
                    {
                        a = byte.MaxValue;
                    }
                }
                else
                {
                    if (transparentColor)
                    {
                        a = byte.MaxValue / 2;
                    }
                    else
                    {
                        a = byte.MaxValue;
                    }
                }
            }
            else
            {
                if (black)
                {
                    if (transparentColor)
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
                    if (transparentColor)
                    {
                        a = byte.MaxValue;
                    }
                    else
                    {
                        a = byte.MaxValue;
                    }
                }
            }

            var r = (byte)Math.Round(R * 255.0d / 31.0d);
            var g = (byte)Math.Round(G * 255.0d / 31.0d);
            var b = (byte)Math.Round(B * 255.0d / 31.0d);

            return new Color32(r, g, b, a);
        }

        public override string ToString()
        {
            return $"{nameof(R)}: {R}, {nameof(G)}: {G}, {nameof(B)}: {B}, {nameof(A)}: {A}";
        }

        public bool Equals(TransparentColor other)
        {
            return Value == other.Value;
        }

        public override bool Equals(object? obj)
        {
            return obj is TransparentColor other && Equals(other);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public static bool operator ==(TransparentColor left, TransparentColor right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(TransparentColor left, TransparentColor right)
        {
            return !left.Equals(right);
        }
    }
}