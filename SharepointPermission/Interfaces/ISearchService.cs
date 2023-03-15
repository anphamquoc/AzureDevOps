namespace SharepointPermission.Interfaces;

public interface ISearchService
{
    public List<Dictionary<string, string>> SearchItemsUnderList(string listName, string searchProperty,
        string searchValue);

    public List<Dictionary<string, string>> SearchUsers(Dictionary<string, string> userProperties);
}