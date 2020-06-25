using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using ChatServerSignalRWithIdentity.Data;
using ChatServerSignalRWithIdentity.Data.DTO;
using ChatServerSignalRWithIdentity.Models;
using Microsoft.EntityFrameworkCore;

namespace ChatServerSignalRWithIdentity
{
    public class FileService
    {
        private readonly IMapper _mapper;
        private readonly ApplicationDbContext db;

        public FileService(ApplicationDbContext db, IMapper mapper)
        {
            this.db = db;
            _mapper = mapper;
        }

        public async Task Create(FileResponse fileDTO)
        {
            var file = _mapper.Map<FileResponse, File>(fileDTO);
            await db.Files.AddAsync(file);
            await db.SaveChangesAsync();
        }

        public async Task<FileResponse> GetById(Guid id)
        {
            var file = await db.Files.FindAsync(id);

            if (file == null)
                throw new ApplicationException("File not found");

            return _mapper.Map<File, FileResponse>(file);
        }
    }
}
