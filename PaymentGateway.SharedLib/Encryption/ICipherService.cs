using System;
using System.Collections.Generic;
using System.Text;

namespace PaymentGateway.SharedLib.Encryption
{
    public  interface ICipherService
    {
   
        string Encrypt(string input);

        string Decrypt(string cipherText);
    
    }
}
