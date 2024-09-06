using Dapper;
using Etiket.Api.Entity;
using Microsoft.Data.SqlClient;

namespace Etiket.Api.Repository
{
    public class KumasRepository
    {
        private readonly string _connectionString;
        public KumasRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<IEnumerable<KumasTop>> GetKumasTopAsync(string topNo)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string sql = @"SELECT Cari.CariAdi AS CariAdi, RTRIM(Cari.CariNo) AS CariNo, trim(KumasTop.SiparisNo) AS SiparisNo, trim(KumasTop.TopNo) AS TopNo, KumasTop.NetMt - KumasTop.IskontoMt AS NetMt, KumasTop.NetKg, KumasTop.BrutKg, KumasTop.PartiNo,

                         KumasTop.SiparisSiraNo, KumasTop.KontrolTarihi, KumasTop.TopEni, KumasTop.IskontoMt, Stok.OzelAlan6 AS En,trim(Stok.StokKodu) AS StokKodu,trim(Stok.TicariStokKodu) AS TicariStokKodu

                         FROM            KumasTop WITH (nolock) LEFT OUTER JOIN

                         HAVUZ_STOK_KODLARI ON KumasTop.StokKodu = HAVUZ_STOK_KODLARI.StokKodu LEFT OUTER JOIN

                         Siparis ON KumasTop.SiparisNo = Siparis.SiparisNo AND KumasTop.SiparisSiraNo = Siparis.SiraNo LEFT OUTER JOIN

                         Stok WITH (nolock) ON KumasTop.StokKodu = Stok.StokKodu LEFT OUTER JOIN

                         Cari WITH (nolock) RIGHT OUTER JOIN

                         SiparisAna WITH (nolock) ON Cari.CariNo = SiparisAna.CariNo ON KumasTop.SiparisNo = SiparisAna.SiparisNo

                        WHERE        (KumasTop.TopNo =@TopNo) AND (Stok.LotNo = '1')";

                return await connection.QueryAsync<KumasTop>(sql, new { TopNo = topNo });
            }
        }
        public async Task<bool> KumasTopExists(string topNo)
        {
            using(var connection=new SqlConnection(_connectionString))
            {
                string sql = @"SELECT COUNT(1) From KumasTop Where TopNo=@TopNo";
                var count = await connection.ExecuteScalarAsync<int>(sql, new { TopNo = topNo });
                    return count > 0;
            }    
        }
    }
}
