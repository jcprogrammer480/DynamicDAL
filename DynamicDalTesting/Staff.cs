using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DynamicDalTesting
{
    public class Staff : CommonFields
    {
        string _CategoryName;
        public string CategoryName
        {
            get
            {
                return _CategoryName; ;
            }
            set
            {
                _CategoryName = value;
            }
        }

        string _DesignationName;
        public string DesignationName
        {
            get
            {
                return _DesignationName;
            }
            set
            {
                _DesignationName = value;
            }
        }

        string _Name;
        public string Name
        {
            get
            {
                return _Name;
            }
            set
            {
                _Name = value;
            }
        }


        string _Image;
        public string Image
        {
            get
            {
                return _Image;

            }
            set
            {
                _Image = value;
            }
        }

        string _Qualification;
        public string Qualification
        {
            get
            {
                return _Qualification;
            }
            set
            {
                _Qualification = value;
            }
        }
    }
}
