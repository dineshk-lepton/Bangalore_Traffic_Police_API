using Bangalore_Traffic_API.Model;
using Bangalore_Traffic_API.BAL;

namespace Bangalore_Traffic_API
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> logger;
        private readonly IConfiguration configuration;
        private readonly IBangaloreAPIHit bangaloreAPIHit;

        public Worker(ILogger<Worker> _logger, IConfiguration _configuration,IBangaloreAPIHit _bangaloreAPIHit)
        {
            logger = _logger;
            configuration = _configuration;
            bangaloreAPIHit = _bangaloreAPIHit;

        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {

            while (!stoppingToken.IsCancellationRequested)
            {
                int timeInterval = int.Parse(configuration["TimeInterval:interval"]);                
                logger.LogInformation("Execution Start for Bangalore Traffic API method. \n ------------------------------------------------------------------------------------");
                try
                {
                                     
                    int dataInserted = await bangaloreAPIHit.bangaloreTrafficLoginAPIConsume();
                    
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, ex.Message + " \n ----------------------------------------------------------------------------------");
                }
                await Task.Delay(timeInterval * 60000, stoppingToken);
            }
        }
        public override Task StartAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Bangalore Traffic API Hit Feed Service Started. \n ---------------------------------------------------------------------------------");
            return base.StartAsync(cancellationToken);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Bangalore Traffic API Hit Feed Service Stopped. \n ------------------------------------------------------------------------------------");
            return base.StopAsync(cancellationToken);
        }
    
    }
}