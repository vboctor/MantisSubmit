using System;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using MantisConnect;

namespace MantisSubmit2
{
    public class MantisData
    {
        #region Constants

        private const string CacheFileName = "MantisCache.xml";

        #endregion

        #region Properties

        public string[] Projects { get; set; }
        public SerializableDictionary<string, string[]> Categories { get; set; }
        public string[] Priorities { get; set; }
        public string[] Etas { get; set; }
        public SerializableDictionary<string, string[]> Versions { get; set; }
        public string[] Severities { get; set; }
        public string[] Reproducibilities { get; set; }
        public SerializableDictionary<string, User[]> Users { get; set; }

        #endregion

        #region Constructors

        public MantisData()
        {
            Projects = new string[0];
            Categories = new SerializableDictionary<string, string[]>();
            Priorities = new string[0];
            Etas = new string[0];
            Versions = new SerializableDictionary<string, string[]>();
            Severities = new string[0];
            Reproducibilities = new string[0];
            Users = new SerializableDictionary<string, User[]>();
        }

        #endregion

        #region Public methods

        public void UpdateFromMantis(Session mantisSession)
        {
            Categories.Clear();
            Versions.Clear();
            Users.Clear();

            Project[] projects = mantisSession.Request.UserGetAccesibleProjects();
            Projects = projects.Select(w => w.Name).ToArray();
            foreach (Project project in projects)
            {
                Categories.Add(project.Name,
                    mantisSession.Request.ProjectGetCategories(project.Id));

                Versions.Add(project.Name,
                    mantisSession.Request.ProjectGetVersionsReleased(project.Id)
                        .Concat(mantisSession.Request.ProjectGetVersionsUnreleased(project.Id))
                        .OrderBy(v => v.Name)
                        .Select(w => w.Name)
                        .ToArray());

                Users.Add(project.Name, mantisSession.Request.ProjectGetUsers(project.Id).ToArray());
            }
            Priorities = mantisSession.Config.PriorityEnum.Labels;
            Etas = mantisSession.Config.EtaEnum.Labels;
            Severities = mantisSession.Config.SeverityEnum.Labels;
            Reproducibilities = mantisSession.Config.ReproducibilityEnum.Labels;
        }

        public void SaveToCache()
        {
            using (FileStream fs = new FileStream(CacheFileName, FileMode.Create))
            {
                XmlSerializer xml = new XmlSerializer(typeof(MantisData));
                xml.Serialize(fs, this);
            }
        }

        public static MantisData LoadFromCache()
        {
            if (!File.Exists(CacheFileName))
                return new MantisData();

            try
            {
                using (FileStream fs = new FileStream(CacheFileName, FileMode.Open))
                {
                    XmlSerializer xml = new XmlSerializer(typeof(MantisData));
                    return (MantisData)xml.Deserialize(fs);
                }
            }
            catch
            {
                return new MantisData();
            }
        }

        #endregion
    }
}
