using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Windows.Forms;

namespace New_Anti
{
    class Class2
    {
        public string t1, t2, t3;
        public string[] column_verti = new string[100];//垂直分割的单元名
        public int[] colunsql_int_num = new int[100];//垂直分割的单元数量
        public string[] sql_all = new string[100];//SQL文件的TABLE结构
        public string[] sql_name = new string[100];//SQL文件的TABLE名字
        public int sql_int;//SQL文件的TABLE数量
        public int vv;//垂直分割计数器
        private bool flag;
        public Class2(string s1, string s2, string s3) {
            t1 = s1;
            t2 = s2;
            t3 = s3;
        }

        //分析开始
        public void dowork()
        {
            ExecuteSqlScript(t1);
            v_split_txt(t3);
            ModifySqlScript_verti(Form1.output + @"垂直分割.sql");
        }

        //对SQL文件进行分析的方法
        public void ExecuteSqlScript(string sqlFile)
        {
            using (StreamReader sr = new StreamReader(sqlFile))
            {
                string line = string.Empty;
                char spaceChar = ' ';
                string newLIne = "\r\n", semicolon = ";";
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
                            int s = sql.IndexOf('(');
                            int m = 0;
                            sql_all[sql_int] = sql;
                            /*while (sql.Substring(s + 1, 1) == " ")
                            {
                                n++;
                                s++;
                            }*/
                            while (sql.Substring(s + 1, 1) != " ")
                            {
                                m++;
                                s++;
                            }
                            string tempp = sql_all[sql_int].Substring(sql.IndexOf('(') + 1, m);
                            sql_name[sql_int] = sql_all[sql_int].Substring(13, sql_all[sql_int].IndexOf("(") - 13) + "." + tempp;
                            sql_int++;
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

        //垂直分割对日志文件开始分析的方法
        public void v_split_txt(string p)
        {
            StreamReader SR;
            string s1 = "";
            using (SR = new StreamReader(p))
            {
                do
                {
                    s1 = SR.ReadLine();
                    if (s1 == null)
                        break;
                    if (s1.IndexOf("abs(") > -1 || s1.IndexOf("length(") > -1)
                        continue;
                    string v_temp = s1;
                    if (s1.IndexOf("insert") > -1)
                    {
                        string table = s1.Substring(12, s1.IndexOf("(") - 12);
                        if (s1.IndexOf("values") > s1.IndexOf(","))
                        {
                            while (s1.IndexOf("values") > s1.IndexOf(","))
                            {
                                if (vv == 0)
                                {
                                    column_verti[vv] = table + "." + s1.Substring(s1.IndexOf("(") + 1, s1.IndexOf(",") - s1.IndexOf("(") - 1);
                                    colunsql_int_num[vv] = 1;
                                    vv++;
                                }
                                else
                                {
                                    for (int a = 0; a < vv; a++)
                                    {
                                        flag = false;
                                        if (column_verti[a] == table + "." + s1.Substring(s1.IndexOf("(") + 1, s1.IndexOf(",") - s1.IndexOf("(") - 1))
                                        {
                                            colunsql_int_num[a]++;
                                            break;
                                        }
                                        else
                                        {
                                            flag = true;
                                        }
                                    }
                                    if (flag)
                                    {
                                        column_verti[vv] = table + "." + s1.Substring(s1.IndexOf("(") + 1, s1.IndexOf(",") - s1.IndexOf("(") - 1);
                                        colunsql_int_num[vv] = 1;
                                        vv++;
                                    }
                                }
                                s1 = s1.Remove(0, s1.IndexOf(",") + 1);
                            }
                            if (vv == 0)
                            {
                                column_verti[vv] = table + "." + s1.Substring(0, s1.IndexOf(")"));
                                colunsql_int_num[vv] = 1;
                                vv++;
                            }
                            else
                            {
                                for (int a = 0; a < vv; a++)
                                {
                                    flag = false;
                                    if (column_verti[a] == table + "." + s1.Substring(0, s1.IndexOf(")")))
                                    {
                                        colunsql_int_num[a]++;
                                        break;
                                    }
                                    else
                                    {
                                        flag = true;
                                    }
                                }
                                if (flag)
                                {
                                    column_verti[vv] = table + "." + s1.Substring(0, s1.IndexOf(")"));
                                    colunsql_int_num[vv] = 1;
                                    vv++;
                                }
                            }
                        }
                        /*else {
                            if (vv == 0)
                            {
                                column_verti[vv] = table + "." + s1.Substring(s1.IndexOf("(") + 1, s1.IndexOf(")") - s1.IndexOf("(") - 1);
                                colunsql_int_num[vv] = 1;
                                vv++;
                            }
                            else
                            {
                                for (int a = 0; a < vv; a++)
                                {
                                    flag = false;
                                    if (column_verti[a] == table + "." + s1.Substring(s1.IndexOf("(") + 1, s1.IndexOf(")") - s1.IndexOf("(") - 1))
                                    {
                                        colunsql_int_num[a]++;
                                        break;
                                    }

                                    else
                                    {
                                        flag = true;
                                    }
                                }
                                if (flag)
                                {
                                    column_verti[vv] = table + "." + s1.Substring(s1.IndexOf("(") + 1, s1.IndexOf(")") - s1.IndexOf("(") - 1);
                                    colunsql_int_num[vv] = 1;
                                    vv++;
                                }
                            }
                        }*/
                    }
                    else if (s1.IndexOf(" select ") > -1 && s1.IndexOf(" from ") > -1 && s1.IndexOf(",") == -1)
                    {
                        while (s1.IndexOf(".") > 0)
                        {
                            int n = s1.IndexOf(".");
                            int x = 1;
                            int y = 1;
                            if (n != -1)
                            {
                                while (s1.Substring(n - x, 1) != " ")
                                    x++;
                            }
                            if (n != -1)
                            {
                                while (s1.Substring(n + y, 1) != " ")
                                    y++;
                            }
                            if (vv == 0)
                            {
                                column_verti[vv] = s1.Substring(n - x + 1, x + y);
                                colunsql_int_num[vv] = 1;
                                vv++;
                            }
                            else
                            {
                                for (int a = 0; a < vv; a++)
                                {
                                    flag = false;
                                    if (column_verti[a] == s1.Substring(n - x + 1, x + y))
                                    {
                                        colunsql_int_num[a]++;
                                        break;
                                    }
                                    else
                                    {
                                        flag = true;
                                    }
                                }
                                if (flag)
                                {
                                    column_verti[vv] = s1.Substring(n - x + 1, x + y);
                                    colunsql_int_num[vv] = 1;
                                    vv++;
                                }
                            }
                            s1 = s1.Remove(0, n + y);
                        }
                    }
                } while (s1 != null);
            }
        }
        //生成对MYSQL数据库进行“垂直分割”操作的SQL文件的方法
        public void ModifySqlScript_verti(string sqlFile)
        {
            using (StreamWriter sw = new StreamWriter(sqlFile, false))
            {
                //垂直分割时先创建新表
                for (int n = 0; n < sql_int; n++)
                {
                    /*
                    bool SF = false;
                    for (int k = 0; k < d_temp_n; k++)
                    {
                        if (sql_all[n].IndexOf("CREATE TABLE " + d_temp[k] + "(") != -1) {
                            SF = true;
                            break;
                        }
                    }
                    if (SF == true)
                        continue;*/
                    bool flag = false;
                    for (int h = 0; h < vv; h++)
                    {
                        string bs = "CREATE TABLE " + column_verti[h].Substring(0, column_verti[h].IndexOf("."));
                        if (sql_all[n].IndexOf(bs) > -1)
                        {
                            flag = true;
                            break;
                        }
                    }
                    if (!flag)
                        continue;
                    sw.WriteLine(sql_all[n].Substring(0, sql_all[n].IndexOf("(")) + "_verti(");
                    string primary_string = sql_all[n].Substring(sql_all[n].IndexOf("(") + 1, sql_all[n].IndexOf(",") - sql_all[n].IndexOf("(") - 1);
                    sw.WriteLine(primary_string + ",");
                    for (int h = 0; h < vv; h++)
                    {
                        string sb = sql_all[n].Substring(13, sql_all[n].IndexOf("(") - 13);
                        string bs = column_verti[h].Substring(0, column_verti[h].IndexOf("."));
                        if (sql_all[n].Substring(13, sql_all[n].IndexOf("(") - 13) == column_verti[h].Substring(0, column_verti[h].IndexOf(".")))
                        {
                            string m = column_verti[h].Remove(0, column_verti[h].IndexOf(".") + 1);
                            int p = sql_all[n].IndexOf(m);
                            int f = sql_all[n].IndexOf(",", p);
                            if (f > 0)
                            {
                                string z = sql_all[n].Substring(p, f - p);
                                if (z != primary_string)
                                {
                                    if (h == vv - 1)
                                        sw.WriteLine(z + ");");
                                    else
                                        sw.WriteLine(z + ",");
                                }
                            }
                            else
                            {
                                string z = sql_all[n].Substring(p, sql_all[n].Length - p);
                                if (z != primary_string)
                                {
                                    if (h == vv - 1)
                                        sw.WriteLine(z + ");");
                                    else
                                        sw.WriteLine(z + ",");
                                }
                            }
                        }
                    }
                }
                //将原表中数据放入新表中
                for (int n = 0; n < sql_int; n++)
                {
                    /*
                    bool SF = false;
                    for (int k = 0; k < d_temp_n; k++)
                    {
                        if (sql_all[n].IndexOf("CREATE TABLE " + d_temp[k] + "(") != -1) {
                            SF = true;
                            break;
                        }
                    }
                    if (SF == true)
                        continue;*/
                    bool flag = false;
                    for (int h = 0; h < vv; h++)
                    {
                        string bs = "CREATE TABLE " + column_verti[h].Substring(0, column_verti[h].IndexOf("."));
                        if (sql_all[n].IndexOf(bs) > -1)
                        {
                            flag = true;
                            break;
                        }
                    }
                    if (!flag)
                        continue;
                    string b = "";
                    for (int h = 0; h < vv; h++)
                    {
                        if (sql_all[n].Substring(13, sql_all[n].IndexOf("(") - 13) == column_verti[h].Substring(0, column_verti[h].IndexOf(".")))
                        {
                            string m = column_verti[h].Remove(0, column_verti[h].IndexOf(".") + 1);
                            b = b + m + ",";
                        }
                    }
                    sw.WriteLine("insert into " + sql_all[n].Substring(13, sql_all[n].IndexOf("(") - 13) + "_verti select " + sql_name[n].Remove(0, sql_name[n].IndexOf(".") + 1) + "," + b.TrimEnd(',') + "from " + sql_all[n].Substring(13, sql_all[n].IndexOf("(") - 13) + ";");
                }
                //删除原表的数据
                for (int n = 0; n < sql_int; n++)
                {
                    /*
                    bool SF = false;
                    for (int k = 0; k < d_temp_n; k++)
                    {
                        if (sql_all[n].IndexOf("CREATE TABLE " + d_temp[k] + "(") != -1) {
                            SF = true;
                            break;
                        }
                    }
                    if (SF == true)
                        continue;*/
                    bool flag = false;
                    for (int h = 0; h < vv; h++)
                    {
                        string bs = "CREATE TABLE " + column_verti[h].Substring(0, column_verti[h].IndexOf("."));
                        if (sql_all[n].IndexOf(bs) > -1)
                        {
                            flag = true;
                            break;
                        }
                    }
                    if (!flag)
                        continue;
                    string b = "";
                    for (int h = 0; h < vv; h++)
                    {
                        if (sql_all[n].Substring(13, sql_all[n].IndexOf("(") - 13) == column_verti[h].Substring(0, column_verti[h].IndexOf(".")))
                        {
                            string m = column_verti[h].Remove(0, column_verti[h].IndexOf(".") + 1);
                            b = m + ",";
                        }
                    }
                    sw.WriteLine("alter table " + sql_all[n].Substring(13, sql_all[n].IndexOf("(") - 13) + " drop column " + b.TrimEnd(',') + ";");
                }
            }
        }
    }
}
