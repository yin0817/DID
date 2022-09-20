using Dao.Models.Base;
using DID.Common;

namespace Dao.Common
{
    public class WalletHelp
    {
        /// <summary>
        /// 获取钱包地址ID
        /// </summary>
        /// <param name="walletAddress"></param>
        /// <param name="otype"></param>
        /// <param name="sign"></param>
        /// <returns></returns>
        public static string GetWalletId(DaoBaseReq req)
        {
            using var db = new NDatabase();
            var walletId =  db.SingleOrDefault<string>("select WalletId from Wallet where WalletAddress = @0 and Otype = @1 and Sign = @2 and IsLogout = 0 and IsDelete = 0",
                                                                req.WalletAddress, req.Otype, req.Sign);
            return walletId;
        }
    }
}