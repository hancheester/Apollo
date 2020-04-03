using Apollo.Core.Caching;
using Apollo.Core.Logging;
using Apollo.Core.Model.Entity;
using Apollo.Core.Services.Interfaces;
using Apollo.DataAccess;
using Apollo.DataAccess.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Apollo.Core.Services.Common
{
    public class GenericAttributeService : IGenericAttributeService
    {
        #region Fields

        private readonly IDbContext _dbContext;
        private readonly IRepository<GenericAttribute> _genericAttributeRepository;
        private readonly ICacheManager _cacheManager;
        private readonly ILogger _logger;

        #endregion

        #region Ctor
        
        public GenericAttributeService(
            IDbContext dbContext,
            IRepository<GenericAttribute> genericAttributeRepository,
            ILogBuilder logBuilder,
            ICacheManager cacheManager)
        {
            _dbContext = dbContext;
            _genericAttributeRepository = genericAttributeRepository;
            _cacheManager = cacheManager;
            _logger = logBuilder.CreateLogger(GetType().FullName);
        }

        #endregion

        public GenericAttribute GetAttributeById(int attributeId)
        {
            if (attributeId == 0)
                return null;

            return _genericAttributeRepository.Return(attributeId);
        }

        public void UpdateAttribute(GenericAttribute attribute)
        {
            if (attribute == null) throw new ArgumentNullException("attribute");

            try
            {
                Retry.Do(() => _genericAttributeRepository.Update(attribute), TimeSpan.FromSeconds(3));
            }            
            catch (Exception ex)
            {
                _logger.InsertLog(
                    LogLevel.Fatal, 
                    string.Format("Error while updating attribute. Entity ID={{{0}}}, Entity Name={{{1}}}, Key={{{2}}}, Value={{{3}}}", 
                        attribute.EntityId, attribute.KeyGroup, attribute.Key, attribute.Value),
                    ex);
                throw ex;
            }

            //cache
            _cacheManager.Remove(string.Format(CacheKey.GENERIC_ATTRIBUTE_BY_ENTITY_ID_AND_KEY_GROUP_KEY, attribute.EntityId, attribute.KeyGroup));
        }
        
        public void DeleteAttribute(GenericAttribute attribute)
        {
            if (attribute == null) throw new ArgumentNullException("attribute");

            var genericAttribute = _genericAttributeRepository.Return(attribute.Id);
            if (genericAttribute != null) _genericAttributeRepository.Delete(genericAttribute);

            //cache
            _cacheManager.Remove(string.Format(CacheKey.GENERIC_ATTRIBUTE_BY_ENTITY_ID_AND_KEY_GROUP_KEY, attribute.EntityId, attribute.KeyGroup));
        }

        public void InsertAttribute(GenericAttribute attribute)
        {
            if (attribute == null)
                throw new ArgumentNullException("attribute");

            _genericAttributeRepository.Create(attribute);

            //cache
            _cacheManager.Remove(string.Format(CacheKey.GENERIC_ATTRIBUTE_BY_ENTITY_ID_AND_KEY_GROUP_KEY, attribute.EntityId, attribute.KeyGroup));
        }

        public int DeleteOldGenericAttribute(DateTime? lastUpdateOnDateFrom)
        {
            var pLastUpdatedOnDateFrom = new SqlParameter("LastUpdatedOnDateFrom", DBNull.Value);

            if (lastUpdateOnDateFrom.HasValue)
            {
                pLastUpdatedOnDateFrom = new SqlParameter("LastUpdatedOnDateFrom", SqlDbType.DateTime);
                pLastUpdatedOnDateFrom.Value = lastUpdateOnDateFrom.Value;
            }

            var pTotalRecordsDeleted = new SqlParameter();
            pTotalRecordsDeleted.ParameterName = "TotalRecordsDeleted";
            pTotalRecordsDeleted.DbType = DbType.Int32;
            pTotalRecordsDeleted.Direction = ParameterDirection.Output;
                        
            //invoke stored procedure
            _dbContext.ExecuteSqlCommand(
                "EXEC [DeleteOldGenericAttribute] @LastUpdatedOnDateFrom, @TotalRecordsDeleted = @TotalRecordsDeleted OUTPUT",
                pLastUpdatedOnDateFrom,
                pTotalRecordsDeleted);

            int totalRecordsDeleted = (pTotalRecordsDeleted.Value != DBNull.Value) ? Convert.ToInt32(pTotalRecordsDeleted.Value) : 0;
            return totalRecordsDeleted;
        }

        public IList<GenericAttribute> GetAttributesForEntity(int entityId, string keyGroup)
        {
            string key = string.Format(CacheKey.GENERIC_ATTRIBUTE_BY_ENTITY_ID_AND_KEY_GROUP_KEY, entityId, keyGroup);

            var items = _cacheManager.GetWithExpiry(key, () =>
            {
                return _genericAttributeRepository.TableNoTracking
                    .Where(g => g.EntityId == entityId)
                    .Where(g => g.KeyGroup == keyGroup)
                    .ToList();
            }, new DateTimeOffset(DateTime.Now.AddMinutes(10D)));
            
            return items;
        }
        
        public void SaveAttribute(int entityId, string entityName, string key, string value, int storeId = 0)
        {
            if (key == null) throw new ArgumentNullException("key");
            string keyGroup = entityName;
            
            var props = GetAttributesForEntity(entityId, keyGroup)
                .Where(x => x.StoreId == storeId)
                .ToList();

            var prop = props.FirstOrDefault(ga =>
                ga.Key.Equals(key, StringComparison.InvariantCultureIgnoreCase));

            if (prop != null)
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    //delete
                    DeleteAttribute(prop);
                }
                else
                {
                    //update
                    prop = GetAttributeById(prop.Id);
                    prop.Value = value;
                    prop.UpdatedOnDate = DateTime.Now;
                    UpdateAttribute(prop);
                }
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    //insert
                    prop = new GenericAttribute
                    {
                        EntityId = entityId,
                        Key = key,
                        KeyGroup = keyGroup,
                        Value = value,
                        StoreId = storeId,
                        CreatedOnDate = DateTime.Now,
                        UpdatedOnDate = DateTime.Now
                    };
                    InsertAttribute(prop);
                }
            }            
        }
    }
}
