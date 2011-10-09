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
	/// A class to handle enumerations that are defined in Mantis.
	/// </summary>
	/// <remarks>
	/// The format of Mantis enumerations is as follows:
	/// "10:viewer,25:reporter,40:updater,55:developer,70:manager,90:administrator"
	/// </remarks>
    public sealed class MantisEnum
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="enumeration">Enumeration string to work with.</param>
        public MantisEnum( string enumeration )
        {
            this.enumeration = enumeration;
        }

        /// <summary>
        /// Given an id, this indexer returns the corresponding enumeration name.
        /// </summary>
        /// <param name="id">The enumeration value id.</param>
        public string this [int id]
        {
            get 
            {
                string[] entries = enumeration.Split( ',' );

                foreach ( string entry in entries ) 
                {
                    string[] details = entry.Split( ':' );

                    if ( details[0] == id.ToString() )
                        return details[1];
                }

                return string.Format( "@{0}@", id );
            }
        }

        /// <summary>
        /// Given a name, this indexer returns the corresponding enumeration id.
        /// </summary>
        /// <param name="name">The enumeration value name.</param>
        public int this [string name]
        {
            get 
            {
                string[] entries = enumeration.Split( ',' );

                foreach ( string entry in entries ) 
                {
                    string[] details = entry.Split( ':' );

                    if ( details[1] == name )
                        return Convert.ToInt32( details[0] );
                }

                return 0;
            }
        }

        /// <summary>
        /// Returns an array of strings containing the labels in the enumerations.
        /// </summary>
        /// <value>Array of enumeration labels.</value>
        public string[] Labels
        {
            get
            {
                string[] entries = enumeration.Split( ',' );

                string[] labels = new string[ entries.Length ];

                int i = 0;
                foreach ( string entry in entries ) 
                {
                    string[] details = entry.Split( ':' );
                    labels[i] = details[1];
                    ++i;
                }
                
                return labels;
            }
        }

        /// <summary>
        /// The enumeration string supplied at construction time.
        /// </summary>
        private readonly string enumeration;
    }
}
