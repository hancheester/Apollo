using Apollo.Core.Model.Entity;
using System;
using System.Collections.Generic;

namespace Apollo.Core.Services.Interfaces
{
    public interface IGenericAttributeService
    {
        int DeleteOldGenericAttribute(DateTime? lastUpdateOnDateFrom);
        GenericAttribute GetAttributeById(int attributeId);
        void InsertAttribute(GenericAttribute attribute);
        void DeleteAttribute(GenericAttribute attribute);
        void UpdateAttribute(GenericAttribute attribute);
        IList<GenericAttribute> GetAttributesForEntity(int entityId, string keyGroup);
        void SaveAttribute(int entityId, string entityName, string key, string value, int storeId = 0);
    }
}
