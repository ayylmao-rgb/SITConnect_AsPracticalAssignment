using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SITConnect_AsPracticalAssignment
{
    public partial class buystationary : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserID"] != null && Session["AuthToken"] != null && Request.Cookies["AuthToken"] != null)
            {
                if (!Session["AuthToken"].ToString().Equals(Request.Cookies["AuthToken"].Value))
                {
                    Response.Redirect("Login.aspx", false);
                }
                else
                {
                    if (!IsPostBack)
                    {

                        DataSet dset = new DataSet();
                        SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["MYDBConnection"].ToString());
                        using (conn)
                        {
                            conn.Open();
                            SqlDataAdapter adapter = new SqlDataAdapter();
                            SqlCommand cmd = new SqlCommand("SELECT stationaryID, nameS, brandS, priceS FROM StationarySIT", conn);
                            cmd.CommandType = CommandType.Text;
                            adapter.SelectCommand = cmd;
                            adapter.Fill(dset);
                            gvUserInfo.DataSource = dset;
                            gvUserInfo.DataBind();
                        }
                    }
                }
            }
            else
            {
                Response.Redirect("Login.aspx", false);
            }


        }
        protected void BtnSubmit_Click(object sender, EventArgs e)
        {
            DataSet dset = new DataSet();
            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["MYDBConnection"].ToString());
            using (conn)
            {
                if (txtUserID.Text != string.Empty)
                {
                    try
                    {
                        conn.Open();
                        SqlDataAdapter adapter = new SqlDataAdapter();
                        string sqlQuery = string.Format("SELECT stationaryID, nameS, brandS, priceS FROM StationarySIT WHERE stationaryID =@0", txtUserID.Text);
                        SqlCommand cmd = new SqlCommand(sqlQuery, conn);
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@0", txtUserID.Text);
                        adapter.SelectCommand = cmd;
                        adapter.Fill(dset);
                        gvUserInfo.DataSource = dset;
                        gvUserInfo.DataBind();
                    }
                    catch (SqlException ex)
                    {
                        Label1.Text = "Invalid search input!";
                    }
                }
            }
        }
    }
}