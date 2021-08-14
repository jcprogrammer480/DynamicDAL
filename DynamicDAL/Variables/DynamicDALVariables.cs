using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlMagic.Monitor.Sql;

namespace TechnoLegend.Library.DynamicDAL
{
    public partial class DynamicDAL
    {
        //This Sql Object will be used through out for any opertion realted to SQL
        private Sql _oSql;

        private string _DBServer = string.Empty;
        public string DBServer
        {
            get
            {
                return _DBServer;
            }
            set
            {
                _DBServer = value;
            }
        }

        private string _UserName = string.Empty;
        public string UserName
        {
            get
            {
                return _UserName;
            }
            set
            {
                _UserName = value;
            }
        }

        private string _Password = string.Empty;
        public string Password
        {
            get
            {
                return _Password;
            }
            set
            {
                _Password = value;
            }
        }

        private string _DBName = string.Empty;
        public string DBName
        {
            get
            {
                return _DBName;
            }
            set
            {
                _DBName = value;
            }
        }

        private string _ConnectionString = string.Empty;
        public string ConnectionString
        {
            get
            {
                if (_ConnectionString == string.Empty)
                {
                    _ConnectionString = MakeConnectionString(DBServer, UserName, Password, DBName);
                }
                return _ConnectionString;
            }
            set
            {
                _ConnectionString = value;
            }
        }

        public string TablePrefix { get; set; } = string.Empty;
        public string TableSuffix { get; set; } = string.Empty;

        private bool _isInitiated = false;
    }
}
