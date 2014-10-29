namespace CleanCollections
{
    public struct ChunkedIndex
    {
        public readonly short ChunkIndex;
        public readonly int LocalIndex;
        public readonly int AbsoluteIndex;

        public ChunkedIndex(short chunkIndex, int localIndex, int absoluteIndex) : this()
        {
            ChunkIndex = chunkIndex;
            LocalIndex = localIndex;
            AbsoluteIndex = absoluteIndex;
        }
    }
}