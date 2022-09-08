using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.OrLog.Models.Hand
{
    public class CardData
    {


        public CardData(string str)
        {
            switch (str)
            {
                case "18":
                    {
                        Suit = CardSuit.Diamond;
                        Rank = CardRank.Two;
                        break;
                    }
                case "19":
                    {
                        Suit = CardSuit.Diamond;
                        Rank = CardRank.Three;
                        break;
                    }
                case "20":
                    {
                        Suit = CardSuit.Diamond;
                        Rank = CardRank.Four;
                        break;
                    }
                case "21":
                    {
                        Suit = CardSuit.Diamond;
                        Rank = CardRank.Five;
                        break;
                    }
                case "22":
                    {
                        Suit = CardSuit.Diamond;
                        Rank = CardRank.Six;
                        break;
                    }
                case "23":
                    {
                        Suit = CardSuit.Diamond;
                        Rank = CardRank.Seven;
                        break;
                    }
                case "24":
                    {
                        Suit = CardSuit.Diamond;
                        Rank = CardRank.Eight;
                        break;
                    }
                case "25":
                    {
                        Suit = CardSuit.Diamond;
                        Rank = CardRank.Nine;
                        break;
                    }
                case "26":
                    {
                        Suit = CardSuit.Diamond;
                        Rank = CardRank.Ten;
                        break;
                    }
                case "27":
                    {
                        Suit = CardSuit.Diamond;
                        Rank = CardRank.Jack;
                        break;
                    }
                case "28":
                    {
                        Suit = CardSuit.Diamond;
                        Rank = CardRank.Queen;
                        break;
                    }
                case "29":
                    {
                        Suit = CardSuit.Diamond;
                        Rank = CardRank.King;
                        break;
                    }
                case "30":
                    {
                        Suit = CardSuit.Diamond;
                        Rank = CardRank.Ace;
                        break;
                    }

                    // New Suit Clubs
                case "34":
                    {
                        Suit = CardSuit.Club;
                        Rank = CardRank.Two;
                        break;
                    }
                case "35":
                    {
                        Suit = CardSuit.Club;
                        Rank = CardRank.Three;
                        break;
                    }
                case "36":
                    {
                        Suit = CardSuit.Club;
                        Rank = CardRank.Four;
                        break;
                    }
                case "37":
                    {
                        Suit = CardSuit.Club;
                        Rank = CardRank.Five;
                        break;
                    }
                case "38":
                    {
                        Suit = CardSuit.Club;
                        Rank = CardRank.Six;
                        break;
                    }
                case "39":
                    {
                        Suit = CardSuit.Club;
                        Rank = CardRank.Seven;
                        break;
                    }
                case "40":
                    {
                        Suit = CardSuit.Club;
                        Rank = CardRank.Eight;
                        break;
                    }
                case "41":
                    {
                        Suit = CardSuit.Club;
                        Rank = CardRank.Nine;
                        break;
                    }
                case "42":
                    {
                        Suit = CardSuit.Club;
                        Rank = CardRank.Ten;
                        break;
                    }
                case "43":
                    {
                        Suit = CardSuit.Club;
                        Rank = CardRank.Jack;
                        break;
                    }
                case "44":
                    {
                        Suit = CardSuit.Club;
                        Rank = CardRank.Queen;
                        break;
                    }
                case "45":
                    {
                        Suit = CardSuit.Club;
                        Rank = CardRank.King;
                        break;
                    }
                case "46":
                    {
                        Suit = CardSuit.Club;
                        Rank = CardRank.Ace;
                        break;
                    }

                //new suit hears
                case "50":
                    {
                        Suit = CardSuit.Hearts;
                        Rank = CardRank.Two;
                        break;
                    }
                case "51":
                    {
                        Suit = CardSuit.Hearts;
                        Rank = CardRank.Three;
                        break;
                    }
                case "52":
                    {
                        Suit = CardSuit.Hearts;
                        Rank = CardRank.Four;
                        break;
                    }
                case "53":
                    {
                        Suit = CardSuit.Hearts;
                        Rank = CardRank.Five;
                        break;
                    }
                case "54":
                    {
                        Suit = CardSuit.Hearts;
                        Rank = CardRank.Six;
                        break;
                    }
                case "55":
                    {
                        Suit = CardSuit.Hearts;
                        Rank = CardRank.Seven;
                        break;
                    }
                case "56":
                    {
                        Suit = CardSuit.Hearts;
                        Rank = CardRank.Eight;
                        break;
                    }
                case "57":
                    {
                        Suit = CardSuit.Hearts;
                        Rank = CardRank.Nine;
                        break;
                    }
                case "58":
                    {
                        Suit = CardSuit.Hearts;
                        Rank = CardRank.Ten;
                        break;
                    }
                case "59":
                    {
                        Suit = CardSuit.Hearts;
                        Rank = CardRank.Jack;
                        break;
                    }
                case "60":
                    {
                        Suit = CardSuit.Hearts;
                        Rank = CardRank.Queen;
                        break;
                    }
                case "61":
                    {
                        Suit = CardSuit.Hearts;
                        Rank = CardRank.King;
                        break;
                    }
                case "62":
                    {
                        Suit = CardSuit.Hearts;
                        Rank = CardRank.Ace;
                        break;
                    }
                //new suit spade
                case "66":
                    {
                        Suit = CardSuit.Hearts;
                        Rank = CardRank.Two;
                        break;
                    }
                case "67":
                    {
                        Suit = CardSuit.Hearts;
                        Rank = CardRank.Three;
                        break;
                    }
                case "68":
                    {
                        Suit = CardSuit.Hearts;
                        Rank = CardRank.Four;
                        break;
                    }
                case "69":
                    {
                        Suit = CardSuit.Hearts;
                        Rank = CardRank.Five;
                        break;
                    }
                case "70":
                    {
                        Suit = CardSuit.Hearts;
                        Rank = CardRank.Six;
                        break;
                    }
                case "71":
                    {
                        Suit = CardSuit.Hearts;
                        Rank = CardRank.Seven;
                        break;
                    }
                case "72":
                    {
                        Suit = CardSuit.Hearts;
                        Rank = CardRank.Eight;
                        break;
                    }
                case "73":
                    {
                        Suit = CardSuit.Hearts;
                        Rank = CardRank.Nine;
                        break;
                    }
                case "74":
                    {
                        Suit = CardSuit.Hearts;
                        Rank = CardRank.Ten;
                        break;
                    }
                case "75":
                    {
                        Suit = CardSuit.Hearts;
                        Rank = CardRank.Jack;
                        break;
                    }
                case "76":
                    {
                        Suit = CardSuit.Hearts;
                        Rank = CardRank.Queen;
                        break;
                    }
                case "77":
                    {
                        Suit = CardSuit.Hearts;
                        Rank = CardRank.King;
                        break;
                    }
                case "78":
                    {
                        Suit = CardSuit.Hearts;
                        Rank = CardRank.Ace;
                        break;
                    }
                default:
                    break;
                    
            }
        }
        public CardRank Rank { get; set; }
        public CardSuit Suit { get; set; }

        public override string ToString()
        {
            string suit = "";
            string rank = "";
            switch (this.Rank)
            {
                case CardRank.Two:
                    rank = "2";
                    break;
                case CardRank.Three:
                    rank = "3";
                    break;
                case CardRank.Four:
                    rank = "4";
                    break;
                case CardRank.Five:
                    rank = "5";
                    break;
                case CardRank.Six:
                    rank = "6";
                    break;
                case CardRank.Seven:
                    rank = "7";
                    break;
                case CardRank.Eight:
                    rank = "8";
                    break;
                case CardRank.Nine:
                    rank = "9";
                    break;
                case CardRank.Ten:
                    rank = "T";
                    break;
                case CardRank.Jack:
                    rank = "J";
                    break;
                case CardRank.Queen:
                    rank = "Q";
                    break;
                case CardRank.King:
                    rank = "K";
                    break;
                case CardRank.Ace:
                    rank = "A";
                    break;



            }

            switch (this.Suit)
            {
                case CardSuit.Diamond:
                    suit = "d";
                    break;
                case CardSuit.Club:
                    suit = "c";
                    break;
                case CardSuit.Hearts:
                    suit = "h";
                    break;
                case CardSuit.Spade:
                    suit = "s";
                    break;

            }

            return $"{rank}{suit}";
        }
    }
}
