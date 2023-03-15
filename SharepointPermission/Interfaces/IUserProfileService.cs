namespace SharepointPermission.Interfaces;

public interface IUserProfileService
{
    public List<Dictionary<string, string>> ListSomePropertiesOfAllUsers();
    public void UpdateUserPropertySingleValue(string accountName, Dictionary<string, string> valuesUpdated);
    public void UpdateUserPropertyMultiValue(string accountName, Dictionary<string, List<string>> valuesUpdated);
}