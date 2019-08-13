using DTO;
using Refit;

namespace DataModelingService.Models
{
    public class QueryParameters
    {
        [AliasAs("key")]
        public string ApiKey { get; set; }

        [AliasAs("location")]
        public Location Location { get; set; }

        [AliasAs("radius")]
        public string Radius { get; set; }

        #region optional parameters

        [AliasAs("keyword")]        
        public string Keyword { get; set; }
        
        [AliasAs("type")]
        public string Type { get; set; }

        [AliasAs("pagetoken")]
        public string PageToken { get; set; }

        #endregion        
    }
}
