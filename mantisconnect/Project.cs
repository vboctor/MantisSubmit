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
using System.Collections.Generic;

namespace MantisConnect
{
    /// <summary>
    /// 
    /// </summary>
    public class Project
    {
        /// <summary>
        /// Default Constructor
        /// </summary>
        public Project()
        {
        }

        /// <summary>
        /// Constructor to create a project from the web service project data.
        /// </summary>
        /// <param name="projectData">An instance returned by the webservice.</param>
        internal Project(MantisConnectWebservice.ProjectData projectData)
        {
            this.Id = Convert.ToInt32(projectData.id);
            this.Name = projectData.name;
            this.Status = new ObjectRef(projectData.status);
            this.Enabled = projectData.enabled;
            this.ViewState = new ObjectRef(projectData.view_state);
            this.AccessMin = new ObjectRef(projectData.access_min);
            this.FilePath = projectData.file_path;
            this.Description = projectData.description;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="projectData"></param>
        /// <param name="prefix"></param>
        /// <returns></returns>
        internal static Project[] ConvertArray(MantisConnectWebservice.ProjectData[] projectData, string prefix)
        {
            if (projectData == null)
                return null;

            List<Project> projects = new List<Project>();

            for (int i = 0; i < projectData.Length; ++i)
            {
                var project = new Project(projectData[i]);
                project.Caption = prefix + project.Name;
                projects.Add(project);
                projects.AddRange(ConvertArray(projectData[i].subprojects, prefix + "   »"));
            }

            return projects.ToArray();
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
        public string Caption { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ObjectRef Status
        {
            get { return status; }
            set { status = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool Enabled
        {
            get { return enabled; }
            set { enabled = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public ObjectRef ViewState
        {
            get { return viewState; }
            set { viewState = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public ObjectRef AccessMin
        {
            get { return accessMin; }
            set { accessMin = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public string FilePath
        {
            get { return filePath; }
            set { filePath = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        #region Private Members
        private int id;
        private string name;
        private ObjectRef status;
        private bool enabled;
        private ObjectRef viewState;
        private ObjectRef accessMin;
        private string filePath;
        private string description;
        #endregion
    }
}
