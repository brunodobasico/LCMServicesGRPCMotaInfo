using AMoverGRPC.Data;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;

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
            // Extrair token do header Authorization
            var authHeader = context.RequestHeaders.FirstOrDefault(h => h.Key == "authorization")?.Value;

            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
                throw new RpcException(new Status(StatusCode.Unauthenticated, "Token JWT não fornecido"));

            var token = authHeader.Substring("Bearer ".Length);
            var handler = new JwtSecurityTokenHandler();

            if (!handler.CanReadToken(token))
                throw new RpcException(new Status(StatusCode.Unauthenticated, "Token inválido"));

            var jwt = handler.ReadJwtToken(token);

            // Extrair o preferred_username (ou podes usar sub)
            var username = jwt.Claims.FirstOrDefault(c => c.Type == "preferred_username")?.Value;

            if (username == null)
                throw new RpcException(new Status(StatusCode.Unauthenticated, "Utilizador não identificado"));

            Console.WriteLine($"Utilizador autenticado: {username}");

            // Buscar mota que pertence ao utilizador
            var mota = await _context.Motas
                .Where(m => m.VIN == request.Vin)
                .FirstOrDefaultAsync();

            if (mota == null)
            {
                Console.WriteLine($"Mota não encontrada ou sem permissão para o VIN: {request.Vin}");
                throw new RpcException(new Status(StatusCode.PermissionDenied, "Não tens acesso a esta mota"));
            }

            return new MotaResponse
            {
                Vin = mota.VIN,
                Battery = mota.Battery,
                Kilometers = mota.Kilometers,
                Latitude = mota.Latitude,
                Longitude = mota.Longitude
            };
        }
    }
}