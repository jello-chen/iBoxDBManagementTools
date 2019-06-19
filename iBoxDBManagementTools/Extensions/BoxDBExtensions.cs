using iBoxDB.LocalServer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace iBoxDBManagementTools.Extensions
{
    internal static class BoxDBExtensions
    {
        public static DataTable ToDataTable(this List<Local> locals)
        {
            if (locals == null)
                throw new ArgumentNullException(nameof(locals));

            var dt = new DataTable();
            if(locals.Count > 0)
            {
                for (int i = 0; i < locals.Count; i++)
                {
                    var dr = dt.NewRow();
                    var local = locals[i];
                    for (int j = 0; j < local.Count; j++)
                    {
                        if (i == 0 && j == 0)
                        {
                            Console.WriteLine(local.Entity.TableName);
                            dt.Columns.AddRange(local.Keys.Select(k => new DataColumn(k)).ToArray());
                        }
                        dr[j] = local.Values.ElementAt(j);
                    }
                    dt.Rows.Add(dr);
                }
            }
            return dt;
        }
    }
}
