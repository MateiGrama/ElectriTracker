using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;

namespace ElectriTrackerService.Controllers
{

    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class TrackerController : ApiController
    {
        [HttpGet]
        public string[] GetAllInformation()
        {
            /*
            Format mesaj:
            "Nume,Latitudine,Longitudine,Baterie,Status,Urgenta,Telefon"
             * Status: 0=liber; 1=ocupat; 2=indisponibil
             * Urgenta: 0=nu; 1=da
            */
            List<string> items = new List<string>();

            string rootPath = HttpContext.Current.Server.MapPath("~");

            SqlConnection myConnection = new SqlConnection(@"Data Source=(LocalDB)\v11.0;AttachDbFilename=" + rootPath + @"ElectriTracker.mdf;Integrated Security=True;Connect Timeout=30");
            myConnection.Open();

            SqlCommand myCommand = new SqlCommand("Select * From [Electricieni] ", myConnection);

            SqlDataReader myReader = myCommand.ExecuteReader();

            while (myReader.Read())
            {
                int status = 2;
                if (myReader["Status"] != null && myReader["Status"] != DBNull.Value)
                {
                    status = (int)myReader["Status"];
                }
                

                DateTime ultimaActualizare = new DateTime(1000, 1, 1);
                if (myReader["UltimaActualizare"] != null && myReader["UltimaActualizare"] != DBNull.Value)
                {
                    ultimaActualizare = (DateTime)myReader["UltimaActualizare"];
                }
                    
                DateTime dataCurenta = DateTime.Now;
                if (dataCurenta - ultimaActualizare > new TimeSpan(0, 30, 0))
                {
                    status = 2;
                }

                string deAfisat = myReader["Nume"].ToString() + "," + myReader["Latitudine"] + "," + myReader["Longitudine"] + "," + myReader["Baterie"] + "," + status + "," + myReader["Urgenta"] + "," + myReader["Contact"];
                items.Add(deAfisat);
            }

            myConnection.Close();
            return items.ToArray();
        }

        [HttpPost]
        public string SendAllInformation([FromBody]string information)
        {
            /*
            Format mesaj:
            "Nume,Latitudine,Longitudine,Baterie,Status,Urgenta"
                * Status: 0=liber; 1=ocupat
                * Urgenta: 0=nu; 1=da
             * "Sorin Peste,44.87405309180625,25.629332788207837,15,0,0",
            */
            information = HttpContext.Current.Request["information"];
            char[] delimiterChars = { ',' };
            string[] words = information.Split(delimiterChars);

            string rootPath = HttpContext.Current.Server.MapPath("~");

            SqlConnection myConnection = new SqlConnection(@"Data Source=(LocalDB)\v11.0;AttachDbFilename=" + rootPath + @"ElectriTracker.mdf;Integrated Security=True;Connect Timeout=30");
            myConnection.Open();

            SqlCommand myCommand = new SqlCommand("UPDATE [Electricieni] SET  Latitudine = " + words[1] + ", Longitudine = " + words[2] + ", Baterie= " + words[3] + ", Status  = " + words[4] + ", Urgenta = " + words[5] + ", UltimaActualizare = GETDATE() WHERE Nume = '" + words[0] + "'", myConnection);
            myCommand.ExecuteNonQuery();

            myConnection.Close();
            return "OK";
        }

        [HttpPost]
        public string RegisterUser([FromBody]string userdata)
        {
            userdata = HttpContext.Current.Request["userdata"];
            char[] delimiterChars = { ',' };

            string[] words = userdata.Split(delimiterChars);

            string rootPath = HttpContext.Current.Server.MapPath("~");

            SqlConnection myConnection = new SqlConnection(@"Data Source=(LocalDB)\v11.0;AttachDbFilename=" + rootPath + @"ElectriTracker.mdf;Integrated Security=True;Connect Timeout=30");
            myConnection.Open();

            SqlCommand myCommand = new SqlCommand("UPDATE [Electricieni] SET  Contact = '" + words[1] + "' WHERE Nume = '" + words[0] + "'", myConnection);
            int rows = myCommand.ExecuteNonQuery();

            if (rows == 0)
            {
                myCommand = new SqlCommand("INSERT INTO [Electricieni] (Nume, Contact) VALUES ('" + words[0] + "','" + words[1] + "')", myConnection);
                myCommand.ExecuteNonQuery();
            }

            myConnection.Close();
            return "OK";
        }

    }
}