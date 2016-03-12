namespace Web.Appliaction.Values
{
    using System;
    using System.Collections.Generic;
    using Domain;
    using Domain.ValuesProvider;
    using Microsoft.AspNet.Mvc;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.OptionsModel;

    public class ValuesController : Controller
    {
        private readonly ValuesControllerConfiguration _configuration;

        private readonly IValuesProvider _valuesProvider;
        private readonly ILogger<ValuesController> _logger;


        public ValuesController(IOptions<ValuesControllerConfiguration> optionsAccessor, IValuesProvider valuesProvider, ILogger<ValuesController> logger)
        {
            if (optionsAccessor?.Value == null)
                throw new ArgumentNullException(nameof(optionsAccessor));
            if (valuesProvider == null)
                throw new ArgumentNullException(nameof(valuesProvider));
            if(logger == null)
                throw new ArgumentNullException(nameof(logger));

            _configuration = optionsAccessor.Value;
            _valuesProvider = valuesProvider;
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<string> Get()
        {
            _logger.LogInformation("Get all values");
            return new[] {_configuration.Value1, _configuration.Value2};
        }

        [HttpGet("{id}")]
        public string Get(int id)
        {
            _logger.LogInformation("Get one value");

            if (id > 1)
                throw new ArgumentException("id");

            return _valuesProvider.GetValue(_configuration) + id;
        }
    }
}