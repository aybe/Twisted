namespace Unity.Extensions.General
{
    public interface IBinaryObject
    {
        long Position { get; }

        long Length { get; }

        byte[] GetObjectData();
    }
}