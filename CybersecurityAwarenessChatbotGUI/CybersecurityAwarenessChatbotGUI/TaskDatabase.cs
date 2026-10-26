using System;
using MySql.Data.MySqlClient;
using System.Collections.Generic;

namespace CybersecurityAwarenessChatbotGUI
{
    public class TaskDatabase
    {
        private string connectionString = "Server=localhost;Database=cybersecurity_db;Uid=root;Pwd=Kozo@201607L;";

        // methods for adding, viewing, completing, and deleting tasks will go here
        public void AddTask(string title, string description, DateTime? reminder)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                string query = "INSERT INTO tasks (Title, Description, Reminder) VALUES (@title, @description, @reminder)";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@title", title);
                    cmd.Parameters.AddWithValue("@description", description);
                    cmd.Parameters.AddWithValue("@reminder", (object)reminder ?? DBNull.Value);

                    cmd.ExecuteNonQuery();
                }
            }
        }


                    // Reads every row from the tasks table and returns them as a list
                    // of CyberTask objects, so the rest of the program can work with
                    // them as normal C# objects instead of raw database rows.
public List<CyberTask> GetAllTasks()
        {
            // This list will hold all the tasks we read from the database.
            List<CyberTask> tasks = new List<CyberTask>();

            // "using" ensures the connection is properly closed afterwards,
            // even if something goes wrong while reading.
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                // The SQL query: get every column, for every row, in tasks.
                string query = "SELECT TaskId, Title, Description, Reminder, IsCompleted FROM tasks";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                // ExecuteReader() is used (instead of ExecuteNonQuery) because
                // we're READING data back, not just running an action like INSERT.
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    // reader.Read() moves to the next row each time it's called,
                    // and returns false once there are no more rows left.
                    while (reader.Read())
                    {
                        // Build one CyberTask object from the current row.
                        CyberTask task = new CyberTask
                        {
                            TaskId = reader.GetInt32("TaskId"),
                            Title = reader.GetString("Title"),

                            // Description might be NULL in the database (it's optional),
                            // so we check for that before trying to read it as a string.
                            Description = reader.IsDBNull(reader.GetOrdinal("Description"))
                                ? ""
                                : reader.GetString("Description"),

                            // Same idea for Reminder — it's optional, so it might be NULL.
                            Reminder = reader.IsDBNull(reader.GetOrdinal("Reminder"))
                                ? (DateTime?)null
                                : reader.GetDateTime("Reminder"),

                            IsCompleted = reader.GetBoolean("IsCompleted")
                        };

                        // Add this task to our list before moving to the next row.
                        tasks.Add(task);
                    }
                }
            }

            // Hand back the full list of tasks once we've read every row.
            return tasks;
        }
    }
}