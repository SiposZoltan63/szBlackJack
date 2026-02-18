using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace szblackjack
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly Random _rng = new Random();
        private List<Card> _deck = new List<Card>();
        private readonly List<Card> _player = new List<Card>();
        private readonly List<Card> _dealer = new List<Card>();

        private bool _roundOver = false;
        private bool _hideDealerSecondCard = true;

        public MainWindow()
        {
            InitializeComponent();
            StartNewGame();
        }

        private void Btnew_Click(object sender, RoutedEventArgs e)
        {
            StartNewGame();
        }

        private void BtnHit_Click(object sender, RoutedEventArgs e)
        {
            PlayerHit();
        }

        private void BtnStand_Click(object sender, RoutedEventArgs e)
        {
            PlayerStand();
        }

        private void StartNewGame()
        {
            _roundOver = false;
            _hideDealerSecondCard = true;

            _deck = CreateDeck();
            Shuffle(_deck);

            _player.Clear();
            _dealer.Clear();

            _player.Add(Draw());
            _dealer.Add(Draw());
            _player.Add(Draw());
            _dealer.Add(Draw());

            StatusText.Text = "Játék indul! Kérsz lapot vagy megállsz?";
            SetButtonsEnabled(true, true);

            RefreshUI();

            if (BestScore(_player) == 21)
            {
                _hideDealerSecondCard = false;
                RefreshUI();
                EndRound(DecideWinner());
            }
        }

        private void PlayerHit()
        {
            if (_roundOver) return;

            _player.Add(Draw());
            RefreshUI();

            int pScore = BestScore(_player);

            if (pScore > 21)
            {
                _hideDealerSecondCard = false;
                RefreshUI();
                EndRound("Besokalltál (Bust). Vesztettél.");
            }
            else if (pScore == 21)
            {
                PlayerStand();
            }
        }

        private void PlayerStand()
        {
            if (_roundOver) return;

            _hideDealerSecondCard = false;

            while (BestScore(_dealer) < 17)
                _dealer.Add(Draw());

            RefreshUI();
            EndRound(DecideWinner());
        }

        private string DecideWinner()
        {
            int p = BestScore(_player);
            int d = BestScore(_dealer);

            if (p > 21) return "Besokalltál (Bust). Vesztettél.";
            if (d > 21) return "Az osztó besokallt (Bust). Nyertél!";

            if (p > d) return "Nyertél! (" + p + " vs " + d + ")";
            if (d > p) return "Vesztettél. (" + p + " vs " + d + ")";
            return "Döntetlen (Push). (" + p + " vs " + d + ")";
        }

        private void EndRound(string message)
        {
            _roundOver = true;
            StatusText.Text = message;
            SetButtonsEnabled(false, false);
        }

        private void SetButtonsEnabled(bool hit, bool stand)
        {
            BtnHit.IsEnabled = hit;
            BtnStand.IsEnabled = stand;
        }

        private Card Draw()
        {
            if (_deck.Count == 0)
            {
                _deck = CreateDeck();
                Shuffle(_deck);
            }

            Card c = _deck[0];
            _deck.RemoveAt(0);
            return c;
        }

        private void RefreshUI()
        {
            PlayerCards.ItemsSource = _player.Select(c => c.ToShortString()).ToList();

            List<string> dealerView = new List<string>();
            if (_hideDealerSecondCard && _dealer.Count >= 2)
            {
                dealerView.Add(_dealer[0].ToShortString());
                dealerView.Add("🂠 (rejtett)");
            }
            else
            {
                dealerView = _dealer.Select(c => c.ToShortString()).ToList();
            }

            DealerCards.ItemsSource = dealerView;

            int pScore = BestScore(_player);
            PlayerScoreText.Text = "Pont: " + pScore + (pScore > 21 ? " (Bust)" : "");

            if (_hideDealerSecondCard && _dealer.Count >= 2)
            {
                DealerScoreText.Text = "Pont: " + BestScore(new List<Card> { _dealer[0] }) + " + ?";
            }
            else
            {
                int dScore = BestScore(_dealer);
                DealerScoreText.Text = "Pont: " + dScore + (dScore > 21 ? " (Bust)" : "");
            }
        }

        private static int BestScore(List<Card> hand)
        {
            int sum = 0;
            int aces = 0;

            foreach (Card c in hand)
            {
                if (c.Rank == Rank.Ace)
                {
                    aces++;
                    sum += 1;
                }
                else
                {
                    sum += c.Value;
                }
            }

            while (aces > 0 && sum + 10 <= 21)
            {
                sum += 10;
                aces--;
            }

            return sum;
        }

        private List<Card> CreateDeck()
        {
            List<Card> deck = new List<Card>();
            foreach (Suit s in Enum.GetValues(typeof(Suit)))
                foreach (Rank r in Enum.GetValues(typeof(Rank)))
                    deck.Add(new Card(s, r));
            return deck;
        }

        private void Shuffle(List<Card> list)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = _rng.Next(i + 1);
                Card temp = list[i];
                list[i] = list[j];
                list[j] = temp;
            }
        }
    }

    public enum Suit { Clubs, Diamonds, Hearts, Spades }

    public enum Rank
    {
        Two = 2, Three = 3, Four = 4, Five = 5, Six = 6,
        Seven = 7, Eight = 8, Nine = 9, Ten = 10,
        Jack = 11, Queen = 12, King = 13, Ace = 14
    }

    public class Card
    {
        public Suit Suit { get; private set; }
        public Rank Rank { get; private set; }

        public Card(Suit suit, Rank rank)
        {
            Suit = suit;
            Rank = rank;
        }

        public int Value
        {
            get
            {
                if (Rank == Rank.Jack || Rank == Rank.Queen || Rank == Rank.King)
                    return 10;
                if (Rank == Rank.Ace)
                    return 11;
                return (int)Rank;
            }
        }

        public string ToShortString()
        {
            string r;
            switch (Rank)
            {
                case Rank.Ace: r = "A"; break;
                case Rank.King: r = "K"; break;
                case Rank.Queen: r = "Q"; break;
                case Rank.Jack: r = "J"; break;
                default: r = ((int)Rank).ToString(); break;
            }

            string s;
            switch (Suit)
            {
                case Suit.Clubs: s = "♣"; break;
                case Suit.Diamonds: s = "♦"; break;
                case Suit.Hearts: s = "♥"; break;
                case Suit.Spades: s = "♠"; break;
                default: s = "?"; break;
            }

            return r + s;
        }
    }
}
