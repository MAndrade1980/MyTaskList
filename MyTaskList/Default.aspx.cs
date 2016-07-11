using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using Microsoft.AspNet.Identity;

namespace MyTaskList
{
    public partial class _Default : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            // Check authentication
            if (!this.User.Identity.IsAuthenticated)
            {
                Response.Redirect("/Account/Login");
                return;
            }
            

            if (!Page.IsPostBack)
            {

                if (!string.IsNullOrEmpty(Request.QueryString["action"]) && (Request.QueryString["action"].ToString() == "sortItems"))
                {
                    if (!string.IsNullOrEmpty(Request.QueryString["data"])) {
                        // Resort items
                        OrderTasks(Request.QueryString["data"]);
                    }

                }

                LoadTasks();

            }



        }

        protected void AddNewTaskButton_Click(object sender, EventArgs e)
        {
            // Add the task
            AddNewTask();

            // Re-bind the grid
            LoadTasks();

            // Update status label
            StatusLabel.ForeColor = System.Drawing.Color.Green;
            StatusLabel.Text = "Task added successfully!";
        }

        protected void DeleteTask_Click(object sender, ImageClickEventArgs e)
        {
            // Delete task item
            DeleteTask((ImageButton)sender);

            // Re-bind the grid
            LoadTasks();

            // Update status label
            StatusLabel.ForeColor = System.Drawing.Color.Green;
            StatusLabel.Text = "Task deleted successfully!";

        }



        private void LoadTasks()
        {
            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
            conn.Open();
            SqlCommand cmd = new SqlCommand("dbo.GetTasks", conn);

            cmd.CommandType = CommandType.StoredProcedure;
            SqlDataAdapter adp = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();

            SqlParameter p1 = cmd.CreateParameter();
            p1.ParameterName = "UserId";
            p1.DbType = DbType.String;
            p1.Value = this.User.Identity.GetUserId();
            cmd.Parameters.Add(p1);

            SqlParameter p2 = cmd.CreateParameter();
            p2.ParameterName = "Active";
            p2.DbType = DbType.Boolean;
            p2.Value = (ActiveOnly.Checked || BothStatus.Checked);
            cmd.Parameters.Add(p2);

            SqlParameter p3 = cmd.CreateParameter();
            p3.ParameterName = "Completed";
            p3.DbType = DbType.Boolean;
            p3.Value = (CompletedOnly.Checked || BothStatus.Checked);
            cmd.Parameters.Add(p3);

            adp.Fill(ds);

            conn.Close();
            
            TasksGrid.DataSource = ds;
            TasksGrid.DataBind();

        }

        private void AddNewTask()
        {
            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
            conn.Open();
            SqlCommand cmd = new SqlCommand("dbo.AddNewTask", conn);

            cmd.CommandType = CommandType.StoredProcedure;
            
            SqlParameter p1 = cmd.CreateParameter();
            p1.ParameterName = "Name";
            p1.DbType = DbType.String;
            p1.Value = TaskName.Text.Trim();
            cmd.Parameters.Add(p1);

            SqlParameter p2 = cmd.CreateParameter();
            p2.ParameterName = "UserId";
            p2.DbType = DbType.String;
            p2.Value = this.User.Identity.GetUserId();
            cmd.Parameters.Add(p2);

            SqlParameter p3 = cmd.CreateParameter();
            p3.ParameterName = "BackColor";
            p3.DbType = DbType.String;
            p3.Value = ColorList.SelectedValue;
            cmd.Parameters.Add(p3);

            cmd.ExecuteNonQuery();

            conn.Close();

            TaskName.Text = "";
        }

        private void OrderTasks(String ids)
        {

            ids = ids.Trim('|');

            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
            conn.Open();
            SqlCommand cmd = new SqlCommand("dbo.OrderTasks", conn);

            cmd.CommandType = CommandType.StoredProcedure;

            SqlParameter p1 = cmd.CreateParameter();
            p1.ParameterName = "data";
            p1.DbType = DbType.String;
            p1.Value = ids;
            cmd.Parameters.Add(p1);

            cmd.ExecuteNonQuery();

            conn.Close();

        }

        private void DeleteTask(ImageButton btn)
        {

            HiddenField TaskIdDelete = (HiddenField)btn.Parent.FindControl("TaskIdDelete");


            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
            conn.Open();
            SqlCommand cmd = new SqlCommand("dbo.UpdateTaskStatus", conn);

            cmd.CommandType = CommandType.StoredProcedure;
            
            SqlParameter p1 = cmd.CreateParameter();
            p1.ParameterName = "Id";
            p1.DbType = DbType.String;
            p1.Value = TaskIdDelete.Value;
            cmd.Parameters.Add(p1);
            
            SqlParameter p2 = cmd.CreateParameter();
            p2.ParameterName = "Status";
            p2.DbType = DbType.String;
            p2.Value = "Deleted";
            cmd.Parameters.Add(p2);

            cmd.ExecuteNonQuery();

            conn.Close();

        }

        protected void Completed_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox CompletedCheckBox = (CheckBox)sender;
            HiddenField TaskIdHiddenField = (HiddenField)CompletedCheckBox.Parent.FindControl("TaskIdHiddenField");

            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
            conn.Open();
            SqlCommand cmd = new SqlCommand("dbo.UpdateTaskStatus", conn);

            cmd.CommandType = CommandType.StoredProcedure;

            SqlParameter p1 = cmd.CreateParameter();
            p1.ParameterName = "Id";
            p1.DbType = DbType.String;
            p1.Value = TaskIdHiddenField.Value;
            cmd.Parameters.Add(p1);

            SqlParameter p2 = cmd.CreateParameter();
            p2.ParameterName = "Status";
            p2.DbType = DbType.String;
            p2.Value = (CompletedCheckBox.Checked ? "Completed" : "Active");
            cmd.Parameters.Add(p2);

            cmd.ExecuteNonQuery();

            conn.Close();

            // Reload the tasks
            LoadTasks();

            // Update status label
            StatusLabel.ForeColor = System.Drawing.Color.Green;
            StatusLabel.Text = "Task updated successfully!";

        }

        protected void TasksGrid_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow) { 
                e.Row.BackColor = System.Drawing.Color.FromName(((DataRowView)e.Row.DataItem)["BackColor"].ToString());
            }
        }

        protected void ActiveOnly_CheckedChanged(object sender, EventArgs e)
        {
            // Reload the tasks
            LoadTasks();

            // Update status label
            StatusLabel.Text = "";
        }

        protected void CompletedOnly_CheckedChanged(object sender, EventArgs e)
        {
            // Reload the tasks
            LoadTasks();

            // Update status label
            StatusLabel.Text = "";
        }

        protected void BothStatus_CheckedChanged(object sender, EventArgs e)
        {
            // Reload the tasks
            LoadTasks();

            // Update status label
            StatusLabel.Text = "";
        }

    }
}