﻿using WebSocketSharp;

namespace Backend.Models.Users
{
    public static class UserManager
    {
        public static List<User> UsersList { get; set; } = new List<User>();

        // Infractions
        public static void Mute(User user) { }
        public static void Unmute(User user) { }
        public static void Kick(User user) { }
        public static void Ban(User user) { }
        public static void Unban(User user) { }
        public static void BanIp(string ip) { }
        public static void UnbanIp(string ip) { }

        // Connection
        public static User Connect(WebSocket socket, string ip)
        {
            User newUser = new User(socket, ip);
            newUser.SaveToDb();
            UsersList.Add(newUser);
            return newUser;
        }

        public static bool IsUserAdmin(User user) 
        {
            return user.Ip == "127.0.0.1";
        }
        public static User? GetUserByUsername(string username) { return null; }
        public static void Authenticate(WebSocket socket, string username) { }
        public static void Disconnect(User user) { }
        private static User? GetUserBySocket(WebSocket socket) { return null; }
    }
}
