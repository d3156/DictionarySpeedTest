using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;

namespace ComputerComponents
{
    public class ManagerDB
    {
        List<string> dictioanry = new();
        public SqlConnection speed1;
        public SqlConnection speed2;

        public ManagerDB()
        {
            
            string connStrSpeed1 = "Data Source=D3156\\SQLEXPRESS;Initial Catalog=speed1;User id=d3156;Password=3156;trustServerCertificate=true";
            string connStrSpeed2 = "Data Source=D3156\\SQLEXPRESS;Initial Catalog=speed2;User id=d3156;Password=3156;trustServerCertificate=true";
            speed1 = new SqlConnection(connStrSpeed1);// Создание подключения
            speed2 = new SqlConnection(connStrSpeed2);// Создание подключения
            try
            {
                speed1.Open();
                speed2.Open();
            }
            catch (SqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        private static Random random = new Random();
        public static string RandomString(int length)
        {
            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public TimeSpan speed1Check()
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            for (int i = 0; i < dictioanry.Count; i++)
            {
                SqlCommand cmd = new SqlCommand($"SELECT * FROM TableMain WHERE v5 = '{dictioanry[i]}'", speed1);
                cmd.ExecuteNonQuery();
            }
            stopWatch.Stop();
            return stopWatch.Elapsed;
        }
        public TimeSpan speed2Check()
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            for (int i = 0; i < dictioanry.Count; i++)
            {
                SqlCommand cmd = new SqlCommand($"SELECT * FROM MainTable WHERE v5 = {i}", speed2);
                cmd.ExecuteNonQuery();
            }
            stopWatch.Stop();
            return stopWatch.Elapsed;
        }
        public void run()
        {
            for (int i = 0; i < 10; i++)
            {
                dictioanry.Insert(i, RandomString(190));
                SqlCommand cmd =  new SqlCommand($"INSERT DictionaryV5(id, v5) VALUES ({i}, '{dictioanry[i]}')", speed2);              
                cmd.ExecuteNonQuery();                
            }

            string res = ";Скорость поиска по таблице;Скорость поиска по таблице со словарем\n"; ;
            for (int i = 0; i < 100001; i++)
            {
                SqlCommand cmd = new SqlCommand($"INSERT TableMain(id,v1,v2,v3,v4,v5) VALUES ({i}, '{RandomString(190)}', '{RandomString(190)}', '{RandomString(190)}', '{RandomString(190)}', '{dictioanry[i % 10]}')", speed1);
                cmd.ExecuteNonQuery();
                cmd = new SqlCommand($"INSERT MainTable(id,v1,v2,v3,v4,v5) VALUES ({i}, '{RandomString(190)}', '{RandomString(190)}', '{RandomString(190)}', '{RandomString(190)}', '{i % 10}')", speed2);
                cmd.ExecuteNonQuery();
                if (i % 100 == 0) res += $"{i};{speed1Check()};{speed2Check()}\n";
            }
            res = res.Replace(".", ",");
            res = res.Replace("00:00:0", "");
            File.WriteAllText("1.csv", res);
        }
    }
}
