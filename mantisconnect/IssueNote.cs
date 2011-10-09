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
using System.Collections;

namespace MantisConnect
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class IssueNote
    {
        /// <summary>
        /// Default Constructor
        /// </summary>
        public IssueNote()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="issueNoteData"></param>
        internal IssueNote(MantisConnectWebservice.IssueNoteData issueNoteData)
        {
            this.Id = Convert.ToInt32(issueNoteData.id);
            this.Author = new User(issueNoteData.reporter);
            this.Text = issueNoteData.text;
            this.ViewState = new ObjectRef(issueNoteData.view_state);
            this.DateSubmitted = issueNoteData.date_submitted;
            this.LastModified = issueNoteData.last_modified;
        }

        /// <summary>
        /// Converts the this instance into the webservice issue note type.
        /// </summary>
        /// <returns>An instance of webservice issue note.</returns>
        internal MantisConnectWebservice.IssueNoteData ToWebservice()
        {
            MantisConnectWebservice.IssueNoteData note = new MantisConnect.MantisConnectWebservice.IssueNoteData();

            note.id = Id.ToString();
            note.reporter = Author != null ? Author.ToWebservice() : null;
            note.text = Text;
            note.view_state = ViewState != null ? ViewState.ToWebservice() : null;
            note.date_submitted = DateSubmitted;
            note.last_modified = LastModified;

            return note;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="issueNotesData"></param>
        /// <returns></returns>
        internal static IssueNote[] ConvertArray(MantisConnectWebservice.IssueNoteData[] issueNotesData)
        {
            if (issueNotesData == null)
                return null;

            IssueNote[] notes = new IssueNote[issueNotesData.Length];

            for (int i = 0; i < issueNotesData.Length; ++i)
                notes[i] = new IssueNote(issueNotesData[i]);

            return notes;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="notes"></param>
        /// <returns></returns>
        internal static MantisConnectWebservice.IssueNoteData[] ConvertArrayToWebservice(IssueNote[] notes)
        {
            if (notes == null)
                return null;

            MantisConnectWebservice.IssueNoteData[] notesForWebservice = new MantisConnectWebservice.IssueNoteData[notes.Length];

            int i = 0;
            foreach (IssueNote note in notes)
                notesForWebservice[i++] = note.ToWebservice();

            return notesForWebservice;
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
        public User Author
        {
            get { return author; }
            set { author = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Text
        {
            get { return text; }
            set { text = value; }
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
        public DateTime DateSubmitted
        {
            get { return dateSubmitted; }
            set { dateSubmitted = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public DateTime LastModified
        {
            get { return lastModified; }
            set { lastModified = value; }
        }

        #region Private Members
        private int id;
        private User author;
        private string text;
        private ObjectRef viewState;
        private DateTime dateSubmitted;
        private DateTime lastModified;
        #endregion
    }
}
