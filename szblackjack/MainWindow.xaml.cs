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
        private List<Card> _deck = new();
        public MainWindow()
        {
            InitializeComponent();
            StartNewGame();
        }

        private void Btnew_Click(object sender, RoutedEventArgs e) => StartNewGame();
        private void Btnew_Click(object sender, RoutedEventArgs e) => PlayerHit();
        private void Btnew_Click(object sender, RoutedEventArgs e) => PlayerStand();

        private void StartNewGame()
        {
            _roundover = false;
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
            StatusEnabled(HitTestCore: true, BtnStand: true);
            RefreshUI();

            var pScore = BestScore(_player);
        }
        private void PlayerHit()
        {
            _player.Add(Draw());
            RefreshUI();
            var pScore = BestScore(_player);
            if (pScore > 21)
            {
                _hideDealerSecondCard = false;
                RefreshUI();
            }
        }
        private void PlayerStand()
        {
            if (_roundover) return;
            _hideDealerSecondCard = false;
            while (BestScore(_dealer) < 17)
                _dealer.Add(Draw());
            RefreshUI();
        }

    }
    public enum Suit {Clubs,Diamonds,Hearts,Spades}
    public enum Rank
    {
        Two=2,Three=3,Four=4,five=5,six=6,seven=7,eight=8,Nine=9,Ten=10,eleven=11,twelve=12,
    }
    public sealed class Card
    {
        public Suit Suit { get; set; }
        public Rank Rank { get; set; }
        public Card(Suit suit, Rank rank)
        {
            Suit = suit;
            Rank = rank;
        }
        public int Value => Rank switch
        {
            Rank.Jack => 10,
            Rank.Queen => 10,
            Rank.King => 10,
            Rank.Ace => 10,
            _=> (int)Rank
        };
        public string ToshortString()
        {
            string r = Rank switch
            {
                Rank.Ace => "A",
                Rank.King => "K",
                Rank.Queen => "Q",
                Rank.Jack => "J",
                _=> ((int)Rank).ToString()
            };
            string s = Suit switch
            {
                Suit.Clubs => "",
                Suit.Diamonds => "",
                Suit.Hearts => "",
                Suit.Spades => "",
                _=> "?"
            };
            return $
            }
    }
}
