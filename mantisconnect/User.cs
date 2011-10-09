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
    public sealed class User
    {
        /// <summary>
        /// 
        /// </summary>
        public User()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="accountData"></param>
        internal User(MantisConnectWebservice.AccountData accountData)
        {
            this.Id = Convert.ToInt32(accountData.id);
            this.Name = accountData.name;
            this.RealName = accountData.real_name;
            this.Email = accountData.email;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.Name;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal MantisConnectWebservice.AccountData ToWebservice()
        {
            MantisConnectWebservice.AccountData accountData = new MantisConnectWebservice.AccountData();
            accountData.id = this.Id.ToString();
            accountData.name = this.Name;
            accountData.real_name = this.RealName;
            accountData.email = this.Email;

            return accountData;
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
        public string RealName
        {
            get { return realName; }
            set { realName = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Email
        {
            get { return email; }
            set { email = value; }
        }

        #region Private Members
        private int id;
        private string name;
        private string realName;
        private string email;
        #endregion
    }
}
