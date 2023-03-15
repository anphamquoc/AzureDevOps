using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Search.Query;
using SharepointPermission.Interfaces;

namespace SharepointPermission.Services;

public class SearchService : ISearchService
{
    private readonly ClientContext _ctx;

    public SearchService(ClientContext ctx)
    {
        _ctx = ctx;
    }

    public List<Dictionary<string, string>> SearchItemsUnderList(string listName, string searchProperty,
        string searchValue)
    {
        var list = _ctx.Web.Lists.GetByTitle(listName);
        var result = new List<Dictionary<string, string>>();

        _ctx.Load(list);
        _ctx.ExecuteQuery();

        var query = new KeywordQuery(_ctx)
        {
            QueryText = $"listId:{list.Id} AND ContentClass:STS_ListItem AND {searchProperty}:*{searchValue}*",
            RowLimit = 10
        };
        // Remove duplicates from search results or not
        // query.TrimDuplicates = false;

        var searchExecutor = new SearchExecutor(_ctx);
        ClientResult<ResultTableCollection> results = searchExecutor.ExecuteQuery(query);
        _ctx.ExecuteQuery();

        // get all data
        foreach (var resultRow in results.Value[0].ResultRows)
        {
            var resultItem = new Dictionary<string, string>();
            // Access the desired properties, such as the "Title" field
            var title = resultRow["Title"].ToString();
            var path = resultRow["Path"].ToString();

            resultItem.Add("Title", title ?? string.Empty);
            resultItem.Add("Path", path ?? string.Empty);

            result.Add(resultItem);
        }

        return result;
    }

    public List<Dictionary<string, string>> SearchUsers(Dictionary<string, string> userProperties)
    {
        var queryText = "";
        var result = new List<Dictionary<string, string>>();

        foreach (var userProperty in userProperties) queryText += $"{userProperty.Key}:*{userProperty.Value}* AND ";

        queryText = queryText.Substring(0, queryText.Length - 5);
        var keywordQuery = new KeywordQuery(_ctx)
        {
            TrimDuplicates = true,
            // Source id of local people results
            SourceId = Guid.Parse("b09a7990-05ea-4af9-81ef-edfab16c4e31"),
            QueryText = queryText
        };

        var searchExecutor = new SearchExecutor(_ctx);
        ClientResult<ResultTableCollection> results = searchExecutor.ExecuteQuery(keywordQuery);

        _ctx.ExecuteQuery();

        foreach (var resultRow in results.Value[0].ResultRows)
        {
            // Access the desired properties, such as the "Title" field
            var resultItem = new Dictionary<string, string>();
            var email = resultRow["WorkEmail"].ToString();
            var title = resultRow["Title"].ToString();
            var accountName = resultRow["AccountName"].ToString();

            resultItem.Add("Title", title ?? string.Empty);
            resultItem.Add("Email", email ?? string.Empty);
            resultItem.Add("AccountName", accountName ?? string.Empty);

            result.Add(resultItem);
        }

        return result;
    }
}