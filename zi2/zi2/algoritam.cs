using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.IO;


namespace zi2
{
    class algoritam
    {
        Hashtable table;
        string letters;
        string key;
        string file;
        string destinationFolder;

        public void setLetters(string letters)
        {
            this.letters = letters;
        }
        public void setKey(string key)
        {
            this.key = key;
        }
        public void setFile(string file)
        {
            this.file = file;
        }
        public void makeDictionary()
        {
            table = new Hashtable();
            for (int i = 0; i < key.Length; i++)
            {
                table.Add(letters[i], key[i]);
            }
        }
        public void makeDDictionary()
        {
            table = new Hashtable();
            for (int i = 0; i < key.Length; i++)
            {
                table.Add(key[i], letters[i]);
            }
        }
        public string algStartE()
        {
            makeDictionary();
            char[] s = File.ReadAllText(file).ToCharArray();
            for (int i = 0; i < s.Length; i++)
            {
                if (table[s[i]] != null)
                    if (table[s[i]].ToString() != "")
                        if (table[s[i]].ToString() != " ")
                            s[i] = (char)table[s[i]];
            }
            string s1 = new string(s);
            return s1;
        }
        public string algStartD()
        {
            makeDDictionary();
            char[] s = File.ReadAllText(file).ToCharArray();
            for (int i = 0; i < s.Length; i++)
            {
                if (table[s[i]] != null)
                    if (table[s[i]].ToString() != "")
                        if (table[s[i]].ToString() != " ")
                            s[i] = (char)table[s[i]];
            }
            string s1 = new string(s);
            return s1;
        }
    }
}
