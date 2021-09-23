using System.Data.SqlClient;
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

namespace JobApplicationAPI.Controllers
{
    
    public class JobApplicationController : Controller
    {
        
          
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
            Console.WriteLine(rdr.FieldCount);
            UserData user = new UserData();
            while (rdr.Read())
            {
                ReadSingleRow(rdr, user);
            }

            rdr.GetSchemaTable();
            
            CacheData data = new CacheData();
            
            if(!_cache.TryGetValue("userdata", out data))
            {
                data = new CacheData();
                return Ok(data);
            }
            
            return Ok(rdr);

            
        }
        public void ReadSingleRow(IDataRecord record, CacheData userData)
        {
            
                        
            string[] values = new string[record.FieldCount];
            for (int i = 0; i < record.FieldCount; i++)
            {
                values[i] = record[i].ToString();
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
            
            
            bool AlreadyExist = _cache.TryGetValue("userdata", out data);
            if (!AlreadyExist)
            {
                
                data = new CacheData();
                data.value = new List<UserData>();
                data.value.Add(userData);
                _cache.Set("userdata", data);
                return Ok();
            }
            else
            {
                userData.UserId++;
                data.value.Add(userData);
                _cache.Set("userdata", data);
                return Ok();
            }
            

        }

    }
}
