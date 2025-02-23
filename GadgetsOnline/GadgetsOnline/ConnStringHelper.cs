using System;
using System.Configuration;
using System.Diagnostics;

namespace GadgetsOnline
{
    public static class ConnStringHelper
    {
        public static string GetConnectionString()
        {
            var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");
            if (string.IsNullOrEmpty(connectionString))
            {
                Console.WriteLine("Defaulting back to Web.Config Conn string");
                Debug.WriteLine("Defaulting back to Web.Config Conn string");
                // Fallback to Web.config if environment variable is not set
                connectionString = "Server=52.87.215.223;Database=gadgetsonlinedb;User Id=sa;Password=Dv55569998dv@;TrustServerCertificate=True;Connection Timeout=60;";
                return connectionString;
            }
            Console.WriteLine("Getting Conn from Env Variable.");
            Debug.WriteLine("Getting Conn from Env Variable.");
            return connectionString;
        }
    }
}
