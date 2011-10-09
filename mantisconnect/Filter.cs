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
	public class Filter
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="filterData"></param>
		internal Filter( MantisConnectWebservice.FilterData filterData )
		{
			this.id = Convert.ToInt32( filterData.id );
			this.owner = new User( filterData.owner );
			this.projectId = Convert.ToInt32( filterData.project_id );
			this.isPublic = filterData.is_public;
			this.name = filterData.name;
			this.filterString = filterData.filter_string;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="filtersData"></param>
		/// <returns></returns>
		internal static Filter[] ConvertArray( MantisConnectWebservice.FilterData[] filtersData )
		{
			if ( filtersData == null )
				return null;

			Filter[] filters = new Filter[filtersData.Length];

			for ( int i = 0; i < filtersData.Length; ++i )
				filters[i] = new Filter( filtersData[i] );

			return filters;
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
		public User Owner
		{
			get { return owner; }
		}

		/// <summary>
		/// 
		/// </summary>
		public int ProjectId
		{
			get { return projectId; }
		}

		/// <summary>
		/// 
		/// </summary>
		public bool IsPublic
		{
			get { return isPublic; }
		}

		/// <summary>
		/// 
		/// </summary>
		public string Name
		{
			get { return name; }
		}

		/// <summary>
		/// 
		/// </summary>
		public string FilterString
		{
			get { return filterString; }
		}

		#region Private Members
		private int id;
		private User owner;
		private int projectId;
		private bool isPublic;
		private string name;
		private string filterString;
		#endregion
	}
}
