using CENZURAZO_CQW1QQ_MESTER.Models;
using Microsoft.EntityFrameworkCore;

namespace CENZURAZO_CQW1QQ_MESTER.Data
{
    public class CensorRepository : ICensorRepository
    {
        CensorDbContext db;

        public CensorRepository(CensorDbContext db)
        {
            this.db = db;
        }

        public void Create(ReplacementData data)
        {
            this.db.ReplacementDatas.Add(data);
            db.SaveChanges();
        }

        public IEnumerable<ReplacementData> Read()
        {
            return this.db.ReplacementDatas;
        }

        public ReplacementData? Read(int id)
        {
            return this.db.ReplacementDatas.FirstOrDefault(x => x.ID == id);
        }

        public void Update(ReplacementData data)
        {
            ReplacementData toUpdate = this.Read(data.ID);

            toUpdate.Word = data.Word;
            toUpdate.Alternatives = data.Alternatives;


            db.SaveChanges();
        }
        public void Delete(int id)
        {
            ReplacementData toDelete = this.Read(id);
            this.db.ReplacementDatas.Remove(toDelete);

            db.SaveChanges();
        }
    }
