using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;

namespace Anne.Service
{

    public class DB
    {
        
        public static List<T> ListData<T>(string pSPNm, SqlParameter[] pSqlParams) where T : new()
        {
            List<T> returnData = new List<T>();

            using (SqlConnection conn = new SqlConnection("connString")) {

                SqlCommand cmd = new SqlCommand(pSPNm, conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddRange(pSqlParams);

                conn.Open();
                SqlDataReader sqlDataReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                List<string> cols = new List<string>();

                for (int i = 0; i < sqlDataReader.VisibleFieldCount; i++) {
                    cols.Add(sqlDataReader.GetName(i));
                }

                System.Reflection.PropertyInfo[] pi = typeof(T).GetProperties();

                while (sqlDataReader.Read()) {

                    T t = new T();

                    foreach (var p in pi.Where(p => cols.Contains(p.Name))) {
                        p.SetValue(t, sqlDataReader[p.Name].ToString(), null);
                    }

                    returnData.Add(t);
                }
            }

            return returnData;
        }
    }
}
