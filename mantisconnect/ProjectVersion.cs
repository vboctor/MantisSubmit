#region Copyright © 2004 Victor Boctor
//
// MantisConnect is copyrighted to Victor Boctor
//
// This program is distributed under the terms and conditions of the GPL
// See LICENSE file for details.
//
// For commercial applications to link with or modify MantisConnect, they require the
// purchase of a MantisConnect commerical license.
//
#endregion

using System;
using System.Diagnostics;
using System.Globalization;

namespace MantisConnect
{
    /// <summary>
    /// 
    /// </summary>
    public class ProjectVersion
    {
        private MantisConnectWebservice.MantisConnect mc = new MantisConnect.MantisConnectWebservice.MantisConnect();
        /// <summary>
        /// 
        /// </summary>
        public ProjectVersion()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="projectVersionData"></param>
        /// <exception cref="FormatException"></exception>
        internal ProjectVersion(MantisConnectWebservice.ProjectVersionData projectVersionData)
        {
            this.Id = Convert.ToInt32(projectVersionData.id);
            this.Name = projectVersionData.name;
            this.ProjectId = Convert.ToInt32(projectVersionData.project_id);
            this.DateOrder = projectVersionData.date_order;
            this.Description = projectVersionData.description;
            this.IsReleased = projectVersionData.released;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="projectVersionDataArray"></param>
        /// <returns></returns>
        internal static ProjectVersion[] ConvertArray(MantisConnectWebservice.ProjectVersionData[] projectVersionDataArray)
        {
            if (projectVersionDataArray == null)
                return null;

            ProjectVersion[] projectVersions = new ProjectVersion[projectVersionDataArray.Length];

            for (int i = 0; i < projectVersionDataArray.Length; ++i)
                projectVersions[i] = new ProjectVersion(projectVersionDataArray[i]);

            return projectVersions;
        }

        /// <summary>
        /// 
        /// </summary>
        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public int ProjectId
        {
            get { return projectId; }
            set { projectId = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public DateTime DateOrder
        {
            get { return dateOrder; }
            set { dateOrder = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsReleased
        {
            get { return isReleased; }
            set { isReleased = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="prjct"></param>
        /// <param name="session"></param>
        /// <returns></returns>
        public int ProjectVersionAdd(ProjectVersion prjct, Session session)
        {
            ValidateProjectId(prjct.Id);

            DateTime before = DateTime.Now;
            try
            {
                MantisConnect.MantisConnectWebservice.ProjectVersionData vd = new MantisConnect.MantisConnectWebservice.ProjectVersionData();
                vd.project_id = prjct.ProjectId.ToString();
                vd.id = prjct.id.ToString();
                vd.name = prjct.Name;
                return Convert.ToInt32(mc.mc_project_version_add(session.Username, session.Password, vd));
            }
            finally
            {
                TimeSpan timeSpan = new TimeSpan(DateTime.Now.Ticks - before.Ticks);
                Debug.WriteLine(string.Format(CultureInfo.CurrentCulture, "{0}: AddIssue()", timeSpan.ToString()));
            }
        }
        private static void ValidateProjectId(int projectId)
        {
            if (projectId < -1)
                throw new ArgumentOutOfRangeException("projectId");
        }
        #region Private Members
        private int id;
        private string name;
        private int projectId;
        private DateTime dateOrder;
        private string description;
        private bool isReleased;
        #endregion
    }
}
