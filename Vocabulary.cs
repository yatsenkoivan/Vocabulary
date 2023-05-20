﻿using System.Runtime.Serialization.Formatters.Binary;

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
    private Vocabulary? current_voc;
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
            "Remove vocabulary",
            "Go to vocabulary",
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
                        RemoveVocabulary();
                        break;
                    case 2:
                        GoToVocabulary();
                        break;
                    case 3:
                        return;
                }
                Console.BackgroundColor = ConsoleColor.DarkGray;
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
            "Back"
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
                            data.WriteData();
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
    public void RemoveVocabulary()
    {
        Console.Clear();
        string title = "Create vocabulary";
        string[] msg = {
            "First language:",
            "Second language:",
            "Submit",
            "Back"
        };

        string lang1 = "";
        string lang2 = "";

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
                        else
                        {
                            int index = data.Vocabularies.FindIndex(voc => voc.Languages == (lang1, lang2));
                            if (index == -1)
                            {
                                MSG("! Vocabulary do not exist !");
                                break;
                            }
                            data.Vocabularies.RemoveAt(index);
                            data.WriteData();
                            MSG("Vocabulary removed");
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
    public void GoToVocabulary()
    {
        Console.Clear();
        string title = "Go to vocabulary";
        string[] msg = {
            "First language:",
            "Second language:",
            "Submit",
            "Back"
        };

        string lang1 = "";
        string lang2 = "";

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
                        else
                        {
                            Vocabulary? v = data.Vocabularies.Find(voc => voc.Languages == (lang1, lang2));
                            if (v == null)
                            {
                                MSG("! Vocabulary does not exist !");
                                break;
                            }
                            current_voc = v;
                            VocabularyMenu();
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
    public void VocabularyMenu()
    {
        if (current_voc == null) return;
        Console.BackgroundColor = ConsoleColor.DarkBlue;
        Console.Clear();
        string title = $"Vocabulary {current_voc.Languages.Item1} - {current_voc.Languages.Item2}";
        string[] msg = {
            "Find word",
            "Add word",
            "Remove word",
            "Back"
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
                        FindWord();
                        break;
                    case 1:
                        AddWord();
                        break;
                    case 2:
                        //RemoveWord();
                        break;
                    case 3:
                        return;
                }
                Console.Clear();
                Show(title, msg, move);
            }
        } while (move != limit);
    }
    public void FindWord()
    {
        if (current_voc == null) return;
        Console.Clear();
        string title = $"{current_voc.Languages.Item1} - {current_voc.Languages.Item2}\tFind word";
        string[] msg = {
            "Word:",
            "Submit",
            "Back"
        };
        string word = "";
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
                        EnterValue(out word);
                        break;
                    case 1:
                        if (word == "") MSG("! Word cannot be empty !");
                        else if (current_voc.Translations.ContainsKey(word) == false) MSG("! Word is not in the vocabulary !");
                        else WordMenu(word);
                        break;
                    case 2:
                        return;
                }
                Console.Clear();
                Show(title, msg, move);
                ShowValue(word, 0);
            }
        } while (move != limit);
    }
    public void AddWord()
    {
        if (current_voc == null) return;
        Console.Clear();
        string title = $"{current_voc.Languages.Item1} - {current_voc.Languages.Item2}\tAdd word";
        string[] msg = {
            "Word:",
            "Translation:",
            "Submit",
            "Back"
        };
        string word = "";
        string translation = "";
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
                        EnterValue(out word);
                        break;
                    case 1:
                        EnterValue(out translation);
                        break;
                    case 2:
                        if (word == "") MSG("! Word cannot be empty !");
                        else if (translation == "") MSG("! Translation cannot be empty! ");
                        else if (current_voc.Translations.ContainsKey(word) == true) MSG("! Word already exist !");
                        else
                        {
                            current_voc.Translations.Add(word, new List<string>(new[] { translation }));
                            data.WriteData();
                            MSG("Word Added");
                            return;
                        }
                        break;
                    case 3:
                        return;
                }
                Console.Clear();
                Show(title, msg, move);
                ShowValue(word, 0);
                ShowValue(translation, 1);
            }
        } while (move != limit);
    }
    public void WordMenu(string word)
    {
        if (current_voc == null) return;
        if (current_voc.Translations.ContainsKey(word) == false) return;
        Console.Clear();
        string title = $"{current_voc.Languages.Item1} - {current_voc.Languages.Item2}\tWord: {word}";
        string[] msg = {
            "Show translations",
            "Add translation",
            "Remove translation",
            "Back"
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
                        //Show
                        break;
                    case 1:
                        //Add
                        break;
                    case 2:
                        //Remove
                        break;
                    case 3:
                        return;
                }
                Console.Clear();
                Show(title, msg, move);
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
    private Dictionary<string, List<string>> translations;
    public Vocabulary(string lang1, string lang2)
    {
        this.lang1 = lang1;
        this.lang2 = lang2;
        translations = new Dictionary<string, List<string>>();
    }
    public Dictionary<string, List<string>> Translations
    {
        get { return translations; }
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
        try
        {
            vocabularies = (List<Vocabulary>)bf.Deserialize(fs);
        }
        catch (Exception)
        {
            vocabularies = new List<Vocabulary>();
        }
        fs.Close();
    }
    public void WriteData()
    {
        BinaryFormatter bf = new();
        FileStream fs = new FileStream(data_path, FileMode.OpenOrCreate, FileAccess.Write);
        bf.Serialize(fs, vocabularies);
        fs.Close();
    }
}

