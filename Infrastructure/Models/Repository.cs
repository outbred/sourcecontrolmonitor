using System;
using System.Runtime.Serialization;
using System.Text;
using Infrastructure.Utilities;

namespace Infrastructure.Models
{
	[DataContract]
	public class Repository : ViewModelBase
	{
		private string _name;
		private string _type;

		private Uri _path;

		[DataMember]
		public Uri Path
		{
			get { return _path; }
			set
			{
				_path = value;
				NotifyPropertyChanged("Path");
			}
		}

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

		[DataMember]
		public string Type
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

		private string _password;

		[DataMember]
		public string EncodedPassword { get; set; }

		public string Password
		{
			get { return _password; }
			set
			{
				EncodedPassword = null;
				if(!string.IsNullOrWhiteSpace(value))
				{
					EncodedPassword = Convert.ToBase64String(Encoding.UTF8.GetBytes(value));
				}
				_password = value;
				NotifyPropertyChanged("Password");
			}
		}
	}
}
