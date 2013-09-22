using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Windows.Forms;

namespace New_Anti
{
    class Class1//水平分割
    {
        public string t1, t2, t3;
        public string[] table_name = new string[100];//水平分割from后的表名
        public string[] table_condition = new string[100];//水平分割where后的条件
        public int table_int;//水平分割的数量
        public string[] sql_all = new string[100];//SQL文件的TABLE结构
        public string[] sql_name = new string[100];//SQL文件的TABLE名字
        public int sql_int;//SQL文件的TABLE数量
        public Class1(string s1, string s2, string s3) {
            t1 = s1;
            t2 = s2;
            t3 = s3;
        }

        //分析开始
        public void dowork() {
            ExecuteSqlScript(t1);
            h_split_txt(t3);
            ModifySqlScript_horiz(Form1.output + @"水平分割.sql");
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
                            /*while(sql.Substring(s+1, 1) == " "){
                                n++;
                                s++;
                            }*/
                            while (sql.Substring(s + 1, 1) != " ")
                            {
                                m++;
                                s++;
                            }
                            string tem = sql.Substring(13, sql.IndexOf("(") - 13);
                            string temppp = sql.Substring(sql.IndexOf("(") + 1, m);
                            sql_name[sql_int] = tem + "." + temppp;
                            sql_all[sql_int] = tem + "." + sql.Substring(sql.IndexOf('(') + 1, sql.Length - (sql.IndexOf('(') + 1));
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
        //水平分割对日志文件开始分析的方法
        public void h_split_txt(string p)
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
                    if (s1.IndexOf(" select ") > -1 && s1.IndexOf(" from ") > -1 && s1.IndexOf(",") == -1)
                    {
                        //不需要WHERE
                        int n = s1.IndexOf(" where ");
                        if (n != -1)
                        {
                            table_name[table_int] = s1.Substring(s1.IndexOf("from") + 5, s1.IndexOf("where") - s1.IndexOf("from") - 6);
                            table_condition[table_int] = s1.Remove(0, s1.IndexOf(" where "));
                            table_int++;
                        }
                        else
                        {
                            int temp1 = s1.IndexOf(" from ") + 6;
                            int temp2 = s1.IndexOf(";", temp1);
                            table_name[table_int] = s1.Substring(temp1, s1.Length - temp1);
                            table_condition[table_int] = ";";
                            table_int++;
                        }
                    }/*
                    if (s1.EndsWith(";"))
                    {
                        if (s1 != null)
                        {
                            if (s1.IndexOf(" select ")>-1 && s1.IndexOf(" from ")>-1)
                            {
                                //不需要WHERE
                                int n = s1.IndexOf(" where ");
                                if (n != -1)
                                {
                                    table_name[table_int] = s1.Substring(s1.IndexOf("from") + 5, s1.IndexOf("where") - s1.IndexOf("from") - 6);
                                    table_condition[table_int] = s1.Remove(0, s1.IndexOf(" where "));
                                    table_int++;
                                }
                                else
                                {
                                    int temp1 = s1.IndexOf(" from ") + 6;
                                    int temp2 = s1.IndexOf(";", temp1);
                                    table_name[table_int] = s1.Substring(temp1, temp2 - temp1);
                                    table_condition[table_int] = ";";
                                    table_int++;
                                }
                            }
                            if (s1.IndexOf("update")>-1)
                            {
                                int n = s1.IndexOf(" where ");
                                table_name[table_int] = s1.Substring(7, s1.IndexOf("set") - 8);
                                table_condition[table_int] = s1.Remove(0, s1.IndexOf(" where ") + 7);
                                table_int++;
                            }
                        }
                    }*/
                } while (s1 != null);
            }
        }
        //生成对MYSQL数据库进行“水平分割”操作的SQL文件的方法
        public void ModifySqlScript_horiz(string sqlFile)
        {
            using (StreamWriter sw = new StreamWriter(sqlFile, false))
            {
                string[] ff = new string[table_int];
                //修改表student增加flag标识列
                for (int n = 0; n < table_int; n++)
                {
                    bool xx = true;
                    for (int zx = 0; zx < n; zx++)
                    {
                        if (ff[zx] == table_name[n])
                        {
                            xx = false;
                            continue;
                        }
                    }
                    if (xx)
                        sw.WriteLine(@"alter table " + table_name[n] + " add column flag int default '0';");
                    ff[n] = table_name[n];
                }

                //根据用户提供的SELECT语句，修改相应查询项的flag标识
                for (int n = 0; n < table_int; n++)
                {
                    if (table_condition[n] == ";")
                        continue;
                    sw.WriteLine(@"update " + table_name[n] + " set flag = 1" + table_condition[n]);
                }

                //创建触发器，一段时间比如30天后调用该触发器，能够提取出历史数据，前n行分割出去放入新表student_horiz中
                for (int n = 0; n < 1; n++)// for (int n = 0; n < table_int; n++)
                {
                    for (int f = 0; f < sql_int; f++)
                    {
                        if (sql_name[f].Substring(0, sql_name[f].IndexOf(".")) == table_name[n])
                        {
                            sw.Write(@"delimiter //
CREATE PROCEDURE horiz_split(OUT i INT)
BEGIN
DECLARE n int;
DECLARE flag_2 int;
SET i = 0;
SELECT COUNT(*) INTO n FROM " + table_name[n] + @" WHERE flag = 0;
loop1:WHILE i <= n DO
SELECT flag INTO flag_2 FROM " + table_name[n] + " WHERE " + sql_name[f].Remove(0, sql_name[f].IndexOf(".") + 1) + @" = n;
IF flag_2 = 0 THEN
SET i = i + 1;
ELSE
LEAVE loop1;
END IF;
END WHILE loop1;
END//
delimiter ;");


                            //调用该触发器
                            sw.WriteLine("\r\n" + @"CALL horiz_split(@n);");

                            //删除该触发器
                            sw.WriteLine("\r\n" + "DROP PROCEDURE horiz_split;");

                            //创建存储过程
                            sw.Write("\r\n" + "delimiter //" + "\r\n" + @"CREATE PROCEDURE horiz_create()
BEGIN
IF @n > 0 THEN");
                            //提示用户后，若用户选择水平分割，则执行下面操作
                            //新建表student_horiz
                            char[] ch = new char[2];
                            ch[0] = ')';
                            ch[1] = ';';
                            sw.WriteLine("\r\n" + "CREATE TABLE " + table_name[n] + "_horiz(");
                            sw.Write(sql_all[f].Remove(0, sql_all[f].IndexOf(".") + 1).TrimEnd(ch) + @"),");
                            sw.WriteLine("flag int DEFAULT 0);");

                            //向表中插入历史数据
                            sw.Write("\r\n" + "insert into " + table_name[n] + "_horiz(SELECT * from " + table_name[n] + " where " + sql_name[f].Remove(0, sql_name[f].IndexOf(".") + 1) + " < @n);");

                            //删除原表中数据
                            sw.Write("\r\n" + "delete from " + table_name[n] + " where " + sql_name[f].Remove(0, sql_name[f].IndexOf(".") + 1) + " < @n;");

                            //结束
                            sw.Write("\r\n" + @"END IF;
END//
delimiter ;");

                            //调用该存储过程
                            sw.Write("\r\n" + @"CALL horiz_create();");

                            //删除该存储过程
                            sw.Write("\r\n" + @"DROP PROCEDURE horiz_create;");
                        }
                    }
                }

            }
        }
    }
}
