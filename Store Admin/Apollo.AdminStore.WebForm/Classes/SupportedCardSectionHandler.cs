using System;
using System.Collections.Generic;
using System.Configuration;
using System.Xml;

namespace Apollo.AdminStore.WebForm.Classes
{
    public class SupportedCardSectionHandler : IConfigurationSectionHandler
    {
        private const string CARD = "card";
        private const string NAME = "name";
        private const string CARD_TYPE = "cardType";
        private const string HAS_START_DATE = "hasStartDate";
        private const string HAS_ISSUE_NUMBER = "hasIssueNumber";

        private Dictionary<string, SupportedCard> _cards;

        public Dictionary<string, SupportedCard> Cards
        {
            get { return _cards; }
        }

        #region IConfigurationSectionHandler Members

        object IConfigurationSectionHandler.Create(object parent, object configContext, XmlNode section)
        {
            _cards = new Dictionary<string, SupportedCard>();
            for (int i = 0; i < section.SelectNodes(CARD).Count; i++)
                _cards.Add(section.SelectNodes(CARD)[i].Attributes[NAME].Value, new SupportedCard(section.SelectNodes(CARD)[i].Attributes[CARD_TYPE].Value,
                                                                                                  Convert.ToBoolean(section.SelectNodes(CARD)[i].Attributes[HAS_START_DATE].Value),
                                                                                                  Convert.ToBoolean(section.SelectNodes(CARD)[i].Attributes[HAS_ISSUE_NUMBER].Value)));

            return this;
        }

        #endregion
    }
}