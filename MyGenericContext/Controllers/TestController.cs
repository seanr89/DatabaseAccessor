using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MyGenericContext.Utilities;
using MyGenericContext.Models;

namespace MyGenericContext.Controllers
{
    [Route("api/[controller]")]
    public class TestController : Controller
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public TestController()
        {

        }

        [HttpGet]
        public IEnumerable<string> Get()
        {
            //GenericObjectParser parser = new GenericObjectParser();

            List<Dictionary<string,string>> test = new List<Dictionary<string, string>>();
            Dictionary<string, string> Dict = new Dictionary<string, string>();
            Dict.Add("1", "2");
            test.Add(Dict);
            
            NestedObjectModel Model = new NestedObjectModel();
            UtilityMethods.PrintProperties(Model, 0);
            //parser.CreateObjectListFromDictionaryList<NestedObjectModel>(test);

            return new string[] { "value1", "value2" };
        }
    }
}