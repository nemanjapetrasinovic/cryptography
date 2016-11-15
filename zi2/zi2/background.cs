using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;

namespace zi2
{
    
    class background
    {
        private Form1 form;
        algoritam alg;
        algoritam2 alg2;
        algoritam3 alg3;
        byte []keystream1;
        byte[] filebytes;
        byte[] teabytes;
        byte[] iv;
        Queue<string> red;
        private Object thislock = new Object();
        public background(Form1 form)
        {
            this.form = form;
            red = new Queue<string>();
            keystream1 = new byte[15];
            filebytes = new byte[15];
            teabytes = new byte[8];
            iv = new byte[8];
        }
        Thread producer;
        Thread consumer;
        Thread producerD;
        Thread consumerD;
        public void start()
        {
            if (form.getIndex() == 0)
                alg = new algoritam();
            else if (form.getIndex() == 1)
                alg2 = new algoritam2();
            else
                alg3 = new algoritam3();
            producer = new Thread(producerWork);
            consumer = new Thread(consumerWork);
            producer.Start();
            consumer.Start();
        }
        public void startD()
        {
            if (form.getIndex() == 0)
                alg = new algoritam();
            else if (form.getIndex() == 1)
                alg2 = new algoritam2();
            else
                alg3 = new algoritam3();
            producerD = new Thread(producerDWork);
            consumerD = new Thread(consumerDWork);
            producerD.Start();
            consumerD.Start();
        }
        public void resume()
        {
            string[] files = File.ReadAllLines(form.getSourceFolder() + "\\resume.dat");
            string[] filesindir = Directory.GetFiles(form.getSourceFolder(), "*.*");
            for (int i = 0; i < filesindir.Length; i++)
            {
                if (files.FirstOrDefault(s => s.Contains(filesindir[i])) == null)
                {
                    form.addListItem(filesindir[i]);
                    red.Enqueue(filesindir[i]);
                }
            }
        }
        public void resumeD()
        {
            string[] files = File.ReadAllLines(form.getSourceFolder() + "\\resumeD.dat");
            string[] filesindir = Directory.GetFiles(form.getSourceFolder(), "*.*");
            for (int i = 0; i < filesindir.Length; i++)
            {
                if (files.FirstOrDefault(s => s.Contains(filesindir[i])) == null)
                {
                    form.addListItem(filesindir[i]);
                    red.Enqueue(filesindir[i]);
                }
            }
        }
        public void producerWork()
        {
            FileSystemWatcher watcher = new FileSystemWatcher();
            watcher.Path = form.getSourceFolder();
            watcher.NotifyFilter = NotifyFilters.LastWrite;
            watcher.Filter = "*.*";
            watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite
           | NotifyFilters.FileName | NotifyFilters.DirectoryName;
            watcher.Created += new FileSystemEventHandler(OnChanged);
            watcher.Deleted += new FileSystemEventHandler(OnDeleted);
            watcher.EnableRaisingEvents = true;
        }
        public void producerDWork()
        {
            FileSystemWatcher watcher = new FileSystemWatcher();
            watcher.Path = form.getSourceFolder();
            watcher.NotifyFilter = NotifyFilters.LastWrite;
            watcher.Filter = "*.*";
            watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite
           | NotifyFilters.FileName | NotifyFilters.DirectoryName;
            watcher.Created += new FileSystemEventHandler(OnChanged);
            watcher.Deleted += new FileSystemEventHandler(OnDeleted);
            watcher.EnableRaisingEvents = true;
        }
        public void consumerWork()
        {
            while(true)
            {
                //while (!form.listIsEmpty())
                while(red.Count!=0)
                {
                    if (form.getIndex() == 0)
                    {
                        //string s = form.getListItem();
                        string s;
                        lock (thislock)
                        {
                            s = red.Dequeue();
                        }
                        StreamReader plaintext;
                        try
                        {
                            plaintext = new StreamReader(s);
                        }
                        catch (IOException)
                        {
                            Thread.Sleep(3000);
                            plaintext = new StreamReader(s);
                        }
                        char[] sp = { '\\', '\\' };
                        string[] s1 = s.Split(sp);
                        int br = s1.Length - 1;
                        StreamWriter saveWork = File.AppendText(form.getSourceFolder() + "\\resume.dat");
                        saveWork.WriteLine(s);
                        saveWork.Close();
                        string[] s2 = s1[br].Split('.');
                        string s3 = "";
                        for (int i = 0; i < s2.Length; i++)
                        {
                            if (s2[i] != "txt")
                                s3 += s2[i];
                        }
                        s3 += ".code";
                        StreamWriter coded = new StreamWriter(form.getDestinationFolder() + "\\" + s3);
                        alg.setFile(s);
                        alg.setKey(form.getKey());
                        alg.setLetters(form.getLetters());
                        coded.WriteLine(alg.algStartE());
                        coded.Close();
                        plaintext.Close();
                        form.deleteListItem(s);
                    }
                    else if (form.getIndex() == 1)
                    {
                        string s;
                        lock (thislock)
                        {
                            s = red.Dequeue();
                        }
                        form.deleteListItem(s);
                        StreamWriter saveWork = File.AppendText(form.getSourceFolder() + "\\resume.dat");
                        saveWork.WriteLine(s);
                        saveWork.Close();
                        GetKey();
                        char[] sp = { '\\', '\\' };
                        string[] s1 = s.Split(sp);
                        int br = s1.Length - 1;
                        string[] s2 = s1[br].Split('.');
                        string s3 = s2[s2.Length - 2];
                        byte[] result = new byte[15];
                        using (BinaryReader reader = new BinaryReader(new FileStream(s, FileMode.Open)))
                        {
                            using (BinaryWriter writer = new BinaryWriter(File.Open(form.getDestinationFolder() + "\\" + s3 + ".code", FileMode.Create)))
                            {
                                while (reader.BaseStream.Position != reader.BaseStream.Length)
                                {
                                    for (int i = 0; i < 15; i++)
                                    {
                                        filebytes[i] = 0x00;
                                    }
                                    reader.Read(filebytes, 0, 15);
                                    for (int i = 0; i < 15; i++)
                                    {
                                        result[i] = 0x00;
                                        result[i] = (byte)(filebytes[i] ^ keystream1[i]);
                                        if (result[i] != 0x00)
                                            writer.Write(result[i]);
                                    }
                                }
                            }
                        }
                    }
                    else if (form.getIndex() == 2)
                    {
                        string s;
                        lock (thislock)
                        {
                            s = red.Dequeue();
                        }
                        form.deleteListItem(s);
                        StreamWriter saveWork = File.AppendText(form.getSourceFolder() + "\\resume.dat");
                        saveWork.WriteLine(s);
                        saveWork.Close();
                        uint[] kljuc = new uint[4];
                        kljuc[0] = 0x2b;
                        kljuc[1] = 0x3a;
                        kljuc[2] = 0x4c;
                        kljuc[3] = 0x1d;
                        char[] sp = { '\\', '\\' };
                        string[] s1 = s.Split(sp);
                        int br = s1.Length - 1;
                        string[] s2 = s1[br].Split('.');
                        string s3 = s2[s2.Length - 2];
                        byte[] result = new byte[8];
                        iv = Encoding.ASCII.GetBytes(form.getKey());
                        using (BinaryReader reader = new BinaryReader(new FileStream(s, FileMode.Open)))
                        {
                            using (BinaryWriter writer = new BinaryWriter(File.Open(form.getDestinationFolder() + "\\" + s3 + ".code", FileMode.Create)))
                            {
                                while (reader.BaseStream.Position != reader.BaseStream.Length)
                                {
                                    for (int i = 0; i < 8; i++)
                                    {
                                        teabytes[i] = 0x00;
                                    }
                                    reader.Read(teabytes, 0, 8);
                                    for (int i = 0; i < 8; i++)
                                    {
                                        result[i] = 0x00;
                                        result[i] = (byte)(teabytes[i] ^ iv[i]);
                                    }
                                    uint[] resultcon = new uint[2];
                                    resultcon[0] = BitConverter.ToUInt32(result, 0);
                                    resultcon[1] = BitConverter.ToUInt32(result, 4);
                                    iv = alg3.EncodeTEA(resultcon, kljuc).SelectMany(BitConverter.GetBytes).ToArray();
                                    //writer.Write(alg3.EncodeTEA(resultcon, kljuc).SelectMany(BitConverter.GetBytes).ToArray());
                                    writer.Write(iv);
                                    for(int i = 0; i < 8; i++)
                                    {
                                        iv[i] = (byte)(iv[i] ^ teabytes[i]);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        string s;
                        lock (thislock)
                        {
                            s = red.Dequeue();
                        }
                        form.deleteListItem(s);
                        StreamWriter saveWork = File.AppendText(form.getSourceFolder() + "\\resume.dat");
                        saveWork.WriteLine(s);
                        saveWork.Close();
                        uint[] kljuc = new uint[4];
                        kljuc[0] = 0x2b;
                        kljuc[1] = 0x3a;
                        kljuc[2] = 0x4c;
                        kljuc[3] = 0x1d;
                        char[] sp = { '\\', '\\' };
                        string[] s1 = s.Split(sp);
                        int br = s1.Length - 1;
                        string[] s2 = s1[br].Split('.');
                        string s3 = s2[s2.Length - 2];
                        byte[] result = new byte[8];
                        iv = Encoding.ASCII.GetBytes(form.getKey());
                        using (BinaryReader reader = new BinaryReader(new FileStream(s, FileMode.Open)))
                        {
                            using (BinaryWriter writer = new BinaryWriter(File.Open(form.getDestinationFolder() + "\\" + s3 + ".code", FileMode.Create)))
                            {
                                while (reader.BaseStream.Position != reader.BaseStream.Length)
                                {
                                    for (int i = 0; i < 8; i++)
                                    {
                                        teabytes[i] = 0x00;
                                    }
                                    reader.Read(teabytes, 0, 8);
                                    for (int i = 0; i < 8; i++)
                                    {
                                        result[i] = 0x00;
                                        result[i] = (byte)(teabytes[i] ^ iv[i]);
                                    }
                                    uint[] resultcon = new uint[2];
                                    resultcon[0] = BitConverter.ToUInt32(result, 0);
                                    resultcon[1] = BitConverter.ToUInt32(result, 4);
                                    iv = alg3.EncodeXTEA(resultcon, kljuc).SelectMany(BitConverter.GetBytes).ToArray();
                                    //writer.Write(alg3.EncodeTEA(resultcon, kljuc).SelectMany(BitConverter.GetBytes).ToArray());
                                    writer.Write(iv);
                                    for (int i = 0; i < 8; i++)
                                    {
                                        iv[i] = (byte)(iv[i] ^ teabytes[i]);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        public void consumerDWork()
        {
            while (true)
            {
                while (!form.listIsEmpty())
                {
                    if (form.getIndex() == 0)
                    {
                        string s;
                        StreamReader plaintext;
                        lock (thislock)
                        {
                            s = red.Dequeue();
                        }
                        try
                        {
                            plaintext = new StreamReader(s);
                        }
                        catch (IOException)
                        {
                            Thread.Sleep(3000);
                            plaintext = new StreamReader(s);
                        }
                        char[] sp = { '\\', '\\' };
                        string[] s1 = s.Split(sp);
                        int br = s1.Length - 1;
                        StreamWriter saveWork = File.AppendText(form.getSourceFolder() + "\\resumeD.dat");
                        saveWork.WriteLine(s);
                        saveWork.Close();
                        string[] s2 = s1[br].Split('.');
                        string s3 = "";
                        for (int i = 0; i < s2.Length; i++)
                        {
                            if (s2[i] != "code")
                                s3 += s2[i];
                        }
                        s3 += ".txt";
                        StreamWriter coded = new StreamWriter(form.getDestinationFolder() + "\\" + s3);
                        alg.setFile(s);
                        alg.setKey(form.getKey());
                        alg.setLetters(form.getLetters());
                        coded.WriteLine(alg.algStartD());
                        coded.Close();
                        plaintext.Close();
                        form.deleteListItem(s);
                    }
                    else if (form.getIndex() == 1)
                    {
                        string s;
                        lock (thislock)
                        {
                            s = red.Dequeue();
                        }
                        form.deleteListItem(s);
                        StreamWriter saveWork = File.AppendText(form.getSourceFolder() + "\\resumeD.dat");
                        saveWork.WriteLine(s);
                        saveWork.Close();
                        GetKey();
                        char[] sp = { '\\', '\\' };
                        string[] s1 = s.Split(sp);
                        int br = s1.Length - 1;
                        string[] s2 = s1[br].Split('.');
                        string s3 = s2[s2.Length - 2];
                        byte[] result = new byte[15];
                        using (BinaryReader reader = new BinaryReader(new FileStream(s, FileMode.Open)))
                        {
                            using (BinaryWriter writer = new BinaryWriter(File.Open(form.getDestinationFolder() + "\\" + s3+".txt", FileMode.Create)))
                            {
                                while (reader.BaseStream.Position != reader.BaseStream.Length)
                                {
                                    for (int i = 0; i < 15; i++)
                                    {
                                        filebytes[i] = 0x00;
                                    }
                                    reader.Read(filebytes, 0, 15);
                                    for (int i = 0; i < 15; i++)
                                    {
                                        result[i] = 0x00;
                                        result[i] = (byte)(filebytes[i] ^ keystream1[i]);
                                        if (result[i] != 0x00)
                                            writer.Write(result[i]);
                                    }
                                }
                            }
                        }
                    }
                    else if (form.getIndex() == 1)
                    {

                        string s;
                        lock (thislock)
                        {
                            s = red.Dequeue();
                        }
                        form.deleteListItem(s);
                        StreamWriter saveWork = File.AppendText(form.getSourceFolder() + "\\resumeD.dat");
                        saveWork.WriteLine(s);
                        saveWork.Close();
                        uint[] kljuc = new uint[4];
                        kljuc[0] = 0x2b;
                        kljuc[1] = 0x3a;
                        kljuc[2] = 0x4c;
                        kljuc[3] = 0x1d;
                        char[] sp = { '\\', '\\' };
                        string[] s1 = s.Split(sp);
                        int br = s1.Length - 1;
                        string[] s2 = s1[br].Split('.');
                        string s3 = s2[s2.Length - 2];
                        byte[] result = new byte[15];
                        iv = Encoding.ASCII.GetBytes(form.getKey());
                        using (BinaryReader reader = new BinaryReader(new FileStream(s, FileMode.Open)))
                        {
                            using (BinaryWriter writer = new BinaryWriter(File.Open(form.getDestinationFolder() + "\\" + s3 + ".txt", FileMode.Create)))
                            {
                                while (reader.BaseStream.Position != reader.BaseStream.Length)
                                {
                                    for (int i = 0; i < 8; i++)
                                    {
                                        teabytes[i] = 0x00;
                                    }
                                    reader.Read(teabytes, 0, 8);
                                    byte []decres = new byte[8];
                                    uint[] resultcon = new uint[2];
                                    resultcon[0] = BitConverter.ToUInt32(teabytes, 0);
                                    resultcon[1] = BitConverter.ToUInt32(teabytes, 4);
                                   // alg3.DecodeTEA(resultcon, kljuc);
                                    decres = alg3.DecodeTEA(resultcon, kljuc).SelectMany(BitConverter.GetBytes).ToArray();
                                    for (int i = 0; i < 8; i++)
                                    {
                                        result[i] = 0x00;
                                        result[i] = (byte)(decres[i] ^ iv[i]);
                                        if (result[i] != 0x00)
                                            writer.Write(result[i]);
                                    }
                                    for (int i = 0; i < 8; i++)
                                    {
                                        iv[i] = (byte)(teabytes[i] ^ result[i]);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        string s;
                        lock (thislock)
                        {
                            s = red.Dequeue();
                        }
                        form.deleteListItem(s);
                        StreamWriter saveWork = File.AppendText(form.getSourceFolder() + "\\resumeD.dat");
                        saveWork.WriteLine(s);
                        saveWork.Close();
                        uint[] kljuc = new uint[4];
                        kljuc[0] = 0x2b;
                        kljuc[1] = 0x3a;
                        kljuc[2] = 0x4c;
                        kljuc[3] = 0x1d;
                        char[] sp = { '\\', '\\' };
                        string[] s1 = s.Split(sp);
                        int br = s1.Length - 1;
                        string[] s2 = s1[br].Split('.');
                        string s3 = s2[s2.Length - 2];
                        byte[] result = new byte[15];
                        iv = Encoding.ASCII.GetBytes(form.getKey());
                        using (BinaryReader reader = new BinaryReader(new FileStream(s, FileMode.Open)))
                        {
                            using (BinaryWriter writer = new BinaryWriter(File.Open(form.getDestinationFolder() + "\\" + s3 + ".txt", FileMode.Create)))
                            {
                                while (reader.BaseStream.Position != reader.BaseStream.Length)
                                {
                                    for (int i = 0; i < 8; i++)
                                    {
                                        teabytes[i] = 0x00;
                                    }
                                    reader.Read(teabytes, 0, 8);
                                    byte[] decres = new byte[8];
                                    uint[] resultcon = new uint[2];
                                    resultcon[0] = BitConverter.ToUInt32(teabytes, 0);
                                    resultcon[1] = BitConverter.ToUInt32(teabytes, 4);
                                    // alg3.DecodeTEA(resultcon, kljuc);
                                    decres = alg3.DecodeXTEA(resultcon, kljuc).SelectMany(BitConverter.GetBytes).ToArray();
                                    for (int i = 0; i < 8; i++)
                                    {
                                        result[i] = 0x00;
                                        result[i] = (byte)(decres[i] ^ iv[i]);
                                        if (result[i] != 0x00)
                                            writer.Write(result[i]);
                                    }
                                    for (int i = 0; i < 8; i++)
                                    {
                                        iv[i] = (byte)(teabytes[i] ^ result[i]);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        private void OnChanged(object source, FileSystemEventArgs e)
        {
            form.addListItem(e.FullPath);
            lock(thislock)
            {
                red.Enqueue(e.FullPath);
            }
        }
        private void OnDeleted(object source, FileSystemEventArgs e)
        {
            form.deleteListItem(e.FullPath);
        }
        public void GetKey()
        {
            byte[] key = { 0x00, 0xfc, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff };
            ushort frame = 0x21;

            byte[] AtoB = new byte[15];
            byte[] BtoA = new byte[15];

            algoritam2.keysetup(key, frame);
            keystream1 = algoritam2.run(keystream1, BtoA);
        }
    }
}
