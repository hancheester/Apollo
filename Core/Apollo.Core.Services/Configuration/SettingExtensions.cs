using Apollo.Core.Configuration;
using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Apollo.Core.Services.Configuration
{
    public static class SettingExtensions
    {
        public static string GetSettingKey<T, TPropType>(this T entity, Expression<Func<T, TPropType>> keySelector) where T : ISettings, new()
        {
            var member = keySelector.Body as MemberExpression;
            if (member == null)
            {
                throw new ArgumentException(string.Format("Expression '{0}' refers to a method, not a property.", keySelector));
            }

            var propInfo = member.Member as PropertyInfo;
            if (propInfo == null)
            {
                throw new ArgumentException(string.Format("Expression '{0}' refers to a field, not a property.", keySelector));
            }

            var key = typeof(T).Name + "." + propInfo.Name;
            return key;
        }
    }
}
