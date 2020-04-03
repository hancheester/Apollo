using Apollo.Core.Model.Entity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Web;

namespace Apollo.AdminStore.WebForm.Classes
{
    public static class SessionFacade
    {
        private const string CHOSEN_PRODUCTS = "ChosenProducts";
        private const string NOT_CHOSEN_PRODUCTS = "NotChosenProducts";
        private const string CHOSEN_LINE_ITEM_LITES = "ChosenLineItemLites";
        private const string NOT_CHOSEN_LINE_ITEM_LITES = "NotChosenLineItemLites";
        private const string PICKING_ITEMS = "PickingItems";
        private const string PICK_IN_PROGRESS_ITEMS = "PickInProgressItems";
        private const string ACCOUNT_LITE = "AccountLite";
               
        public static List<LineItemLite> ChosenLineItemLites
        {
            get
            {
                if (HttpContext.Current.Session[CHOSEN_LINE_ITEM_LITES] == null)
                    HttpContext.Current.Session[CHOSEN_LINE_ITEM_LITES] = new List<LineItemLite>();

                return (List<LineItemLite>)HttpContext.Current.Session[CHOSEN_LINE_ITEM_LITES];
            }
            set { HttpContext.Current.Session[CHOSEN_LINE_ITEM_LITES] = value; }
        }
        public static List<LineItemLite> NotChosenLineItemLites
        {
            get
            {
                if (HttpContext.Current.Session[NOT_CHOSEN_LINE_ITEM_LITES] == null)
                    HttpContext.Current.Session[NOT_CHOSEN_LINE_ITEM_LITES] = new List<LineItemLite>();

                return (List<LineItemLite>)HttpContext.Current.Session[NOT_CHOSEN_LINE_ITEM_LITES];
            }
            set { HttpContext.Current.Session[NOT_CHOSEN_LINE_ITEM_LITES] = value; }
        }
        /// <summary>
        /// Format: Product Id, Product Price Id, Price, Quantity
        /// </summary>
        public static List<Tuple<int, int, decimal, int>> ChosenToAddItems
        {
            get
            {
                if (HttpContext.Current.Session[CHOSEN_PRODUCTS] == null)
                    HttpContext.Current.Session[CHOSEN_PRODUCTS] = new List<Tuple<int, int, decimal, int>>();

                return (List<Tuple<int, int, decimal, int>>)HttpContext.Current.Session[CHOSEN_PRODUCTS];
            }
            set { HttpContext.Current.Session[CHOSEN_PRODUCTS] = value; }
        }
        public static List<Tuple<int, int, decimal, int>> NotChosenToAddItems
        {
            get
            {
                if (HttpContext.Current.Session[NOT_CHOSEN_PRODUCTS] == null)
                    HttpContext.Current.Session[NOT_CHOSEN_PRODUCTS] = new List<Tuple<int, int, decimal, int>>();

                return (List<Tuple<int, int, decimal, int>>)HttpContext.Current.Session[NOT_CHOSEN_PRODUCTS];
            }
            set { HttpContext.Current.Session[NOT_CHOSEN_PRODUCTS] = value; }
        }        
        public static DataTable PickingItems
        {
            get
            {
                if (HttpContext.Current.Session[PICKING_ITEMS] == null)
                    return null;

                return (DataTable)HttpContext.Current.Session[PICKING_ITEMS];
            }
            set { HttpContext.Current.Session[PICKING_ITEMS] = value; }
        }
        public static IList<PickingLineItem> PickingLineItems
        {
            get
            {
                if (HttpContext.Current.Session[PICK_IN_PROGRESS_ITEMS] == null)
                    return new List<PickingLineItem>();

                return (IList<PickingLineItem>)HttpContext.Current.Session[PICK_IN_PROGRESS_ITEMS];
            }
            set { HttpContext.Current.Session[PICK_IN_PROGRESS_ITEMS] = value; }
        }
        public static AccountLite AccountLite
        {
            get
            {
                if (HttpContext.Current.Session[ACCOUNT_LITE] == null)
                    return null;

                return (AccountLite)HttpContext.Current.Session[ACCOUNT_LITE];
            }
            set { HttpContext.Current.Session[ACCOUNT_LITE] = value; }
        }
    }
}