using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using XSD.Models;

namespace XSD
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers()
                .AddXmlSerializerFormatters();

            builder.Services.AddEndpointsApiExplorer();

            var app = builder.Build();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();

            app.Run();
        }
    }
}
