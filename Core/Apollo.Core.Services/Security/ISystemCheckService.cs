using Apollo.Core.Model.Entity;
using System.Collections.Generic;

namespace Apollo.Core.Services.Security
{
    public interface ISystemCheckService
    {
        SystemCheck ProcessSystemChecking(Order order, string email, string name, bool avsCheck);
        Dictionary<string, int> CalculateSystemCheckScore(int orderId);
    }
}
