using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Windows.Forms;

namespace New_Anti
{
    class Class3
    {
        public string t1, t2, t3, t4;//t4是集函数名,eg : abs(返回参数的绝对值)
        public string[] sql_all = new string[100];//SQL文件的TABLE结构
        public string[] sql_name = new string[100];//SQL文件的TABLE名字
        public int sql_int;//SQL文件的TABLE数量
        public string set_flag;//根据不同的集函数生成不同的方法
        public string set_kernel;//集函数的重要数据
        public string str1;//集函数的重要数据
        public string str2;//集函数的重要数据
        public int set_int;//设定集函数出现的阀值，默认100
        public bool flag;//判断向量

        public Class3(string s1, string s2, string s3, string s4) {
            t1 = s1;
            t2 = s2;
            t3 = s3;
            t4 = s4.TrimEnd(';');
        }

        //分析开始
        public void dowork()
        {
            flag = true;
            set_int = 0;
            ExecuteSqlScript(t1);
            d_split_txt(t3);
            ModifySqlScript_deriv(Form1.output + @"新建派生列.sql");
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

        //新建派生列对日志文件开始分析的方法
        public void d_split_txt(string p)
        {
            StreamReader SR;
            string s1 = "";
            if (t4.IndexOf(" abs(") > -1)
            {
                set_flag = "abs";
                set_kernel = t4.Substring(t4.IndexOf("(") + 1, t4.IndexOf(")") - t4.IndexOf("(") - 1);
                using (SR = new StreamReader(p))
                {
                    do
                    {
                        s1 = SR.ReadLine();
                        if (s1 != null && s1.IndexOf(t4)>-1)
                        {
                            set_int++;
                        }
                    } while (s1 != null);
                }
            }
            else if (t4.IndexOf(" length(") > -1)
            {
                set_flag = "length";
                set_kernel = t4.Substring(t4.IndexOf("(") + 1, t4.IndexOf(")") - t4.IndexOf("(") - 1);
                using (SR = new StreamReader(p))
                {
                    do
                    {
                        s1 = SR.ReadLine();
                        if (s1 != null)
                        {
                            set_int++;
                        }
                    } while (s1 != null);
                }
            }
        }

        //生成对MYSQL数据库进行“新建派生列”操作的SQL文件的方法
        public void ModifySqlScript_deriv(string sqlFile)
        {
            using (StreamWriter sw = new StreamWriter(sqlFile, false))
            {
                if (set_int > 99)
                {
                    str1 = set_kernel.Substring(0, set_kernel.IndexOf("."));
                    str2 = set_kernel.Remove(0, set_kernel.IndexOf(".") + 1);
                    for (int n = 0; n < sql_int; n++)
                    {
                        if (sql_all[n].IndexOf("CREATE TABLE " + str1 + "(") != -1)
                        {
                            int int3 = sql_all[n].IndexOf(str2) + str2.Length + 1;
                            if (sql_all[n].IndexOf(",", int3) != -1)
                            {
                                if (set_flag == "abs")
                                {
                                    string str3 = sql_all[n].Substring(int3, sql_all[n].IndexOf(",", int3+1) - int3);
                                    sw.Write("\r\n" + @"alter table " + str1 + " add column add_" + str2 + " " + str3 + ";");
                                    sw.Write("\r\n" + @"update " + str1 + " set add_" + str2 + " = abs(" + set_kernel + ");");
                                }
                                if (set_flag == "length")
                                {
                                    sw.Write("\r\n" + @"alter table " + str1 + " add column add_" + str2 + " INT;");
                                    sw.Write("\r\n" + @"update " + str1 + " set add_" + str2 + " = length(" + set_kernel + ");");
                                }
                            }
                            else
                            {
                                if (set_flag == "abs")
                                {
                                    string str3 = sql_all[n].Substring(int3, sql_all[n].Length - int3 - 2);
                                    sw.Write("\r\n" + @"alter table " + str1 + " add column add_" + str2 + " " + str3 + ";");
                                    sw.Write("\r\n" + @"update " + str1 + " set add_" + str2 + " = abs(" + set_kernel + ");");
                                }
                                if (set_flag == "length")
                                {
                                    sw.Write("\r\n" + @"alter table " + str1 + " add column add_" + str2 + " INT;");
                                    sw.Write("\r\n" + @"update " + str1 + " set add_" + str2 + " = length(" + set_kernel + ");");
                                }
                            }
                        }
                    }
                }
                else {
                    flag = false;
                }
            }
        }
    }
}
