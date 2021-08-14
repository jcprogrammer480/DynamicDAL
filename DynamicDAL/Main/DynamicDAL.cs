using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlMagic.Monitor.Items.Results;
using System.Data;

namespace TechnoLegend.Library.DynamicDAL
{
    /// <summary>
    /// A Class which handles Insert,Update,Delete,Select (CRUD) operations for any Class having equivalent DB Entity
    /// </summary>
    public partial class DynamicDAL
    {
        public bool Insert<T>(T objEntity, string pIdentityColumn = "", List<string> columnsToIgnore = null)
        {
            if (_isInitiated == false)
            {
                throw new Exception("Please initiate the DynamicDal first. Use DynamiDal.Init() method.");
            }

            GenericEntity<T> genericInsert = new GenericEntity<T>(SqlOption.Insert, TablePrefix, TableSuffix, pIdentityColumn);
            string insertQuery = genericInsert.GetQuery(objEntity, UpdateParameter.All_Columns, columnsToIgnore);
            SqlResult result = _oSql.Execute(insertQuery, System.Data.CommandType.Text);
            if (result == null)
            {
                return false;
            }
            return result.Success;
        }



        //JC/28-Apr-2018/For Updating specified columns only
        public bool Update<T>(T objEntity, string pIdentityColumn = "", List<string> pPrimaryKeys = null, List<string> pColumns = null, UpdateParameter updateParameterType = UpdateParameter.All_Columns)
        {
            if (_isInitiated == false)
            {
                throw new Exception("Please initiate the DynamicDal first. Use DynamiDal.Init() method.");
            }

            GenericEntity<T> genericUpdate = new GenericEntity<T>(SqlOption.Update, TablePrefix, TableSuffix, pIdentityColumn, pPrimaryKeys);
            string updateQuery = genericUpdate.GetQuery(objEntity, updateParameterType, pColumns);
            SqlResult result = _oSql.Execute(updateQuery, System.Data.CommandType.Text);
            if (result == null)
            {
                return false;
            }
            return result.Success;
        }


        public bool Delete<T>(T objEntity, List<string> pPrimaryKeys = null)
        {
            if (_isInitiated == false)
            {
                throw new Exception("Please initiate the DynamicDal first. Use DynamiDal.Init() method.");
            }

            GenericEntity<T> genericDelete = new GenericEntity<T>(SqlOption.Delete, TablePrefix, TableSuffix, "", pPrimaryKeys);
            string deleteQuery = genericDelete.GetQuery(objEntity);
            SqlResult result = _oSql.Execute(deleteQuery, System.Data.CommandType.Text);
            if (result == null)
            {
                return false;
            }
            return result.Success;
        }

        public List<T> SelectList<T>(int pTop = 0, string pCustomeWhere = "", Dictionary<string, OrderBy> pOrderBy = null)
        {

            if (_isInitiated == false)
            {
                throw new Exception("Please initiate the DynamicDal first. Use DynamiDal.Init() method.");
            }
            Type type = typeof(T);
            List<T> listToReturn = null;
            GenericEntity<T> genericSelect = new GenericEntity<T>(SqlOption.Select, TablePrefix, TableSuffix);
            string selectQuery = genericSelect.GetQuery(pTop, "", pCustomeWhere, pOrderBy);
            SqlResultWithDataSet oResult = _oSql.Open(selectQuery);
            listToReturn = DataTableToList<T>(oResult.Results.Tables[0]);

            return listToReturn;
        }

        public DataTable SelectDataTable<T>(int pTop = 0, string pCustomeWhere = "", Dictionary<string, OrderBy> pOrderBy = null)
        {

            if (_isInitiated == false)
            {
                throw new Exception("Please initiate the DynamicDal first. Use DynamiDal.Init() method.");
            }
            Type type = typeof(T);
            GenericEntity<T> genericSelect = new GenericEntity<T>(SqlOption.Select, TablePrefix, TableSuffix);
            string selectQuery = genericSelect.GetQuery(pTop, "", pCustomeWhere, pOrderBy);
            SqlResultWithDataSet oResult = _oSql.Open(selectQuery);
            return oResult.Results.Tables[0];
        }

        public DataTable SelectDataTable<T>(int pTop = 0, string pCustomColumns = "", string pCustomeWhere = "", Dictionary<string, OrderBy> pOrderBy = null)
        {

            if (_isInitiated == false)
            {
                throw new Exception("Please initiate the DynamicDal first. Use DynamiDal.Init() method.");
            }
            Type type = typeof(T);
            GenericEntity<T> genericSelect = new GenericEntity<T>(SqlOption.Select, TablePrefix, TableSuffix);
            string selectQuery = genericSelect.GetQuery(pTop, pCustomColumns, pCustomeWhere, pOrderBy);
            SqlResultWithDataSet oResult = _oSql.Open(selectQuery);
            return oResult.Results.Tables[0];
        }

        public DataTable SelectCustom(string pSelectQuery)
        {
            if (_isInitiated == false)
            {
                throw new Exception("Please initiate the DynamicDal first. Use DynamiDal.Init() method.");
            }
            SqlResultWithDataSet oResult = _oSql.Open(pSelectQuery);
            return oResult.Results.Tables[0];
        }

        public bool SelectScalar(string pSelectQuery, out object pOutput)
        {
            pOutput = null;
            if (_isInitiated == false)
            {
                throw new Exception("Please initiate the DynamicDal first. Use DynamiDal.Init() method.");
            }
            SqlResultWithDataSet oResult = _oSql.Open(pSelectQuery);

            if (oResult.Results.Tables[0] == null)
            {
                return false;
            }
            DataTable res = oResult.Results.Tables[0];
            if (res.Rows.Count > 0)
            {
                pOutput = res.Rows[0][0];
                return true;
            }

            return false;
        }
    }
}
