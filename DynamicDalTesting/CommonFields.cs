using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DynamicDalTesting
{
    public class CommonFields
    {
        protected int _ID;
        public int ID
        {
            get
            {
                return _ID;
            }
            set
            {
                _ID = value;
            }
        }

        protected bool? _IsDeleted;
        public bool? IsDeleted
        {
            get
            {
                return _IsDeleted;
            }
            set
            {
                _IsDeleted = value;
            }
        }

        protected DateTime? _EnteredDate;
        public DateTime? EnteredDate
        {
            get
            {
                return _EnteredDate;
            }
            set
            {
                _EnteredDate = value;
            }
        }

        protected DateTime? _ModifiedDate;
        public DateTime? ModifiedDate
        {
            get
            {
                return _ModifiedDate;
            }
            set
            {
                _ModifiedDate = value;
            }
        }
    }
}
