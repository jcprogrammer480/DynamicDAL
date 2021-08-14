using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace TechnoLegend.Library.DynamicDAL
{
    public class GenericEntity<T>
    {
        SqlOption _sqlOption;
        string _tablePrefix = string.Empty;
        string _tableSuffix = string.Empty;
        string _entityName;
        string _identityColumn = string.Empty;
        List<string> _primaryKeys;
        public GenericEntity()
        {
            _sqlOption = SqlOption.Select;
            Type type = typeof(T);
            _entityName = type.Name.Replace("Entity", string.Empty);
        }
        public GenericEntity(SqlOption psqlOption) : this(psqlOption, "")
        {

        }
        public GenericEntity(SqlOption psqlOption, string pTablePrefix) : this(psqlOption, pTablePrefix, "")
        {

        }
        public GenericEntity(SqlOption psqlOption, string pTablePrefix, string pTableSuffix) : this(psqlOption, pTablePrefix, pTableSuffix, "")
        {

        }
        public GenericEntity(SqlOption psqlOption, string pTablePrefix, string pTableSuffix, string pIdentityCol) : this(psqlOption, pTablePrefix, pTableSuffix, pIdentityCol, null)
        {

        }
        public GenericEntity(SqlOption psqlOption, string pTablePrefix, string pTableSuffix, string pIdentityCol, List<string> pPrimaryKeys)
        {
            _sqlOption = psqlOption;
            _tablePrefix = pTablePrefix;
            _tableSuffix = pTableSuffix;
            _identityColumn = pIdentityCol;
            _primaryKeys = pPrimaryKeys;
            Type type = typeof(T);
            _entityName = _tablePrefix + type.Name.Replace("Entity", string.Empty) + _tableSuffix;
        }

        public string GetQuery(int pTop = 0, string pCustomCols = "", string pCustomeWhere = "", Dictionary<string, OrderBy> pOrderBy = null)
        {
            if (_sqlOption == SqlOption.Select)
                return GetSelect(pTop, pCustomCols, pCustomeWhere, pOrderBy);
            else if (_sqlOption == SqlOption.Update)
                return GetUpdate();
            else if (_sqlOption == SqlOption.Delete)
                return GetDelete();
            else
                return GetInsert();
        }

        public string GetQuery(T pObj, UpdateParameter updateParameterType = UpdateParameter.All_Columns, List<string> pColumns = null)
        {
            if (_sqlOption == SqlOption.Select)
                return GetSelect();
            else if (_sqlOption == SqlOption.Update)
            {
                return GetUpdate(pObj, updateParameterType, pColumns);
            }
            else if (_sqlOption == SqlOption.Delete)
                return GetDelete(pObj);
            else
                return GetInsert(pObj, pColumns);
        }

        private string GetDelete()
        {
            Type type = typeof(T);
            StringBuilder sbQry = new StringBuilder();
            System.Reflection.PropertyInfo[] propInfo = type.GetProperties();

            sbQry.AppendFormat("Delete From {0} Where {1}={2}", _entityName, propInfo[0].Name, "{0}");

            return sbQry.ToString();
        }

        /// <summary>
        /// Jayesh Chauhan
        /// 20-Jan-2018
        /// To get delete query for perticular object. The primary key columns's property should be the first one
        /// </summary>
        /// <param name="pObj">object for which the quey will be generated</param>
        /// <returns></returns>
        private string GetDelete(T pObj)
        {
            Type type = typeof(T);
            StringBuilder sbQry = new StringBuilder();
            List<PropertyInfo> propInfo = type.GetProperties().ToList();

            foreach (string primaryKey in _primaryKeys)
            {
                PropertyInfo pi = propInfo.FirstOrDefault(prop => prop.Name.ToLower() == primaryKey.ToLower());
                if (pi == null)
                {
                    throw new Exception("Primary property named '" + primaryKey + "' not found. Make sure your class have mentioned property.");
                }

                object objPriPropertyVal = pi.GetValue(pObj, null);
                if (objPriPropertyVal == null)
                {
                    throw new Exception("Primary property value should not be null.");
                }
                string value = "'" + objPriPropertyVal.ToString() + "'";
                if (sbQry.ToString() == string.Empty)
                    sbQry.AppendFormat("Delete From {0} Where ({1}={2}", _entityName, pi.Name, value);
                else
                    sbQry.AppendFormat(" and {0}={1} ", pi.Name, value);
            }
            if (sbQry.ToString() != string.Empty)
            {
                sbQry.Append(")");
            }

            return sbQry.ToString();
        }


        private string GetUpdate()
        {
            int ctr = 0;
            Type type = typeof(T);
            StringBuilder sbQry = new StringBuilder();
            System.Reflection.PropertyInfo[] propInfo = type.GetProperties();
            foreach (System.Reflection.PropertyInfo pi in propInfo)
            {
                //we can not consider an identity column for insert or update operation
                if (pi.Name.ToLower() == _identityColumn.ToLower())
                {
                    continue;
                }
                if (sbQry.ToString() == string.Empty)
                    sbQry.AppendFormat("Update {0} Set {1}={2}", _entityName, pi.Name, "[" + ctr + "]");
                else
                    sbQry.AppendFormat(", {0}={1}", pi.Name, "[" + ctr + "]");

                ctr++;
            }

            if (sbQry.ToString() != string.Empty)
                sbQry.AppendFormat(" Where {0}={1} ", propInfo[0].Name, "[" + ctr + "]");

            sbQry.Replace("[", "{").Replace("]", "}");

            return sbQry.ToString();
        }



        private string GetUpdate(T pObj, UpdateParameter updateParameterType, List<string> columns)
        {
            switch (updateParameterType)
            {
                case UpdateParameter.All_Columns:
                    return GetUpdate(pObj);

                case UpdateParameter.Columns_To_Update:
                    return GetUpdateIncludeColumns(pObj, columns);
                case UpdateParameter.Columns_To_Ignore:
                    return GetUpdateIgnoreColumns(pObj, columns);
                default:
                    return GetUpdate(pObj);
            }
        }

        /// <summary>
        /// Jayesh Chauhan
        /// 20-Jan-2018
        /// To get update query for perticular object. The primary key columns's property should be the first one
        /// </summary>
        /// <param name="pObj">object for which the quey will be generated</param>
        /// <returns></returns>
        private string GetUpdate(T pObj)
        {
            Type type = typeof(T);
            StringBuilder sbQry = new StringBuilder();
            List<PropertyInfo> propInfo = type.GetProperties().ToList();
            foreach (System.Reflection.PropertyInfo pi in propInfo)
            {
                //we can not consider an identity column for insert or update operation
                if ((pi.Name.ToLower() == _identityColumn.ToLower()) || _primaryKeys.FirstOrDefault(key => key.ToLower() == pi.Name.ToLower()) != null)
                {
                    continue;
                }
                object propVal = pi.GetValue(pObj, null);
                string value = propVal == null ? "NULL" : "'" + propVal.ToString() + "'";
                if (sbQry.ToString() == string.Empty)
                    sbQry.AppendFormat("Update {0} Set {1}={2}", _entityName, pi.Name, value);
                else
                    sbQry.AppendFormat(", {0}={1}", pi.Name, value);

            }
            StringBuilder sbWhere = new StringBuilder();
            foreach (string primaryKey in _primaryKeys)
            {
                PropertyInfo pi = propInfo.FirstOrDefault(prop => prop.Name.ToLower() == primaryKey.ToLower());
                if (pi == null)
                {
                    throw new Exception("Primary property named '" + primaryKey + "' not found. Make sure your class have mentioned property.");
                }

                object objPriPropertyVal = pi.GetValue(pObj, null);
                if (objPriPropertyVal == null)
                {
                    throw new Exception("Primary property value should not be null.");
                }
                string value = "'" + objPriPropertyVal.ToString() + "'";
                if (sbWhere.ToString() == string.Empty)
                    sbWhere.AppendFormat(" Where ({0}={1}", pi.Name, value);
                else
                    sbWhere.AppendFormat(" and {0}={1} ", pi.Name, value);
            }
            if (sbWhere.ToString() != string.Empty)
            {
                sbWhere.Append(")");
            }
            sbQry.Append(sbWhere);
            sbQry.Replace("[", "{").Replace("]", "}");

            return sbQry.ToString();
        }


        private string GetUpdateIncludeColumns(T pObj, List<string> pColumnsToUpdate)
        {
            if (pColumnsToUpdate == null)
            {
                throw new Exception("Please specify the columns to be updated in pColumnsToUpdate list!!!");
            }

            Type type = typeof(T);
            StringBuilder sbQry = new StringBuilder();
            List<PropertyInfo> propInfo = type.GetProperties().ToList();

            foreach (string column in pColumnsToUpdate)
            {
                PropertyInfo propForColumn = propInfo.FirstOrDefault(prop => prop.Name.ToLower() == column.ToLower());
                if (propForColumn == null)
                {
                    throw new Exception("The column " + column + " must be available in object's property!!!");
                }
                if ((propForColumn.Name.ToLower() == _identityColumn.ToLower()) || _primaryKeys.FirstOrDefault(key => key.ToLower() == propForColumn.Name.ToLower()) != null)
                {
                    throw new Exception("The column " + column + " must not be primary key of Identity column.!!!");
                }

                object propVal = propForColumn.GetValue(pObj, null);
                string value = propVal == null ? "NULL" : "'" + propVal.ToString() + "'";
                if (sbQry.ToString() == string.Empty)
                    sbQry.AppendFormat("Update {0} Set {1}={2}", _entityName, propForColumn.Name, value);
                else
                    sbQry.AppendFormat(", {0}={1}", propForColumn.Name, value);
            }

            StringBuilder sbWhere = new StringBuilder();
            foreach (string primaryKey in _primaryKeys)
            {
                PropertyInfo pi = propInfo.FirstOrDefault(prop => prop.Name.ToLower() == primaryKey.ToLower());
                if (pi == null)
                {
                    throw new Exception("Primary property named '" + primaryKey + "' not found. Make sure your class have mentioned property.");
                }

                object objPriPropertyVal = pi.GetValue(pObj, null);
                if (objPriPropertyVal == null)
                {
                    throw new Exception("Primary property value should not be null.");
                }
                string value = "'" + objPriPropertyVal.ToString() + "'";
                if (sbWhere.ToString() == string.Empty)
                    sbWhere.AppendFormat(" Where ({0}={1}", pi.Name, value);
                else
                    sbWhere.AppendFormat(" and {0}={1} ", pi.Name, value);
            }
            if (sbWhere.ToString() != string.Empty)
            {
                sbWhere.Append(")");
            }
            sbQry.Append(sbWhere);
            sbQry.Replace("[", "{").Replace("]", "}");

            return sbQry.ToString();
        }

        private string GetUpdateIgnoreColumns(T pObj, List<string> columnsToIgnore)
        {
            if (columnsToIgnore == null)
            {
                throw new Exception("Please specify the columns to be ignored in columnsToIgnore list!!!");
            }
            Type type = typeof(T);
            StringBuilder sbQry = new StringBuilder();
            List<PropertyInfo> propInfo = type.GetProperties().ToList();


            foreach (System.Reflection.PropertyInfo pi in propInfo)
            {
                //we can not consider an identity column for insert or update operation
                if ((pi.Name.ToLower() == _identityColumn.ToLower()) || (_primaryKeys.FirstOrDefault(key => key.ToLower() == pi.Name.ToLower()) != null) || (columnsToIgnore.FirstOrDefault(col => col.ToLower() == pi.Name.ToLower()) != null)
                    )
                {
                    continue;
                }


                object propVal = pi.GetValue(pObj, null);
                string value = propVal == null ? "NULL" : "'" + propVal.ToString() + "'";
                if (sbQry.ToString() == string.Empty)
                    sbQry.AppendFormat("Update {0} Set {1}={2}", _entityName, pi.Name, value);
                else
                    sbQry.AppendFormat(", {0}={1}", pi.Name, value);

            }
            StringBuilder sbWhere = new StringBuilder();
            foreach (string primaryKey in _primaryKeys)
            {
                PropertyInfo pi = propInfo.FirstOrDefault(prop => prop.Name.ToLower() == primaryKey.ToLower());
                if (pi == null)
                {
                    throw new Exception("Primary property named '" + primaryKey + "' not found. Make sure your class have mentioned property.");
                }

                object objPriPropertyVal = pi.GetValue(pObj, null);
                if (objPriPropertyVal == null)
                {
                    throw new Exception("Primary property value should not be null.");
                }
                string value = "'" + objPriPropertyVal.ToString() + "'";
                if (sbWhere.ToString() == string.Empty)
                    sbWhere.AppendFormat(" Where ({0}={1}", pi.Name, value);
                else
                    sbWhere.AppendFormat(" and {0}={1} ", pi.Name, value);
            }
            if (sbWhere.ToString() != string.Empty)
            {
                sbWhere.Append(")");
            }
            sbQry.Append(sbWhere);
            sbQry.Replace("[", "{").Replace("]", "}");

            return sbQry.ToString();
        }

        private string GetInsert()
        {
            StringBuilder sb = new StringBuilder();
            int ctr = 0;

            Type type = typeof(T);
            StringBuilder sbQry = new StringBuilder();
            System.Reflection.PropertyInfo[] propInfo = type.GetProperties();
            foreach (System.Reflection.PropertyInfo pi in propInfo)
            {
                //we can not consider an identity column for insert or update operation
                if (pi.Name.ToLower() == _identityColumn.ToLower())
                {
                    continue;
                }
                if (sbQry.ToString() == string.Empty)
                    sbQry.AppendFormat("INSERT INTO {0} ({1}", _entityName, pi.Name);
                else
                {
                    sbQry.AppendFormat(", {0}", pi.Name);
                    sb.Append(",");
                }
                sb.Append("{" + ctr++ + "}");
            }

            if (sbQry.ToString() != string.Empty)
                sbQry.AppendFormat(") VALUES({0})", sb.ToString());

            return sbQry.ToString();
        }

        /// <summary>
        /// Jayesh Chauhan
        /// 20-Jan-2018
        /// To get insert query for perticular object. The primary key columns's property should be the first one
        /// </summary>
        /// <param name="pObj">object for which the quey will be generated</param>
        /// <returns></returns>
        private string GetInsert(object pObj, List<string> columnsToIgnore = null)
        {
            StringBuilder sb = new StringBuilder();

            Type type = typeof(T);
            StringBuilder sbQry = new StringBuilder();
            System.Reflection.PropertyInfo[] propInfo = type.GetProperties();
            foreach (System.Reflection.PropertyInfo pi in propInfo)
            {
                //we can not consider an identity column for insert or update operation
                if (pi.Name.ToLower() == _identityColumn.ToLower())
                {
                    continue;
                }
                //Check if property/columns contained in ignore list or not
                if (columnsToIgnore != null && (columnsToIgnore.FirstOrDefault(col => col.ToLower() == pi.Name.ToLower()) != null))
                {
                    continue;
                }

                object propVal = pi.GetValue(pObj, null);
                string value = propVal == null ? "NULL" : "'" + propVal.ToString() + "'";

                if (sbQry.ToString() == string.Empty)
                    sbQry.AppendFormat("INSERT INTO {0} ({1}", _entityName, pi.Name);
                else
                {
                    sbQry.AppendFormat(", {0}", pi.Name);
                    sb.Append(",");
                }
                sb.Append(value);
            }

            if (sbQry.ToString() != string.Empty)
                sbQry.AppendFormat(") VALUES({0})", sb.ToString());

            return sbQry.ToString();
        }

        private string GetSelect(int pTop = 0, string pCustomCols = "", string pCustomeWhere = "", Dictionary<string, OrderBy> pOrderBy = null)
        {

            Type type = typeof(T);
            StringBuilder sbQry = new StringBuilder();
            PropertyInfo[] propInfo = type.GetProperties();
            string topQry = pTop == 0 ? "" : "top(" + pTop.ToString() + ")";
            string whereQry = pCustomeWhere == "" ? "" : "where " + pCustomeWhere;
            string orderByQry = string.Empty;
            if (pOrderBy != null)
            {
                orderByQry = "order by ";
                foreach (KeyValuePair<string, OrderBy> pair in pOrderBy)
                {
                    string order = string.Empty;
                    switch (pair.Value)
                    {
                        case OrderBy.Ascending:
                            order = "asc";
                            break;
                        case OrderBy.Descending:
                            order = "desc";
                            break;
                        default:
                            order = "asc";
                            break;
                    }
                    orderByQry += pair.Key + " " + order + ",";
                }
                orderByQry = orderByQry.TrimEnd(',');
            }


            if (pCustomCols == "")
            {
                foreach (System.Reflection.PropertyInfo pi in propInfo)
                {
                    if (sbQry.ToString() == string.Empty)
                        sbQry.AppendFormat("Select {0} {1}", topQry, pi.Name);
                    else
                        sbQry.AppendFormat(", {0}", pi.Name);
                }
            }
            else
            {
                if (sbQry.ToString() == string.Empty)
                    sbQry.AppendFormat("Select {0} {1}", topQry, pCustomCols);
                else
                    sbQry.AppendFormat(", {0}", pCustomCols);
            }



            if (sbQry.ToString() != string.Empty)
            {
                sbQry.AppendFormat(" From {0} ", _entityName);
                sbQry.Append(" " + whereQry);
                sbQry.Append(" " + orderByQry);
            }

            return sbQry.ToString();
        }
    }

    public enum SqlOption
    {
        Select,
        Insert,
        Update,
        Delete
    }

    public enum OrderBy
    {
        Ascending,
        Descending
    }
}
