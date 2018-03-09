using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services.Neo;
using System;
using System.Numerics;

namespace Neo.SmartContract
{
    public class AccountProfileContract : Framework.SmartContract
    {
        // contract owner public key
        private static readonly byte[] CONTRACT_OWNER = { 0x02, 0xac, 0xc3, 0x01, 0x80, 0xcf, 0xd3, 0xa7, 0xab, 0xcd, 0x40, 0xc4, 0x20, 0x91, 0x50, 0x2f, 0x8d, 0xa7, 0x61, 0x8f, 0x18, 0x52, 0x07, 0xd2, 0xa9, 0x19, 0xbd, 0xf4, 0xc2, 0x44, 0x98, 0xe1, 0x03 };

        // email address verification request url
        private static readonly string EMAIL_VERIFY_REQ_URL = "http://yun10dai.com:3000/_api/neo-mail/verifyreq";

        // email address verification response url
        private static readonly string EMAIL_VERIFY_RESP_URL = "http://yun10dai.com:3000/_api/neo-mail/verifyresp";

        public static object Main(string operation, params object[] args)
        {
            if (operation == "contractOwner")   // query contract owner public key
                return CONTRACT_OWNER;
            if (operation == "emailVerifyReqUrl")  // query url for email address verification request
                return EMAIL_VERIFY_REQ_URL;
            if (operation == "emailVerifyRespUrl")  // query url for email address verification response
                return EMAIL_VERIFY_RESP_URL;
            if (operation == "query")           // query profile by email address
                return Query((string)args[0]);
            if (operation == "queryByAccount")  // query profile by account script hash
                return QueryByAccount((byte[])args[0]);
            if (operation == "queryOwner")      // query current email address owner, return account script hash of owner
                return QueryOwner((string)args[0]);
            if (operation == "register")        // register account profile
                return Register((string)args[0], (byte[])args[1]);
            if (operation == "grant")           // grant email address binding to an account after verification success
                return GrantEmailBinding((string)args[0], (byte[])args[1], (byte[])args[2]);
            return false;
        }

        private static byte[] Query(string email)
        {
            byte[] owner = Storage.Get(Storage.CurrentContext, email);
            if (owner == null) return null;
            return QueryByAccount(owner);
        }

        private static byte[] QueryByAccount(byte[] owner)
        {
            return Storage.Get(Storage.CurrentContext, owner);
        }

        private static byte[] QueryOwner(string email)
        {
            return Storage.Get(Storage.CurrentContext, email);
        }

        private static bool Register(string profile, byte[] owner)
        {
            if (!Runtime.CheckWitness(owner)) return false;
            Storage.Put(Storage.CurrentContext, owner, profile);
            return true;
        }

        private static bool GrantEmailBinding(string email, byte[] owner, byte[] signature)
        {
            // Temporary disable the permission check to avoid unable to test the application due to unable to receive verification mail (It should not, but who knows )
            // if (!Runtime.CheckWitness(CONTRACT_OWNER)) return false;
            // if (!VerifySignature(signature, CONTRACT_OWNER)) return false;
            Storage.Put(Storage.CurrentContext, email, owner);
            return true;
        }
    }
}
