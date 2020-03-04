using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;


namespace GalleriaSupermarket.Reports
{
    [Authorize(Roles ="Admin, Manager")]
    public class ReportRdlcController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        ReportDataSet ds = new ReportDataSet();
        public ActionResult ReportProduct()
        {
            ReportViewer reportViewer = new ReportViewer();
            reportViewer.ProcessingMode = ProcessingMode.Local;
            reportViewer.SizeToReportContent = true;
            reportViewer.Width = Unit.Percentage(100);
            reportViewer.Height = Unit.Percentage(100);

            var connectionString = ConfigurationManager.ConnectionStrings["DbCon"].ConnectionString;


            SqlConnection con = new SqlConnection(connectionString);
            var sqlQuery = "SELECT * FROM AllProduct";

            SqlDataAdapter adp = new SqlDataAdapter(sqlQuery, con);

            adp.Fill(ds, ds.AllProduct.ToString());

            reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"Reports\AllProductSearch.rdlc";
            reportViewer.LocalReport.DataSources.Add(new ReportDataSource("ReportDataSet", ds.Tables[0]));
            ViewBag.ReportViewer = reportViewer;

            return View();
        }


        public ActionResult GetCategoryWiseProduct(string catId)
        {
            ReportViewer reportViewer = new ReportViewer();
            reportViewer.ProcessingMode = ProcessingMode.Local;
            reportViewer.SizeToReportContent = true;
            reportViewer.Width = Unit.Percentage(100);
            reportViewer.Height = Unit.Percentage(100);

            ReportDataSetTableAdapters.SP_GetCatwiseProductTableAdapter ta = new ReportDataSetTableAdapters.SP_GetCatwiseProductTableAdapter();
            ReportDataSet.SP_GetCatwiseProductDataTable dt = new ReportDataSet.SP_GetCatwiseProductDataTable();
            ta.Fill(dt, Convert.ToInt32(catId));
            ReportDataSource rds = new ReportDataSource();
            rds.Name = "CatWiseDataSet";
            rds.Value = dt;
            ReportParameter rp = new ReportParameter("CatID", catId);

            reportViewer.LocalReport.DataSources.Clear();
            reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"Reports\CategoryWiseProduct.rdlc";
            reportViewer.LocalReport.SetParameters(new ReportParameter[] { rp });
            reportViewer.LocalReport.DataSources.Add(rds);
            reportViewer.LocalReport.Refresh();

            ViewBag.ReportViewer = reportViewer;

            return View();

        }
        public ActionResult LocalSaleDaterange(string fromDate, string toDate)
        {
            ReportViewer reportViewer = new ReportViewer();
            reportViewer.ProcessingMode = ProcessingMode.Local;
            reportViewer.SizeToReportContent = true;
            reportViewer.Width = Unit.Percentage(100);
            reportViewer.Height = Unit.Percentage(100);

            ReportDataSetTableAdapters.SP_InternalSaleDaterangeTableAdapter ta = new ReportDataSetTableAdapters.SP_InternalSaleDaterangeTableAdapter();
            ReportDataSet.SP_InternalSaleDaterangeDataTable dt = new ReportDataSet.SP_InternalSaleDaterangeDataTable();
            ta.Fill(dt, Convert.ToDateTime(fromDate), Convert.ToDateTime(toDate));
            ReportDataSource rds = new ReportDataSource();
            rds.Name = "ReportDataSet";
            rds.Value = dt;
            ReportParameter rp = new ReportParameter("fromDate", fromDate);
            ReportParameter rp1 = new ReportParameter("toDate", toDate);

            reportViewer.LocalReport.DataSources.Clear();
            reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"Reports\DaterangeLocalSales.rdlc";
            reportViewer.LocalReport.SetParameters(new ReportParameter[] { rp, rp1 });
            reportViewer.LocalReport.DataSources.Add(rds);
            reportViewer.LocalReport.Refresh();

            ViewBag.ReportViewer = reportViewer;

            return View();

        }


    }
}