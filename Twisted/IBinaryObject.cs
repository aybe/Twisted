namespace Twisted;

public interface IBinaryObject
{
    long Position { get; }

    long Length { get; }

    byte[] GetObjectData();
}