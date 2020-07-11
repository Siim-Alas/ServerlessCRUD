using Microsoft.Azure.Cosmos.Table;
using Newtonsoft.Json;

namespace ServerlessCrudClassLibrary.TableEntities
{
    public class TableMetadataEntity : TableEntity
    {
        public TableMetadataEntity() : base()
        {

        }
        public TableMetadataEntity(string tableName)
        {
            PartitionKey = tableName;
            RowKey = "metadata";
        }
        [IgnoreProperty]
        [JsonIgnore]
        public string TableName 
        { 
            get { return PartitionKey; }
            set { PartitionKey = value; }
        }
    }
}
