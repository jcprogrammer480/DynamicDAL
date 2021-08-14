using SqlMagic.Monitor.Sql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechnoLegend.Library.DynamicDAL
{
    public partial class DynamicDAL
    {
        static string MakeConnectionString(string pDBServer, string pUserName, string pPassword, string pDBName)
        {
            string result;
            result = "Data Source= " + pDBServer + ";Initial Catalog=" + pDBName + ";Persist Security Info=True;User ID=" + pUserName + ";Password=" + pPassword + ";Connection Timeout=30";
            return result;
        }

        public bool TestConnection(Sql pOSql)
        {
            bool result = false;

            SqlConnection objSqlCon = null;
            pOSql.TryCreateConnection(out objSqlCon);
            if (objSqlCon != null)
            {
                objSqlCon.Close();
                result = true;
            }
            else
            {
                result = false;
            }
            return result;
        }
        public bool Init()
        {
            bool result = false;
            _oSql = new Sql(ConnectionString, 0, true);
            result = TestConnection(_oSql);
            if (result == true)
            {
                _isInitiated = true;
            }
            return result;
        }


        /// <summary>
        /// Converts a DataTable to a list with generic objects
        /// </summary>
        /// <typeparam name="T">Generic object</typeparam>
        /// <param name="table">DataTable</param>
        /// <returns>List with generic objects</returns>
        public List<T> DataTableToList<T>(DataTable table)
        {
            try
            {
                List<T> list = new List<T>();

                foreach (var row in table.AsEnumerable())
                {
                    var obj = ToObject<T>(row);

                    list.Add(obj);
                }

                return list;
            }
            catch
            {
                return null;
            }
        }

        public static T ToObject<T>(DataRow row)
        {
            T obj = Activator.CreateInstance<T>();

            foreach (var prop in obj.GetType().GetProperties())
            {
                try
                {
                    if (prop.PropertyType.IsGenericType && prop.PropertyType.Name.Contains("Nullable"))
                    {
                        if (!string.IsNullOrEmpty(row[prop.Name].ToString()))
                            prop.SetValue(obj, Convert.ChangeType(row[prop.Name], Nullable.GetUnderlyingType(prop.PropertyType)), null);
                        //else do nothing
                    }
                    else
                        prop.SetValue(obj, Convert.ChangeType(row[prop.Name], prop.PropertyType), null);
                }
                catch
                {
                    continue;
                }
            }
            return obj;
        }
        //private bool CheckInitiation()
        //{
        //    if (_isInitiated == true)
        //    {
        //        return true;
        //    }

        //}

    }
}
