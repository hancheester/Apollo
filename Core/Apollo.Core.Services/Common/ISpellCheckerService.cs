using System.Collections.Generic;

namespace Apollo.Core.Services.Common
{
    public interface ISpellCheckerService
    {
        bool Spell(string word);
        IList<string> Suggest(string word);
    }
}
