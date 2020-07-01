using System;
using System.Collections.Generic;
using System.Text;

namespace ServerlessCrudClassLibrary
{
    public class TableMetadata
    {
        public TableMetadata(int numberOfEntities)
        {
            NumberOfEntities = numberOfEntities;
        }
        public int NumberOfEntities { get; set; }
    }
}
