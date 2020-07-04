using System;
using System.Collections.Generic;
using System.Text;

namespace Client.Payments.Api.Utils
{
    public static class RegexValidator
    {
       

        public const string UK_VALID_POSTCODE =
            @"^[A-Za-z]{1,2}[0-9A-Za-z]{1,2}[ ]?[0-9]{0,1}[A-Za-z]{2}$"; // http://regexlib.com/REDetails.aspx?regexp_id=260&AspxAutoDetectCookieSupport=1

        public const string VALID_UUID = @"^[0-9a-fA-F]{8}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{12}$";


        public const string VALID_CARD_NUMBER = @"^(1298|1267|4512|4567|8901|8933)([\-\s]?[0-9]{4}){3}$";
        public const string VALID_CARD_CCV = @"^\d{3}$";
    }
}
