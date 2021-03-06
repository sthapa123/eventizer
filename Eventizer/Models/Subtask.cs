﻿using Eventizer.Helpers;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Eventizer.Models
{
    public class Subtask
    {
        public int ID { set; get; }
        public string Name { set; get; }
        public DateTime DateCreated { set; get; }
        public Employee CreatedBy { set; get; }
        public Employee AssignedTo { set; get; }
        public string Description { set; get; }
        public int LaboursRequired { set; get; }
        public DateTime Deadline { set; get; }
        public bool Status { set; get; }
        public List<Asset> Assets { set; get; }

        public static Subtask FetchSubtaskByID(int ID)
        {
            List<SqlParameter> Param = new List<SqlParameter>();
            Param.Add(new SqlParameter("@id", ID));
            Subtask subtask = new Subtask();

            using (SqlDataReader reader = (new Database()).ExecProcedureWithResult("usp_get_subtask_by_id", Param))
            {
                while (reader.Read())
                {
                    subtask.ID = (int)reader["id"];
                    subtask.Name = reader["name"].ToString();
                    subtask.Description = reader["description"].ToString();
                    subtask.Status = (bool)reader["status"];
                    subtask.LaboursRequired = (int)reader["labours_required"];
                    subtask.CreatedBy = Employee.FetchEmployeeByID((int)reader["created_by"]);
                    subtask.AssignedTo = Employee.FetchEmployeeByID((int)reader["assigned_to"]);
                    subtask.DateCreated = DateTime.Parse(reader["date_created"].ToString());
                    subtask.Deadline = DateTime.Parse(reader["deadline"].ToString());
                    subtask.Assets = (reader["assets"].ToString() != "0,") ? Subtask.FetchAssetsArray(reader["assets"].ToString()) : new List<Asset>();
                }
            }

            return subtask;
        }

        public static List<Asset> FetchAssetsArray(string rawAssets)
        {
            string[] IDArray = rawAssets.TrimEnd(',').Split(',');
            List<Asset> Assets = new List<Asset>();
            for (int i = 0; i < IDArray.Length; i++)
            {
                Asset A = Asset.FetchAssetByID(Convert.ToInt32(IDArray[i]));
                Assets.Add(A);
            }
            return Assets;
        }
        public int TotalAssets()
        {
            return 0;
            //return Assets.Count;
        }

        public static List<Subtask> FetchAllSubtasks(SqlDataReader reader)
        {
            List<Subtask> Subtasks = new List<Subtask>();

            while (reader.Read())
            {

                Subtask E = new Subtask();
                E.ID = (int)reader["id"];
                E.Name = reader["name"].ToString();
                E.Description = reader["description"].ToString();
                E.Status = (bool)reader["status"];
                E.CreatedBy = Employee.FetchEmployeeByID((int)reader["created_by"]);
                E.DateCreated = DateTime.Parse(reader["date_created"].ToString());
                E.Deadline = DateTime.Parse(reader["deadline"].ToString());
                E.LaboursRequired = (int)reader["labours_required"];
                Subtasks.Add(E);
            }
            reader.Close();
            reader.Dispose();
            return Subtasks;
        }
        public static int GetPendingSubtasks(List<Subtask> Subtasks)
        {
            int count = 0;
            foreach (Subtask S in Subtasks)
            {
                if (!S.Status)
                {
                    count++;
                }
            }
            return count;
        }
    }
}