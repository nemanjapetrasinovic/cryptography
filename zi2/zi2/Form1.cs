using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.IO;

namespace zi2
{
    public partial class Form1 : Form
    {
        algoritam alg;
        public Form1()
        {
            InitializeComponent();
            bcg = new background(this);
            alg = new algoritam();
        }
        public void setDestinationFolder(string value)
        {
            if (InvokeRequired)
            {
                this.Invoke(new Action<string>(setDestinationFolder), new object[] { value });
                return;
            }
            textBox3.Text += value;
        }
        public string getDestinationFolder()
        {
            return textBox3.Text;
        }
        public string getSourceFolder()
        {
            return textBox2.Text;
        }
        public void setSourceFolder(string value)
        {
            if (InvokeRequired)
            {
                this.Invoke(new Action<string>(setSourceFolder), new object[] { value });
                return;
            }
            textBox2.Text += value;
        }
        public void addListItem(string value)
        {
            if (InvokeRequired)
            {
                this.Invoke(new Action<string>(addListItem), new object[] { value });
                return;
            }
            listBox1.Items.Add(value);
        }
        public void deleteListItem(string value)
        {
            if (InvokeRequired)
            {
                this.Invoke(new Action<string>(deleteListItem), new object[] { value });
                return;
            }
            listBox1.Items.Remove(value);
        }
        public bool listIsEmpty()
        {
            if (listBox1.Items.Count == 0)
                return true;
            return false;
        }
        public string getListItem()
        {
            return (listBox1.GetItemText(listBox1.Items[0]));
        }
        public string getLetters()
        {
            return textBox1.Text;
        }
        public string getKey()
        {
            return textBox4.Text;
        }
        public int getIndex()
        {
            int i=0;
            this.Invoke((MethodInvoker)delegate ()
            {
                i= comboBox1.Items.IndexOf(comboBox1.SelectedItem);
            });
            return i;
        }







        private background bcg;
        private void button1_Click(object sender, EventArgs e)
        {
            bcg.start();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folder = new FolderBrowserDialog();
            folder.ShowDialog();
            textBox2.Text = folder.SelectedPath;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folder = new FolderBrowserDialog();
            folder.ShowDialog();
            textBox3.Text = folder.SelectedPath;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            alg.setLetters(textBox1.Text);
            alg.setKey(textBox4.Text);
            alg.setFile("C:\\Users\\Nemanja\\Desktop\\New Text Document.txt");
            alg.makeDictionary();
            alg.algStartE();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            bcg.resume();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            OpenFileDialog of = new OpenFileDialog();
            of.ShowDialog();
            StreamReader keyLoad = new StreamReader(of.FileName);
            textBox4.Text = keyLoad.ReadLine();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            bcg.startD();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            bcg.resumeD();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.Items.IndexOf(comboBox1.SelectedItem) == 0)
                textBox1.Text = "abcdefghijklmnopqrstuvwxyz";
            else textBox1.Text = "";
        }
    }
}
