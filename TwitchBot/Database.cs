﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SQLite;
using TwitchBot.Commands.Permissions;
using TwitchBot.Commands;

namespace TwitchBot
{
    /// <summary>
    /// Database object holds a connection open to a SQL database for synchronizing the permissions, commands, broadcasts, etc
    /// </summary>
    internal class Database
    {
        private SQLiteConnection dbConnection;

        /// <summary>
        /// Initializes the Database
        /// </summary>
        public Database()
        {
            dbConnection = new SQLiteConnection("Data Source=Database.sqlite;Version=3;");
            dbConnection.Open();
            InitDatabase();
        }

        /// <summary>
        /// Creates the tables to the SQL database
        /// </summary>
        private void InitDatabase()
        {
            string basic = "CREATE TABLE basiccommands (channel VARCHAR(26), name VARHCHAR(32), response VARHCHAR(256) )";
            string broadcast = "CREATE TABLE broadcasts (channel VARHCHAR(26), delay INT, id INT, message VARHCHAR(512) )";
            string permissions = "CREATE TABLE permissions (channel VARHCHAR(26), permission INT, name VARHCHAR(26) )";
            string channels = "CREATE TABLE channels (name VARHCHAR(26) )";

            try
            {
                new SQLiteCommand(basic, dbConnection).ExecuteNonQuery();
                new SQLiteCommand(broadcast, dbConnection).ExecuteNonQuery();
                new SQLiteCommand(permissions, dbConnection).ExecuteNonQuery();
                new SQLiteCommand(channels, dbConnection).ExecuteNonQuery();
            } catch(SQLiteException)
            {

            }
        }

        /// <summary>
        /// Retrieves complete list of channels this bot is active on
        /// </summary>
        /// <returns>List of channel names</returns>
        public List<string> QueryChannels()
        {
            List<string> channels = new List<string>();
            var query = new SQLiteCommand("SELECT * FROM channels", dbConnection);
            SQLiteDataReader reader = query.ExecuteReader();
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
        public bool AddChannel(ChannelName name)
        {
            try
            {
                string command = "INSERT into channels (name) values (@Name)";
                var sqlCommand = new SQLiteCommand(command, dbConnection);
                sqlCommand.Parameters.AddWithValue("Name", name.ToString());
                return sqlCommand.ExecuteNonQuery() > 0;
            }
            catch (SQLiteException)
            {
                return false;
            }
        }

        /// <summary>
        /// Deletes all entries in the database for the given channel name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool RemoveChannel(ChannelName name)
        {
            string deleteChannel = "DELETE FROM channels WHERE name = @Name";
            string deletePermission = "DELETE FROM permissions WHERE channel = @Channel";
            string deleteCommand = "DELETE FROM basiccommands WHERE channel = @Channel";
            string deleteBroadcast = "DELETE FROM broadcasts WHERE channel = @Channel";

            var sqlDeleteChannel = new SQLiteCommand(deleteChannel, dbConnection);
            var sqlDeletePermission = new SQLiteCommand(deletePermission, dbConnection);
            var sqlDeleteCommand = new SQLiteCommand(deleteCommand, dbConnection);
            var sqlDeleteBroadcast = new SQLiteCommand(deleteBroadcast, dbConnection);

            sqlDeleteChannel.Parameters.AddWithValue("Name", name.ToString());
            sqlDeletePermission.Parameters.AddWithValue("Channel", name.ToString());
            sqlDeleteCommand.Parameters.AddWithValue("Channel", name.ToString());
            sqlDeleteBroadcast.Parameters.AddWithValue("Channel", name.ToString());

            bool deletedEntries = false;
            deletedEntries |= sqlDeleteChannel.ExecuteNonQuery() > 0;
            deletedEntries |= sqlDeletePermission.ExecuteNonQuery() > 0;
            deletedEntries |= sqlDeleteCommand.ExecuteNonQuery() > 0;
            deletedEntries |= sqlDeleteBroadcast.ExecuteNonQuery() > 0;

            return deletedEntries;
        }

        /// <summary>
        /// Queries all permissions from the database
        /// </summary>
        /// <returns>Dictionary that contains all permissions</returns>
        public Dictionary<ChannelUsernamePair, int> QueryPermissions()
        {
            Dictionary<ChannelUsernamePair, int> permissions = new Dictionary<ChannelUsernamePair, int>();
            var query = new SQLiteCommand("SELECT * FROM permissions", dbConnection);
            SQLiteDataReader reader = query.ExecuteReader();
            while (reader.Read())
            {
                if(reader["channel"] == null)
                    permissions.Add(new ChannelUsernamePair(null, reader["name"].ToString(), true), (int) reader["permissions"]);
                else
                    permissions.Add(new ChannelUsernamePair(new ChannelName(reader["channel"].ToString()), reader["name"].ToString(), false), (int) reader["permission"]);
                Console.WriteLine("Channel: " + reader["channel"] + "\nName: " + reader["name"] + "\npermission: " + reader["permission"]);
            }
            return permissions;
        }

        /// <summary>
        /// Updates a single permission for a user on the given channel
        /// </summary>
        /// <param name="pair">ChannelUserNamePair that contains the user whose permission if to be updated</param>
        /// <param name="permission">Permission level to set</param>
        /// <returns>True if a permission was updated, false if not</returns>
        public bool UpdatePermission(ChannelUsernamePair pair, int permission)
        {
            string commandUpdateLocal = "UPDATE permissions SET permission = @Permission WHERE channel = @Channel AND name = @Name";
            string commandUpdateGlobal = "UPDATE permissions SET permission = @Permission WHERE channel IS NULL AND name = @Name";
            SQLiteCommand command;
            if (pair.IsGlobal)
            {
                command = new SQLiteCommand(commandUpdateGlobal, dbConnection);
                command.Parameters.AddWithValue("Permission", permission);
                command.Parameters.AddWithValue("Name", pair.Username);
            }
            else
            {
                command = new SQLiteCommand(commandUpdateLocal, dbConnection);
                command.Parameters.AddWithValue("Channel", pair.Channel.ToString());
                command.Parameters.AddWithValue("Permission", permission);
                command.Parameters.AddWithValue("Name", pair.Username);
            }
            try
            {
                return command.ExecuteNonQuery() > 0;
            }
            catch(SQLiteException)
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
        public bool AddPermission(ChannelUsernamePair pair, int permission)
        {
            string command = "INSERT into permissions (channel, permission, name) values (@Channel, @Permission, @Name)";
            var sqlCommand = new SQLiteCommand(command, dbConnection);
            sqlCommand.Parameters.AddWithValue("Channel", pair.Channel.ToString());
            sqlCommand.Parameters.AddWithValue("Permission", permission);
            sqlCommand.Parameters.AddWithValue("Name", pair.Username);
            try
            {
                return sqlCommand.ExecuteNonQuery() > 0;
            }
            catch (SQLiteException)
            {
                return false;
            }
        }

        /// <summary>
        /// Deletes the given permission entry from the database
        /// </summary>
        /// <param name="pair">ChannelUsernamePair to be deleted</param>
        /// <returns></returns>
        public bool DeletePermission(ChannelUsernamePair pair)
        {
            string deleteLocal = "DELETE FROM permission WHERE channel = @Channel AND name = @Name";
            string deleteGlobal = "DELETE FROM permission WHERE channel IS NULL AND name = @Name";
            SQLiteCommand sqlCommand;
            if (pair.IsGlobal) {
                sqlCommand = new SQLiteCommand(deleteLocal, dbConnection);
                sqlCommand.Parameters.AddWithValue("Name", pair.Username);
            }
            else
            {
                sqlCommand = new SQLiteCommand(deleteGlobal, dbConnection);
                sqlCommand.Parameters.AddWithValue("Channel", pair.Channel.ToString());
                sqlCommand.Parameters.AddWithValue("Name", pair.Username);
            }
            return sqlCommand.ExecuteNonQuery() > 0;
        }

        /// <summary>
        /// Queries all basic commands for the give channel
        /// </summary>
        /// <param name="channel">Channel name to query commands for</param>
        /// <returns>Dictionary containing name/response pairs</returns>
        public Dictionary<string, Command> QueryBasicCommands(ChannelName channel)
        {
            string query = "SELECT * FROM basiccommands WHERE channel = @Channel";
            Dictionary<string, Command> basicCommands = new Dictionary<string, Command>();
            var sqlQuery = new SQLiteCommand(query, dbConnection);
            sqlQuery.Parameters.AddWithValue("Channel", channel.ToString());
            SQLiteDataReader reader = sqlQuery.ExecuteReader();
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
        public bool AddBasicCommand(ChannelName channel, string name, string response)
        {
            string insert = "INSERT into basiccommands (channel, name, response) values (@Channel, @Name, @Response)";
            var sqlCommand = new SQLiteCommand(insert, dbConnection);
            sqlCommand.Parameters.AddWithValue("Channel", channel.ToString());
            sqlCommand.Parameters.AddWithValue("Name", name);
            sqlCommand.Parameters.AddWithValue("Response", response);
            try
            {
                return sqlCommand.ExecuteNonQuery() > 0;
            }
            catch (SQLiteException)
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
        public bool UpdateBasicCommand(ChannelName channel, string name, string response)
        {
            string updateCommand = "UPDATE basiccommands SET response = @Response WHERE channel = @Channel AND name = @Name";
            SQLiteCommand  command = new SQLiteCommand(updateCommand, dbConnection);
            command.Parameters.AddWithValue("Response", response);
            command.Parameters.AddWithValue("Channel", channel.ToString());
            command.Parameters.AddWithValue("Name", name);
            try
            {
                return command.ExecuteNonQuery() > 0;
            }
            catch (SQLiteException)
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
        public bool RemoveBasicCommand(ChannelName channel, string name)
        {
            string updateCommand = "DELETE FROM basiccommands WHERE channel = @Channel AND name = @Name";
            SQLiteCommand command = new SQLiteCommand(updateCommand, dbConnection);
            command.Parameters.AddWithValue("Channel", channel.ToString());
            command.Parameters.AddWithValue("Name", name);
            try
            {
                return command.ExecuteNonQuery() > 0;
            }
            catch (SQLiteException)
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