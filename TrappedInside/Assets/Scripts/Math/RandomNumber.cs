public static class RandomNumber 
{
    private static System.Random rnd = new System.Random();

    public static int Next(int minValue, int maxValue)
        => rnd.Next(minValue, maxValue);

    public static double NextDouble()
        => rnd.NextDouble();
}
