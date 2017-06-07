using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MyGenericContext.Utilities;
using MyGenericContext.Models;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;
using MyGenericContext.Filters;

namespace MyGenericContext.Controllers
{
    [Route("api/[controller]")]
    public class TestController : Controller
    {
        private readonly ILogger _Logger;
        /// <summary>
        /// Constructor
        /// </summary>
        public TestController(ILogger<TestController> logger)
        {
            _Logger = logger;
        }

        [TypeFilter(typeof(LogFilterAttribute))]
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

        [TypeFilter(typeof(LogFilterAttribute))]
        // GET api/values/5
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            //_Logger.LogInformation(LoggingEvents.GENERIC_MESSAGE, $"Method call {UtilityMethods.GetCallerMemberName()}");
            try
            {
                ObjectPropertyGenerator generator = new ObjectPropertyGenerator();
                var result = generator.ReadObjectAndParseProperties(new NestedObjectModel());
                string JSONString = JsonConvert.SerializeObject(result);

                if(string.IsNullOrWhiteSpace(JSONString))
                {
                    return BadRequest("No json data available");
                }
                else
                {
                    return Ok(JSONString);
                }
            }
            catch(Exception e)
            {
                return BadRequest($"exception caught: {e.Message}");
            }
        }
    }
}