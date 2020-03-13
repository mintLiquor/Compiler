using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace LexAnalysis
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        myanalysis ma = new myanalysis();

        //打开文件
        private void button1_Click(object sender, EventArgs e)
        {
            //获取要打开的文件目录
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Multiselect = true;
            fileDialog.Title = "请选择文件";
            fileDialog.Filter = "所有文件(**)|*.*"; //设置要选择的文件的类型
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                string file = fileDialog.FileName;//返回文件的完整路径         

                //打开并读取文件，加入富文本框中

                //使用流读取文件
                richTextBox1.Text = "";
                StreamReader sr = new StreamReader(@file, Encoding.Default);
                char[] a = new char[100];
                while (sr.ReadBlock(a, 0, 90) != 0)
                {
                    richTextBox1.AppendText(new string(a));
                    a = new char[100];
                }

                //使用单个字符串读取，可用于小文件
                //string str = File.ReadAllText(@file, Encoding.Default);
                //richTextBox1.Text = str;
            }


        }

        //词法分析
        private void button2_Click(object sender, EventArgs e)
        {
            listView1.GridLines = true;
            //添加表头（列）
            listView1.Clear();
            listView1.Columns.Add("NO", 50, HorizontalAlignment.Center);
            listView1.Columns.Add("KEY", 100, HorizontalAlignment.Center);
            listView1.Columns.Add("ID", 100, HorizontalAlignment.Center);

            
            int i = 1;
            foreach (String line in richTextBox1.Lines)
            {
                String output = String.Empty;
                output += ma.analysis(line);
                while (ma.token.Count > 0)
                {
                    myanalysis.Data d = ma.token.Dequeue();

                    ListViewItem item = new ListViewItem();
                    item.SubItems.Clear();

                    item.SubItems[0].Text = i.ToString();
                    item.SubItems.Add(d.value);
                    string str = d.name.Remove(0, 3);
                    item.SubItems.Add(str);
                    listView1.Items.Add(item);

                    i++;
                    
                }
            }
        }

        //代码高亮
        private void button3_Click(object sender, EventArgs e)
        {
            richTextBox3.Text = String.Empty;
            foreach (String line in richTextBox1.Lines)
            {
                String output = String.Empty;
                output += ma.analysis(line);
                
                if(line == String.Empty)
                {
                    richTextBox3.AppendText("\n");
                    continue;
                }

                for (int i = 0;i< line.Length; i++)
                {
                    if (line[i] == '\t' || line[i] == '\r' || line[i] == '\n' || line[i] == 32)
                        richTextBox3.AppendText(line[i].ToString());
                    else
                        break;
                }
                while (ma.token.Count > 0)
                {
                    myanalysis.Data d = ma.token.Dequeue();

                    if (d.value == "//" || d.value == "/*" || d.value == "*/")
                        continue;

                    int start = richTextBox3.TextLength;
                    richTextBox3.AppendText(d.value + " ");
                    richTextBox3.Select(start, start + d.value.Length);

                   

                    if ((int)d.code <=4 )
                        richTextBox3.SelectionColor = Color.Blue;
                    else if ((int)d.code <=41)
                        richTextBox3.SelectionColor = Color.Red;
                    else
                        richTextBox3.SelectionColor = Color.Green;
                }
                richTextBox3.AppendText("\n");
            }
        }
    }
}
