public static HandValue EvaluateHand(Card[] tableCards, Card[] handCards)
{
    HandValue handValue = new HandValue();
    HandValue tempHandValue = new HandValue();
    List<Card> cards = new List<Card>(handCards).Concat(tableCards).ToList();
    
    cards = SortCardsByValue(cards);

    List<Card> straightCards;

    tempHandValue = FindRelated(cards);

    if (HasStraight(cards, out straightCards, out handValue))
    {
        if (HasFlush(straightCards, out handValue))
        {
            handValue.Hand = Hands.StraightFlush;
            if (straightCards[0].Value == (int)Card.NumberEnum.Ten && straightCards.Last().Value == (int)Card.NumberEnum.Ace)
            {
                handValue.Hand = Hands.RoyalFlush;
            }
        }
    }

    if (handValue.Hand < tempHandValue.Hand)
    {
        handValue = tempHandValue;
    }

    if (HasFlush(cards, out tempHandValue))
    {
        if (tempHandValue.Hand > handValue.Hand)
        {
            handValue = tempHandValue;
        }
    }

    return handValue;
}

static List<Card> SortCardsByValue(List<Card> cards)
{
    List<Card> sortedCards = new List<Card>();

    int totalValidCards = cards.Count;
    for (int i = 0; i < cards.Count; i++)
    {
        if (cards[i] == null)
        {
            totalValidCards = i;
            break;
        }
    }

    cards.RemoveRange(totalValidCards, cards.Count - totalValidCards);

    foreach (Card c in cards.OrderBy(c => c.Value))
    {
        sortedCards.Add(c);
    }

    return sortedCards;
}