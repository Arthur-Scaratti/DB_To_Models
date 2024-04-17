using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Models
{
    public class ORACLE_ALL_TABLES
    {
    public string? OWNER { get; set; }
    public string? TABLE_NAME { get; set; }
    public string? TABLESPACE_NAME { get; set; }
    public string? CLUSTER_NAME { get; set; }
    public string? IOT_NAME { get; set; }
    public string? STATUS { get; set; }
    public int? PCT_FREE { get; set; }
    public int? PCT_USED { get; set; }
    public int? INI_TRANS { get; set; }
    public int? MAX_TRANS { get; set; }
    public int? INITIAL_EXTENT { get; set; }
    public int? NEXT_EXTENT { get; set; }
    public int? MIN_EXTENTS { get; set; }
    public int? MAX_EXTENTS { get; set; }
    public int? PCT_INCREASE { get; set; }
    public int? FREELISTS { get; set; }
    public int? FREELIST_GROUPS { get; set; }
    public string? LOGGING { get; set; }
    public string? BACKED_UP { get; set; }
    public int? NUM_ROWS { get; set; }
    public int? BLOCKS { get; set; }
    public int? EMPTY_BLOCKS { get; set; }
    public int? AVG_SPACE { get; set; }
    public int? CHAIN_CNT { get; set; }
    public int? AVG_ROW_LEN { get; set; }
    public int? AVG_SPACE_FREELIST_BLOCKS { get; set; }

    }
}