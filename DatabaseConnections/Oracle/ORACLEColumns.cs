using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Models
{
    public class ORACLE_ALL_TAB_COLUMNS
    {
    public string? OWNER { get; set; }
    public string? TABLE_NAME { get; set; }
    public string? COLUMN_NAME { get; set; }
    public string? DATA_TYPE { get; set; }
    public string? DATA_TYPE_MOD { get; set; }
    public string? DATA_TYPE_OWNER { get; set; }
    public int? DATA_LENGTH { get; set; }
    public int? DATA_PRECISION { get; set; }
    public int? DATA_SCALE { get; set; }
    public string? NULLABLE { get; set; }
    public int? COLUMN_ID { get; set; }
    }
}