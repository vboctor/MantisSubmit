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
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace MantisConnect
{
    /// <summary>
    /// A wrapper around <see cref="MantisConnectWebservice"/> to provide a more friendly
    /// interface for the rest of the C# code.
    /// </summary>
    /// <remarks>
    /// Some methods will do do pre or post processing of data to convert them from the
    /// webservice format to one that is easier to access.  For example, the webservice
    /// may return information about project ids/names as a serialised string, which then
    /// gets deserialised by this wrapper into a <see cref="DataTable"/> for easier
    /// access and binding to standard controls.
    /// </remarks>
    public sealed class Request
    {
        // Hack, should check wheater it's work correctly without this hack
        private class MyMantisConnect : MantisConnectWebservice.MantisConnect
        {
            protected override System.Net.WebRequest GetWebRequest(Uri uri)
            {
                var result = base.GetWebRequest(uri) as System.Net.HttpWebRequest;
                result.KeepAlive = false;
                return result;
            }
        }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="session">The session to use for all communication with the webservice.
        /// The user name and password are used from their to provide such details to the
        /// webservice with each call without exposing such detail to the user of the 
        /// library.</param>
        public Request(Session session)
        {
            this.session = session;

            mc = new MyMantisConnect();
            mc.Url = session.Url;

            if (session.NetworkCredential != null)
                mc.Credentials = session.NetworkCredential;
        }

        /// <summary>
        /// Add the specified issue to Mantis database.
        /// </summary>
        /// <param name="issue">The issue details.  Issue id is ignored.</param>
        /// <remarks>
        /// TODO: Consider a generic and easy way to time operations.
        /// </remarks>
        /// <returns>The id of the added issue</returns>
        public int IssueAdd(Issue issue)
        {
            ValidateIssue(issue);

            DateTime before = DateTime.Now;
            try
            {
                return Convert.ToInt32(mc.mc_issue_add(session.Username, session.Password, issue.ToWebservice()));
            }
            finally
            {
                TimeSpan timeSpan = new TimeSpan(DateTime.Now.Ticks - before.Ticks);
                Debug.WriteLine(string.Format(CultureInfo.CurrentCulture, "{0}: AddIssue()", timeSpan.ToString()));
            }
        }

        /// <summary>
        /// Delete the issue with the specified id
        /// </summary>
        /// <param name="issueId">Id of issue to delete</param>
        public void IssueDelete(int issueId)
        {
            ValidateIssueId(issueId);

            mc.mc_issue_delete(session.Username, session.Password, issueId.ToString());
        }

        /// <summary>
        /// Get information related to the specified issue id.
        /// </summary>
        /// <param name="issueId">The id of the issue to retrieve information for.</param>
        /// <returns>The issue details, this does not include related information in other
        /// tables, like issue notes, ...etc.</returns>
        /// <exception cref="ArgumentOutOfRangeException">The issue id is 0 or negative.</exception>
        public Issue IssueGet(int issueId)
        {
            ValidateIssueId(issueId);

            MantisConnectWebservice.IssueData issueData;
            issueData = mc.mc_issue_get(session.Username, session.Password, issueId.ToString());

            return (issueData == null) ? null : new Issue(issueData);
        }

        /// <summary>
        /// Check if there exists an issue with the specified id.
        /// </summary>
        /// <param name="issueId">Id of issue to check for.</param>
        /// <returns>true: exists, false: does not exist</returns>
        /// <exception cref="ArgumentOutOfRangeException">The issue id is 0 or negative.</exception>
        public bool IssueExists(int issueId)
        {
            ValidateIssueId(issueId);

            return Convert.ToBoolean(mc.mc_issue_exists(session.Username, session.Password, issueId.ToString()));
        }

        /// <summary>
        /// Search for an issue with the specified summary and return its issue id.
        /// </summary>
        /// <remarks>
        /// This is useful to allow a software which is automatically reporting issues due
        /// to exceptions or whatever reason to check first that the issue was not reported
        /// before.  And if it was, then it knows the issue id and hence is able to add
        /// a note or do whatever with this id.  Other applications may decide to delete 
        /// the issue and create a new one, basically it is up to the client application
        /// to decide how to use the returned issue id.
        /// </remarks>
        /// <param name="summary">The summary to search for.</param>
        /// <returns>0: not found, otherwise issue id</returns>
        /// <exception cref="ArgumentNullException">Summary field is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Summary field is empty or too long.</exception>
        public int IssueGetIdFromSummary(string summary)
        {
            if (summary == null)
                throw new ArgumentNullException("summary");

            if ((summary.Trim().Length == 0) || (summary.Length > 128))
                throw new ArgumentOutOfRangeException("summary");

            return Convert.ToInt32(mc.mc_issue_get_id_from_summary(session.Username, session.Password, summary));
        }

        /// <summary>
        /// Get the id of the last "reported" issue that is accessible to the logged in user.
        /// </summary>
        /// <remarks>
        /// This is useful for applications that need to know when new issues are being submitted
        /// to refresh a certain view or do something with such knowledge.
        /// </remarks>
        /// <param name="projectId">-1: default, 0: all projects, otherwise: project id</param>
        /// <returns>0: no issues accessible to logged in user, otherwise Id of the last reported issue.</returns>
        /// <exception cref="ArgumentOutOfRangeException">The project id is invalid.</exception>
        public int IssueGetLastId(int projectId)
        {
            ValidateProjectId(projectId);

            return Convert.ToInt32(mc.mc_issue_get_biggest_id(session.Username, session.Password, projectId.ToString()));
        }

        /// <summary>
        /// Get projects accessible to the currently logged in user.
        /// </summary>
        /// <remarks>
        /// This returns a table ("Projects") which includes two columns ("project_id", "name").
        /// </remarks>
        /// <returns>An array of projects.</returns>
        public Project[] UserGetAccesibleProjects()
        {
            return Project.ConvertArray(mc.mc_projects_get_user_accessible(session.Username, session.Password), "");
        }

        /// <summary>
        /// Gets the filters that are available to the current user and the specified project.
        /// </summary>
        /// <param name="projectId">0: all projects, otherwise project id</param>
        /// <returns>An array of filters.</returns>
        /// <exception cref="ArgumentOutOfRangeException">The project id is invalid.</exception>
        public Filter[] UserGetFilters(int projectId)
        {
            ValidateProjectId(projectId);

            return Filter.ConvertArray(mc.mc_filter_get(session.Username, session.Password, projectId.ToString()));
        }

        /// <summary>
        /// Gets the issues based on the specified filter, page, and number per page.
        /// </summary>
        /// <param name="projectId">Stored project id to use.</param>
        /// <param name="filterId">Stored filter id to use.</param>
        /// <param name="pageNumber">Page number to get the issues for.</param>
        /// <param name="issuesPerPage">Number of issues per page.</param>
        /// <returns>An array of issues.</returns>
        public Issue[] GetIssues(int projectId, int filterId, int pageNumber, int issuesPerPage)
        {
            return Issue.ConvertArray(mc.mc_filter_get_issues(session.Username, session.Password, projectId.ToString(), filterId.ToString(), pageNumber.ToString(), issuesPerPage.ToString()));
        }

        /// <summary>
        /// Get categories defined for the project with the specified id.
        /// </summary>
        /// <remarks>
        /// TODO: Support CategoryData and Category.
        /// </remarks>
        /// <param name="projectId">project id (greater than 0)</param>
        /// <returns>An array of categories.</returns>
        /// <exception cref="ArgumentOutOfRangeException">The project id is invalid.</exception>
        public string[] ProjectGetCategories(int projectId)
        {
            ValidateProjectId(projectId);

            string[] categories = mc.mc_project_get_categories(session.Username, session.Password, projectId.ToString());
            return categories;
        }

        /// <summary>
        /// Get versions defined for the project with the specified id.
        /// </summary>
        /// <param name="projectId">project id (greater than 0)</param>
        /// <returns>An array of project versions.</returns>
        /// <exception cref="ArgumentOutOfRangeException">The project id is invalid.</exception>
        public ProjectVersion[] ProjectGetVersions(int projectId)
        {
            ValidateProjectId(projectId);

            return ProjectVersion.ConvertArray(mc.mc_project_get_versions(session.Username, session.Password, projectId.ToString()));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        public User[] ProjectGetUsers(int projectId)
        {
            ValidateProjectId(projectId);

            return mc.mc_project_get_users(session.Username, session.Password, projectId.ToString(), "0")
                .Select(ad => new User(ad))
                .ToArray();
        }

        /// <summary>
        /// Get released versions defined for the project with the specified id.
        /// </summary>
        /// <param name="projectId">project id (greater than 0)</param>
        /// <returns>An array of released project versions.</returns>
        /// <exception cref="ArgumentOutOfRangeException">The project id is invalid.</exception>
        public ProjectVersion[] ProjectGetVersionsReleased(int projectId)
        {
            ValidateProjectId(projectId);

            return ProjectVersion.ConvertArray(mc.mc_project_get_released_versions(session.Username, session.Password, projectId.ToString()));
        }

        /// <summary>
        /// Get unreleased versions defined for the project with the specified id.
        /// </summary>
        /// <param name="projectId">project id (greater than 0)</param>
        /// <returns>An array of released project versions.</returns>
        /// <exception cref="ArgumentOutOfRangeException">The project id is invalid.</exception>
        public ProjectVersion[] ProjectGetVersionsUnreleased(int projectId)
        {
            ValidateProjectId(projectId);

            return ProjectVersion.ConvertArray(mc.mc_project_get_unreleased_versions(session.Username, session.Password, projectId.ToString()));
        }

        /// <summary>
        /// Get string value of the specified configuration option.
        /// </summary>
        /// <remarks>
        /// If the caller attempts to retrieve sensitive configuration options like 
        /// passwords, database name, ...etc, an exception will be raised.
        /// 
        /// TODO: Overload this method to get more types of configurations.
        /// </remarks>
        /// <param name="config">Name of configuration option (without the $g_ part)</param>
        /// <param name="str">An output parameter to hold the value of the configuration option</param>
        /// <exception cref="ArgumentNullException">config parameter is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">config parameter is empty or blank.</exception>
        public void ConfigGet(string config, out string str)
        {
            ValidateConfigName(config);

            str = mc.mc_config_get_string(session.Username, session.Password, config);
        }

        /// <summary>
        /// Adds a note to the specified issue.
        /// </summary>
        /// <param name="issueId">Issue id to add note to.</param>
        /// <param name="note">The note to add</param>
        /// <remarks>
        /// The user must have write access to the issue and the issue must not be readonly.
        /// </remarks>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Note is null</exception>
        /// <exception cref="ArgumentOutOfRangeException">The issue id is 0 or negative.  Or note is empty or blank.</exception>
        public int IssueNoteAdd(int issueId, IssueNote note)
        {
            ValidateIssueId(issueId);
            ValidateIssueNote(note);

            return Convert.ToInt32(mc.mc_issue_note_add(session.Username, session.Password, issueId.ToString(CultureInfo.InvariantCulture), note.ToWebservice()));
        }

        /// <summary>
        /// Issues the add file.
        /// </summary>
        /// <param name="issueId">The issue id.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="fileType">Type of the file.</param>
        /// <param name="content">The content.</param>
        public void IssueAddFile(int issueId, string fileName, string fileType, byte[] content)
        {
            mc.mc_issue_attachment_add(session.Username, session.Password, issueId.ToString(CultureInfo.InvariantCulture), fileName, fileType, content);
        }

        /// <summary>
        /// Delete the issue note with the specified id
        /// </summary>
        /// <param name="issueNoteId">Id of issue note to delete</param>
        /// <exception cref="ArgumentOutOfRangeException">The issue note id is 0 or negative.</exception>
        public void IssueNoteDelete(int issueNoteId)
        {
            ValidateIssueNoteId(issueNoteId);

            mc.mc_issue_note_delete(session.Username, session.Password, issueNoteId.ToString());
        }

        #region Private

        /// <summary>
        /// Validates a project id and raises an exception if it is not valid.
        /// </summary>
        /// <param name="projectId">Project Id</param>
        /// <exception cref="ArgumentOutOfRangeException">The project id is smaller than -1.</exception>
        private static void ValidateProjectId(int projectId)
        {
            if (projectId < -1)
                throw new ArgumentOutOfRangeException("projectId");
        }

        /// <summary>
        /// Validates an issue id and raises an exception if it is not valid.
        /// </summary>
        /// <param name="issueId">Issue Id</param>
        /// <exception cref="ArgumentOutOfRangeException">The issue id is 0 or negative.</exception>
        private static void ValidateIssueId(int issueId)
        {
            if (issueId < 1)
                throw new ArgumentOutOfRangeException("issueId");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="issue"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        private static void ValidateIssue(Issue issue)
        {
            if (issue == null)
                throw new ArgumentNullException("issue");

            if (issue.Summary == null)
                throw new ArgumentNullException("issue.Summary");

            if (issue.Summary.Trim().Length == 0)
                throw new ArgumentOutOfRangeException("issue.Summary");

            if (issue.Description == null)
                throw new ArgumentNullException("issue.Description");

            if (issue.Description.Trim().Length == 0)
                throw new ArgumentOutOfRangeException("issue.Description");

            if (issue.Notes != null)
                foreach (IssueNote note in issue.Notes)
                    ValidateIssueNote(note);
        }

        /// <summary>
        /// Validates an issue note id and raises an exception if it is not valid.
        /// </summary>
        /// <param name="issueNoteId">Issue Note Id</param>
        /// <exception cref="ArgumentOutOfRangeException">The issue note id is 0 or negative.</exception>
        private static void ValidateIssueNoteId(int issueNoteId)
        {
            if (issueNoteId < 1)
                throw new ArgumentOutOfRangeException("issueNoteId");
        }

        /// <summary>
        /// Validates an issue note and raises an exception if it is not valid.
        /// </summary>
        /// <param name="note">The note to be validated</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        private static void ValidateIssueNote(IssueNote note)
        {
            if (note == null)
                throw new ArgumentNullException("note");

            if (note.Text == null)
                throw new ArgumentNullException("note.Text");

            if (note.Text.Trim().Length == 0)
                throw new ArgumentOutOfRangeException("note");
        }

        /// <summary>
        /// Validates the name of a configuration option and raises an exception if it is not valid.
        /// </summary>
        /// <param name="config">configuration option</param>
        /// <exception cref="ArgumentOutOfRangeException">The configuration option is invalid.</exception>
        private static void ValidateConfigName(string config)
        {
            if (config == null)
                throw new ArgumentNullException("configOption");

            if (config.Trim().Length == 0)
                throw new ArgumentOutOfRangeException("configOption");

            char[] invalidChars = new char[] { ' ' };
            if (config.IndexOfAny(invalidChars) != -1)
                throw new ArgumentOutOfRangeException("configOption");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="rows"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public static DataTable ArrayToDataTable(Type type, object[] rows, string tableName)
        {
            DataTable table = new DataTable(tableName);

            PropertyInfo[] properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
            for (int i = 0; i < properties.Length; ++i)
                table.Columns.Add(properties[i].Name, properties[i].PropertyType);

            foreach (object row in rows)
            {
                if (row.GetType() != type)
                    throw new ArgumentException("row type not matching expected table type");

                DataRow dataRow = table.NewRow();

                for (int i = 0; i < properties.Length; ++i)
                {
                    object val = properties[i].GetValue(row, null);
                    //if ( val == null )
                    //    val = DBNull.Value;
                    //else if ( val is ICollection )
                    //    val = (val as ICollection).Count;
                    //else if (!(val is ObjectRef))
                    //    val = val.ToString();

                    dataRow[table.Columns[i]] = val;
                }

                table.Rows.Add(dataRow);
            }

            return table;
        }

        /// <summary>
        /// Session to retrieve the user name / password of the current session
        /// from.
        /// </summary>
        private readonly Session session;

        /// <summary>
        /// Webservice auto-generated proxy.
        /// </summary>
        private MantisConnectWebservice.MantisConnect mc;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="prjct"></param>
        /// <returns></returns>
        public int ProjectVersionAdd(ProjectVersion prjct)
        {
            ValidateProjectId(prjct.Id);

            DateTime before = DateTime.Now;
            try
            {
                MantisConnect.MantisConnectWebservice.ProjectVersionData vd = new MantisConnect.MantisConnectWebservice.ProjectVersionData();
                vd.project_id = prjct.ProjectId.ToString();
                vd.released = true;
                vd.description = "new version";
                vd.name = prjct.Name;
                return Convert.ToInt32(mc.mc_project_version_add(session.Username, session.Password, vd));
            }
            catch (Exception ex)
            {
                Debug.WriteLine(string.Format(CultureInfo.CurrentCulture, "{0} {1} : Error", ex.Message, ex.StackTrace));
                return -1;
            }
            finally
            {
                TimeSpan timeSpan = new TimeSpan(DateTime.Now.Ticks - before.Ticks);
                Debug.WriteLine(string.Format(CultureInfo.CurrentCulture, "{0}: AddIssue()", timeSpan.ToString()));
            }
        }
        #endregion
    }
}
