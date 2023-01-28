internal class Program
{
    private static void Main(string[] args)
    {
        JsonObject jo = new JsonObject("player.json");
        jo.print();
    }
}
