using System.Runtime.Serialization.Formatters.Binary;

namespace Cursor
{
    class Cursor
    {
        static public char symbol = '■';
        static public int offset_x = 10;
        static public int offset_y = 5;
        static public int dif = 2;
        static public int Move(int limit)
        {
            ConsoleKey key = Console.ReadKey(true).Key;
            int change_pos = 0;
            int current_pos = Console.GetCursorPosition().Top - offset_y;
            switch (key)
            {
                case ConsoleKey.W:
                case ConsoleKey.UpArrow:
                    change_pos = -1;
                    break;
                case ConsoleKey.S:
                case ConsoleKey.DownArrow:
                    change_pos = 1;
                    break;
                case ConsoleKey.Spacebar:
                case ConsoleKey.Enter:
                    return current_pos;
            }
            if (current_pos + change_pos > limit || current_pos + change_pos < 0) return -1;
            current_pos += change_pos;
            Hide();
            Console.SetCursorPosition(offset_x, offset_y + current_pos);
            Show();
            return -1;
        }
        static public void Hide()
        {
            (int x, int y) = Console.GetCursorPosition();
            Console.Write(' ');
            Console.SetCursorPosition(x, y);
        }
        static public void Show()
        {
            (int x, int y) = Console.GetCursorPosition();
            Console.Write(symbol);
            Console.SetCursorPosition(x, y);
        }
    }
}

class Manager
{
    readonly static private int tab_size = 20;
    readonly static private int error_level = 4;
    private Data data;
    public Manager() {
        data = new();
    }

    public void Menu()
    {
        Console.BackgroundColor = ConsoleColor.DarkGray;
        Console.Clear();
        string title = "Menu";
        string[] msg = {
            "Add vocabulary",
            "Edit vocabulary",
            "Remove vocabulary",
            "Exit"
        };
        Show(title, msg);
        int limit = msg.Length - 1;
        int move;
        do
        {
            move = Cursor.Cursor.Move(limit);
            if (move != -1)
            {
                switch (move)
                {
                    case 0:
                        CreateVocabulary();
                        break;
                    case 1:
                        //EditVocabulary();
                        break;
                    case 2:
                        //RemoveVocabulary();
                        break;
                    case 3:
                        return;
                }
                Console.Clear();
                Show(title, msg, move);
            }
        } while (move != limit);

    }
    public void CreateVocabulary()
    {
        Console.Clear();
        string title = "Create vocabulary";
        string[] msg = {
            "First language:",
            "Second language:",
            "Submit",
            "Back",
        };

        string lang1="";
        string lang2="";

        Show(title, msg);
        int limit = msg.Length - 1;
        int move;
        do
        {
            move = Cursor.Cursor.Move(limit);
            if (move != -1)
            {
                switch (move)
                {
                    case 0:
                        EnterValue(out lang1);
                        break;
                    case 1:
                        EnterValue(out lang2);
                        break;
                    case 2:
                        if (lang1 == "" || lang2 == "") MSG("! Language cannot be empty !");
                        else if (lang1 == lang2) MSG("! Languages must be different !");
                        else if (data.Vocabularies.Any(voc => voc.Languages == (lang1, lang2))) MSG("! Vocabulary already exist !");
                        else
                        {
                            Vocabulary v = new Vocabulary(lang1, lang2);
                            data.Vocabularies.Add(v);
                            data.AppendData(v);
                            MSG("Vocabulary created");
                            return;
                        }
                        break;
                    case 3:
                        return;
                }
                Console.Clear();
                Show(title, msg, move);
                ShowValue(lang1, 0);
                ShowValue(lang2, 1);
            }
        } while (move != limit);
    }
    static private void Show(string title, string[] msg, int cursor_y = 0)
    {
        Console.SetCursorPosition(Cursor.Cursor.offset_x - 2, Cursor.Cursor.offset_y - 2);
        Console.WriteLine(title);
        Console.SetCursorPosition(Cursor.Cursor.offset_x + Cursor.Cursor.dif, Cursor.Cursor.offset_y);
        int current = Console.GetCursorPosition().Top;

        foreach (string m in msg)
        {
            Console.Write(m);
            current++;
            Console.SetCursorPosition(Cursor.Cursor.offset_x + Cursor.Cursor.dif, current);
        }
        Console.SetCursorPosition(Cursor.Cursor.offset_x, Cursor.Cursor.offset_y + cursor_y);
        Cursor.Cursor.Show();
    }
    static private void ShowValue<T>(T value, int level)
    {
        (int x, int y) = Console.GetCursorPosition();
        Console.SetCursorPosition(Cursor.Cursor.offset_x + Cursor.Cursor.dif + tab_size, Cursor.Cursor.offset_y + level);
        Console.WriteLine(value);
        Console.SetCursorPosition(x, y);
    }
    static public void EnterValue(out string value, int offset = 0)
    {
        (int x, int y) = Console.GetCursorPosition();
        Console.SetCursorPosition(x + tab_size + offset, y);
        Console.Write("                     ");
        Console.SetCursorPosition(x + tab_size + offset, y);
        value = Console.ReadLine() ?? "";
        Console.SetCursorPosition(x, y);
    }
    static public void EnterValue(out int value, int offset = 0)
    {
        (int x, int y) = Console.GetCursorPosition();
        Console.SetCursorPosition(x + tab_size + offset, y);
        Console.Write("                     ");
        Console.SetCursorPosition(x + tab_size + offset, y);
        int.TryParse(Console.ReadLine(), out value);
        Console.SetCursorPosition(x, y);
    }
    static public void MSG(string msg)
    {
        (int x, int y) = Console.GetCursorPosition();
        Console.SetCursorPosition(x, y + error_level);
        Console.WriteLine(msg);
        Console.ReadKey(true);
        Console.SetCursorPosition(x, y);
    }
}
[Serializable]
class Vocabulary
{
    private string lang1;
    private string lang2;
    public Vocabulary(string lang1, string lang2)
    {
        this.lang1 = lang1;
        this.lang2 = lang2;
    }

    public (string,string) Languages
    {
        get { return (lang1, lang2); }
    }
}
class Data
{
    static private string data_path = "data.bin";
    List<Vocabulary> vocabularies;
    public List<Vocabulary> Vocabularies
    {
        get { return vocabularies; }
    }
    public Data()
    {
        BinaryFormatter bf = new();
        FileStream fs = new FileStream(data_path, FileMode.OpenOrCreate, FileAccess.Read);
        vocabularies = new List<Vocabulary>();
        try
        {
            vocabularies = (List<Vocabulary>)bf.Deserialize(fs);
        }
        catch (Exception) { }
        fs.Close();
    }
    public void AppendData(Vocabulary v)
    {
        BinaryFormatter bf = new();
        if (File.Exists(data_path) == false)
        {
            File.Create(data_path);
        }
        FileStream fs = new FileStream(data_path, FileMode.Append, FileAccess.Write);
        bf.Serialize(fs, v);
        fs.Close();
    }
}

