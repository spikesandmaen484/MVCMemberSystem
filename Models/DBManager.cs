using Microsoft.Ajax.Utilities;
using System;
using System.Data;
using System.Data.SqlClient;
using System.EnterpriseServices;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Xml.Linq;

namespace Exam.Models
{
    public class DBManager
    {
        private readonly string connStr = WebConfigurationManager.ConnectionStrings["MSSQL"].ConnectionString;

        public DataTable GetAccountData(string name)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @" SELECT *
                                           FROM [msdb].[dbo].[Member]
                                          WHERE [Name]  = @Name "
                        ;

                    cmd.Parameters.AddWithValue("@Name", name);
                    conn.Open();
                    dt.Load(cmd.ExecuteReader());
                    conn.Close();
                }
            }

            return dt;
        }

        public DataTable GetAccountDataByEmail(string email)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @" SELECT *
                                           FROM [msdb].[dbo].[Member]
                                          WHERE [Email]  = @Email "
                        ;

                    cmd.Parameters.AddWithValue("@Email", email);
                    conn.Open();
                    dt.Load(cmd.ExecuteReader());
                    conn.Close();
                }
            }

            return dt;
        }

        public DataTable GetMemberData(Search search)
        {
            DataTable dt = new DataTable();
            string sql = string.Empty;
            string whereSql = string.Empty;
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    sql = @" SELECT Name, Age, phoneNumber, Email
                               FROM [msdb].[dbo].[Member]
                              WHERE 1 = 1
                                {0} "
                        ;

                    if (!string.IsNullOrEmpty(search.Name)) 
                    {
                        whereSql += " AND [Name]  = @Name ";
                        cmd.Parameters.AddWithValue("@Name", search.Name);
                    }
                    if (search.Year > 0)
                    {
                        whereSql += " AND [birthday]  LIKE (@birthday + '%') ";
                        cmd.Parameters.AddWithValue("@birthday", search.Year.ToString());
                    }

                    cmd.CommandText = string.Format(sql, whereSql);
                    conn.Open();
                    dt.Load(cmd.ExecuteReader());
                    conn.Close();
                }
            }

            return dt;
        }

        public DataTable GetAllMemberName()
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @" SELECT Name
                                           FROM [msdb].[dbo].[Member]
                                           "
                        ;

                    conn.Open();
                    dt.Load(cmd.ExecuteReader());
                    conn.Close();
                }
            }

            return dt;
        }

        public DataTable GetActivity(Activity activity)
        {
            DataTable dt = new DataTable();
            string sql = string.Empty;
            string whereSql = string.Empty;
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    sql = @" SELECT *
                               FROM [msdb].[dbo].[Activity]
                              WHERE 1 = 1
                                {0}  "
                    ;

                    if (!string.IsNullOrEmpty(activity.Name))
                    {
                        whereSql += " AND [Name]  = @Name ";
                        cmd.Parameters.AddWithValue("@Name", activity.Name);
                    }
                    if (!string.IsNullOrEmpty(activity.ActivityDateStart))
                    {
                        whereSql += " AND CONVERT(date, ActivityDateStart, 111)  <= CONVERT(date, @ActivityDateStart, 111) ";
                        cmd.Parameters.AddWithValue("@ActivityDateStart", activity.ActivityDateStart);
                    }
                    if (!string.IsNullOrEmpty(activity.ActivityDateEnd))
                    {
                        whereSql += " AND CONVERT(date, ActivityDateEnd, 111)  >= CONVERT(date, @ActivityDateEnd, 111) ";
                        cmd.Parameters.AddWithValue("@ActivityDateEnd", activity.ActivityDateEnd);
                    }

                    cmd.CommandText = string.Format(sql, whereSql);
                    conn.Open();
                    dt.Load(cmd.ExecuteReader());
                    conn.Close();
                }
            }

            return dt;
        }

        public DataTable CheckAccount(LoginValid loginValid)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @" SELECT Name, Email
                                           FROM [msdb].[dbo].[Member]
                                          WHERE [Email] = @Email
                                            AND [Password] = @Password "
                        ;

                    cmd.Parameters.AddWithValue("@Email", loginValid.Email);
                    cmd.Parameters.AddWithValue("@Password", loginValid.Password);
                    conn.Open();
                    dt.Load(cmd.ExecuteReader());
                    conn.Close();
                }
            }

            return dt;
        }

        public string CheckTime(string timeS, string timeE)
        {
            string errMsg = string.Empty;
            if (string.IsNullOrEmpty(timeS) && string.IsNullOrEmpty(timeE))
            {
                return errMsg;
            }

            DateTime result;
            string sDateS = string.Empty;
            string sDateE = string.Empty;
            if (!string.IsNullOrEmpty(timeS)) 
            {
                string[] strDateS = timeS.Split('/');
                sDateS = strDateS[1] + "/" + strDateS[2] + "/" + strDateS[0];
                if (!DateTime.TryParse(sDateS, out result))
                {
                    errMsg = "日期起驗證錯誤!";
                    return errMsg;
                }
            }
            if (!string.IsNullOrEmpty(timeE)) 
            {
                string[] strDateE = timeE.Split('/');
                sDateE = strDateE[1] + "/" + strDateE[2] + "/" + strDateE[0];
                if (!DateTime.TryParse(sDateE, out result))
                {
                    errMsg = "日期訖驗證錯誤!";
                    return errMsg;
                }
            }

            if (!string.IsNullOrEmpty(timeS) && !string.IsNullOrEmpty(timeE))
            {
                DateTime dateTimeS;
                DateTime dateTimeE;
                DateTime.TryParse(sDateS, out dateTimeS);
                DateTime.TryParse(sDateE, out dateTimeE);
                if (dateTimeS > dateTimeE)
                {
                    errMsg = "活動日期起日需小於訖日!";
                }
            }

            return errMsg;
        }

        public bool ExistEmail(string email) 
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @" SELECT 1
                                           FROM [msdb].[dbo].[Member]
                                          WHERE [Email] = @Email "
                    ;

                    cmd.Parameters.AddWithValue("@Email", email);
                    conn.Open();
                    dt.Load(cmd.ExecuteReader());
                    conn.Close();
                }
            }

            if (dt.Rows.Count > 0)
                return true;
            else
                return false;

        }

        public bool ExistPhone(string phoneNumber)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @" SELECT 1
                                           FROM [msdb].[dbo].[Member]
                                          WHERE [phoneNumber] = @phoneNumber "
                    ;

                    cmd.Parameters.AddWithValue("@phoneNumber", phoneNumber);
                    conn.Open();
                    dt.Load(cmd.ExecuteReader());
                    conn.Close();
                }
            }

            if (dt.Rows.Count > 0)
                return true;
            else
                return false;

        }

        public bool ExistID(string id)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @" SELECT 1
                                           FROM [msdb].[dbo].[Member]
                                          WHERE [ID] = @ID "
                    ;

                    cmd.Parameters.AddWithValue("@ID", id);
                    conn.Open();
                    dt.Load(cmd.ExecuteReader());
                    conn.Close();
                }
            }

            if (dt.Rows.Count > 0)
                return true;
            else
                return false;

        }

        public bool ExistActivityName(string name)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @" SELECT 1
                                           FROM [msdb].[dbo].[Activity]
                                          WHERE [Name] = @Name "
                    ;

                    cmd.Parameters.AddWithValue("@Name", name);
                    conn.Open();
                    dt.Load(cmd.ExecuteReader());
                    conn.Close();
                }
            }

            if (dt.Rows.Count > 0)
                return true;
            else
                return false;

        }

        public int AddNewEmail(Member member) 
        {
            int count = 0;
            using (SqlConnection conn = new SqlConnection(connStr)) 
            {
                using (SqlCommand cmd = conn.CreateCommand()) 
                {
                    cmd.CommandText = @" INSERT INTO [msdb].[dbo].[Member]
                                              ([Email]
                                               ,[phoneNumber]
                                               ,[Password]
                                               ,[Name]
                                               ,[RegisterTime]
                                               )
                                         VALUES
                                               (@Email
                                               , @phoneNumber
                                               , @Password
                                               , @Name
                                               , SYSDATETIME()
                                               );"
                        ;

                    cmd.Parameters.AddWithValue("@Email", member.Email);
                    cmd.Parameters.AddWithValue("@phoneNumber", member.phoneNumber);
                    cmd.Parameters.AddWithValue("@Password", member.Password);
                    cmd.Parameters.AddWithValue("@Name", member.Name);
                    conn.Open();
                    count = cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
            return count;
        }

        public int AddNewMember(Member member)
        {
            int count = 0;
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @" INSERT INTO [msdb].[dbo].[Member]
                                              ([Email]
                                               ,[phoneNumber]
                                               ,[Name]
                                               ,[Gender]
                                               ,[Age]
                                               ,[ID]
                                               ,[birthday]
                                               ,[Address]
                                               ,[Department]
                                               ,[School]
                                               ,[RegisterTime]
                                               )
                                         VALUES
                                               (@Email
                                               ,@phoneNumber
                                               ,@Name
                                               ,@Gender
                                               ,@Age
                                               ,@ID
                                               ,@birthday
                                               ,@Address
                                               ,@Department
                                               ,@School
                                               , SYSDATETIME()
                                               );"
                        ;

                    cmd.Parameters.AddWithValue("@Email", member.Email);
                    cmd.Parameters.AddWithValue("@phoneNumber", member.phoneNumber);
                    cmd.Parameters.AddWithValue("@Name", member.Name);
                    cmd.Parameters.AddWithValue("@Gender", member.Gender);
                    cmd.Parameters.AddWithValue("@Age", member.Age);
                    cmd.Parameters.AddWithValue("@ID", member.ID);
                    cmd.Parameters.AddWithValue("@birthday", member.birthday);
                    cmd.Parameters.AddWithValue("@Address", member.Address);
                    cmd.Parameters.AddWithValue("@Department", member.Department);
                    cmd.Parameters.AddWithValue("@School", member.School);
                    conn.Open();
                    count = cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
            return count;
        }

        public int AddNewActivity(Activity activity, string attendance)
        {
            int count = 0;
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @" INSERT INTO [msdb].[dbo].[Activity]
                                              ([Name]
                                               ,[ActivityDateStart]
                                               ,[ActivityDateEnd]
                                               ,[Address]
                                               ,[Fee]
                                               ,[attendance])
                                         VALUES
                                               (@Name
                                               , @ActivityDateStart
                                               , @ActivityDateEnd
                                               , @Address
                                               , @Fee
                                               , @attendance);"
                    ;

                    cmd.Parameters.AddWithValue("@Name", activity.Name);
                    cmd.Parameters.AddWithValue("@ActivityDateStart", activity.ActivityDateStart);
                    cmd.Parameters.AddWithValue("@ActivityDateEnd", activity.ActivityDateEnd);
                    cmd.Parameters.AddWithValue("@Address", activity.Address);
                    cmd.Parameters.AddWithValue("@Fee", activity.Fee);
                    cmd.Parameters.AddWithValue("@attendance", attendance);

                    conn.Open();
                    count = cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
            return count;
        }

        public int EditPersonalInfo(PersonalInfo personalInfo, string subscribe, byte[] imgBytes)
        {
            int count = 0;
            string sql = string.Empty;
            string photoSql = string.Empty; 
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    sql = @" UPDATE [msdb].[dbo].[Member]
                                SET [phoneNumber] = @phoneNumber
                                    ,[Name] = @Name
                                    ,[Gender] = @Gender
                                    ,[EnglishName] = @EnglishName
                                    ,[birthday] = @birthday
                                    ,[IsSubscribe] = @IsSubscribe
                                    ,[Address] = @Address
                                    ,[RegisterTime] = SYSDATETIME()
                                    {0}
                              WHERE Email = @Email "
                    ;

                    cmd.Parameters.AddWithValue("@phoneNumber", personalInfo.phoneNumber);
                    cmd.Parameters.AddWithValue("@Name", personalInfo.Name);
                    cmd.Parameters.AddWithValue("@Gender", personalInfo.Gender);
                    cmd.Parameters.AddWithValue("@EnglishName", personalInfo.EnglishName);
                    cmd.Parameters.AddWithValue("@birthday", personalInfo.birthday);
                    cmd.Parameters.AddWithValue("@IsSubscribe", subscribe);
                    cmd.Parameters.AddWithValue("@Address", personalInfo.Address);

                    if (imgBytes != null)
                    {
                        photoSql = ", [photo] = @photo ";
                        cmd.Parameters.AddWithValue("@photo", imgBytes);
                    }

                    cmd.Parameters.AddWithValue("@Email", personalInfo.Email);
                    cmd.CommandText = string.Format(sql, photoSql);
                    conn.Open();
                    count = cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }

            return count;
        }

        public int UpdateMember(Member member, string oriEmail)
        {
            int count = 0;
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @" UPDATE [msdb].[dbo].[Member]
                                            SET [Email] = @Email_New
                                               ,[phoneNumber] = @phoneNumber
                                               ,[Name] = @Name
                                               ,[Gender] = @Gender
                                               ,[Age] = @Age
                                               ,[ID] = @ID
                                               ,[birthday] = @birthday
                                               ,[Address] = @Address
                                               ,[Department] = @Department
                                               ,[School] = @School
                                               ,[RegisterTime] = SYSDATETIME()
                                         WHERE Email = @Email_Ori
                                               "
                        ;

                    cmd.Parameters.AddWithValue("@Email_New", member.Email);
                    cmd.Parameters.AddWithValue("@phoneNumber", member.phoneNumber);
                    cmd.Parameters.AddWithValue("@Name", member.Name);
                    cmd.Parameters.AddWithValue("@Gender", member.Gender);
                    cmd.Parameters.AddWithValue("@Age", member.Age);
                    cmd.Parameters.AddWithValue("@ID", member.ID);
                    cmd.Parameters.AddWithValue("@birthday", member.birthday);
                    cmd.Parameters.AddWithValue("@Address", member.Address);
                    cmd.Parameters.AddWithValue("@Department", member.Department);
                    cmd.Parameters.AddWithValue("@School", member.School);
                    cmd.Parameters.AddWithValue("@Email_Ori", oriEmail);
                    conn.Open();
                    count = cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
            return count;
        }
    }
}