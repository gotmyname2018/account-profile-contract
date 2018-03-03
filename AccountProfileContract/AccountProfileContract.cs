using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services.Neo;
using System;
using System.Numerics;

namespace Neo.SmartContract
{
    public class AccountProfileContract : Framework.SmartContract
    {
        // contract owner public key
        private static readonly byte[] CONTRACT_OWNER = { 0x02, 0xf1, 0x17, 0xf5, 0xc0, 0x57, 0x88, 0x76, 0x9d, 0x84, 0x03, 0x71, 0x55, 0xa0, 0x93, 0x2a, 0xdd, 0x29, 0xf5, 0x69, 0xfb, 0x9f, 0xdc, 0xfc, 0x61, 0xe9, 0x0e, 0x0f, 0x63, 0x77, 0xfa, 0x5b, 0x38 };

        // contract owner url
        private static readonly string EMAIL_VERIFY_URL = "http://localhost:8088/_api/verify";

        public static object Main(string operation, params object[] args)
        {
            switch (operation)
            {
                case "contractOwner":   // query contract owner public key
                    return CONTRACT_OWNER;
                case "emailVerifyUrl":   // query url for email address verification
                    return EMAIL_VERIFY_URL;
                case "query":   // query by email address
                    return Query((string)args[0]);
                case "queryByKey":  // query by account public key
                    return QueryByKey((byte[])args[0]);
                case "register":    // register account profile
                    return Register((string)args[0], (byte[])args[1]);
                case "grant":   // grant email address binding to an account after verification success
                    return GrantEmailBinding((string)args[0], (byte[])args[1], (byte[])args[2]);
                default:
                    return false;
            }
        }

        private static byte[] Query(string email)
        {
            byte[] owner = Storage.Get(Storage.CurrentContext, email);
            if (owner == null) return null;
            return QueryByKey(owner);
        }

        private static byte[] QueryByKey(byte[] owner)
        {
            return Storage.Get(Storage.CurrentContext, owner);
        }

        private static bool Register(string profile, byte[] owner)
        {
            if (!Runtime.CheckWitness(owner)) return false;
            Storage.Put(Storage.CurrentContext, owner, profile);
            return true;
        }

        private static bool GrantEmailBinding(string email, byte[] owner, byte[] signature)
        {
            if (!Runtime.CheckWitness(CONTRACT_OWNER)) return false;
            // if (!VerifySignature(signature, CONTRACT_OWNER)) return false;
            Storage.Put(Storage.CurrentContext, email, owner);
            return true;
        }
    }
}
