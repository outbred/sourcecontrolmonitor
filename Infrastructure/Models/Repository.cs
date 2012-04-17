using System;
using System.Runtime.Serialization;
using System.Text;
using Infrastructure.Utilities;
using Infrastructure.Interfaces;
using System.ComponentModel;

namespace Infrastructure.Models
{
	[DataContract]
	public class Repository : ViewModelBase, IDataErrorInfo
	{
		private string _name;
		private RepositoryType _type;
		private Uri _path;

		public DelegateCommand OnEditClick
		{
			get
			{
				return new DelegateCommand(ignore =>
				{
					Mediator.NotifyColleaguesAsync<EditRepositoryEvent>(this);
				});
			}
		}

		public DelegateCommand OnDeleteClick
		{
			get
			{
				return new DelegateCommand(ignore =>
				{
					Mediator.NotifyColleaguesAsync<DeleteRepositoryEvent>(this);
				});
			}
		}

		public string PathString
		{
			get { return Path != null ? Path.ToString() : null; }
			set
			{
				Uri test = null;
				try
				{
					test = new Uri(value, UriKind.Absolute);
				}
				catch(Exception)
				{
					test = null;
				}
				Path = test;
				NotifyPropertyChanged("PathString");
			}
		}

		[DataMember]
		public Uri Path { get; set; }

		[DataMember]
		public string Name
		{
			get { return _name; }
			set
			{
				_name = value;
				NotifyPropertyChanged("Name");
			}
		}

		public enum RepositoryType { Svn, Tfs }

		[DataMember]
		public RepositoryType Type
		{
			get { return _type; }
			set
			{
				_type = value;
				NotifyPropertyChanged("Type");
			}
		}

		private string _userName;

		[DataMember]
		public string UserName
		{
			get { return _userName; }
			set
			{
				_userName = value;
				NotifyPropertyChanged("UserName");
			}
		}

		[DataMember]
		public string EncodedPassword { get; set; }

		public string Password
		{
			get { return !string.IsNullOrWhiteSpace(EncodedPassword) ? Encoding.UTF8.GetString(Convert.FromBase64String(EncodedPassword)) : null; }
			set
			{
				EncodedPassword = null;
				if(!string.IsNullOrWhiteSpace(value))
				{
					EncodedPassword = Convert.ToBase64String(Encoding.UTF8.GetBytes(value));
				}
				NotifyPropertyChanged("Password");
			}
		}

		private int? _secondsToTimeoutDownload;
		[DataMember]
		public int SecondsToTimeoutDownload
		{
			// default is 15
			get { return _secondsToTimeoutDownload ?? 15; }
			set
			{
				_secondsToTimeoutDownload = value;
				NotifyPropertyChanged("SecondsToTimeoutDownload");
			}
		}

		#region Implementation of IDataErrorInfo

		public string Error
		{
			get { throw new NotImplementedException(); }
		}

		public string this[string columnName]
		{
			get
			{
				string result = null;
				switch(columnName)
				{
					case "PathString":
						if(string.IsNullOrEmpty(PathString) || !new Uri(PathString).IsAbsoluteUri)
						{
							result = "Please enter an absolute Url.";
						}
						break;
					case "Name":
						if(string.IsNullOrEmpty(Name))
						{
							result = "Please enter a nickname for this repository.";
						}
						break;
					case "Password":
						if(string.IsNullOrEmpty(Password) && !string.IsNullOrWhiteSpace(UserName))
						{
							result = "Your password is weak.  Weakness will not be tolerated.";
						}
						break;
				}
				return result;
			}
		}

		#endregion

		public void Overwrite(Repository repoBeforeEdit)
		{
			this.Path = repoBeforeEdit.Path;
			this.UserName = repoBeforeEdit.UserName;
			this.Name = repoBeforeEdit.Name;
			this.Type = repoBeforeEdit.Type;
			this.Password = repoBeforeEdit.Password;
		}
	}
}
