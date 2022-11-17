using Npgsql;
using System.Threading;

namespace Prsi
{
    public static class Values
    {
        internal const string CONNECTION_STRING = "User Id = hrac; Password = heslo; Server = db.udfszyxwuvjzxhptzout.supabase.co; Port = 5432; Database = postgres";
        private static string? playerName;
        private static string? playerPassword;

        public static NpgsqlConnection? Connection { get; set; }

        public static NpgsqlConnection? Connection_Listen { get; set; }

        public static string PlayerName
        {
            get
            {
                if (playerName != null)
                    return playerName;
                throw new NoNameException();
            }
            set => playerName = value;
        }

        public static string PlayerPassword
        {
            get
            {
                if (playerPassword != null)
                    return playerPassword;
                throw new NoNameException();
            }
            set => playerPassword = value;
        }

        public static string Channel { get => $"{PlayerName}${PlayerPassword}"; }

        public static bool IsFormClosed { get; set; }

        public static CancellationToken FormClosedToken { get => new(IsFormClosed); }

        public static int GameSeed { get; set; }

        public static Game? Game { get; set; }

        public static int MOVE_LENGTH { get => 30; }
    }
}
