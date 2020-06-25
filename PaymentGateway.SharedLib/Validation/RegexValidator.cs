using System;
using System.Collections.Generic;
using System.Text;

namespace PaymentGateway.SharedLib.Validation
{
    public static class RegexValidator
    {
        public const string NWA_VALID_POSTCODE = @"^[a-zA-Z]{1,2}[0-9rR][0-9a-zA-Z]? ?[0-9][abABd-hD-HjJlLnNp-uP-Uw-zW-Z]{2}$";

        public const string UK_VALID_POSTCODE =
            @"^[A-Za-z]{1,2}[0-9A-Za-z]{1,2}[ ]?[0-9]{0,1}[A-Za-z]{2}$"; // http://regexlib.com/REDetails.aspx?regexp_id=260&AspxAutoDetectCookieSupport=1

        public const string VALID_UUID = @"^[0-9a-fA-F]{8}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{12}$";

        public const string QUOTE_GENERAL_STRING = @"^[a-zA-Z]*((-|'|\s)*[a-zA-Z])*(')?$";

        public const string QUOTE_PETNAME = @"^[a-zA-Z]{1}[a-zA-Z0-9']*$";

        public const string QUOTE_POSTCODE = @"^a-zA-Z{1,2}0-9rR0-9a-zA-Z? ?0-9abABd-hD-HjJlLnNp-uP-Uw-zW-Z{2}$";

        public const string QUOTE_DATE = @"^[0-9]{4}-([0][0-9]|[1][0-2])-([0][0-9]|[1][0-9]|[2][0-9]|[3][0-1])$";

        public const string CONTACT_NUMBER = @"^(\\+|00)?([0-9]{2})?([(]{1}[0-9]{1}[)]{1})?([0-9]+)$";

        public const string SORT_CODE = "^\\d{6}$";
    }
}
