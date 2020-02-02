using System;
using System.Collections.Generic;
using TwitchBot.Commands.Permissions;
using TwitchBot.Commands;
using Microsoft.Data.Sqlite;

namespace TwitchBot
{
    /// <summary>
    /// Database object holds a connection open to a SQL database for synchronizing the permissions, commands, broadcasts, etc
    /// Note that we assume that no 2 instances of the bot with different name are on the same channel
    /// Some queries use only the channel name and not the botname
    /// </summary>
    internal class Database
    {
        private SqliteConnection dbConnection;
        private static readonly string CHANNELS_TABLE = "channels";
        private static readonly string BASIC_COMMANDS_TABLE = "basiccommands";
        private static readonly string PERMISSIONS_TABLE = "permissions";
        private static readonly string BROADCASTS_TABLE = "broadcasts";

        /// <summary>
        /// Initializes the Database
        /// </summary>
        public Database()
        {
            dbConnection = new SqliteConnection("Data Source=Database.sqlite");
            dbConnection.Open();
            InitDatabase();
        }

        /// <summary>
        /// Creates the tables to the SQL database
        /// </summary>
        private void InitDatabase()
        {
            string basic       = $"CREATE TABLE {BASIC_COMMANDS_TABLE} (channel VARCHAR(26), name VARHCHAR(32), response VARHCHAR(256), botname VARCHAR(32) )";
            string broadcast   = $"CREATE TABLE {BROADCASTS_TABLE} (channel VARHCHAR(26), delay INT, id INT, message VARHCHAR(512), botname VARCHAR(32) )";
            string permissions = $"CREATE TABLE {PERMISSIONS_TABLE} (channel VARHCHAR(26), permission INT, name VARHCHAR(26), botname VARCHAR(32) )";
            string channels    = $"CREATE TABLE {CHANNELS_TABLE} (name VARHCHAR(26), botname VARCHAR(32) )";

            try
            {
                new SqliteCommand(basic, dbConnection).ExecuteNonQuery();
                new SqliteCommand(broadcast, dbConnection).ExecuteNonQuery();
                new SqliteCommand(permissions, dbConnection).ExecuteNonQuery();
                new SqliteCommand(channels, dbConnection).ExecuteNonQuery();
            } catch(SqliteException)
            {

            }
        }

        /// <summary>
        /// Retrieves complete list of channels this bot is active on
        /// </summary>
        /// <param name="botname">Bot name to query joined channels for</param>
        /// <returns>List of channel names</returns>
        public List<string> QueryChannels(string botname)
        {
            List<string> channels = new List<string>();
            var query = new SqliteCommand($"SELECT * FROM {CHANNELS_TABLE} WHERE botname = @Botname", dbConnection);
            query.Parameters.AddWithValue("Botname", botname);
            SqliteDataReader reader = query.ExecuteReader();
            while (reader.Read())
            {
                channels.Add(reader["name"].ToString());
            }
            return channels;
        }

        /// <summary>
        /// Adds a new channel to the database
        /// </summary>
        /// <param name="name">Channel name</param>
        /// <returns>True if channel was added, false if failed</returns>
        public bool AddChannel(ChannelName name, string botname)
        {
            try
            {
                string command = $"INSERT into {CHANNELS_TABLE} (name, botname) values (@Name, @Botname)";
                var sqlCommand = new SqliteCommand(command, dbConnection);
                sqlCommand.Parameters.AddWithValue("Name", name.ToString());
                sqlCommand.Parameters.AddWithValue("Botname", botname);
                return sqlCommand.ExecuteNonQuery() > 0;
            }
            catch (SqliteException)
            {
                return false;
            }
        }

        /// <summary>
        /// Deletes all entries in the database for the given channel name
        /// </summary>
        /// <param name="name">Channel to remove all entries for</param>
        /// <returns>True if ssuccessfull, false if failed</returns>
        public bool RemoveChannel(ChannelName name, string botname)
        {
            string deleteChannel    = $"DELETE FROM {CHANNELS_TABLE} WHERE name = @Name AND botname = @Botname";
            string deletePermission = $"DELETE FROM {PERMISSIONS_TABLE} WHERE channel = @Channe AND botname = @Botname";
            string deleteCommand    = $"DELETE FROM {BASIC_COMMANDS_TABLE} WHERE channel = @Channel AND botname = @Botname";
            string deleteBroadcast  = $"DELETE FROM {BROADCASTS_TABLE} WHERE channel = @Channel AND botname = @Botname";

            var sqlDeleteChannel = new SqliteCommand(deleteChannel, dbConnection);
            var sqlDeletePermission = new SqliteCommand(deletePermission, dbConnection);
            var sqlDeleteCommand = new SqliteCommand(deleteCommand, dbConnection);
            var sqlDeleteBroadcast = new SqliteCommand(deleteBroadcast, dbConnection);

            sqlDeleteChannel.Parameters.AddWithValue("Botname", botname);
            sqlDeleteChannel.Parameters.AddWithValue("Name", name.ToString());
            sqlDeletePermission.Parameters.AddWithValue("Channel", name.ToString());
            sqlDeletePermission.Parameters.AddWithValue("Botname", botname);
            sqlDeleteCommand.Parameters.AddWithValue("Channel", name.ToString());
            sqlDeleteCommand.Parameters.AddWithValue("Botname", botname);
            sqlDeleteBroadcast.Parameters.AddWithValue("Channel", name.ToString());
            sqlDeleteBroadcast.Parameters.AddWithValue("Botname", botname);

            bool deletedEntries = false;
            deletedEntries |= sqlDeleteChannel.ExecuteNonQuery() > 0;
            deletedEntries |= sqlDeletePermission.ExecuteNonQuery() > 0;
            deletedEntries |= sqlDeleteCommand.ExecuteNonQuery() > 0;
            deletedEntries |= sqlDeleteBroadcast.ExecuteNonQuery() > 0;

            return deletedEntries;
        }

        /// <summary>
        /// Removes all entries in the database for the given bot name
        /// </summary>
        /// <param name="botname">Bot name to remove entries for</param>
        /// <returns>True if ssuccessfull, false if failed</returns>
        public bool RemoveBot(string botname)
        {
            string deleteChannel    = $"DELETE FROM {CHANNELS_TABLE} WHERE botname = @Botname";
            string deletePermission = $"DELETE FROM {PERMISSIONS_TABLE} WHERE botname = @Botname";
            string deleteCommand    = $"DELETE FROM {BASIC_COMMANDS_TABLE} WHERE botname = @Botname";
            string deleteBroadcast  = $"DELETE FROM {BROADCASTS_TABLE} WHERE botname = @Botname";

            var sqlDeleteChannel = new SqliteCommand(deleteChannel, dbConnection);
            var sqlDeletePermission = new SqliteCommand(deletePermission, dbConnection);
            var sqlDeleteCommand = new SqliteCommand(deleteCommand, dbConnection);
            var sqlDeleteBroadcast = new SqliteCommand(deleteBroadcast, dbConnection);

            sqlDeleteChannel.Parameters.AddWithValue("Botname", botname);
            sqlDeletePermission.Parameters.AddWithValue("Botname", botname);
            sqlDeleteCommand.Parameters.AddWithValue("Botname", botname);
            sqlDeleteBroadcast.Parameters.AddWithValue("Botname", botname);

            bool deletedEntries = false;
            deletedEntries |= sqlDeleteChannel.ExecuteNonQuery() > 0;
            deletedEntries |= sqlDeletePermission.ExecuteNonQuery() > 0;
            deletedEntries |= sqlDeleteCommand.ExecuteNonQuery() > 0;
            deletedEntries |= sqlDeleteBroadcast.ExecuteNonQuery() > 0;

            return deletedEntries;
        }

        public int QueryPermission(string channel, string name, string botname)
        {
            var query = new SqliteCommand($"SELECT permission FROM {PERMISSIONS_TABLE} WHERE channel = @Channel AND name = @Name AND botname = @Botname", dbConnection);
            query.Parameters.AddWithValue("Channel", channel);
            query.Parameters.AddWithValue("Name", name);
            query.Parameters.AddWithValue("Botname", botname);
            SqliteDataReader reader = query.ExecuteReader();
            if(reader.Read())
            {
                return reader.GetInt32(0);
            }
            // If no permission entry was found that means it defaults to 0 => no permissions
            return 0;
        }

        /// <summary>
        /// Updates a single permission for a user on the given channel
        /// </summary>
        /// <param name="pair">ChannelUserNamePair that contains the user whose permission if to be updated</param>
        /// <param name="permission">Permission level to set</param>
        /// <returns>True if a permission was updated, false if not</returns>
        public bool UpdatePermission(ChannelUsernamePair pair, int permission, string botname)
        {
            if(permission == 0)
            {
                // 0 is default value which means we can delete the entry
                return DeletePermission(pair, botname);
            }
            string commandUpdateLocal  = $"UPDATE {PERMISSIONS_TABLE} SET permission = @Permission WHERE channel = @Channel AND name = @Name AND botname = @Botname";
            string commandUpdateGlobal = $"UPDATE {PERMISSIONS_TABLE} SET permission = @Permission WHERE channel IS NULL AND name = @Name";
            SqliteCommand command;
            if (pair.IsGlobal)
            {
                command = new SqliteCommand(commandUpdateGlobal, dbConnection);
                command.Parameters.AddWithValue("Permission", permission);
                command.Parameters.AddWithValue("Name", pair.Username);
            }
            else
            {
                command = new SqliteCommand(commandUpdateLocal, dbConnection);
                command.Parameters.AddWithValue("Channel", pair.Channel.ToString());
                command.Parameters.AddWithValue("Permission", permission);
                command.Parameters.AddWithValue("Name", pair.Username);
                command.Parameters.AddWithValue("Botname", botname);
            }
            try
            {
                return command.ExecuteNonQuery() > 0;
            }
            catch(SqliteException)
            {
                return false;
            }
        }

        /// <summary>
        /// Adds new permission entry to the database
        /// </summary>
        /// <param name="pair">ChannelUsernamePair to set permission for</param>
        /// <param name="permission">Permission level to set</param>
        /// <returns>True if permission was updated, false if not</returns>
        public bool AddPermission(ChannelUsernamePair pair, int permission, string botname)
        {
            if(permission == 0)
            {
                // Don't add default values
                return DeletePermission(pair, botname);
            }
            string command = $"INSERT into {PERMISSIONS_TABLE} (channel, permission, name, botname) values (@Channel, @Permission, @Name, @Botname)";// ON CONFLICT(channel, name, botname) DO UPDATE SET permission=@Permission";
            var sqlCommand = new SqliteCommand(command, dbConnection);
            sqlCommand.Parameters.AddWithValue("Channel", pair.Channel.ToString());
            sqlCommand.Parameters.AddWithValue("Permission", permission);
            sqlCommand.Parameters.AddWithValue("Name", pair.Username);
            sqlCommand.Parameters.AddWithValue("Botname", botname);
            try
            {
                int modified = sqlCommand.ExecuteNonQuery();
                Console.WriteLine(modified);
                return modified > 0;
            }
            catch (SqliteException e)
            {
                throw e;
                //return false;
            }
        }

        /// <summary>
        /// Deletes the given permission entry from the database
        /// </summary>
        /// <param name="pair">ChannelUsernamePair to be deleted</param>
        /// <returns></returns>
        public bool DeletePermission(ChannelUsernamePair pair, string botname)
        {
            string deleteLocal  = $"DELETE FROM {PERMISSIONS_TABLE} WHERE channel = @Channel AND name = @Name AND botname = @Botname";
            string deleteGlobal = $"DELETE FROM {PERMISSIONS_TABLE} WHERE channel IS NULL AND name = @Name";
            SqliteCommand sqlCommand;
            if (pair.IsGlobal) {
                sqlCommand = new SqliteCommand(deleteGlobal, dbConnection);
                sqlCommand.Parameters.AddWithValue("Name", pair.Username);
            }
            else
            {
                sqlCommand = new SqliteCommand(deleteLocal, dbConnection);
                sqlCommand.Parameters.AddWithValue("Channel", pair.Channel.ToString());
                sqlCommand.Parameters.AddWithValue("Name", pair.Username);
                sqlCommand.Parameters.AddWithValue("Botname", botname);
            }
            return sqlCommand.ExecuteNonQuery() > 0;
        }

        /// <summary>
        /// Queries all basic commands for the give channel
        /// </summary>
        /// <param name="channel">Channel name to query commands for</param>
        /// <returns>Dictionary containing name/response pairs</returns>
        public Dictionary<string, Command> QueryBasicCommands(ChannelName channel, string botname)
        {
            string query = $"SELECT * FROM {BASIC_COMMANDS_TABLE} WHERE channel = @Channel AND botname = @Botname";
            Dictionary<string, Command> basicCommands = new Dictionary<string, Command>();
            var sqlQuery = new SqliteCommand(query, dbConnection);
            sqlQuery.Parameters.AddWithValue("Channel", channel.ToString());
            sqlQuery.Parameters.AddWithValue("Botname", botname);
            SqliteDataReader reader = sqlQuery.ExecuteReader();
            while (reader.Read())
            {
                basicCommands.Add(reader["name"].ToString(), new BasicCommand(reader["name"].ToString(), reader["response"].ToString()));
            }
            return basicCommands;
        }

        /// <summary>
        /// Adds a new basic command
        /// </summary>
        /// <param name="channel">Channel to add the command to</param>
        /// <param name="name">Name of the command without prefix</param>
        /// <param name="response">Response to the command</param>
        /// <returns>True if command was added, false if not</returns>
        public bool AddBasicCommand(ChannelName channel, string name, string response, string botname)
        {
            string insert  = $"INSERT into {BASIC_COMMANDS_TABLE} (channel, name, response, botname) values (@Channel, @Name, @Response, @Botname)";
            var sqlCommand = new SqliteCommand(insert, dbConnection);
            sqlCommand.Parameters.AddWithValue("Channel", channel.ToString());
            sqlCommand.Parameters.AddWithValue("Name", name);
            sqlCommand.Parameters.AddWithValue("Response", response);
            sqlCommand.Parameters.AddWithValue("Botname", botname);
            try
            {
                return sqlCommand.ExecuteNonQuery() > 0;
            }
            catch (SqliteException)
            {
                return false;
            }
        }

        /// <summary>
        /// Updates an existing basic command
        /// </summary>
        /// <param name="channel">Channel name the command belongs to</param>
        /// <param name="name">Name of the command</param>
        /// <param name="response">Response to the command</param>
        /// <returns>True if command was updated, false if not</returns>
        public bool UpdateBasicCommand(ChannelName channel, string name, string response, string botname)
        {
            string updateCommand  = $"UPDATE {BASIC_COMMANDS_TABLE} SET response = @Response WHERE channel = @Channel AND name = @Name AND botname = @Botname";
            SqliteCommand command = new SqliteCommand(updateCommand, dbConnection);
            command.Parameters.AddWithValue("Response", response);
            command.Parameters.AddWithValue("Channel", channel.ToString());
            command.Parameters.AddWithValue("Name", name);
            command.Parameters.AddWithValue("Botname", botname);
            try
            {
                return command.ExecuteNonQuery() > 0;
            }
            catch (SqliteException)
            {
                return false;
            }
        }

        /// <summary>
        /// Removes a basic command from the given channel
        /// </summary>
        /// <param name="channel">Channel the command belongs to</param>
        /// <param name="name">Name of the command</param>
        /// <returns></returns>
        public bool RemoveBasicCommand(ChannelName channel, string name, string botname)
        {
            string updateCommand = $"DELETE FROM {BASIC_COMMANDS_TABLE} WHERE channel = @Channel AND name = @Name AND botname = @Botname";
            SqliteCommand command = new SqliteCommand(updateCommand, dbConnection);
            command.Parameters.AddWithValue("Channel", channel.ToString());
            command.Parameters.AddWithValue("Name", name);
            command.Parameters.AddWithValue("Botname", botname);
            try
            {
                return command.ExecuteNonQuery() > 0;
            }
            catch (SqliteException)
            {
                return false;
            }
        }

        /// <summary>
        /// Closes the SQL database connection
        /// </summary>
        public void Close()
        {
            dbConnection.Close();
        }
    }
}
