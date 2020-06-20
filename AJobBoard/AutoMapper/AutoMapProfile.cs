using AJobBoard.Models;
using AJobBoard.Models.Data;
using AJobBoard.Models.DTO;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jobtransparency.AutoMapper
{
    public class AutoMapProfile : Profile
    {
        public AutoMapProfile()
        {
            CreateMap<JobPosting, JobPostingDTO>();
            CreateMap<JobPostingDTO, JobPosting>();

            CreateMap<KeyPhraseDTO, KeyPhrase>();
            CreateMap<KeyPhrase, KeyPhraseDTO>();
        }
    }
}
