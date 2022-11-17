using Npgsql;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Prsi
{
    /// <summary>
    /// Interakční logika pro Game.xaml
    /// </summary>
    public partial class Game : UserControl
    {
        public string? LastPlayed { get; private set; }

        private Random? r = null;

        private readonly List<Card> thrownOut = new();
        private readonly List<Card> cards = new();
        private List<Card> deck = new();

        public Game()
        {
            InitializeComponent();
        }

        public void StartGame(int seed, string opponentName, bool host)
        {
            r = new(seed);
            Dispatcher.Invoke(() => txtNames.Text = $"{Values.PlayerName}\n×\n{opponentName}");

            deck = ((Card[])Values.Cards.Clone()).ToList();
            cards.Clear();

            for (int i = 0; i < 4; i++)
            {
                if (host)
                {
                    DrawCard();
                    DrawCardForEnemy();
                }
                else
                {
                    DrawCardForEnemy();
                    DrawCard();
                }
            }
        }

        public void VisualizeCards()
        {
            Dispatcher.Invoke(() =>
            {
                grid.Children.Clear();
                for (int i = 0; i < cards.Count; i++)
                {
                    double angle = i * 180 / cards.Count + 90 / cards.Count - 90;

                    var transformGroup = new TransformGroup();

                    transformGroup.Children.Add(new RotateTransform(angle));

                    CardControl cardControl = new(cards[i], i + 1, angle)
                    {
                        Width = 120,
                        Height = 200,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Bottom,
                        RenderTransformOrigin = new Point(.5, 1),
                        RenderTransform = transformGroup
                    };

                    grid.Children.Add(cardControl);
                    Panel.SetZIndex(cardControl, cardControl.Index);
                }
            });
        }

        public void DrawCard()
        {
            if (r == null)
                return;

            int index = r.Next(deck.Count);
            Card card = deck[index];

            if (deck.Count == 0)
                NewDeck();
            deck.RemoveAt(index);

            cards.Add(card);

            VisualizeCards();
        }

        public void DrawCardForEnemy()
        {
            if (r == null)
                return;

            int index = r.Next(deck.Count);

            if (deck.Count == 0)
                NewDeck();
            deck.RemoveAt(index);

        }

        public async void PlayCard(Card card)
        {
            using NpgsqlCommand cmd = new("select tahni(@jmenoin, @hesloin, @tahin)", Values.Connection);
            cmd.Parameters.AddWithValue("@jmenoin", Values.PlayerName);
            cmd.Parameters.AddWithValue("@hesloin", Values.PlayerPassword);
            cmd.Parameters.AddWithValue("@tahin", card.Name);
            await cmd.ExecuteNonQueryAsync();

            cards.Remove(card);
            thrownOut.Add(card);
            LastPlayed = card.Name;
            VisualizeCards();
        }

        public void RemoveCardFromDeck(string name)
        {
            if (deck.Count == 0)
                NewDeck();
            deck.RemoveAll(c =>
            {
                thrownOut.Add(c);
                return c.Name == name;
            }
            );
        }

        private void NewDeck() => deck = thrownOut.GetRange(0, thrownOut.Count);
    }
}
