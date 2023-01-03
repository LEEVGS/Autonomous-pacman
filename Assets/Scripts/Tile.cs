public class Tile
{
    public int x { get; set; }
    public int y { get; set; }
    public int index { get; set; }
    public bool occupied { get; set; }
    public bool powerup { get; set; }
    public int adjacentCount { get; set; }
    public bool isIntersection { get; set; }

    public Tile left, right, up, down;

    public Tile(int x_in, int y_in, int index_in)
    {
        x = x_in; y = y_in;
        occupied = false;
        left = right = up = down = null;
        index = index_in;
    }
};
