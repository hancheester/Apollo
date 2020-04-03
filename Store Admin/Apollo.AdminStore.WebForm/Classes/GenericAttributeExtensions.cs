using Apollo.Core;
using Apollo.Core.Model.Entity;
using Apollo.Core.Services.Interfaces;
using System;
using System.Linq;

namespace Apollo.AdminStore.WebForm.Classes
{
    public static class GenericAttributeExtensions
    {
        public static TPropType GetAttribute<TPropType>(this BaseEntity entity, string keyGroup, string key, IUtilityService utilityService, int storeId = 0)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            var props = utilityService.GetAttributesForEntity(entity.Id, keyGroup);

            //little hack here (only for unit testing). we should write expect-return rules in unit tests for such cases
            if (props == null)
                return default(TPropType);

            props = props.Where(x => x.StoreId == storeId).ToList();

            if (props.Count == 0)
                return default(TPropType);

            var prop = props.FirstOrDefault(ga =>
                ga.Key.Equals(key, StringComparison.InvariantCultureIgnoreCase)); //should be culture invariant

            if (prop == null || string.IsNullOrEmpty(prop.Value))
                return default(TPropType);

            return CommonHelper.To<TPropType>(prop.Value);
        }
    }
}