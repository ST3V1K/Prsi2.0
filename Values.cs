using Npgsql;
using System.Threading;

namespace Prsi
{
    public static class Values
    {
        public enum Players
        {
            Player, Opponent, None
        }

        internal const string CONNECTION_STRING = "User Id=hrac; Password=heslo; Server=db.udfszyxwuvjzxhptzout.supabase.co; Port=5432; Database=postgres; SSL Mode=Require; Trust Server Certificate=true";
        private static string? playerName;
        private static string? playerPassword;

        public static NpgsqlConnection? Connection { get; set; }

        public static NpgsqlConnection? Connection_Listen { get; set; }

        private static readonly Game game = new();

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

        public static Game Game { get => game; }

        public static int MOVE_LENGTH { get => 30; }

        public static Card[] Cards
        {
            get => new Card[]
            {
                new("s7", Properties.Resources.s7),
                new("s8", Properties.Resources.s8),
                new("s9", Properties.Resources.s9),
                new("s10", Properties.Resources.s10),
                new("s11", Properties.Resources.s11),
                new("s12", Properties.Resources.s12),
                new("s13", Properties.Resources.s13),
                new("s14", Properties.Resources.s14),

                new("z7", Properties.Resources.z7),
                new("z8", Properties.Resources.z8),
                new("z9", Properties.Resources.z9),
                new("z10", Properties.Resources.z10),
                new("z11", Properties.Resources.z11),
                new("z12", Properties.Resources.z12),
                new("z13", Properties.Resources.z13),
                new("z14", Properties.Resources.z14),

                new("l7", Properties.Resources.l7),
                new("l8", Properties.Resources.l8),
                new("l9", Properties.Resources.l9),
                new("l10", Properties.Resources.l10),
                new("l11", Properties.Resources.l11),
                new("l12", Properties.Resources.l12),
                new("l13", Properties.Resources.l13),
                new("l14", Properties.Resources.l14),

                new("k7", Properties.Resources.k7),
                new("k8", Properties.Resources.k8),
                new("k9", Properties.Resources.k9),
                new("k10", Properties.Resources.k10),
                new("k11", Properties.Resources.k11),
                new("k12", Properties.Resources.k12),
                new("k13", Properties.Resources.k13),
                new("k14", Properties.Resources.k14),
            };
        }

        public static Card CardBack { get => new("zdk", Properties.Resources.zdk); }

        private readonly static MainMenu mainMenu = new();
        public static MainMenu GetMainMenu() {
            mainMenu.SetName();
            return mainMenu; 
        }
    }
}
