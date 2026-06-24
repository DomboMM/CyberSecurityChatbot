using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using CyberSecurityChatbotGUI.Models;

namespace CyberSecurityChatbotGUI.Services
{
    public class DatabaseHelper
    {
        // ⚠️ CHANGE THIS PASSWORD TO YOUR MYSQL PASSWORD ⚠️
        private const string Server = "localhost";
        private const string Database = "cybersecurity_chatbot";
        private const string UserId = "root";
        private const string Password = "Mpho@2006";

        private string ConnectionString =>
            $"Server={Server};Database={Database};Uid={UserId};Pwd={Password};";

        public bool TestConnection(out string errorMessage)
        {
            errorMessage = "";
            try
            {
                using (var conn = new MySqlConnection(ConnectionString))
                {
                    conn.Open();
                    return true;
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return false;
            }
        }

        public int AddTask(string title, string description, DateTime? reminderDate)
        {
            using (var conn = new MySqlConnection(ConnectionString))
            {
                conn.Open();
                string sql = @"INSERT INTO tasks (Title, Description, ReminderDate, IsCompleted)
                               VALUES (@title, @desc, @reminder, FALSE);
                               SELECT LAST_INSERT_ID();";

                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@title", title);
                    cmd.Parameters.AddWithValue("@desc", description ?? "");
                    cmd.Parameters.AddWithValue("@reminder", reminderDate ?? (object)DBNull.Value);

                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }

        public List<TaskItem> GetAllTasks()
        {
            var tasks = new List<TaskItem>();

            using (var conn = new MySqlConnection(ConnectionString))
            {
                conn.Open();
                string sql = "SELECT * FROM tasks ORDER BY IsCompleted ASC, CreatedAt DESC;";

                using (var cmd = new MySqlCommand(sql, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        tasks.Add(new TaskItem
                        {
                            TaskId = reader.GetInt32("TaskId"),
                            Title = reader.GetString("Title"),
                            Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? "" : reader.GetString("Description"),
                            ReminderDate = reader.IsDBNull(reader.GetOrdinal("ReminderDate")) ? (DateTime?)null : reader.GetDateTime("ReminderDate"),
                            IsCompleted = reader.GetBoolean("IsCompleted"),
                            CreatedAt = reader.GetDateTime("CreatedAt")
                        });
                    }
                }
            }

            return tasks;
        }

        public void MarkTaskCompleted(int taskId)
        {
            using (var conn = new MySqlConnection(ConnectionString))
            {
                conn.Open();
                string sql = "UPDATE tasks SET IsCompleted = TRUE WHERE TaskId = @id;";

                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", taskId);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void DeleteTask(int taskId)
        {
            using (var conn = new MySqlConnection(ConnectionString))
            {
                conn.Open();
                string sql = "DELETE FROM tasks WHERE TaskId = @id;";

                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", taskId);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void AddLogEntry(string description, string category)
        {
            using (var conn = new MySqlConnection(ConnectionString))
            {
                conn.Open();
                string sql = @"INSERT INTO activity_log (Description, Category)
                               VALUES (@desc, @category);";

                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@desc", description);
                    cmd.Parameters.AddWithValue("@category", category);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public List<ActivityLogEntry> GetRecentLogs(int count = 10)
        {
            var logs = new List<ActivityLogEntry>();

            using (var conn = new MySqlConnection(ConnectionString))
            {
                conn.Open();
                string sql = "SELECT * FROM activity_log ORDER BY Timestamp DESC LIMIT @count;";

                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@count", count);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            logs.Add(new ActivityLogEntry
                            {
                                LogId = reader.GetInt32("LogId"),
                                Description = reader.GetString("Description"),
                                Category = reader.GetString("Category"),
                                Timestamp = reader.GetDateTime("Timestamp")
                            });
                        }
                    }
                }
            }

            return logs;
        }
    }
}
