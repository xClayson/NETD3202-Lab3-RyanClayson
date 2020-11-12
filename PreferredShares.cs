/* NAME: Ryan Clayson
 * ID: 100558837
 * NETD3202 - Lab 3 - Communication Activity
 * Date: Nov 12 2020
 */
using System;
using System.Collections.Generic;
using System.Text;

namespace NETD3202_Lab3_RyanClayson
{
    class PreferredShares:Shares
    {
        //Variable Declarations
        const int PreferredPrice = 100;
        const int votingPower = 10;

        //Constructor
        public PreferredShares(string name, string date, int numOfShares, string shareType) : 
            base(name, date, numOfShares, shareType)
        {
            this.buyerName = base.buyerName;
            this.purchasedDate = base.purchasedDate;
            this.numShares = base.numShares;
            this.shareType = base.shareType;
        }
        //"Get" Declarations for voting power and share price
        public int SharePrice
        {
            get { return PreferredPrice; }

        }
        public int VotePower
        {
            get { return votingPower; }

        }
    }
}
