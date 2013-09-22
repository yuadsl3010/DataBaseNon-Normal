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
    class Class4
    {
        public string t1, t2, t3, t4;
        public int set_int;//默认为100的阀值
        public bool flag;
        public string[] r_redun = new string[2];//临时变量
        public string qyqx;//亲翔君生孩子？！
        public Hashtable redun = new Hashtable();//存储数据结构的哈希表
        public int kernel_int;//行数
        public string[] kernel_str;

        public Class4(string s1, string s2, string s3, string s4, int s5) {
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
            split_txt(t3, Form1.output + @"temp.sql");
            if (set_int > 99)
                ModifySqlScript_refer(Form1.output + @"新建冗余列.sql", t1, Form1.output + @"temp.sql");
            else
                flag = false;
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

        //新建冗余列对日志文件开始分析的方法
        public void split_txt(string p, string SQL)
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
                        for (int i = 1; i < kernel_int; i++) {
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

        //生成对MYSQL数据库进行“重新组表”操作的SQL文件的方法
        public void ModifySqlScript_refer(string sqlFile, string sF, string pa)
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
                            flag = true;
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
                using (StreamWriter sw = new StreamWriter(sqlFile, true))
                {
                    qyqx = "";
                    bool flag3 = false;
                    bool flag4 = false;
                    string tt = null;
                    string mm = null;
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
                            if (tt_temp.IndexOf(" from ") < 0)
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
                            if (r_redun[0] != ts)
                            {
                                if (flag3)
                                {
                                    mm = mm + "\r\n" + ff;
                                    if (ff.IndexOf(r_redun[0]) > -1)
                                    {
                                        flag3 = false;
                                        flag4 = true;
                                    }
                                }
                                if (ff.IndexOf(r_redun[1]) > -1)
                                {
                                    flag3 = true;
                                }
                                if (flag4 && tt.IndexOf(r_redun[0]) > -1)
                                {
                                    string ma = null;
                                    int v = tt.IndexOf(r_redun[0]) + r_redun[0].Length + 1;
                                    int k = tt.IndexOf(",", v) - v;
                                    if (k < 0)
                                    {
                                        ma = tt.Substring(v, tt.IndexOf(" ", v) - v);
                                    }
                                    else
                                    {
                                        ma = tt.Substring(v, k);
                                    }
                                    tt = null;
                                    tt_temp = null;
                                    using (StreamReader sr1 = new StreamReader(sF))
                                    {
                                        string zx = string.Empty;
                                        do
                                        {
                                            zx = sr1.ReadLine();
                                            if (zx == null) break;
                                            if (zx.IndexOf(ma) > -1)
                                            {
                                                qyqx = zx.Remove(zx.Length - 2, 2);
                                                sw.Write("alter table " + r_redun[1] + " add column " + qyqx + ";\r\n");
                                                sw.Write("update " + r_redun[1] + mm + " set " + r_redun[1] + "." + ma + " = " + r_redun[0] + "." + ma + ";\r\n");
                                                flag4 = false;
                                                break;
                                            }
                                        } while (true);
                                    }
                                }
                            }
                            else
                            {
                                break;
                            }
                        } while (true);
                    }
                }
            }
        }
    }
}
