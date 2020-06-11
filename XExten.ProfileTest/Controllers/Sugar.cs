using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XExten.ProfileTest.Controllers
{
	public class Sugar
	{
		public static SqlSugarClient DB
		{
			get
			{
				SqlSugarClient Client = new SqlSugarClient(new ConnectionConfig
				{
					ConnectionString = "Data Source=106.58.174.64;Initial Catalog=ZSY;uid=zsy;pwd=zsy123456!@#$!",
					DbType = DbType.SqlServer,
					IsAutoCloseConnection = true,
					InitKeyType = InitKeyType.SystemTable
				});
				Client.Aop.OnLogExecuted = delegate (string Sql, SugarParameter[] Args)
				{

				};
				return Client;
			}
		}
	}
}
