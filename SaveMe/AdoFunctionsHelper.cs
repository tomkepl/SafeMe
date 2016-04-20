using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Mono.Data.Sqlite;

namespace SaveMe
{
    public static class AdoFunctionsHelper
    {
        public static async void CreateDatabase(Context context)
        {
            var docsFolder = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;
            var pathToDatabase = Path.Combine(docsFolder, "db_tomke.db");

            try
            {
                if (File.Exists(pathToDatabase) == false)
                {
                    SqliteConnection.CreateFile(pathToDatabase);

                    var connectionString = $"Data Source={pathToDatabase};Version=3;";
                    try
                    {
                        using (var conn = new SqliteConnection((connectionString)))
                        {
                            await conn.OpenAsync();
                            using (var command = conn.CreateCommand())
                            {
                                command.CommandText =
                                    "CREATE TABLE Log (Id INTEGER PRIMARY KEY AUTOINCREMENT, Sensor ntext, Value ntext, Time ntext)";
                                command.CommandType = CommandType.Text;
                                await command.ExecuteNonQueryAsync();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        var reason = $"Failed to insert into the database - reason = {ex.Message}";
                        Toast.MakeText(context, reason, ToastLength.Long).Show();
                    }
                }
                else
                {
                    Toast.MakeText(context, "DB_TOMKE exist", ToastLength.Long).Show();
                }
            }
            catch (IOException ex)
            {
                var reason = $"Unable to create the database - reason = {ex.Message}";
                Toast.MakeText(context, reason, ToastLength.Long).Show();
            }
        }

        public static async Task InsertIntoDb(string value, string time, string sensor, OkFlag dbok)
        {
            var docsFolder = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;
            var pathToDatabase = Path.Combine(docsFolder, "db_tomke.db");

            if (dbok.Ok == true || File.Exists(pathToDatabase))
            {
                dbok.Ok = true;
                var connectionString = $"Data Source={pathToDatabase};Version=3;";
                try
                {
                    using (var conn = new SqliteConnection((connectionString)))
                    {
                        await conn.OpenAsync();
                        using (var command = conn.CreateCommand())
                        {
                            command.CommandText =
                                $"INSERT INTO Log (Sensor,Value,Time) VALUES('{sensor}', '{value}', '{time}')";
                            command.CommandType = CommandType.Text;
                            await command.ExecuteNonQueryAsync();
                        }
                    }
                }
                catch (Exception ex)
                {
                    ;
                }
            }
        }

    }

    public class OkFlag
    {
        public bool Ok { get; set; }
    }
}