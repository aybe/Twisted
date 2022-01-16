using JetBrains.Annotations;

namespace Twisted.Tests.PS.V2.Extensions;

[PublicAPI]
public enum Endianness
{
    Native,
    BigEndian,
    LittleEndian
}