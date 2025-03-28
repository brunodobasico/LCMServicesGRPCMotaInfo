using AMoverGRPC.Data;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;

namespace AMoverGRPC.Services
{
    public class MotaService : MotasService.MotasServiceBase
    {
        private readonly MotaDbContext _context;

        public MotaService(MotaDbContext context)
        {
            _context = context;
        }

        public override async Task<MotaResponse> GetMotaInfo(MotaRequest request, ServerCallContext context)
        {
            Console.WriteLine($"Buscando informações da mota ID: {request.Vin}");

            var mota = await _context.Motas
             .Where(m => m.VIN == request.Vin)
             .OrderByDescending(m => m.MotaId)
             .FirstOrDefaultAsync();



            if (mota == null)
            {
                Console.WriteLine($"Mota com ID {request.Vin} não encontrada.");
                throw new RpcException(new Status(StatusCode.NotFound, "Mota não encontrada"));
            }

            Console.WriteLine($"Enviando informações da mota ID: {mota.VIN}");

            return new MotaResponse
            {
                Vin = mota.VIN.ToString(),
                Battery = mota.Battery,
                Kilometers = mota.Kilometers,
                Latitude = mota.Latitude,
                Longitude = mota.Longitude
            };
        }
    }
}