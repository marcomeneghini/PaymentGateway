﻿using System;
using System.Collections.Generic;
using System.Text;

namespace PaymentGateway.IntegrationTests
{
    public class Card : IEquatable<Card>
    {
        public string CardNumber { get; set; }

        /// <summary>
        /// the Card Holder Name of the customer
        /// </summary>
        public string CardHolderName { get; set; }
        /// <summary>
        /// the month of the expiry date
        /// </summary>
        public int MonthExpiryDate { get; set; }
        /// <summary>
        /// the year of the expiry date
        /// </summary>
        public int YearExpiryDate { get; set; }

        public string CVV { get; set; }

        public bool Equals(Card other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return CardNumber == other.CardNumber && 
                   CardHolderName == other.CardHolderName &&
                   MonthExpiryDate == other.MonthExpiryDate && 
                   YearExpiryDate == other.YearExpiryDate &&
                    CVV==other.CVV;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Card)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(CardNumber, CardHolderName, MonthExpiryDate, YearExpiryDate, CVV);
        }
    }
}
