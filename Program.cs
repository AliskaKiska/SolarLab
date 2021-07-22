using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pozdravlyalka
{
    using System;
    using System.Data;
    using System.Data.OleDb;
    public class BirthList
    {
        private static string stringConnect = $@"Provider=Microsoft.Jet.OLEDB.4.0;Data Source={Environment.CurrentDirectory}\BDList.mdb;Persist Security Info=True";
        private OleDbConnection connect;
        private OleDbDataAdapter adapter;
        private OleDbCommand command;
        private DataTable datatable;

        public BirthList()
        {
            connect = new OleDbConnection(stringConnect);
            datatable = new DataTable();
        }

        public DataTable GetAll()
        {
            connect.Open();
            datatable.Clear();
            adapter = new OleDbDataAdapter("SELECT * FROM List ORDER BY MBirth ASC, DBirth ASC;", connect);
            adapter.Fill(datatable);
            connect.Close();
            return datatable;
        }

        public DataTable GetToday()
        {
            connect.Open();
            datatable.Clear();
            adapter = new OleDbDataAdapter($"SELECT * FROM List WHERE DBirth = {DateTime.Today.Day} AND MBirth = {DateTime.Today.Month};", connect);
            adapter.Fill(datatable);
            connect.Close();
            return datatable;
        }

        public DataTable GetSomeNext()
        {
            connect.Open();
            datatable.Clear();
            adapter = new OleDbDataAdapter($"SELECT * FROM List WHERE (MBirth * 100 + DBirth - {DateTime.Today.Month} * 100 - {DateTime.Today.Day}) > 0 AND (MBirth * 100 + DBirth - {DateTime.Today.Month} * 100 - {DateTime.Today.Day}) < 100 ORDER BY MBirth ASC, DBirth ASC;", connect);
            adapter.Fill(datatable);
            connect.Close();
            return datatable;
        }

        public bool delete(int id)
        {
            connect.Open();
            command = new OleDbCommand($"DELETE FROM List WHERE id = {id};", connect);
            bool ret = (command.ExecuteNonQuery() > 0);
            connect.Close();
            return ret;
        }
        public bool insert(string name, DateTime date)
        {
            connect.Open();
            command = new OleDbCommand($"INSERT INTO List (NName, DBirth, MBirth, YBirth) VALUES ('{name}', {date.Day}, {date.Month}, {date.Year});", connect);
            bool ret = command.ExecuteNonQuery() > 0;
            connect.Close();
            return ret;
        }

        public bool update(int id, string name, DateTime date)
        {
            connect.Open();
            command = new OleDbCommand($"UPDATE List SET NName = '{name}', DBirth = {date.Day}, MBirth = {date.Month}, YBirth = {date.Year} WHERE id = {id};", connect);
            bool ret = (command.ExecuteNonQuery() > 0);
            connect.Close();
            return ret;
        }

        public bool find(int id)
        {
            connect.Open();
            adapter = new OleDbDataAdapter($"SELECT * FROM List WHERE id = {id};", connect);
            adapter.Fill(datatable);
            connect.Close();
            if (datatable.Rows.Count == 0) return false;
            else return true;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            int menu(BirthList blist)
            {
                Console.WriteLine();
                for (int i = 0; i < Console.BufferWidth; i++) Console.Write("-");
                if (Console.BufferWidth % 2 == 0)
                {
                    for (int i = 0; i < (Console.BufferWidth - 4) / 2; i++) Console.Write(" ");
                    Console.Write("Меню");
                    for (int i = 0; i < (Console.BufferWidth - 4) / 2; i++) Console.Write(" ");
                }
                else
                {
                    for (int i = 0; i < (Console.BufferWidth - 5) / 2; i++) Console.Write(" ");
                    Console.Write("Меню");
                    for (int i = 0; i < (Console.BufferWidth - 3) / 2; i++) Console.Write(" ");
                }
                for (int i = 0; i < Console.BufferWidth; i++) Console.Write("-");
                Console.WriteLine("\n1) Вывести весь список ДР\n2) Вывести сегодняшние и ближайшие ДР\n3) Добавить запись\n4) Редактировать запись (по id)\n5) Удалить запись (по id)\n\n0) Выход\n");
                int ch;
                try
                {
                    ch = Convert.ToInt32(Console.ReadLine());
                    if (ch < 0 || ch > 5) throw new Exception();
                }
                catch (Exception)
                {
                    Console.Clear();
                    Console.WriteLine("Неверный ввод!");
                    return menu(blist);
                }
                switch (ch)
                {
                    case 0:
                        return 0;
                    case 1:
                        Console.Clear();
                        DataTable dataTable = blist.GetAll();
                        if (dataTable.Rows.Count > 0)
                        {
                            Console.WriteLine("\nID    ФИО    Дата рождения\n");
                            foreach (DataRow r in blist.GetAll().Rows)
                            {
                                Console.WriteLine($"{r[0]}  {r[1]}  {r[2]}.{r[3]}.{r[4]}");
                            }
                        }
                        else Console.WriteLine("\nСписок пуст");
                        break;
                    case 2:
                        Console.Clear();
                        Console.WriteLine($"\nСегодня ({DateTime.Today.ToString()}) ДР празднуют:");
                        dataTable = blist.GetToday();
                        if (dataTable.Rows.Count > 0)
                        {
                            Console.WriteLine("\nID    ФИО\n");
                            foreach (DataRow r in dataTable.Rows)
                            {
                                Console.WriteLine($"{r[0]}  {r[1]}");
                            }
                        }
                        else Console.WriteLine("\nСписок пуст");
                        Console.WriteLine("\nБлижайшие ДР:");
                        dataTable = blist.GetSomeNext();
                        if (dataTable.Rows.Count > 0)
                        {
                            Console.WriteLine("\nID    ФИО     Дата рождения");
                            foreach (DataRow r in dataTable.Rows)
                            {
                                Console.WriteLine($"{r[0]}  {r[1]}  {r[2]}.{r[3]}.{r[4]}");
                            }
                        }
                        else Console.WriteLine("\nСписок пуст");
                        break;
                    case 3:
                        Console.Clear();
                        string n;
                        DateTime d;
                        try
                        {
                            Console.WriteLine("Введите ФИО");
                            n = Console.ReadLine();
                            Console.Clear();
                            if (n == "") throw new Exception();
                            Console.WriteLine("Введите дату рождения (в формате ДД.ММ.ГГГГ)");
                            d = Convert.ToDateTime(Console.ReadLine(), System.Globalization.CultureInfo.CreateSpecificCulture("ru-RU"));
                            Console.Clear();
                            if (!blist.insert(n, d)) Console.WriteLine("Ошибка: запись не добавлена");
                            else Console.WriteLine("Запись добавлена");
                            break;
                        }
                        catch (Exception)
                        {
                            Console.Clear();
                            Console.WriteLine("Неверный ввод!");
                            break;
                        }
                    case 4:
                        Console.Clear();
                        Console.WriteLine("Введите id редактируемой записи");
                        int redid;
                        try
                        {
                            redid = Convert.ToInt32(Console.ReadLine());
                            Console.Clear();
                        }
                        catch (Exception)
                        {
                            Console.Clear();
                            Console.WriteLine("Неверный ввод!");
                            break;
                        }
                        if (blist.find(redid))
                        {
                            try
                            {
                                Console.WriteLine("Введите ФИО");
                                n = Console.ReadLine();
                                Console.Clear();
                                Console.WriteLine("Введите дату рождения (в формате ДД.ММ.ГГГГ)");
                                d = Convert.ToDateTime(Console.ReadLine(), System.Globalization.CultureInfo.CreateSpecificCulture("ru-RU"));
                                Console.Clear();
                            }
                            catch (Exception)
                            {
                                Console.Clear();
                                Console.WriteLine("Неверный ввод!");
                                break;
                            }
                            if (!blist.update(redid, n, d)) Console.WriteLine("Ошибка: запись не изменена!");
                            else Console.WriteLine("Запись изменена");
                            break;
                        }
                        else
                        {
                            Console.WriteLine("Записи с таким id не существует");
                            break;
                        }
                    case 5:
                        Console.Clear();
                        Console.WriteLine("Введите id удаляемой записи");
                        int delid;
                        try
                        {
                            delid = Convert.ToInt32(Console.ReadLine());
                            Console.Clear();
                        }
                        catch (Exception)
                        {
                            Console.WriteLine("Неверный ввод!");
                            break;
                        }
                        if (blist.delete(delid)) Console.WriteLine("Запись удалена");
                        else Console.WriteLine("Ошибка: запись не удалена!");
                        break;
                }
                return menu(blist);
            }
            Console.WriteLine(Environment.CurrentDirectory);
            BirthList list = new BirthList();
            DataTable datatable;
            Console.WriteLine($"\nСегодня ({DateTime.Today.ToString()}) ДР празднуют:");
            datatable = list.GetToday();
            if (datatable.Rows.Count > 0)
            {
                Console.WriteLine("\nID    ФИО\n");
                foreach (DataRow r in datatable.Rows)
                {
                    Console.WriteLine($"{r[0]}  {r[1]}");
                }
            }
            else Console.WriteLine("\nСписок пуст");
            Console.WriteLine("\nБлижайшие ДР:");
            datatable = list.GetSomeNext();
            if (datatable.Rows.Count > 0)
            {
                Console.WriteLine("\nID    ФИО     Дата рождения");
                foreach (DataRow r in datatable.Rows)
                {
                    Console.WriteLine($"{r[0]}  {r[1]}  {r[2]}.{r[3]}.{r[4]}");
                }
            }
            else Console.WriteLine("\nСписок пуст");
            menu(list);
        }
    }
}
