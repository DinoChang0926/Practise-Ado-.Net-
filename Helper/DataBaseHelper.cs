using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Practise.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Practise.Helper
{
    public class DataBaseHelper
    {
        private readonly IConfiguration _config;
        private readonly ILogger<DataBaseHelper> _logger;
        private string DbConnect => _config.GetValue<string>("ConnectionStrings:DBConnection");
        public DataBaseHelper(IConfiguration config, 
                              ILogger<DataBaseHelper> logger)
        {
            _config = config;
            _logger = logger;
        }

        public string DataCreate(Member member)
        {
            string result = "Successed";
            try
            {
                if (CheckMember(member.Email))
                    return "Email重複申請";
                using (SqlConnection con = new SqlConnection(this.DbConnect))
                {
                    SqlCommand cm = new SqlCommand("insert into Member(name,sex,email,cellPhone,phone,status,reportTo,createTime )" +
                                                                "values(@name,@sex,@email,@cellPhone,@phone,@status,@reportTo,@createTime);");
                    cm.Connection = con;
                    con.Open();
                    cm.Parameters.AddWithValue("@name", member.Name);
                    cm.Parameters.AddWithValue("@sex", member.Sex);
                    cm.Parameters.AddWithValue("@email", member.Email);
                    cm.Parameters.AddWithValue("@cellPhone", member.CellPhone);
                    cm.Parameters.AddWithValue("@phone", member.Phone);
                    cm.Parameters.AddWithValue("@status", member.Status);
                    cm.Parameters.AddWithValue("@reportTo", member.ReportTo);
                    cm.Parameters.AddWithValue("@createTime", DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss"));
                    cm.ExecuteNonQuery();
                    _logger.LogInformation("{0} Create Member: {1}",DateTime.Now,member.Email);
                }
                var MemberId = GetMemberId(member.Email);
                using (SqlConnection con = new SqlConnection(this.DbConnect))
                {
                    SqlDataAdapter skillAdapter = new SqlDataAdapter();
                    DataSet ds = new DataSet();
                    skillAdapter.SelectCommand = new SqlCommand("select * from MemberSkill", con);
                    SqlCommandBuilder Skillbuilder = new SqlCommandBuilder(skillAdapter);
                    skillAdapter.Fill(ds, "MemberSkill");
                    foreach (var skill in member.Skills)
                    {
                        DataRow new_row = ds.Tables["MemberSkill"].NewRow();
                        new_row["MemberId"] = MemberId;
                        new_row["skillName"] = skill;
                        ds.Tables["MemberSkill"].Rows.Add(new_row);
                    }
                    SqlDataAdapter AreaAdapter = new SqlDataAdapter();
                    AreaAdapter.SelectCommand = new SqlCommand("select * from Area", con);
                    SqlCommandBuilder Areabuilder = new SqlCommandBuilder(AreaAdapter);
                    AreaAdapter.Fill(ds, "Area");
                    foreach (var area in member.Area)
                    {
                        DataRow new_row = ds.Tables["Area"].NewRow();
                        new_row["MemberId"] = MemberId;
                        new_row["Area"] = area;
                        ds.Tables["Area"].Rows.Add(new_row);
                    }
                    AreaAdapter.Update(ds, "Area");
                    _logger.LogDebug("{0} {1} Area Create Sueccessed",DateTime.Now,member.Email);
                    skillAdapter.Update(ds, "MemberSkill");
                    _logger.LogDebug("{0} {1} Skill Create Sueccessed", DateTime.Now, member.Email);
                }

            }
            catch (Exception e)
            {
                _logger.LogError("{0} {1} Create fail : " + e.ToString(),DateTime.Now, member.Email);
                return "Create Fail";
            }
            return result;
        }

        public string Update(Member member)
        {
            string result = "Successed";
            try
            {  
                using (SqlConnection con = new SqlConnection(this.DbConnect))
                {
                    SqlDataAdapter Adapter = new SqlDataAdapter();
                    DataSet ds = new DataSet();
                    Adapter.SelectCommand = new SqlCommand("select * from Member where memberId = @memberId", con);
                    Adapter.SelectCommand.Parameters.AddWithValue("@memberId",member.Id);
                    SqlCommandBuilder Skillbuilder = new SqlCommandBuilder(Adapter);
                    Adapter.Fill(ds, "Member");          
                    foreach (DataRow row in ds.Tables["Member"].Select("memberId = '" + member.Id + "'"))
                    {
                        row["Name"] = member.Name;
                        row["email"] = member.Email;
                        row["sex"] = member.Sex;
                        row["cellPhone"] = member.CellPhone;
                        row["phone"] = member.Phone;
                        row["status"] = member.Status;
                        row["reportTo"] = member.ReportTo;
                        row["editor"] = member.Editor;
                        row["editTime"] = DateTime.Now;
                    }
                    Adapter.Update(ds, "Member");
                }

                if(UpdateArea(member.Area, member.Id) || UpdateSkill(member.Skills, member.Id))
                    return "update Fail";
            }
            catch (Exception e)
            {
                _logger.LogError("{0} {1} Create fail : " + e.ToString(), DateTime.Now, member.Email);
                return "update Fail";
            }
            return result;
        }




        private long? GetMemberId(string mail)
        {
            long? result = null;
            using (SqlConnection con = new SqlConnection(this.DbConnect))
            {
                SqlCommand cm = new SqlCommand("select top 1 * from Member where email = @email", con);
                con.Open();
                cm.Parameters.AddWithValue("@email", mail);
                SqlDataReader sdr = cm.ExecuteReader();
                if (sdr.Read())
                {
                    result = long.Parse(sdr["memberId"].ToString());
                }
            }
            return result;
        }
        public Member GetMember(int MemberId)
        {
            Member result = new Member { };
            try
            {
                using (SqlConnection con = new SqlConnection(this.DbConnect))
                {
                    SqlCommand cm = new SqlCommand("select * from Member where memberId = @memberId", con);
                    con.Open();
                    cm.Parameters.AddWithValue("@memberId", MemberId);
                    SqlDataReader sdr = cm.ExecuteReader();
                    if(sdr.Read())
                    {
                        result = new Member
                        {
                            Id = int.Parse(sdr["memberId"].ToString()),
                            Name = sdr["Name"].ToString(),
                            Email = sdr["email"].ToString(),
                            Sex = sdr["sex"].ToString(),
                            CellPhone = sdr["cellPhone"].ToString(),
                            Phone = sdr["phone"].ToString(),
                            CreateTime = sdr["createTime"].ToString(),
                            Status = sdr["status"].ToString(),
                            ReportTo = sdr["reportTo"].ToString(),
                            Editor = sdr["editor"].ToString(),
                            EditTime = sdr["editTime"].ToString()
                        };
                        
                    }
                }               
                result.Area = GetArea(result.Id);
                result.Skills = GetSkill(result.Id);                
            }
            catch (Exception ex)
            {
                _logger.LogError("{0} GetMember fail : {1}", DateTime.Now, ex);
            }

            return result;
        }
        public List<Member> GetMembers()
        {
            List<Member> result = new List<Member> { };
            try
            {
                using (SqlConnection con = new SqlConnection(this.DbConnect))
                {
                    SqlDataAdapter sde = new SqlDataAdapter("select * from Member", con);
                    DataSet ds = new DataSet();
                    sde.Fill(ds,"Member");                 
                    foreach (DataRow row in ds.Tables["Member"].Rows)
                    {
                            Member member = new Member
                            {
                                Id = int.Parse(row["memberId"].ToString()),
                                Name = row["Name"].ToString(),
                                Email = row["email"].ToString(),
                                Sex = row["sex"].ToString(),
                                CellPhone = row["cellPhone"].ToString(),
                                Phone = row["phone"].ToString(),
                                CreateTime = row["createTime"].ToString(),
                                Status = row["status"].ToString(),
                                ReportTo = row["reportTo"].ToString(),
                                Editor = row["editor"].ToString(),
                                EditTime = row["editTime"].ToString()
                            };
                            result.Add(member);
                    }   
                }
                foreach(var m in result)
                {
                    m.Area = GetArea(m.Id);
                    m.Skills = GetSkill(m.Id);
                }
            }
            catch(Exception ex)
            {
                _logger.LogError("{0} GetMembers fail : {1}" , DateTime.Now , ex );
            }           
            return result;
        }

         private bool CheckMember(string mail)
        {
            bool result = false;
            using (SqlConnection con = new SqlConnection(this.DbConnect))
            {                
                SqlCommand cm = new SqlCommand("select top 1 * from Member where email = @email",con);
                con.Open();
                cm.Parameters.AddWithValue("@email", mail);
                SqlDataReader sdr = cm.ExecuteReader();
                if (sdr.Read())
                {
                    if (mail.Equals(sdr["email"].ToString()))
                        result = true;
                }
            }
            return result;
        }

        private List<string> GetArea(int MemberId)
        {
            List<string> result = new List<string> { };
            try
            {
                using (SqlConnection con = new SqlConnection(this.DbConnect))
                {
                    SqlCommand cm = new SqlCommand("select * from Area where memberId = @memberId", con);
                    con.Open();
                    cm.Parameters.AddWithValue("@memberId", MemberId);
                    SqlDataReader sdr = cm.ExecuteReader();
                    while (sdr.Read())
                    {
                        result.Add(sdr["Area"].ToString());
                    };    
                }  
            }
            catch (Exception e)
            {
                _logger.LogError("{0} Member Id = {1} GetArea fail : {2} ", DateTime.Now, MemberId , e);
            }
            return result;
        }
        private List<string> GetSkill(int MemberId)
        {
            List<string> result = new List<string> { };
            try
            {
                using (SqlConnection con = new SqlConnection(this.DbConnect))
                {
                    SqlCommand cm = new SqlCommand("select * from MemberSkill where memberId = @memberId", con);
                    con.Open();
                    cm.Parameters.AddWithValue("@memberId", MemberId);
                    SqlDataReader sdr = cm.ExecuteReader();
                    while (sdr.Read())
                    {
                        result.Add(sdr["SkillName"].ToString());
                    };
                }
            }
            catch (Exception e)
            {
                _logger.LogError("{0} Member Id = {1} GetSkill fail : {2} ", DateTime.Now, MemberId, e);
            }

            return result;
        }
        private bool UpdateSkill(List<string> Skills,int MemberId)
        {
            List<string> originalSkill = GetSkill(MemberId);
            List<string> update = Skills.Except(originalSkill).ToList();
            List<string> delete = originalSkill.Except(Skills).ToList();
            try 
            {
                using (SqlConnection con = new SqlConnection(this.DbConnect))
                {
                    SqlDataAdapter skillAdapter = new SqlDataAdapter();
                    DataSet ds = new DataSet();
                    skillAdapter.SelectCommand = new SqlCommand("select * from MemberSkill where memberId = @memberId ", con);
                    skillAdapter.SelectCommand.Parameters.AddWithValue("@memberId", MemberId);
                    SqlCommandBuilder Skillbuilder = new SqlCommandBuilder(skillAdapter);
                    skillAdapter.Fill(ds, "MemberSkill");
                    foreach (var skill in update)
                    {
                        DataRow new_row = ds.Tables["MemberSkill"].NewRow();
                        new_row["MemberId"] = MemberId;
                        new_row["skillName"] = skill;
                        ds.Tables["MemberSkill"].Rows.Add(new_row);
                    }
                   
                    foreach(var skill in delete)
                    {                   
                        foreach (DataRow row in ds.Tables["MemberSkill"].Select("skillName = '" + skill + "' and memberId='" + MemberId + "'"))
                        {
                           row.Delete();
                        }
                    }
                    skillAdapter.Update(ds, "MemberSkill");


                    _logger.LogDebug("{0} {1} Skill Update Sueccessed", DateTime.Now, MemberId);
                }
            }
            catch(Exception ex)
            {
                _logger.LogError("{0} MemberId:{1}  Update Skill Fail :{2}", DateTime.Now, MemberId, ex);
                return true;
            }
            return false;

        }
        private bool UpdateArea(List<string> Areas, int MemberId)
        {
            List<string> originalArea = GetArea(MemberId);
            List<string> update = Areas.Except(originalArea).ToList();
            List<string> delete = originalArea.Except(Areas).ToList();
            try
            {
                using (SqlConnection con = new SqlConnection(this.DbConnect))
                {
                    DataSet ds = new DataSet();
                    SqlDataAdapter AreaAdapter = new SqlDataAdapter();
                    AreaAdapter.SelectCommand = new SqlCommand("select * from Area where memberId = @memberId", con);
                    AreaAdapter.SelectCommand.Parameters.AddWithValue("@memberId", MemberId);
                    SqlCommandBuilder Areabuilder = new SqlCommandBuilder(AreaAdapter);
                    AreaAdapter.Fill(ds, "Area");
                    foreach (var area in update)
                    {
                        DataRow new_row = ds.Tables["Area"].NewRow();
                        new_row["MemberId"] = MemberId;
                        new_row["Area"] = area;
                        ds.Tables["Area"].Rows.Add(new_row);
                    }
                   
                    foreach (var area in delete)
                    {
                        foreach (DataRow row in ds.Tables["Area"].Select("Area = '" + area + "' and memberId='" + MemberId + "'"))
                        {
                            row.Delete();
                        }
                    }
                    AreaAdapter.Update(ds, "Area");
                    _logger.LogDebug("{0} {1} Area Create Sueccessed", DateTime.Now, MemberId);
                }
            }
            catch(Exception ex)
            {
                _logger.LogError("{0} MemberId:{1}  Update Area Fail :{2}", DateTime.Now, MemberId, ex);
                return true;
            }
            return false;
        }

    }
}
