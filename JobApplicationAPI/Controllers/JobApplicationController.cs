using JobApplicationAPI.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Net;
using System.IO;

namespace JobApplicationAPI.Controllers
{
    
    public class JobApplicationController : Controller
    {
        int id;
          
        private readonly IMemoryCache _cache;
        public JobApplicationController(ILogger<JobApplicationController> logger, IMemoryCache memoryCache)
        {
            _cache = memoryCache;
        }
        [HttpGet("/api/userdata")]
        public IActionResult GetUserData()
        {
            SqlConnection con = new SqlConnection("Server=localhost; Database=JobApplicationDb; Integrated Security=true");
            
            SqlCommand cmd = new SqlCommand("SELECT p.*, exp.[Company Name], exp.[Start Year], exp.[End Year], e.Salary, " +
                "e.Additional from personal_info p INNER JOIN experiences exp on p.[User ID] = exp.[User ID]INNER JOIN expectations e on p.[User ID] = e.[User ID]; ", con);
            con.Open();
            SqlDataReader rdr = cmd.ExecuteReader();
            CacheData user = new CacheData();
            user.value = new List<UserData>();
            UserData info = new UserData();
            
            while (rdr.Read())
            {
                ReadSingleRow(rdr, user);
            }

            rdr.GetSchemaTable();
            
            CacheData data = new CacheData();

            //con.Close();
            
            
            return Ok(user);

            
        }
        public void ReadSingleRow(IDataRecord record, CacheData userData)
        {
            
           
            List<IDataRecord> values1 = new List<IDataRecord>();
            string[] values = new string[record.FieldCount];
            for (int i = 0; i < record.FieldCount; i++)
            {
                values[i] = record[i].ToString();
            }
         
            var userId = Convert.ToInt32(values[0]); //User ID yi tablodan alıp bir değişkene atadık.
            var user = userData.value.FirstOrDefault(x => x.UserId == userId); //

            if(user != null)
            {
                user.UserId = userId;
                user.PersonalInfo.FName = values[1];
                user.PersonalInfo.LName = values[2];
                user.PersonalInfo.BDate = Convert.ToDateTime(values[3]);
                user.Experiences.Add(new Experiences
                {
                    CompanyName = values[4],
                    StartYear = Convert.ToInt32(values[5]),
                    EndYear = Convert.ToInt32(values[6])
                });
                user.Expectations.Salary = Convert.ToInt32(values[7]);
                user.Expectations.AdditionalExp = values[8];
            }
            else
            {
                
                userData.value.Add(new UserData //Object initializer ile nesne oluşturma esnasında property'lere değer atama
                {
                    UserId = userId,
                    PersonalInfo = new PersonalInfo //Object initializer ile nesne oluşturma esnasında property'lere değer atama
                    {
                        FName = values[1],
                        LName = values[2],
                        BDate = Convert.ToDateTime(values[3])
                    },
                    Experiences = new List<Experiences>()
                    {
                        new Experiences
                        {
                            CompanyName = values[4],
                            StartYear = Convert.ToInt32(values[5]),
                            EndYear = Convert.ToInt32(values[6])
                        }
                    },
                    Expectations = new Expectations
                    {
                        Salary = Convert.ToInt32(values[7]),
                        AdditionalExp = values[8]
                    }

                }) ; 
            }

            int[] arr = new int[record.FieldCount];
            for (int i = 0; i < record.FieldCount; i++)
            {
                arr[i] = i;
            }
            foreach (int i in arr)
            {
                Console.Write(String.Format("{0} ", values[i]));
            }
            Console.WriteLine();
        }


        [HttpPost("api/post")]
        public IActionResult PostUserData([FromBody] UserData userData)
        {
            CacheData data;


           


                
            
                SqlConnection con = new SqlConnection("Server=localhost; Database=JobApplicationDb; Integrated Security=true");
                con.Open();
                string strCommandPersonalInfo = "INSERT INTO personal_info([First Name], [Last Name], [Birth Date])" +
                    "VALUES (" + "'"+userData.PersonalInfo.FName+"'" + ", " + "'" + userData.PersonalInfo.LName+ "'" + ", " 
                    +"'"+userData.PersonalInfo.BDate + "'" + ")";
                
                SqlCommand cmdPersonalInfo = new SqlCommand(strCommandPersonalInfo,con);
                SqlDataReader rdrPI = cmdPersonalInfo.ExecuteReader();
                rdrPI.Close();
                SqlCommand test = new SqlCommand("SELECT * from personal_info", con);
                Console.WriteLine(strCommandPersonalInfo);
                SqlDataReader userIdReader = test.ExecuteReader();
      
                while (userIdReader.Read())
                {
                    GetUserId(userIdReader);
                    int idd = Convert.ToInt32(userIdReader.GetValue(0));
                }
            userIdReader.Close();
                SqlCommand cmdExperiences;
                string strCommandExperiences = String.Empty;
                for (int i = 0; i < userData.Experiences.Count; i++)
                {
                    var exp = userData.Experiences[i];
                    cmdExperiences = new SqlCommand("INSERT INTO experiences([User ID], [Company Name], [Start Year], [End Year]) VALUES("
                        + id + ", " +"'"+ exp.CompanyName+"'" + ", " + exp.StartYear + ", " + exp.EndYear + ")", con);
                SqlDataReader rdrExperiences = cmdExperiences.ExecuteReader();
                rdrExperiences.Close();
                }
                
                
                
                
                SqlCommand cmdExpectations = new SqlCommand("INSERT INTO expectations([User ID], Salary, Additional)" + 
                    "VALUES (" + id + ", " + userData.Expectations.Salary + ", " + "'"+userData.Expectations.AdditionalExp+"'" + ")", con);

            SqlDataReader rdrExpectations = cmdExpectations.ExecuteReader();
            rdrExpectations.Close();
                con.Close();


                return Ok();
            
            

        }
        public void GetUserId(IDataRecord record)
        {

            id = Convert.ToInt32(record[0]);
           

        }
    }
}
