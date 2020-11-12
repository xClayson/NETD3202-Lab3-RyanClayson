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
    class Shares
    {
        //Data Members
        protected string buyerName;
        protected string purchasedDate;
        protected int numShares;
        protected string shareType;

        //Getters/Setters for each data member
        //Buyer Name
        public string BuyerName
        {
            get { return this.buyerName; }
            set { this.buyerName = value; }
        }
        //Purchased Date
        public string PurchasedDate
        {
            get { return this.purchasedDate; }
            set { this.purchasedDate = value; }
        }
        //Number of Shares
        public int NumShares
        {
            get { return this.numShares; }
            set { this.numShares = value; }
        }
        //Share Type
        public string ShareType
        {
            get { return this.shareType; }
            set { this.shareType = value; }
        }

        //Constructor
        public Shares(string name, string date, int numOfShares, string shareType)
        {
            this.buyerName = name;
            this.purchasedDate = date;
            this.numShares = numOfShares;
            this.shareType = shareType;
        }
    }
}
