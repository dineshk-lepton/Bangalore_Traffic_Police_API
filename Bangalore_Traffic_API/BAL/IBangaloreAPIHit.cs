using Bangalore_Traffic_API.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bangalore_Traffic_API.BAL
{
    public interface IBangaloreAPIHit
    {
        
        public Task<int> bangaloreTrafficLoginAPIConsume();
        public Task<int> bangaloreTrafficGetDataAPIConsume(string accessToken);

        
    }
}
