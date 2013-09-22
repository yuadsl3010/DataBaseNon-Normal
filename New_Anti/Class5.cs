using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Windows.Forms;
using System.Collections; 

namespace New_Anti
{
    class Class5
    {
        public string t1, t2, t3, t4;
        public int set_int;//默认为100的阀值
        public bool flag;
        public Hashtable redun = new Hashtable();//存储数据结构的哈希表
        public string[] r_redun = new string[2];//临时变量
        private string[] n_new = new string[20];
        private string[] n_1 = new string[20];
        private string[] n_2 = new string[20];
        private string[] n_3 = new string[20];
        private int n_3_n = 0;
        private int n_num = 0;
        public int kernel_int;//行数
        public string[] kernel_str;

        public Class5(string s1, string s2, string s3, string s4, int s5) {
            t1 = s1;
            t2 = s2;
            t3 = s3;
            t4 = s4;
            kernel_int = s5;
        }

        //分析开始
        public void dowork()
        {
            kernel_str = new string[kernel_int];
            int temp_int = 0;
            flag = true;
            set_int = 0;
            using (StreamWriter sw = new StreamWriter(Form1.output + @"temp.sql", false))
            {
                sw.Write(t4);
            }
            using (StreamReader sr = new StreamReader(Form1.output + @"temp.sql"))
            {
                string line = string.Empty;
                do
                {
                    line = sr.ReadLine();
                    if (line == null)
                        break;
                    kernel_str[temp_int] = line;
                    temp_int++;
                } while (true);
            }
            ExecuteSqlScript(t1);
            split_txt(t3);
            if (set_int > 99)
                ModifySqlScript_new(Form1.output + @"重新组表.sql", t1, Form1.output + @"temp.sql");
            else
                flag = false;
        }

        //重新组表对日志文件开始分析的方法
        public void split_txt(string p)
        {
            StreamReader SR;
            string s1 = "";
            using (SR = new StreamReader(p))
            {
                do
                {
                    s1 = SR.ReadLine();
                    if (s1 != null && s1.IndexOf(kernel_str[0]) > -1)
                    {
                        for (int i = 1; i < kernel_int; i++)
                        {
                            s1 = SR.ReadLine();
                            if (s1 != null && s1.IndexOf(kernel_str[i].Trim(';')) > -1)
                            {
                                if (i == kernel_int - 1)
                                    set_int++;
                                continue;
                            }
                            else { break; }
                        }
                    }
                } while (s1 != null);
            }
        }

        //对SQL文件进行分析的方法
        public void ExecuteSqlScript(string sqlFile)
        {
            using (StreamReader sr = new StreamReader(sqlFile))
            {
                string line = string.Empty;
                char spaceChar = ' ';
                string semicolon = ";";
                string newLIne = "\r\n";
                char[] newLine = new char[4];
                newLine[0] = '\\';
                newLine[1] = 'r';
                newLine[2] = '\\';
                newLine[3] = 'n';
                string sprit = "/", whiffletree = "-";
                string sql = string.Empty;
                do
                {
                    line = sr.ReadLine();
                    // 文件结束
                    if (line == null) break;
                    // 跳过注释行
                    if (line.StartsWith(sprit) || line.StartsWith(whiffletree)) continue;
                    // 去除右边空格
                    line = line.TrimEnd(spaceChar);
                    line = line.TrimEnd(newLine);
                    sql += line;

                    // 以分号(;)结尾，则执行SQL
                    if (sql.EndsWith(semicolon))
                    {
                        if (sql.StartsWith("CREATE TABLE ") && sql.IndexOf('(') > 0)
                        {
                            int s = sql.IndexOf(" REFERENCES ");
                            if (s == -1)
                            {
                                string temp = sql.Substring(13, sql.IndexOf("(") - 13);
                                redun.Add(temp, "END");
                            }
                            else
                            {
                                string temp = sql.Substring(13, sql.IndexOf("(") - 13);
                                string refer = sql.Substring(s + 12, sql.IndexOf("(", s) - s - 12);
                                string now = sql.Remove(0, s + 12);
                                int f = now.IndexOf(" REFERENCES ");
                                while (f > 0)
                                {
                                    refer = refer + ", " + now.Substring(f + 12, now.IndexOf("(", f) - f - 12);
                                    now = now.Remove(0, f + 12);
                                    f = now.IndexOf(" REFERENCES ");
                                }
                                redun.Add(temp, refer);
                            }
                        }
                        sql = string.Empty;
                    }
                    else
                    {
                        // 添加换行符
                        if (sql.Length > 0) sql += newLIne;
                    }
                } while (true);
            }
        }

        //生成对MYSQL数据库进行“重新组表”操作的SQL文件的方法
        public void ModifySqlScript_new(string sqlFile, string sF, string pa)
        {
            string[,] source = new string[10, 2];
            int ti = 0;
            bool flag = true;
            bool flag1 = true;
            bool flag2 = true;
            string temp = null;
            Hashtable tem = redun;
            while (flag)
            {
                flag = false;
                flag1 = true;
                flag2 = true;
                while (flag1)
                {
                    foreach (DictionaryEntry var in tem)
                    {
                        string father = var.Key.ToString();
                        string son = var.Value.ToString();
                        flag1 = false;
                        if (flag2 && son == "END")
                        {
                            r_redun[0] = father;
                            temp = father;
                            tem.Remove(var.Key);
                            //flag = true;
                            flag2 = false;
                            flag1 = true;
                            break;
                        }
                        if (!flag2 && son.IndexOf(temp) > -1)
                        {
                            temp = father;
                            flag1 = true;
                            break;
                        }
                    }
                }
                r_redun[1] = temp;
                source[ti, 0] = r_redun[0];
                source[ti, 1] = r_redun[1];
                ti++;
                //(string sqlFile, string sF, string pa, string sql)@"E:\refer.sql", s_FileName, path, @"E:\冗余后的查询语句.txt"

                string tt = null;
                bool flag3 = false;
                using (StreamReader sr = new StreamReader(pa))
                {
                    string tt_temp = sr.ReadLine();
                    while (tt_temp.IndexOf(";") > -1 && tt == null)
                        tt_temp = sr.ReadLine();
                    while (true)
                    {
                        if (tt_temp == null)
                            break;
                        tt += tt_temp;
                        if (tt_temp.IndexOf("from") < 0)
                        {
                            tt_temp = sr.ReadLine();
                            continue;
                        }
                        break;
                    }
                    string ts = tt.Remove(0, tt.IndexOf(" from ") + 6).TrimEnd(' ');
                    //string sssss = sr.ReadLine();
                    string ff = null;
                    do
                    {
                        ff = sr.ReadLine();
                        if (ff == null) break;
                        if (ff.IndexOf("where") > -1)
                        {
                            ff = ff.Remove(0, ff.IndexOf("where") + 6);
                            n_new[n_num] = ff.Substring(0, ff.IndexOf(" "));
                            n_num++;
                        }
                    } while (true);
                    tt = tt.Remove(0, tt.IndexOf("select") + 7);
                    while (true)
                    {
                        n_new[n_num] = tt.Substring(0, tt.IndexOf(","));
                        n_num++;
                        tt = tt.Remove(0, tt.IndexOf(",") + 1);
                        if (tt.IndexOf(",") < 0)
                        {
                            n_new[n_num] = tt.Substring(0, tt.IndexOf(" "));
                            break;
                        }
                    }
                    tt = null;
                    tt_temp = null;
                    for (int i = 0; i < n_num + 1; i++)
                    {
                        n_1[i] = n_new[i].Substring(0, n_new[i].IndexOf("."));
                        n_2[i] = n_new[i].Substring(n_new[i].IndexOf(".") + 1, n_new[i].Length - n_new[i].IndexOf(".") - 1);
                    }
                    using (StreamReader sR = new StreamReader(sF))
                    {
                        using (StreamWriter sW = new StreamWriter(sqlFile, true))
                        {
                            string nn = null;
                            sW.Write(@"CREATE TABLE renew_" + r_redun[1] + "(\r\n");
                            do
                            {
                                nn = sR.ReadLine();
                                if (nn.IndexOf("INSERT") > -1)
                                    break;
                                if (nn.IndexOf("CREATE TABLE ") < 0)
                                    continue;
                                if (!flag3 && nn.IndexOf(r_redun[1]) > -1)
                                {
                                    nn = sR.ReadLine();
                                    sW.Write(nn + "\r\n");
                                    flag3 = true;
                                }
                                for (int i = 0; i < n_num + 1; i++)
                                {
                                    if (nn.IndexOf(n_1[i]) > -1)
                                    {
                                        while (true)
                                        {
                                            nn = sR.ReadLine();
                                            if (nn.IndexOf("INSERT ") > -1)
                                                break;
                                            if (nn == "")
                                                break;
                                            if (nn.IndexOf(n_2[i]) > -1)
                                            {
                                                if (nn.IndexOf(",") > -1)
                                                {
                                                    n_3[n_3_n] = nn.Substring(0, nn.IndexOf(","));
                                                    n_3_n++;
                                                }
                                                if (nn.IndexOf(";") > -1)
                                                {
                                                    n_3[n_3_n] = nn.Substring(0, nn.IndexOf(";") - 1);
                                                    n_3_n++;
                                                }
                                                break;
                                            }
                                        }
                                        break;
                                    }
                                }
                            } while (true);
                        }
                    }
                    string vv_temp = null;
                    int ttt = 0;
                    using (StreamReader Sr = new StreamReader(pa))
                    {
                        using (StreamWriter sW = new StreamWriter(sqlFile, true))
                        {
                            while (true)
                            {
                                vv_temp = Sr.ReadLine();
                                if (vv_temp == null)
                                    break;
                                if (vv_temp.IndexOf("left join") > -1)
                                {
                                    string ttemp = vv_temp.Substring(10, vv_temp.IndexOf(" on ") - 10);
                                    for (int i = 0; i < n_num; i++)
                                    {
                                        if (n_3[i].IndexOf(ttemp) > -1)
                                        {
                                            if (ttt == 1)
                                            {
                                                sW.Write(n_3[i] + ");\r\n");
                                                break;
                                            }
                                            sW.Write(n_3[i] + ",\r\n");
                                            ttt++;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            string xx = null;
            using (StreamReader sr1 = new StreamReader(pa))
            {
                using (StreamWriter SS = new StreamWriter(sqlFile, true))
                {
                    string zx = string.Empty;
                    do
                    {
                        zx = sr1.ReadLine();
                        if (zx.IndexOf("where") > -1)
                            break;
                        xx = xx + "\r\n" + zx;
                    } while (true);
                    SS.Write("insert into renew_" + r_redun[1]);
                    SS.Write(xx.Insert(9, n_new[0] + ","));
                    SS.Write(";\r\n");
                }
            }
        }
    }
}
