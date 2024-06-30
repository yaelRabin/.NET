using Serilog;

namespace BarberShopApi
{

    public static class SerilogBuilder
    {
        public static WebApplicationBuilder UseSerilog(this WebApplicationBuilder builder)
        {
            builder
                .Host
                .UseSerilog((
                    hostingContext,
                    loggerConfiguration) =>
                {
                    loggerConfiguration
                    .ReadFrom
                    .Configuration(hostingContext.Configuration);
                });

            return builder;
        }
    }

}
