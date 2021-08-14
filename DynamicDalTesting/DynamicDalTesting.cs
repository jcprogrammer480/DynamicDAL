using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechnoLegend.Library.DynamicDAL;

namespace DynamicDalTesting
{
    class DynamicDalTesting
    {
        public void TestDynamicDal()
        {
            DynamicDAL dal = new DynamicDAL();
            dal.DBServer = @"JAYESH\SQLEXPRESS";
            dal.DBName = "BABS";
            dal.UserName = "sa";
            dal.Password = "jsc11892";
            dal.TablePrefix = "tbl_";
            if (dal.Init())
            {
                Console.WriteLine("DAl initiated successfully.");
            }
            else
            {
                Console.WriteLine("Failed to initialize DAL.");
            }
            Console.Read();
            //TestInsert(dal);
            TestInsertIgnoreColumns(dal);

            //TestUpdate(dal);
            //TestUpdateIncludeColumns(dal);
            //TestUpdateIgnoreColumns(dal);

            //TestSelect(dal);

            //TestDelete(dal);
        }

        private void TestInsert(DynamicDAL pDal)
        {
            Staff oStaff = new Staff();
            oStaff.CategoryName = "JKG";
            oStaff.DesignationName = "Teacher";
            oStaff.EnteredDate = DateTime.Now;
            oStaff.ID = new Random(20).Next();
            oStaff.ModifiedDate = DateTime.Now;
            oStaff.Name = "kanviji";
            oStaff.Qualification = "MCA";


            pDal.Insert<Staff>(oStaff, "ID");

        }

        private void TestInsertIgnoreColumns(DynamicDAL pDal)
        {
            Staff oStaff = new Staff();
            oStaff.CategoryName = "JKG";
            oStaff.DesignationName = "Teacher";
            oStaff.EnteredDate = DateTime.Now;

            oStaff.ModifiedDate = DateTime.Now;
            oStaff.Name = "shaktiman";
            oStaff.Qualification = "MCA";

            List<string> columnsToIgnore = new List<string>();
            columnsToIgnore.Add("Qualification"); //Qaulitication will not be included in insert query

            pDal.Insert<Staff>(oStaff, "ID", columnsToIgnore);

        }

        private void TestUpdate(DynamicDAL pDal)
        {
            Staff oStaff = new Staff();
            oStaff.ID = 11;

            oStaff.CategoryName = "dont know";
            oStaff.DesignationName = "professor";
            oStaff.EnteredDate = DateTime.Now;

            oStaff.ModifiedDate = DateTime.Now;
            oStaff.Name = "sheetal chauhan";
            oStaff.Qualification = "B.ed";
            List<string> primaryKeys = new List<string>();
            primaryKeys.Add("ID");

            pDal.Update<Staff>(oStaff, "ID", primaryKeys);

        }

        private void TestUpdateIncludeColumns(DynamicDAL pDal)
        {
            Staff oStaff = new Staff();
            oStaff.ID = 14;
            oStaff.CategoryName = "philosopher";
            oStaff.Name = "jayesh chauhan";

            List<string> primaryKeys = new List<string>();
            primaryKeys.Add("ID");

            List<string> colsToInclude = new List<string>();
            colsToInclude.Add("Name");
            colsToInclude.Add("CategoryName");

            pDal.Update<Staff>(oStaff, "ID", primaryKeys, colsToInclude, UpdateParameter.Columns_To_Update);

        }

        private void TestUpdateIgnoreColumns(DynamicDAL pDal)
        {
            Staff oStaff = new Staff();
            oStaff.ID = 14;
            oStaff.CategoryName = "philosopher";
            oStaff.Name = "#JC#";
            oStaff.IsDeleted = true;
            oStaff.Qualification = "B.E";

            List<string> primaryKeys = new List<string>();
            primaryKeys.Add("ID");

            List<string> colsToIgnore = new List<string>();
            colsToIgnore.Add("IsDeleted");
            colsToIgnore.Add("DesignationName");
            colsToIgnore.Add("Image");
            colsToIgnore.Add("EnteredDate");
            colsToIgnore.Add("ModifiedDate");

            pDal.Update<Staff>(oStaff, "ID", primaryKeys, colsToIgnore, UpdateParameter.Columns_To_Ignore);

        }

        private void TestDelete(DynamicDAL pDal)
        {
            Staff oStaff = new Staff();
            oStaff.CategoryName = "dont know";
            oStaff.DesignationName = "professor";
            oStaff.EnteredDate = DateTime.Now;
            oStaff.ID = 8;
            oStaff.ModifiedDate = DateTime.Now;
            oStaff.Name = "kanvi savani";
            oStaff.Qualification = "B.ed";
            List<string> primaryKeys = new List<string>();
            primaryKeys.Add("ID");
            primaryKeys.Add("Name");

            pDal.Delete<Staff>(oStaff, primaryKeys);

        }

        private void TestSelect(DynamicDAL pDal)
        {
            List<Staff> staffList = pDal.SelectList<Staff>();
            Dictionary<string, OrderBy> orderBy = new Dictionary<string, OrderBy>();
            orderBy.Add("ID", OrderBy.Descending);

            DataTable result = pDal.SelectDataTable<Staff>(2, "ID,IsDeleted,Name", "ID = '16'", orderBy);

            result = pDal.SelectCustom("select max(ID) from tbl_Staff");

            object intres;
            pDal.SelectScalar("select ID as 'ID' from tbl_Staff where 1=2", out intres);

        }

    }
}
