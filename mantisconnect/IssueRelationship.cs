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
	public class IssueRelationship
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="relationshipData"></param>
		/// <exception cref="FormatException"></exception>
		internal IssueRelationship( MantisConnectWebservice.RelationshipData relationshipData )
		{
			this.Type = new ObjectRef( relationshipData.type );
			this.IssueId = Convert.ToInt32( relationshipData.target_id );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="relationshipsData"></param>
		/// <returns></returns>
		internal static IssueRelationship[] ConvertArray( MantisConnectWebservice.RelationshipData[] relationshipsData )
		{
			if ( relationshipsData == null )
				return null;

			IssueRelationship[] relationships = new IssueRelationship[relationshipsData.Length];

			for ( int i = 0; i < relationshipsData.Length; ++i )
				relationships[i] = new IssueRelationship( relationshipsData[i] );

			return relationships;
		}

		/// <summary>
		/// 
		/// </summary>
		public ObjectRef Type
		{
			get { return type; }
			set { type = value; }
		}

		/// <summary>
		/// 
		/// </summary>
		public int IssueId
		{
			get { return issueId; }
			set { issueId = value; }
		}

		#region Private Members
		private ObjectRef type;
		private int issueId;
		#endregion
	}
}
