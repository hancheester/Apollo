using Apollo.Core.Logging;
using Apollo.Core.Model.Entity;
using Apollo.Core.Services.Accounts;
using Apollo.Core.Services.Interfaces.DataBuilder;
using Apollo.DataAccess.Interfaces;
using System.Linq;

namespace Apollo.Core.Services.DataBuilder
{
    public class AccountBuilder : IAccountBuilder
    {
        private readonly IRepository<Subscriber> _subscriberRepository;
        private readonly IWebMembership _webMembership;
        private readonly ILogger _logger;

        public AccountBuilder(IRepository<Subscriber> subscriberRepository, IWebMembership webMembership, ILogBuilder logBuilder)
        {
            _subscriberRepository = subscriberRepository;
            _webMembership = webMembership;
            _logger = logBuilder.CreateLogger(GetType().FullName);
        }

        public Account Build(Account account)
        {
            var member = _webMembership.GetUser(account.Username);

            if (member == null)
            {
                _logger.InsertLog(LogLevel.Error, "Membership could not be found. Username={{{0}}}", account.Username);
                throw new ApolloException("Membership could not be found. Username={{{0}}}", account.Username);
            }

            var subsciber = _subscriberRepository.Table.Where(x => x.Email.ToLower() == account.Email.ToLower()).FirstOrDefault();
            account.IsSubscribed = subsciber != null ? subsciber.IsActive : false;

            account.IsLockedOut = member.IsLockedOut;
            account.IsApproved = member.IsApproved;
            account.Roles = _webMembership.GetRolesForUser(account.Email);
            account.LastLoginDate = member.LastLoginDate;
            account.CreationDate = member.CreationDate;
            account.Username = member.Username.ToLower();

            return account;
        }
    }
}
