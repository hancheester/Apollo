using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;

namespace Apollo.AdminStore.WebForm.Classes
{
    public static class EnumExtensions
    {
        public static ListItem[] ToListItemArray<TEnum>(this TEnum enumObj, bool markCurrentAsSelected = true, int[] valuesToExclude = null) where TEnum : struct
        {
            if (!typeof(TEnum).IsEnum) throw new ArgumentException("An Enumeration type is required.", "enumObj");

            var optionValues = Enum.GetValues(typeof(TEnum));
            var optionNames = Enum.GetNames(typeof(TEnum));
            var i = 0;
            var items = new List<ListItem>();

            foreach (var item in optionValues)
            {
                var li = new ListItem(optionNames[i], ((int)item).ToString());
                items.Add(li);
                i++;
            }

            return items.ToArray();
        }
    }
}