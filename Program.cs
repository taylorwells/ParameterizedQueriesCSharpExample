using System;
using Microsoft.Data.Sqlite;
using System.IO;

namespace SQLInjection
{
    /// <summary>
    /// This is the MacOS version of the files.
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            // This path needs to be an absolute path (preferred) OR
            // a path relative to the compiled EXECUTABLE (exe) file.
            // This path CANNOT be relative to the program.cs file (like in Python or JS)
            // THe 
            string pathToDbFile = @"<PATH TO SQLITE DB FILE>/WebSecurity.db"; // <== Replace <PATH TO SQLITE DB FILE> to the path of the file on your computer
            string connectionString = "Data Source=" + pathToDbFile;
            if (!File.Exists(pathToDbFile))
            {
                throw new FileNotFoundException("File path is incorrect.");
            }
            else
            {
                Console.WriteLine("File path is correct.");
            }
            SqliteConnection conn = new SqliteConnection(connectionString);
  
            SusceptibleToSQLi(conn);
            //NotSusceptibleToSQLi(conn);
            //FixSQLi(conn);
        }

        public static void SusceptibleToSQLi(SqliteConnection conn)
        {
            try
            {
                conn.Open();
            }
            catch
            {
                Console.WriteLine("Problem connecting to database file.");
            }

            SqliteCommand cmd = conn.CreateCommand();

            string expectedInput = "CA"; // This input is the two letter state code and it an expected input from users of our WebApp
            string maliciousInput = "CA' UNION SELECT ingredients from SecretRecipes;--"; // This input is designed to perform SQL injection. Basically it takes the expected input and adds additional SQL code to get information from another table in our DB.

            string state = expectedInput;
            //state = maliciousInput; // <---- UNCOMMENT LINE TO PERFORM SQLi ATTACK using malicious input

            cmd.CommandText = "SELECT BrandName FROM Competition WHERE State='" + state + "'";

            SqliteDataReader sqlDR = cmd.ExecuteReader();

            while (sqlDR.Read())
            {
                Console.WriteLine(sqlDR.GetString(0));
            }

            conn.Close();
        }

        public static void NotSusceptibleToSQLi(SqliteConnection conn)
        {
            try
            {
                conn.Open();
            }
            catch
            {
                Console.WriteLine("Problem connecting to database file.");
            }

            SqliteCommand cmd = conn.CreateCommand();

            string expectedInput = "CA"; // This input is the two letter state code and it an expected input from users of our WebApp
            string maliciousInput = "CA' UNION SELECT ingredients from SecretRecipes;--"; // This input is designed to perform SQL injection. Basically it takes the expected input and adds additional SQL code to get information from another table in our DB.

            string state = expectedInput;
            //state = maliciousInput; // <---- UNCOMMENT LINE TO PERFORM SQLi ATTACK using malicious input

            cmd.CommandText = "SELECT BrandName FROM Competition WHERE State=@state";
            cmd.Parameters.AddWithValue("@state", state);

            SqliteDataReader sqlDR = cmd.ExecuteReader();

            while (sqlDR.Read())
            {
                Console.WriteLine(sqlDR.GetString(0));
            }

            conn.Close();
        }

        public static void FixSQLi(SqliteConnection conn)
        {
            try
            {
                conn.Open();
            }
            catch
            {
                Console.WriteLine("Problem connecting to database file.");
            }

            SqliteCommand cmd = conn.CreateCommand();

            string expectedDescription = "%" + "origin%"; // We want to search for descriptions that have origin in them (origin, original, originally, etc.). This is our wildcard string. The concatenation is there because percentage symbols need to be quoted or escaped in some 
            string expectedCaneSugar = "1"; // SQLite doesn't have true Booleans (just 1 or 0 ints), so this is looking for brands that use cane sugar as a sweetener
            string maliciousCaneSugar = "1 UNION SELECT ingredients from SecretRecipes;--"; // This input is designed to perform SQL injection. Basically it takes the expected cane sugar input and adds additional SQL code to get information from another table in our DB.

            string sugar = expectedCaneSugar;
            //sugar = maliciousCaneSugar; // <---- UNCOMMENT LINE TO PERFORM SQLi ATTACK using malicious input
            cmd.CommandText = "SELECT BrandName from Competition WHERE Description LIKE '" + expectedDescription + "' and CaneSugar=" + sugar;

            SqliteDataReader sqlDR = cmd.ExecuteReader();

            while (sqlDR.Read())
            {
                Console.WriteLine(sqlDR.GetString(0));
            }

            conn.Close();
        }
    }
}
