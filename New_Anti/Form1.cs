using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

namespace New_Anti
{
    public partial class Form1 : Form
    {
        private string s_FileName;
        private string t_FileName;
        private string path;
        public static string output;
        public Form1()
        {
            InitializeComponent();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.RootFolder = System.Environment.SpecialFolder.Desktop;
            // 设置当前选择的路径
            fbd.SelectedPath = "C:";

            // 允许在对话框中包括一个新建目录的按钮
            fbd.ShowNewFolderButton = true;

            // 设置对话框的说明信息
            fbd.Description = "请选择输出目录";
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                output = fbd.SelectedPath.ToString();
                doc(textBox2, "当前的输出路径为 " + output + "\r\n");
                // 在此添加代码,选择的路径为 folderBrowserDialog1.SelectedPath
                //MessageBox.Show(fbd.SelectedPath.ToString());

            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "SQL文件(*.sql)|*.sql|所有文件(*.*)|*.*";
            if (!ofd.ShowDialog().Equals(null))
            {
                s_FileName = ofd.FileName;
                t_FileName = System.IO.Path.GetFileNameWithoutExtension(ofd.FileName).ToString();
                doc(textBox2, "数据库---" + System.IO.Path.GetFileNameWithoutExtension(ofd.SafeFileName).ToString() + "---已经加载完毕，请选择要输入的日志文件\r\n");
            }
            //connectString = "Database=" +  + ";Data Source=127.0.0.1;User Id=root;Password=yu65193852";//"server=localhost;userid=root;password=yu65193852" + ;
        }
        public void doc(TextBox tb, string o)
        {
            if (output == null)
                tb.Text += "请先选择软件的输出路径\r\n";
            else
            {
                StreamWriter SW;
                string Doc = output + @"Doc.txt";
                SW = File.AppendText(Doc);
                tb.Text += o.ToString();
                SW.WriteLine(o);
                SW.Close();
            }
            //  System.IO.File.WriteAllText( doc, o.ToString(), System.Text.Encoding.GetEncoding("gb2312"));
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "LOG文件(*.log)|*.log|所有文件(*.*)|*.*";
            if (!ofd.ShowDialog().Equals(null))
            {
                path = ofd.FileName;
                doc(textBox2, "日志文件---" + System.IO.Path.GetFileNameWithoutExtension(ofd.SafeFileName).ToString() + "---已经加载完毕，请选择要分析的查询语句\r\n");
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.ToString() == "")
            {
                DialogResult MsgBoxResult;//设置对话框的返回值
                MsgBoxResult = MessageBox.Show("是否进行水平分割操作",//对话框的显示内容
                "提示",//对话框的标题
                MessageBoxButtons.YesNo,//定义对话框的按钮，这里定义了YSE和NO两个按钮
                MessageBoxIcon.Exclamation,//定义对话框内的图表式样，这里是一个黄色三角型内加一个感叹号
                MessageBoxDefaultButton.Button2);//定义对话框的按钮式样
                if (MsgBoxResult == DialogResult.Yes)//如果对话框的返回值是YES（按"Y"按钮）
                {
                    try
                    {
                        Class1 class1 = new Class1(s_FileName, t_FileName, path);
                        class1.dowork();
                        string current = System.DateTime.Now.ToString("G");
                        for (int n = 0; n < class1.table_int; n++)
                        {
                            doc(textBox2, current + "-水平分割-操作-" + (n + 1) + "-表 " + class1.table_name[n] + "----水平分割--新表名 " + class1.table_name[n] + "_horiz\r\n");
                        }
                        DialogResult MsgBoxResult2;//设置对话框的返回值
                        MsgBoxResult2 = MessageBox.Show("是否进行垂直分割操作",//对话框的显示内容
                        "提示",//对话框的标题
                        MessageBoxButtons.YesNo,//定义对话框的按钮，这里定义了YSE和NO两个按钮
                        MessageBoxIcon.Exclamation,//定义对话框内的图表式样，这里是一个黄色三角型内加一个感叹号
                        MessageBoxDefaultButton.Button2);//定义对话框的按钮式样
                        if (MsgBoxResult2 == DialogResult.Yes)
                        {
                            try
                            {
                                Class2 class2 = new Class2(s_FileName, t_FileName, path);
                                class2.dowork();
                                string current2 = System.DateTime.Now.ToString("G");
                                for (int n = 0; n < class2.vv; n++)
                                {
                                    string temp = class2.column_verti[n].Remove(class2.column_verti[n].IndexOf("."));
                                    doc(textBox2, current2 + "-垂直分割-操作-" + (n + 1) + "-表 " + temp + "----垂直分割--新表名 " + temp + "_verti\r\n");
                                }
                            }
                            catch (Exception e2)
                            {
                                doc(textBox2, "出错了哦，请检查一下数据库和日志文件是否选择正确");
                            }
                        }
                    }
                    catch (Exception e1)
                    {
                        doc(textBox2, "出错了哦，请检查一下数据库和日志文件是否选择正确");
                    }
                }
                else if (MsgBoxResult == DialogResult.No)//如果对话框的返回值是NO（按"N"按钮）
                {
                    DialogResult MsgBoxResult2;//设置对话框的返回值
                    MsgBoxResult2 = MessageBox.Show("是否进行垂直分割操作",//对话框的显示内容
                    "提示",//对话框的标题
                    MessageBoxButtons.YesNo,//定义对话框的按钮，这里定义了YSE和NO两个按钮
                    MessageBoxIcon.Exclamation,//定义对话框内的图表式样，这里是一个黄色三角型内加一个感叹号
                    MessageBoxDefaultButton.Button2);//定义对话框的按钮式样
                    if (MsgBoxResult2 == DialogResult.Yes)
                    {
                        try
                        {
                            Class2 class2 = new Class2(s_FileName, t_FileName, path);
                            class2.dowork();
                            string current2 = System.DateTime.Now.ToString("G");
                            for (int n = 0; n < class2.vv; n++)
                            {
                                string temp = class2.column_verti[n].Remove(class2.column_verti[n].IndexOf("."));
                                doc(textBox2, current2 + "-垂直分割-操作-" + (n + 1) + "-表 " + temp + "----垂直分割--新表名 " + temp + "_verti\r\n");
                            }
                        }
                        catch (Exception e1)
                        {
                            doc(textBox2, "出错了哦，请检查一下数据库和日志文件是否选择正确");
                        }
                    }
                }
            }
            else { 
                if(textBox1.Text.IndexOf(" abs(")>-1){
                    DialogResult MsgBoxResult;//设置对话框的返回值
                    MsgBoxResult = MessageBox.Show("是否对abs集函数进行新建派生列操作",//对话框的显示内容
                    "提示",//对话框的标题
                    MessageBoxButtons.YesNo,//定义对话框的按钮，这里定义了YSE和NO两个按钮
                    MessageBoxIcon.Exclamation,//定义对话框内的图表式样，这里是一个黄色三角型内加一个感叹号
                    MessageBoxDefaultButton.Button2);//定义对话框的按钮式样
                    if (MsgBoxResult == DialogResult.Yes)//如果对话框的返回值是YES（按"Y"按钮）
                    {
                        try
                        {
                            Class3 class3 = new Class3(s_FileName, t_FileName, path, textBox1.Text);
                            class3.dowork();
                            if (class3.flag)
                            {
                                string current = System.DateTime.Now.ToString("G");
                                doc(textBox2, current + "-新建派生列-表 " + class3.str1 + "----新建派生列--新列名 add_" + class3.str2 + "\r\n");
                            }
                            else
                            {
                                doc(textBox2, "此条语句使用并不频繁，无需反规范化\r\n");
                            }
                        }
                        catch (Exception e1)
                        {
                            doc(textBox2, "出错了哦，请检查一下数据库和日志文件是否选择正确，待分析的查询语句有无语法错误");
                        }
                    }
                }
                else if (textBox1.Text.IndexOf("left join") > -1) {
                    int i = 1;
                    string temp = textBox1.Text;
                    while (temp.IndexOf("left join") > -1) {
                        temp = temp.Remove(0, temp.IndexOf("left join") + 9);
                        i++;
                    }
                    if (i > 6) {
                        DialogResult MsgBoxResult;//设置对话框的返回值
                        MsgBoxResult = MessageBox.Show("是否进行重新组表操作",//对话框的显示内容
                        "提示",//对话框的标题
                        MessageBoxButtons.YesNo,//定义对话框的按钮，这里定义了YSE和NO两个按钮
                        MessageBoxIcon.Exclamation,//定义对话框内的图表式样，这里是一个黄色三角型内加一个感叹号
                        MessageBoxDefaultButton.Button2);//定义对话框的按钮式样
                        if (MsgBoxResult == DialogResult.Yes)//如果对话框的返回值是YES（按"Y"按钮）
                        {
                            try
                            {
                                Class5 class5 = new Class5(s_FileName, t_FileName, path, textBox1.Text, i + 1);
                                class5.dowork();
                                if (class5.flag)
                                {
                                    string current = System.DateTime.Now.ToString("G");
                                    doc(textBox2, current + "-重新组表-表 " + class5.r_redun[1] + "----重新组表--新表名 renew_" + class5.r_redun[1] + "\r\n");
                                }
                                else
                                {
                                    doc(textBox2, "此条语句使用并不频繁，无需反规范化\r\n");
                                }
                            }
                            catch (Exception e1)
                            {
                                doc(textBox2, "出错了哦，请检查一下数据库和日志文件是否选择正确，待分析的查询语句有无语法错误");
                            }
                        }
                    }
                    else if (i > 2 && i < 7)
                    {
                        DialogResult MsgBoxResult;//设置对话框的返回值
                        MsgBoxResult = MessageBox.Show("是否进行新建冗余列操作",//对话框的显示内容
                        "提示",//对话框的标题
                        MessageBoxButtons.YesNo,//定义对话框的按钮，这里定义了YSE和NO两个按钮
                        MessageBoxIcon.Exclamation,//定义对话框内的图表式样，这里是一个黄色三角型内加一个感叹号
                        MessageBoxDefaultButton.Button2);//定义对话框的按钮式样
                        if (MsgBoxResult == DialogResult.Yes)//如果对话框的返回值是YES（按"Y"按钮）
                        {
                            try
                            {
                                Class4 class4 = new Class4(s_FileName, t_FileName, path, textBox1.Text, i + 1);
                                class4.dowork();
                                if (class4.flag)
                                {
                                    string current = System.DateTime.Now.ToString("G");
                                    doc(textBox2, current + "-新建冗余列-表 " + class4.r_redun[1] + "----新建冗余列--新列名 " + class4.qyqx + "\r\n");
                                }
                                else
                                {
                                    doc(textBox2, "此条语句使用并不频繁，无需反规范化\r\n");
                                }
                            }
                            catch (Exception e1)
                            {
                                doc(textBox2, "出错了哦，请检查一下数据库和日志文件是否选择正确，待分析的查询语句有无语法错误");
                            }
                        }
                    }
                    else {
                        doc(textBox2, "此条语句关联的表不多，无需反规范化\r\n");
                    }
                }
            }
        }
    }
}
