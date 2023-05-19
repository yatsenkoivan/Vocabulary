class Start
{
    static public void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.Unicode;
        Console.Title = "Vocabulary";
        Manager manager = new();
        manager.Menu();
    }
}