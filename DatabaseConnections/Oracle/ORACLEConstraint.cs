using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Models
{
   public class ORACLE_ALL_CONS_COLUMNS
{
    public string? OWNER { get; set; }
    public string? CONSTRAINT_NAME { get; set; }
    public string? TABLE_NAME { get; set; }
    public string? COLUMN_NAME { get; set; }
    public int? POSITION { get; set; }
}

public class ORACLE_ALL_CONSTRAINTS
{
    public string? OWNER { get; set; }
    public string? CONSTRAINT_NAME { get; set; }
    public string? TABLE_NAME { get; set; }
    public string? CONSTRAINT_TYPE { get; set; }
    // Adicione o restante das propriedades aqui...
}
}