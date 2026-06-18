namespace TempuraKitchen.Minigames.WASDGrid
{
    [System.Serializable]
    public struct TileCoordinate
    {
        public int x;
        public int y;

        public TileCoordinate(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is TileCoordinate)) return false;
            TileCoordinate other = (TileCoordinate)obj;
            return this.x == other.x && this.y == other.y;
        }

        public override int GetHashCode()
        {
            return (x << 16) ^ y;
        }
    }
}