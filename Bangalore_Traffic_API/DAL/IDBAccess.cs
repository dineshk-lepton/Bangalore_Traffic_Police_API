using Bangalore_Traffic_API.Model;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bangalore_Traffic_API.DAL
{
    public interface IDBAccess
    {
        public string getAccessToken(string todaydata);
        public int saveAccessToken(string accessToken);
             
        public int saveAPIData(List<BangaloreAPI> list);

        public int saveAPIImageData(List<EventMapImages> list);
       

    }
}
