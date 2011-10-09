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

namespace MantisConnect
{
	/// <summary>
	/// 
	/// </summary>
	public class Attachment
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="attachmentData"></param>
		internal Attachment( MantisConnectWebservice.AttachmentData attachmentData )
		{
			this.id = Convert.ToInt32( attachmentData.id );
			this.fileName = attachmentData.filename;
			this.size = Convert.ToInt32( attachmentData.size );
			this.contentType = attachmentData.content_type;
			this.dateSubmitted = attachmentData.date_submitted;
			this.downloadUrl = attachmentData.download_url;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="attachmentData"></param>
		/// <returns></returns>
		internal static Attachment[] ConvertArray( MantisConnectWebservice.AttachmentData[] attachmentData )
		{
			Attachment[] attachments = new Attachment[attachmentData.Length];

			for ( int i = 0; i < attachmentData.Length; ++i )
				attachments[i] = new Attachment( attachmentData[i] );

			return attachments;
		}

		/// <summary>
		/// 
		/// </summary>
		public int Id
		{
			get { return id; }
		}

		/// <summary>
		/// 
		/// </summary>
		public string FileName
		{
			get { return FileName; }
		}

		/// <summary>
		/// 
		/// </summary>
		public int Size
		{
			get { return size; }
		}

		/// <summary>
		/// 
		/// </summary>
		public string ContentType
		{
			get { return contentType; }
		}

		/// <summary>
		/// 
		/// </summary>
		public DateTime DateSubmitted
		{
			get { return dateSubmitted; }
		}

		/// <summary>
		/// 
		/// </summary>
		public string DownloadUrl
		{
			get { return downloadUrl; }
		}

		private int id;
		private string fileName;
		private int size;
		private string contentType;
		private DateTime dateSubmitted;
		private string downloadUrl;
	}
}
