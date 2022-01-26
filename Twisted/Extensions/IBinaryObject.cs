namespace Twisted.Extensions;

public interface IBinaryObject
{
    long Position { get; }

    long Length { get; }

    byte[] GetObjectData();
}