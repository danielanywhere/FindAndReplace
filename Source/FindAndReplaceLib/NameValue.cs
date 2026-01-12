/*
 * Copyright (c). 2000-2026 Daniel Patterson, MCSD (danielanywhere).
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <https://www.gnu.org/licenses/>.
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;

namespace FindAndReplaceLib
{
	//*-------------------------------------------------------------------------*
	//*	NameValueCollection																											*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Collection of NameValueItem Items.
	/// </summary>
	public class NameValueCollection : List<NameValueItem>
	{
		//*************************************************************************
		//*	Private																																*
		//*************************************************************************
		//*************************************************************************
		//*	Protected																															*
		//*************************************************************************
		//*************************************************************************
		//*	Public																																*
		//*************************************************************************
		//*-----------------------------------------------------------------------*
		//*	SetValue																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Set the value of the specified entry.
		/// </summary>
		/// <param name="name">
		/// </param>
		/// <param name="value">
		/// </param>
		/// <remarks>
		/// If the specified name is legitimate and the entry doesn't exist in
		/// the collection, it will be created and added.
		/// </remarks>
		public void SetValue(string name, string value)
		{
			string localValue = "";
			NameValueItem nameValue = null;

			if(name?.Length > 0)
			{
				if(value?.Length > 0)
				{
					localValue = value;
				}
				nameValue = this.FirstOrDefault(x =>
					StringComparer.OrdinalIgnoreCase.Equals(x.Name, name));
				if(nameValue == null)
				{
					nameValue = new NameValueItem()
					{
						Name = name
					};
					this.Add(nameValue);
				}
				nameValue.Value = localValue;
			}
		}
		//*-----------------------------------------------------------------------*


	}
	//*-------------------------------------------------------------------------*

	//*-------------------------------------------------------------------------*
	//*	NameValueItem																														*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// An item with a name and a value.
	/// </summary>
	public class NameValueItem
	{
		//*************************************************************************
		//*	Private																																*
		//*************************************************************************
		//*************************************************************************
		//*	Protected																															*
		//*************************************************************************
		//*************************************************************************
		//*	Public																																*
		//*************************************************************************
		//*-----------------------------------------------------------------------*
		//*	Name																																	*
		//*-----------------------------------------------------------------------*
		private string mName = "";
		/// <summary>
		/// Get/Set the name of the item.
		/// </summary>
		public string Name
		{
			get { return mName; }
			set { mName = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Value																																	*
		//*-----------------------------------------------------------------------*
		private string mValue = "";
		/// <summary>
		/// Get/Set value of the item.
		/// </summary>
		public string Value
		{
			get { return mValue; }
			set { mValue = value; }
		}
		//*-----------------------------------------------------------------------*

	}
	//*-------------------------------------------------------------------------*

}
