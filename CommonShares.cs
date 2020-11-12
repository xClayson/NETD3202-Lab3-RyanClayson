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
    class CommonShares : Shares
    {
        //Variable Declarations
        const int commonPrice = 42;
        const int votingPower = 1;

        //Constructor
        public CommonShares(string name, string date, int numOfShares, string shareType, int votingPower) :
            base(name, date, numOfShares, shareType)
        {
            this.buyerName = base.buyerName;
            this.purchasedDate = base.purchasedDate;
            this.numShares = base.numShares;
            this.shareType = base.shareType;
        }

        //"Get" Declarations for voting power and commonprice
        public int VotePower
        {
            get { return votingPower; }
        }
        public int SharePrice
        {
            get { return commonPrice; }
        }
    }
}
