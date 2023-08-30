using Prsi.Server;
using System;
using System.Threading;
using static Prsi.Server.GameService;
using static Prsi.Server.PlayerService;

namespace Prsi
{
    public static class Values
    {
        public enum Players
        {
            Player, Opponent, None
        }

        internal const string SERVER_ADDRESS = "http://10.0.0.3:5001";
        private static string? playerName;
        private static string? playerPassword;

        private static GameServiceClient? gameClient;
        private static PlayerServiceClient? playerClient;

        internal static GameServiceClient GameClient
        {
            get
            {
                if (gameClient is not null)
                    return gameClient;
                throw new();
            }
            set => gameClient = value;
        }
        internal static PlayerServiceClient PlayerClient
        {
            get
            {
                if (playerClient is not null)
                    return playerClient;
                throw new();
            }
            set => playerClient = value;
        }

        private static readonly Game game = new();

        public static string PlayerName
        {

            get
            {
                if (playerName != null)
                    return playerName;
                return string.Empty;
            }
            set => playerName = value;
        }

        public static string PlayerPassword
        {
            get
            {
                if (playerPassword != null)
                    return playerPassword;
                return string.Empty;
            }
            set => playerPassword = value;
        }

        public static Player ServerPlayer
        {
            get => new()
            {
                Name = PlayerName,
                Password = PlayerPassword
            };
        }

        public static Server.Game ServerGame
        {
            get => new()
            {
                Uuid = GameUuid.ToString()
            };
        }

        internal static DateTime Deadline
        {
            get => DateTime.UtcNow.AddSeconds(5);
        }

        internal static Guid? GameUuid;

        public static bool IsFormClosed { get; set; }

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
