namespace Unity.Extensions.Binary
{
    public interface IBinaryObject
    {
        long Position { get; }

        long Length { get; }

        byte[] GetObjectData();
    }
}