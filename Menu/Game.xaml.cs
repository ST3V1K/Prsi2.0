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
        private Random? r = null;

        public int Seed { get; private set; }

        public List<Card> cards = new();
        private List<Card> deck = new();
        private List<Card> ec = new();
        public Game()
        {
            InitializeComponent();
        }

        public void StartGame(int seed, string opponentName, bool host)
        {
            Seed = seed;
            r = new(seed);
            Dispatcher.Invoke(() => txtNames.Text = $"{Values.PlayerName}\n×\n{opponentName}");

            deck = ((Card[])Values.Cards.Clone()).ToList();

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
            string a = "";
            foreach (Card c in ec) 
            {
                a += c.Name + " ";
            }
            MessageBox.Show(a);
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
            int index = r.Next(deck.Count);
            Card card = deck[index];

            if (deck.Count == 0)
                deck = (List<Card>)Values.Cards.Clone();
            deck.RemoveAt(index);

            cards.Add(card);

            VisualizeCards();
        }

        public void DrawCardForEnemy()
        {
            int index = r.Next(deck.Count);
            ec.Add(deck[index]);
            if (deck.Count == 0)
                deck = (List<Card>)Values.Cards.Clone();
            deck.RemoveAt(index);

        }

        public void PlayCard(Card card)
        {
            cards.Remove(card);
            VisualizeCards();
        }

        public void RemoveCardFromDeck(string name)
        {
            if (deck.Count == 0)
                deck = (List<Card>)Values.Cards.Clone();
            deck.RemoveAll(c => c.Name == name);
        }
    }
}
