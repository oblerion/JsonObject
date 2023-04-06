internal class Program
{
    private static void Main(string[] args)
    {
        JsonObject jo = new JsonObject("test.json");
        jo.Print();
        jo.SetInt("test",5);
        jo.WriteFile("test.json");
    }
}
